using System;
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
using Sixnet.Threading.Locking;

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
        internal static async Task<List<string>> GetTableNamesAsync(DataCommandExecutionContext context)
        {
            var entityType = context.ActivityQueryable.GetModelType();
            entityType ??= (context?.Command?.GetEntityType());

            SixnetDirectThrower.ThrowArgNullIf(entityType == null, $"Entity type is null");

            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            if (entityConfig != null && entityConfig.SplitTableType != SplitTableType.None) // split table name
            {
                return await GetSplitTableNamesAsync(context, entityConfig).ConfigureAwait(false);
            }
            else // default table name
            {
                var tableName = GetDefaultTableName(context.Server?.DatabaseType, entityType);
                tableName = string.IsNullOrWhiteSpace(tableName) ? context.Command?.TableName : tableName;
                return new List<string>(1) { tableName };
            }
        }

        /// <summary>
        /// Get split table names
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityConfig"></param>
        /// <param name="splitValues"></param>
        /// <returns></returns>
        internal static async Task<List<string>> GetSplitTableNamesAsync(DataCommandExecutionContext context, EntityConfiguration entityConfig)
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));
            SixnetDirectThrower.ThrowArgNullIf(entityConfig == null, nameof(entityConfig));
            SixnetDirectThrower.ThrowNotSupportIf(entityConfig.SplitTableType == SplitTableType.None, $"{entityConfig.EntityType.Name} not support split table.");

            var dataOptions = GetDataOptions();
            var provider = GetSplitTableProvider(dataOptions, entityConfig);
            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"Not set split table provider for {entityConfig.SplitTableProviderName}");

            var splitBehavior = context.GetSplitTableBehavior() ?? _defaultSplitTableBehavior;
            var rootTableName = GetDefaultTableName(context.Server.DatabaseType, entityConfig.EntityType);
            var splitTableNames = provider.ResolveTableNames(new ResolveSplitTableNameParameter()
            {
                EntityConfiguration = entityConfig,
                RootTableName = rootTableName,
                SplitBehavior = splitBehavior
            });

            // all table names
            var serverTableKey = GetDatabaseServerSplitTableCacheKey(entityConfig, context.Server);
            var allTableNames = await GetCachedTableNamesAsync(serverTableKey).ConfigureAwait(false);

            // split table names
            if (context.Command?.OperationType == DataOperationType.Insert)
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(splitTableNames.IsNullOrEmpty(), $"Not assign split table for {entityConfig.EntityType.Name}");
                var diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                if (!diffTables.IsNullOrEmpty())
                {
                    allTableNames = await RefreshTablesAsync(context, rootTableName, serverTableKey, splitBehavior, provider).ConfigureAwait(false);
                    diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                }
                if (!diffTables.IsNullOrEmpty() && dataOptions.AutoCreateSplitTable)
                {
                    var createTableLock = await SixnetLocker.GetCreateTableLockAsync(entityConfig.EntityType).ConfigureAwait(false);
                    if (createTableLock != null)
                    {
                        try
                        {
                            allTableNames = await GetCachedTableNamesAsync(serverTableKey).ConfigureAwait(false);
                            diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                            if (!diffTables.IsNullOrEmpty())
                            {
                                await AutoCreateTablesAsync(context, rootTableName, serverTableKey, entityConfig, diffTables, splitBehavior, provider).ConfigureAwait(false);
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            await createTableLock.Value.ReleaseAsync().ConfigureAwait(false);
                        }
                    }
                }
            }
            else
            {
                if (splitBehavior.IsTakeAllSplitTables(splitTableNames))
                {
                    if (allTableNames.IsNullOrEmpty())
                    {
                        allTableNames = await RefreshTablesAsync(context, rootTableName, serverTableKey, splitBehavior, provider).ConfigureAwait(false);
                    }
                    return allTableNames;
                }
                var diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                if (!diffTables.IsNullOrEmpty())
                {
                    allTableNames = await RefreshTablesAsync(context, rootTableName, serverTableKey, splitBehavior, provider).ConfigureAwait(false);
                    diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                }
                if (!diffTables.IsNullOrEmpty())
                {
                    splitTableNames = splitTableNames.Except(diffTables, _defaultDataTableNameComparer).ToList();
                }
                splitTableNames = provider.GetTableNames(new GetSplitTableNameParameter()
                {
                    ResolvedTableNames = splitTableNames,
                    AllTableNames = allTableNames,
                    RootTableName = rootTableName,
                    Behavior = splitBehavior
                });
            }
            return splitTableNames;
        }

        /// <summary>
        /// Get cached table names
        /// </summary>
        /// <param name="serverTableKey">Server table key</param>
        /// <returns></returns>
        static async Task<List<string>> GetCachedTableNamesAsync(string serverTableKey)
        {
            var setMembersParameter = new SetMembersParameter()
            {
                Key = serverTableKey
            };
            HandleSplitTableCacheParameter(setMembersParameter);
            return (await SixnetCacher.Set.MembersAsync(setMembersParameter).ConfigureAwait(false))?.Members ?? new List<string>(0);
        }

        /// <summary>
        /// Refresh tables
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rootTableName"></param>
        /// <param name="serverTableKey"></param>
        /// <returns></returns>
        static async Task<List<string>> RefreshTablesAsync(DataCommandExecutionContext context, string rootTableName, string serverTableKey
            , SplitTableBehavior splitTableBehavior, ISixnetSplitTableProvider splitTableProvider)
        {
            List<string> allTableNames;
            using (var dataClient = GetClientForConnection(context.DatabaseConnection, true, true, false, context.DatabaseConnection.DataIsolationLevel))
            {
                allTableNames = (await dataClient.GetTablesAsync().ConfigureAwait(false))?.Select(c => c.TableName).ToList();
            }
            allTableNames = splitTableProvider.FilterAllTableNames(new FilterAllSplitTableNameParameter()
            {
                AllTableNames = allTableNames,
                RootTableName = rootTableName,
                Behavior = splitTableBehavior
            }) ?? new List<string>(0);
            if (!allTableNames.IsNullOrEmpty())
            {
                var setAddParameter = new SetAddParameter()
                {
                    Key = serverTableKey,
                    Members = allTableNames
                };
                HandleSplitTableCacheParameter(setAddParameter);
                await SixnetCacher.Set.AddAsync(setAddParameter).ConfigureAwait(false);
            }
            return allTableNames ?? new List<string>(0);
        }

        /// <summary>
        /// Create tables
        /// </summary>
        /// <param name="context">Data command execution context</param>
        /// <param name="rootTableName">Root table name</param>
        /// <param name="serverTableKey">Server table key</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="newTableNames">New table names</param>
        /// <returns></returns>
        static async Task AutoCreateTablesAsync(DataCommandExecutionContext context, string rootTableName, string serverTableKey
            , EntityConfiguration entityConfig, IEnumerable<string> newTableNames, SplitTableBehavior splitTableBehavior
            , ISixnetSplitTableProvider splitTableProvider)
        {
            SixnetDirectThrower.ThrowArgNullIf(newTableNames.IsNullOrEmpty(), nameof(newTableNames));

            using (var dataClient = GetClientForConnection(context.DatabaseConnection, true, true, true, context.DatabaseConnection.DataIsolationLevel))
            {
                await dataClient.MigrateAsync(new MigrationInfo()
                {
                    NewTables = new List<NewTableInfo>()
                    {
                        new NewTableInfo()
                        {
                            TableNames = newTableNames?.ToList(),
                            EntityType = entityConfig.EntityType
                        }
                    }
                }).ConfigureAwait(false);
                await dataClient.CommitAsync().ConfigureAwait(false);
            }
            await RefreshTablesAsync(context, rootTableName, serverTableKey, splitTableBehavior, splitTableProvider).ConfigureAwait(false);
        }
    }
}
