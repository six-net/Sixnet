﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.Set.Parameters;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;

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
            var entityType = queryableEntityType ?? (context?.Command?.GetEntityType());

            SixnetDirectThrower.ThrowArgNullIf(entityType == null, $"Entity type is null");

            // default table name
            var tableName = GetDefaultTableName(context.Server?.DatabaseType, entityType);
            tableName = string.IsNullOrWhiteSpace(tableName) ? context.Command?.TableName : tableName;

            // split table name
            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
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
        static async Task<List<string>> GetSplitTableNamesAsync(DataCommandExecutionContext context, EntityConfiguration entityConfig)
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));
            SixnetDirectThrower.ThrowArgNullIf(entityConfig == null, nameof(entityConfig));
            SixnetDirectThrower.ThrowNotSupportIf(entityConfig.SplitTableType == SplitTableType.None, $"{entityConfig.EntityType.Name} not support split table.");
            SixnetDirectThrower.ThrowSixnetExceptionIf(string.IsNullOrWhiteSpace(entityConfig.SplitTableProviderName), $"{entityConfig.EntityType.Name} not set split table provider name");

            var dataOptions = GetDataOptions();
            var provider = dataOptions.GetSplitTableProvider(entityConfig.SplitTableProviderName);

            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"Not set split table provider for {entityConfig.SplitTableProviderName}");

            var splitBehavior = context.GetSplitTableBehavior();
            var rootTableName = GetDefaultTableName(context.Server.DatabaseType, entityConfig.EntityType);
            var splitTableNames = provider.GetSplitTableNames(new GetSplitTableNameOptions()
            {
                EntityConfiguration = entityConfig,
                RootTableName = rootTableName,
                SplitBehavior = splitBehavior
            });

            // all table names
            var databaseServer = context.Server;
            var serverTableKey = $"{entityConfig.EntityType.Name}:{databaseServer.GetServerIdentityValue()}";
            var hasGotTables = (await SixnetCacher.Keys.ExistAsync(new ExistParameter()
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
                allTableNames = (await SixnetCacher.Set.MembersAsync(new SetMembersParameter()
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
                if (!diffTables.IsNullOrEmpty() && dataOptions.AutoCreateSplitTable)
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
        static async Task<List<string>> RefreshTablesAsync(DatabaseServer server, string rootTableName, string serverTableKey)
        {
            List<string> allTableNames;
            using (var dataClient = GetClient(server, true, false))
            {
                allTableNames = (await dataClient.GetTablesAsync().ConfigureAwait(false))?.Select(c => c.TableName).ToList();
            }
            allTableNames = allTableNames?.Where(t => t.ToLower().StartsWith(rootTableName.ToLower())).ToList() ?? new List<string>(0);
            if (!allTableNames.IsNullOrEmpty())
            {
                await SixnetCacher.Set.AddAsync(new SetAddParameter()
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
        static async Task AutoCreateTablesAsync(DatabaseServer server, Type entityType, IEnumerable<string> newTableNames)
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
