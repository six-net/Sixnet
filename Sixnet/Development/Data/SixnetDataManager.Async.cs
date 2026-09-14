// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache;
using Sixnet.Cache.Set.Parameters;
using Sixnet.Code;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Extensions;
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
        internal static async Task<List<SixnetDatabaseObjectName>> GetTableNamesAsync(SixnetDataCommandExecutionContext context)
        {
            var entityType = context.ActivityQueryable.GetModelType();
            entityType ??= (context?.Command?.GetEntityType());

            SixnetDirectThrower.ThrowArgNullIf(entityType == null, $"Entity type is null");

            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            if (entityConfig != null && entityConfig.SplitTableType != SixnetSplitTableType.None) // split table name
            {
                return await GetSplitTableNamesAsync(context, entityConfig).ConfigureAwait(false);
            }
            else // default table name
            {
                var tableName = GetDefaultTableName(context, entityConfig);
                return new List<SixnetDatabaseObjectName>(1) { tableName };
            }
        }

        /// <summary>
        /// Get split table names
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityConfig"></param>
        /// <returns></returns>
        internal static async Task<List<SixnetDatabaseObjectName>> GetSplitTableNamesAsync(SixnetDataCommandExecutionContext context, SixnetEntityConfiguration entityConfig)
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));
            SixnetDirectThrower.ThrowArgNullIf(entityConfig == null, nameof(entityConfig));
            SixnetDirectThrower.ThrowNotSupportIf(entityConfig.SplitTableType == SixnetSplitTableType.None, $"{entityConfig.EntityType.Name} not support split table.");

            var dataOptions = GetDataOptions();
            var provider = GetSplitTableProvider(dataOptions, entityConfig);
            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"Not set split table provider for {entityConfig.SplitTableProviderName}");

            var splitBehavior = context.GetSplitTableBehavior() ?? _defaultSplitTableBehavior;
            var rootTableName = GetDefaultTableName(context, entityConfig, context.Command?.TableName);
            var splitedTableNames = provider.ResolveTableNames(new SixnetResolveSplitTableNameParameter()
            {
                EntityConfiguration = entityConfig,
                RootTableName = rootTableName,
                SplitBehavior = splitBehavior
            });
            var resolvedTableNames = splitedTableNames;

            // all table names
            var serverTableKey = GetDatabaseServerSplitTableCacheKey(entityConfig, context.Server, rootTableName);
            var allTableNames = await GetCachedTableNamesAsync(serverTableKey, rootTableName).ConfigureAwait(false);
            if (allTableNames.IsNullOrEmpty())
            {
                allTableNames = await RefreshTablesAsync(context, rootTableName, serverTableKey, splitBehavior, provider).ConfigureAwait(false);
            }

            // split table names
            if (context.Command?.OperationType == SixnetDataOperationType.Insert)
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(splitedTableNames.IsNullOrEmpty(), $"Not assign split table for {entityConfig.EntityType.Name}");
                var diffTables = splitedTableNames.Except(allTableNames);
                if (!diffTables.IsNullOrEmpty())
                {
                    allTableNames = await RefreshTablesAsync(context, rootTableName, serverTableKey, splitBehavior, provider).ConfigureAwait(false);
                    diffTables = splitedTableNames.Except(allTableNames);
                }
                if (!diffTables.IsNullOrEmpty() && dataOptions.AutoCreateSplitTable)
                {
                    var createTableLock = await SixnetLocker.GetCreateTableLockAsync(entityConfig.EntityType).ConfigureAwait(false);
                    if (createTableLock != null)
                    {
                        try
                        {
                            allTableNames = await GetCachedTableNamesAsync(serverTableKey, rootTableName).ConfigureAwait(false);
                            diffTables = splitedTableNames.Except(allTableNames);
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
                if (splitBehavior.IsTakeAllSplitTables(splitedTableNames))
                {
                    if (allTableNames.IsNullOrEmpty())
                    {
                        allTableNames = await RefreshTablesAsync(context, rootTableName, serverTableKey, splitBehavior, provider).ConfigureAwait(false);
                    }
                    splitedTableNames = allTableNames;
                }
                var diffTables = splitedTableNames.Except(allTableNames);
                if (!diffTables.IsNullOrEmpty())
                {
                    allTableNames = await RefreshTablesAsync(context, rootTableName, serverTableKey, splitBehavior, provider).ConfigureAwait(false);
                    diffTables = splitedTableNames.Except(allTableNames);
                }
                if (!diffTables.IsNullOrEmpty())
                {
                    splitedTableNames = splitedTableNames.Except(diffTables).ToList();
                }
                splitedTableNames = provider.GetTableNames(new SixnetGetSplitTableNameParameter()
                {
                    ResolvedTableNames = splitedTableNames,
                    AllTableNames = allTableNames,
                    RootTableName = rootTableName,
                    Behavior = splitBehavior
                });
            }
            return splitedTableNames.IsNullOrEmpty() ? resolvedTableNames : splitedTableNames;
        }

        /// <summary>
        /// Get cached table names
        /// </summary>
        /// <param name="serverTableKey">Server table key</param>
        /// <returns></returns>
        static async Task<List<SixnetDatabaseObjectName>> GetCachedTableNamesAsync(string serverTableKey, SixnetDatabaseObjectName rootTableName)
        {
            var setMembersParameter = new SixnetSetMembersParameter()
            {
                Key = serverTableKey
            };
            HandleSplitTableCacheParameter(setMembersParameter);
            return (await SixnetCacher.Set.MembersAsync(setMembersParameter).ConfigureAwait(false))?.Members
                .Select(m => SixnetDatabaseObjectName.Create(m, SixnetDatabaseObjectType.Table, rootTableName.SchemaName)).ToList()
                ?? new List<SixnetDatabaseObjectName>(0);
        }

        /// <summary>
        /// Refresh tables
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rootTableName"></param>
        /// <param name="serverTableKey"></param>
        /// <returns></returns>
        static async Task<List<SixnetDatabaseObjectName>> RefreshTablesAsync(SixnetDataCommandExecutionContext context, SixnetDatabaseObjectName rootTableName, string serverTableKey
            , SixnetSplitTableBehavior splitTableBehavior, ISixnetSplitTableProvider splitTableProvider)
        {
            List<SixnetDatabaseObjectName> allTableNames;
            using (var dataClient = GetClientForConnection(context.DatabaseConnection, true, true, false, context.DatabaseConnection.DataIsolationLevel))
            {
                allTableNames = (await dataClient.GetTablesAsync().ConfigureAwait(false))?.Select(c => SixnetDatabaseObjectName.Create(c.Name, SixnetDatabaseObjectType.Table, c.SchemaName)).ToList();
            }
            allTableNames = splitTableProvider.FilterAllTableNames(new SixnetFilterAllSplitTableNameParameter()
            {
                AllTableNames = allTableNames,
                RootTableName = rootTableName,
                Behavior = splitTableBehavior
            }) ?? new List<SixnetDatabaseObjectName>(0);
            if (!allTableNames.IsNullOrEmpty())
            {
                var setAddParameter = new SixnetSetAddParameter()
                {
                    Key = serverTableKey,
                    Members = allTableNames.Select(c => c.Name).ToList()
                };
                HandleSplitTableCacheParameter(setAddParameter);
                await SixnetCacher.Set.AddAsync(setAddParameter).ConfigureAwait(false);
            }
            return allTableNames ?? new List<SixnetDatabaseObjectName>(0);
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
        static async Task AutoCreateTablesAsync(SixnetDataCommandExecutionContext context, SixnetDatabaseObjectName rootTableName, string serverTableKey
            , SixnetEntityConfiguration entityConfig, IEnumerable<SixnetDatabaseObjectName> newTableNames, SixnetSplitTableBehavior splitTableBehavior
            , ISixnetSplitTableProvider splitTableProvider)
        {
            SixnetDirectThrower.ThrowArgNullIf(newTableNames.IsNullOrEmpty(), nameof(newTableNames));

            using (var dataClient = GetClientForConnection(context.DatabaseConnection, true, true, true, context.DatabaseConnection.DataIsolationLevel))
            {
                await dataClient.MigrateAsync(new SixnetMigrationInfo()
                {
                    NewTables = new List<SixnetNewTableInfo>()
                    {
                        new SixnetNewTableInfo()
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

        /// <summary>
        /// Update database
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static async Task<SixnetUpdateDatabaseResult> UpdateDatabaseAsync(SixnetUpdateDatabaseParameter parameter)
        {
            var reportProcess = parameter.ReportProcess;
            try
            {
                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Begin, parameter));
                if (parameter.ExecuteRecordDirectly)
                {
                    if (!parameter.Records.IsNullOrEmpty())
                    {
                        var sortedRecords = parameter.Records.OrderBy(c => c.Id);
                        SixnetUpdateDatabaseContext context = null;

                        using (var dataClient = GetClient(parameter.DatabaseServer))
                        {
                            context = await GetUpdateDatabaseContextAsync(dataClient, parameter).ConfigureAwait(false);
                        }
                        foreach (var record in sortedRecords)
                        {
                            if (!parameter.ExecuteRecordForRollback)
                            {
                                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Update, parameter, context, record));
                                await record.UpdateAsync(context).ConfigureAwait(false);
                                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.UpdateFinished, parameter, context, record));
                            }
                            else
                            {
                                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Rollback, parameter, context, record));
                                await record.RollbackAsync(context).ConfigureAwait(false);
                                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.RollbackFinished, parameter, context, record));
                            }
                        }
                    }
                }
                else
                {
                    reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.BeginGettingCurrentInfo, parameter));
                    var recordRes = await GetDatabaseUpdateRecordsCoreAsync(parameter).ConfigureAwait(false);
                    var context = recordRes.Item1;
                    reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.EndGettingCurrentInfo, parameter, context, null));

                    if (recordRes.Item2.IsNullOrEmpty())
                    {
                        reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.NoneRecords, parameter));
                    }
                    else
                    {
                        foreach (var record in recordRes.Item2)
                        {
                            if (recordRes.Item3)
                            {
                                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Update, parameter, context, record));
                                await record.UpdateAsync(context).ConfigureAwait(false);
                                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.UpdateFinished, parameter, context, record));
                            }
                            else
                            {
                                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Rollback, parameter, context, record));
                                await record.RollbackAsync(context).ConfigureAwait(false);
                                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.RollbackFinished, parameter, context, record));
                            }
                        }
                    }
                }
                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.End, parameter));
            }
            catch (Exception ex)
            {
                reportProcess?.Invoke(SixnetUpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Error, parameter, ex: ex));
                throw;
            }

            return SixnetUpdateDatabaseResult.SuccessResult("");
        }

        /// <summary>
        /// Get database update records
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static async Task<List<ISixnetDatabaseUpdateRecord>> GetExecutableDatabaseUpdateRecordsAsync(SixnetUpdateDatabaseParameter parameter)
        {
            return (await GetDatabaseUpdateRecordsCoreAsync(parameter).ConfigureAwait(false)).Item2;
        }

        /// <summary>
        /// Get database update records core
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        static async Task<Tuple<SixnetUpdateDatabaseContext, List<ISixnetDatabaseUpdateRecord>, bool>> GetDatabaseUpdateRecordsCoreAsync(SixnetUpdateDatabaseParameter parameter)
        {
            SixnetDirectThrower.ThrowArgErrorIf(parameter?.DatabaseServer == null, "Database server is null");
            SixnetDirectThrower.ThrowArgErrorIf(parameter?.TargetVersion == null, "Target version is null");

            SixnetUpdateDatabaseContext updateContext = null;
            using (var client = GetClient(parameter.DatabaseServer, false, true))
            {
                // create table
                await client.CreateTableAsync(typeof(SixnetAppUpdateRecordEntity));
                await client.CommitAsync();

                // get context
                updateContext = await GetUpdateDatabaseContextAsync(client, parameter).ConfigureAwait(false);
            }

            var currentVersion = updateContext.GetMaxVersion();
            if (currentVersion <= parameter.TargetVersion)
            {
                return new Tuple<SixnetUpdateDatabaseContext, List<ISixnetDatabaseUpdateRecord>, bool>(updateContext, GetForwardRecords(updateContext), true);
            }
            else
            {
                return new Tuple<SixnetUpdateDatabaseContext, List<ISixnetDatabaseUpdateRecord>, bool>(updateContext, GetRollbackRecords(updateContext), false);
            }
        }

        static async Task<SixnetUpdateDatabaseContext> GetUpdateDatabaseContextAsync(ISixnetDataClient dataClient, SixnetUpdateDatabaseParameter parameter)
        {
            var versionGroupMaxRecords = await dataClient.QueryAsync<SixnetAppUpdateRecordEntity>(SixnetQuerier.Create<SixnetAppUpdateRecordEntity>()
                .GroupBy(c => c.AppVersion)
                .Select(c => new
                {
                    c.AppVersion,
                    Id = c.Id.DbMax()
                })).ConfigureAwait(false);

            return new SixnetUpdateDatabaseContext()
            {
                UpdateParameter = parameter,
                VersionMaxRecordIds = versionGroupMaxRecords?.ToDictionary(c => Version.Parse(c.AppVersion), c => c.Id) ?? new Dictionary<Version, long>(0)
            };
        }
    }
}
