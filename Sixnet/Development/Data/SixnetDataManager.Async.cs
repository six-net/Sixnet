using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Cache.Keys;
using Sixnet.Cache.Set;
using Sixnet.Cache;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;
using Sixnet.Cache.Keys.Options;
using Sixnet.Cache.Set.Options;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Data manager
    /// </summary>
    public static partial class SixnetDataManager
    {
        /// <summary>
        /// Gets data command table names
        /// </summary>
        /// <param name="context">Data command execution context</param>
        /// <returns></returns>
        public static async Task<List<string>> GetTableNamesAsync(DataCommandExecutionContext context)
        {
            var queryableEntityType = context.ActivityQueryable.GetModelType();
            var entityType = queryableEntityType == null ? context?.Command?.GetEntityType() : queryableEntityType;
            SixnetDirectThrower.ThrowArgNullIf(entityType == null, $"Entity type is null");

            // default table name
            var tableName = GetDefaultTableName(context.Server?.ServerType, entityType);
            tableName = string.IsNullOrWhiteSpace(tableName) ? context.Command?.TableName : tableName;

            // split table name
            var entityConfig = SixnetEntityManager.GetEntityConfiguration(entityType);
            if (entityConfig != null && entityConfig.SplitTableType != SplitTableType.None)
            {
                return await GetSplitTableNamesAsync(context, entityConfig).ConfigureAwait(false);
            }

            return new List<string>(1) { tableName };
        }

        /// <summary>
        /// Get split table names
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityConfig"></param>
        /// <param name="splitValues"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetSplitTableNamesAsync(DataCommandExecutionContext context, EntityConfiguration entityConfig)
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));
            SixnetDirectThrower.ThrowArgNullIf(entityConfig == null, nameof(entityConfig));
            SixnetDirectThrower.ThrowNotSupportIf(entityConfig.SplitTableType == SplitTableType.None, $"{entityConfig.EntityType.Name} not support split table.");
            SixnetDirectThrower.ThrowSixnetExceptionIf(string.IsNullOrWhiteSpace(entityConfig.SplitTableProviderName), $"{entityConfig.EntityType.Name} not set split table provider name");
            SixnetDirectThrower.ThrowSixnetExceptionIf(!SplitTableProviders.TryGetValue(entityConfig.SplitTableProviderName, out var provider), $"Not set split table provider for {entityConfig.SplitTableProviderName}");

            var splitBehavior = context.GetSplitTableBehavior();
            var rootTableName = GetDefaultTableName(context.Server.ServerType, entityConfig.EntityType);
            var splitTableNames = provider.GetSplitTableNames(new GetSplitTableNameOptions()
            {
                EntityConfiguration = entityConfig,
                RootTableName = rootTableName,
                SplitBehavior = splitBehavior
            });

            // all table names
            var databaseServer = context.Server;
            var serverTableKey = $"{entityConfig.EntityType.Name}:{databaseServer.GetServerIdentityValue()}";
            var hasGotTables = (await SixnetCacher.Keys.ExistAsync(new ExistOptions()
            {
                Keys = new List<CacheKey>() { serverTableKey }
            }).ConfigureAwait(false))?.KeyCount == 1;
            List<string> allTableNames;
            if (!hasGotTables)
            {
                allTableNames = await RefreshTablesAsync(databaseServer, rootTableName, serverTableKey).ConfigureAwait(false);
            }
            else
            {
                allTableNames = (await SixnetCacher.Set.MembersAsync(new SetMembersOptions()
                {
                    Key = serverTableKey
                }).ConfigureAwait(false))?.Members ?? new List<string>(0);
            }

            if (context.Command?.OperationType == DataOperationType.Insert)
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(splitTableNames.IsNullOrEmpty(), $"Not assign split table for {entityConfig.EntityType.Name}");
                var diffTables = splitTableNames.Except(allTableNames);
                if (!diffTables.IsNullOrEmpty() && hasGotTables)
                {
                    allTableNames = await RefreshTablesAsync(databaseServer, rootTableName, serverTableKey).ConfigureAwait(false);
                    diffTables = splitTableNames.Except(allTableNames);
                }
                if (!diffTables.IsNullOrEmpty() && AutoCreateSplitTable)
                {
                    await AutoCreateTablesAsync(databaseServer, entityConfig.EntityType, diffTables).ConfigureAwait(false);
                    await RefreshTablesAsync(databaseServer, rootTableName, serverTableKey).ConfigureAwait(false);
                }
            }
            else
            {
                if (splitBehavior.IsTakeAllSplitTables(splitTableNames))
                {
                    return allTableNames;
                }
                var diffTables = splitTableNames.Except(allTableNames);
                if (!diffTables.IsNullOrEmpty() && hasGotTables)
                {
                    allTableNames = await RefreshTablesAsync(databaseServer, rootTableName, serverTableKey).ConfigureAwait(false);
                    diffTables = splitTableNames.Except(allTableNames);
                }
                if (!diffTables.IsNullOrEmpty())
                {
                    splitTableNames = splitTableNames.Except(diffTables).ToList();
                }
                splitTableNames = provider.GetFinallySplitTableNames(splitBehavior, allTableNames, splitTableNames);
            }
            return splitTableNames;
        }

        /// <summary>
        /// Refresh tables
        /// </summary>
        /// <param name="server"></param>
        /// <param name="rootTableName"></param>
        /// <param name="serverTableKey"></param>
        /// <returns></returns>
        static async Task<List<string>> RefreshTablesAsync(SixnetDatabaseServer server, string rootTableName, string serverTableKey)
        {
            List<string> allTableNames;
            using (var dataClient = GetClient(server, true, false))
            {
                allTableNames = (await dataClient.GetTablesAsync().ConfigureAwait(false))?.Select(c => c.Name).ToList();
            }
            allTableNames = allTableNames?.Where(t => t.ToLower().StartsWith(rootTableName.ToLower())).ToList() ?? new List<string>(0);
            if (!allTableNames.IsNullOrEmpty())
            {
                await SixnetCacher.Set.AddAsync(new SetAddOptions()
                {
                    Key = serverTableKey,
                    Members = allTableNames
                }).ConfigureAwait(false);
            }
            return allTableNames ?? new List<string>(0);
        }

        /// <summary>
        /// Create tables
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="newTableNames">New table names</param>
        /// <returns></returns>
        static async Task AutoCreateTablesAsync(SixnetDatabaseServer server, Type entityType, IEnumerable<string> newTableNames)
        {
            SixnetDirectThrower.ThrowArgNullIf(newTableNames.IsNullOrEmpty(), nameof(newTableNames));
            using (var dataClient = GetClient(server, true, true))
            {
                await dataClient.MigrateAsync(new MigrationInfo()
                {
                    NewTables = new List<NewTableInfo>()
                    {
                        new NewTableInfo()
                        {
                            TableNames = newTableNames?.ToList(),
                            EntityType = entityType
                        }
                    }
                }).ConfigureAwait(false);
                await dataClient.CommitAsync().ConfigureAwait(false);
            }
        }
    }
}
