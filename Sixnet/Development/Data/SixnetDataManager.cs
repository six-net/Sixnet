// "Company © 2025. All rights reserved."

using System.Collections;
using System.Data;
using System.Threading.Tasks;

using Sixnet.Cache;
using Sixnet.Cache.Set.Parameters;
using Sixnet.DependencyInjection;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Command.Event;
using Sixnet.Development.Data.Dapper;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.DataType.Handler;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Data.Parameter.Handler;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Threading.Locking;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Data manager
    /// </summary>
    public static partial class SixnetDataManager
    {
        #region Constructor

        static SixnetDataManager()
        {
            SqlMapper.Settings.ApplyNullValues = true;
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
            SqlMapper.AddTypeHandler(new GuidHandler());
            SqlMapper.AddTypeHandler(new ByteHandler());
            SqlMapper.AddTypeHandler(new UIntHandler());
            SqlMapper.AddTypeHandler(new UShortHandler());
            SqlMapper.AddTypeHandler(new ULongHandler());
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        #endregion

        #region Fields

        static readonly DefaultDateSplitTableProvider _defaultDateSplitTableProvider = new();
        static readonly SplitTableBehavior _defaultSplitTableBehavior = new();
        static readonly SixnetDataTableNameComparer _defaultDataTableNameComparer = new();
        static readonly SortedDictionary<Version, SortedSet<ISixnetDatabaseUpdateRecord>> _updateRecords = new();

        #endregion

        #region Methods

        #region Database server

        /// <summary>
        /// Get data command database servers
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        internal static List<DatabaseServer> GetCommandDatabaseServers(SixnetDataCommand command)
        {
            return GetDataOptions().GetDataCommandDatabaseServers(command);
        }

        /// <summary>
        /// Get default database servers
        /// </summary>
        /// <returns></returns>
        public static List<DatabaseServer> GetDefaultDatabaseServers()
        {
            var allServers = GetAllDatabaseServers();
            if (allServers.IsNullOrEmpty())
            {
                return new List<DatabaseServer>(0);
            }
            var defaultServers = allServers.Where(c => c != null && c.Role == DatabaseServerRole.Default);
            if (defaultServers.IsNullOrEmpty() && allServers.Count == 1)
            {
                return new List<DatabaseServer>(1) { allServers.First() };
            }
            return defaultServers?.ToList() ?? new List<DatabaseServer>(0);
        }

        /// <summary>
        /// Get all config database servers
        /// </summary>
        /// <returns></returns>
        public static List<DatabaseServer> GetAllDatabaseServers()
        {
            return GetDataOptions()?.Servers ?? new List<DatabaseServer>(0);
        }

        /// <summary>
        /// Get config database server by names
        /// </summary>
        /// <param name="serverName">Database server name</param>
        /// <returns></returns>
        public static List<DatabaseServer> GetDatabaseServers(string serverName)
        {
            return GetDatabaseServers(new string[1] { serverName });
        }

        /// <summary>
        /// Get config database server by names
        /// </summary>
        /// <param name="serverNames">Database server names</param>
        /// <returns></returns>
        public static List<DatabaseServer> GetDatabaseServers(IEnumerable<string> serverNames)
        {
            if (serverNames.IsNullOrEmpty())
            {
                return new List<DatabaseServer>(0);
            }
            var allServers = GetAllDatabaseServers();
            return allServers?.Where(s => serverNames.Contains(s.Name)).ToList()
                ?? new List<DatabaseServer>(0);
        }

        #endregion

        #region Database connection

        /// <summary>
        /// Get database connection
        /// </summary>
        /// <param name="server">Database server</param>
        /// <returns></returns>
        public static IDbConnection GetDatabaseConnection(DatabaseServer server)
        {
            return GetDataOptions().GetConnection(server);
        }

        #endregion

        #region Resolve connection string

        /// <summary>
        /// Resolve connection string
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static string ResolveConnectionString(DatabaseServer server)
        {
            return server.ConnectionString;
        }

        #endregion

        #region Database provider

        /// <summary>
        /// Get database provider
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns>Return database provider</returns>
        internal static ISixnetDatabaseProvider GetDatabaseProvider(DatabaseType databaseType)
        {
            var options = GetDataOptions();
            var databaseProvider = options.GetDatabaseProvider(databaseType);

            SixnetDirectThrower.ThrowSixnetExceptionIf(databaseProvider == null, $"Not set database provider for {databaseType}");

            return databaseProvider;
        }

        #endregion

        #region Table name

        /// <summary>
        /// Gets data command table names
        /// </summary>
        /// <param name="context">Data command execution context</param>
        /// <returns></returns>
        internal static List<DatabaseObjectName> GetTableNames(DataCommandExecutionContext context)
        {
            var entityType = context.ActivityQueryable.GetModelType();
            entityType ??= (context?.Command?.GetEntityType());

            SixnetDirectThrower.ThrowArgNullIf(entityType == null, $"Entity type is null");

            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            if (entityConfig != null && entityConfig.SplitTableType != SplitTableType.None) // split table name
            {
                return GetSplitTableNames(context, entityConfig);
            }
            else
            {
                var tableName = GetDefaultTableName(context, entityConfig, context.Command?.TableName);
                return new List<DatabaseObjectName>(1) { tableName };
            }
        }

        /// <summary>
        /// Get split table names
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityConfig"></param>
        /// <returns></returns>
        internal static List<DatabaseObjectName> GetSplitTableNames(DataCommandExecutionContext context, EntityConfiguration entityConfig)
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));
            SixnetDirectThrower.ThrowArgNullIf(entityConfig == null, nameof(entityConfig));
            SixnetDirectThrower.ThrowNotSupportIf(entityConfig.SplitTableType == SplitTableType.None, $"{entityConfig.EntityType.Name} not support split table.");

            var dataOptions = GetDataOptions();
            var provider = GetSplitTableProvider(dataOptions, entityConfig);
            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"Not set split table provider for {entityConfig.SplitTableProviderName}");

            var splitBehavior = context.GetSplitTableBehavior() ?? _defaultSplitTableBehavior;
            var rootTableName = GetDefaultTableName(context, entityConfig, context.Command?.TableName);
            var splitTableNames = provider.ResolveTableNames(new ResolveSplitTableNameParameter()
            {
                EntityConfiguration = entityConfig,
                RootTableName = rootTableName,
                SplitBehavior = splitBehavior
            });

            // all table names
            var serverTableKey = GetDatabaseServerSplitTableCacheKey(entityConfig, context.Server, rootTableName);
            var allTableNames = GetCachedTableNames(serverTableKey, rootTableName);
            if (allTableNames.IsNullOrEmpty())
            {
                allTableNames = RefreshTables(context, rootTableName, serverTableKey, splitBehavior, provider);
            }

            // split table names
            if (context.Command?.OperationType == DataOperationType.Insert)
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(splitTableNames.IsNullOrEmpty(), $"Not assign split table for {entityConfig.EntityType.Name}");
                var diffTables = splitTableNames.Except(allTableNames);
                if (!diffTables.IsNullOrEmpty())
                {
                    allTableNames = RefreshTables(context, rootTableName, serverTableKey, splitBehavior, provider);
                    diffTables = splitTableNames.Except(allTableNames);
                }
                if (!diffTables.IsNullOrEmpty() && dataOptions.AutoCreateSplitTable)
                {
                    var createTableLock = SixnetLocker.GetCreateTableLock(entityConfig.EntityType);
                    if (createTableLock != null)
                    {
                        try
                        {
                            allTableNames = GetCachedTableNames(serverTableKey, rootTableName);
                            diffTables = splitTableNames.Except(allTableNames);
                            if (!diffTables.IsNullOrEmpty())
                            {
                                AutoCreateTables(context, rootTableName, serverTableKey, entityConfig, diffTables, splitBehavior, provider);
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            createTableLock.Value.Release();
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
                        allTableNames = RefreshTables(context, rootTableName, serverTableKey, splitBehavior, provider);
                    }
                    return allTableNames;
                }
                var diffTables = splitTableNames.Except(allTableNames);
                if (!diffTables.IsNullOrEmpty())
                {
                    allTableNames = RefreshTables(context, rootTableName, serverTableKey, splitBehavior, provider);
                    diffTables = splitTableNames.Except(allTableNames);
                }
                if (!diffTables.IsNullOrEmpty())
                {
                    splitTableNames = splitTableNames.Except(diffTables).ToList();
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
        /// Refresh tables
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rootTableName"></param>
        /// <param name="serverTableKey"></param>
        /// <returns></returns>
        static List<DatabaseObjectName> RefreshTables(DataCommandExecutionContext context, DatabaseObjectName rootTableName, string serverTableKey
            , SplitTableBehavior splitTableBehavior, ISixnetSplitTableProvider splitTableProvider)
        {
            List<DatabaseObjectName> allTableNames;
            using (var dataClient = GetClientForConnection(context.DatabaseConnection, true, true, false, context.DatabaseConnection.DataIsolationLevel))
            {
                allTableNames = dataClient.GetTables()?.Select(c => DatabaseObjectName.Create(c.Name, DatabaseObjectType.Table, rootTableName.SchemaName)).ToList();
            }
            allTableNames = splitTableProvider.FilterAllTableNames(new FilterAllSplitTableNameParameter()
            {
                AllTableNames = allTableNames,
                RootTableName = rootTableName,
                Behavior = splitTableBehavior
            }) ?? new List<DatabaseObjectName>(0);
            if (!allTableNames.IsNullOrEmpty())
            {
                var setAddParameter = new SetAddParameter()
                {
                    Key = serverTableKey,
                    Members = allTableNames.Select(c => c.Name).ToList()
                };
                HandleSplitTableCacheParameter(setAddParameter);
                SixnetCacher.Set.Add(setAddParameter);
            }
            return allTableNames ?? new List<DatabaseObjectName>(0);
        }

        /// <summary>
        /// Create tables
        /// </summary>
        /// <param name="context">Data command execution context</param>
        /// <param name="rootTableName">Root table name</param>
        /// <param name="serverTableKey">Server table key</param>
        /// <param name="newTableNames">New table names</param>
        /// <returns></returns>
        static void AutoCreateTables(DataCommandExecutionContext context, DatabaseObjectName rootTableName, string serverTableKey
            , EntityConfiguration entityConfig, IEnumerable<DatabaseObjectName> newTableNames, SplitTableBehavior splitTableBehavior
            , ISixnetSplitTableProvider splitTableProvider)
        {
            SixnetDirectThrower.ThrowArgNullIf(newTableNames.IsNullOrEmpty(), nameof(newTableNames));

            using (var dataClient = GetClientForConnection(context.DatabaseConnection, true, true, true, context.DatabaseConnection.DataIsolationLevel))
            {
                dataClient.Migrate(new MigrationInfo()
                {
                    NewTables = new List<NewTableInfo>()
                    {
                        new NewTableInfo()
                        {
                            TableNames = newTableNames?.ToList(),
                            EntityType = entityConfig.EntityType
                        }
                    }
                });
                dataClient.Commit();
            }
            RefreshTables(context, rootTableName, serverTableKey, splitTableBehavior, splitTableProvider);
        }

        /// <summary>
        /// Get default table name
        /// </summary>
        /// <param name="databaseType">Server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        internal static DatabaseObjectName GetDefaultTableName(DataCommandExecutionContext context
            , EntityConfiguration entityConfig, string defaultTableName = "")
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));
            SixnetDirectThrower.ThrowArgNullIf(entityConfig == null, nameof(entityConfig));

            var entityType = entityConfig.EntityType;
            var databaseType = context.DatabaseConnection?.DatabaseServer?.DatabaseType;
            var tableName = databaseType.HasValue
                ? GetEntitySetting(databaseType.Value, entityType)?.TableName
                : string.Empty;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = SixnetEntityManager.GetTableName(entityType);
            }
            if (string.IsNullOrWhiteSpace(tableName) && !string.IsNullOrWhiteSpace(defaultTableName))
            {
                tableName = defaultTableName;
            }
            var dataOptions = GetDataOptions();
            var schema = entityConfig.Schema;
            if (string.IsNullOrWhiteSpace(schema))
            {
                schema = dataOptions.GetDatabaseSchema(context, entityConfig);
            }
            return DatabaseObjectName.Create(tableName, DatabaseObjectType.Table, schema);
        }

        /// <summary>
        /// Get cached table names
        /// </summary>
        /// <param name="serverTableKey">Server table key</param>
        /// <returns></returns>
        static List<DatabaseObjectName> GetCachedTableNames(string serverTableKey, DatabaseObjectName rootTableName)
        {
            var setMembersParameter = new SetMembersParameter()
            {
                Key = serverTableKey
            };
            HandleSplitTableCacheParameter(setMembersParameter);
            return SixnetCacher.Set.Members(setMembersParameter)?.Members?
                .Select(m => DatabaseObjectName.Create(m, DatabaseObjectType.Table, rootTableName.SchemaName)).ToList()
                ?? new List<DatabaseObjectName>(0);
        }

        /// <summary>
        /// Handle split table cache parameter
        /// </summary>
        /// <param name="parameter"></param>
        static void HandleSplitTableCacheParameter(ISixnetCacheParameter parameter)
        {
            parameter.CacheObject = new CacheObject()
            {
                ObjectName = SixnetCacher.SplitTableCacheObjectName
            };
            parameter.UseInMemoryForDefault = true;
        }

        /// <summary>
        /// Get split table provider
        /// </summary>
        /// <param name="dataOptions"></param>
        /// <param name="entityConfiguration"></param>
        /// <returns></returns>
        internal static ISixnetSplitTableProvider GetSplitTableProvider(SixnetDataOptions dataOptions, EntityConfiguration entityConfiguration)
        {
            return dataOptions.GetSplitTableProvider(entityConfiguration.SplitTableProviderName)
                   ?? GetDefaultSplitTableProvider(entityConfiguration.SplitTableType);
        }

        /// <summary>
        /// Get default split table provider
        /// </summary>
        /// <param name="splitTableType">Split table type</param>
        /// <returns></returns>
        static ISixnetSplitTableProvider GetDefaultSplitTableProvider(SplitTableType splitTableType)
        {
            switch (splitTableType)
            {
                case SplitTableType.Year:
                case SplitTableType.Season:
                case SplitTableType.Month:
                case SplitTableType.Week:
                case SplitTableType.Day:
                    return _defaultDateSplitTableProvider;
                case SplitTableType.None:
                    break;
                case SplitTableType.Custom:
                    break;
            }
            return null;
        }

        /// <summary>
        /// Get database server split table cache key
        /// </summary>
        /// <param name="entityConfiguration"></param>
        /// <param name="databaseServer"></param>
        /// <returns></returns>
        static string GetDatabaseServerSplitTableCacheKey(EntityConfiguration entityConfiguration
            , DatabaseServer databaseServer
            , DatabaseObjectName rootTableName)
        {
            return $"{entityConfiguration.EntityType.Name}:{databaseServer.GetServerIdentityValue()}{(string.IsNullOrWhiteSpace(rootTableName.SchemaName) ? "" : $":{rootTableName.SchemaName}")}";
        }

        #endregion

        #region Entity fields

        /// <summary>
        ///  Get entity field
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="field">Field</param>
        /// <returns></returns>
        public static ISixnetField GetField(DatabaseType databaseType, Type entityType, ISixnetField field)
        {
            return field;
        }

        /// <summary>
        /// Get fields
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="fields">Original fields</param>
        /// <returns></returns>
        public static IEnumerable<ISixnetField> GetFields(DatabaseType databaseType, Type entityType, IEnumerable<ISixnetField> fields)
        {
            return fields ?? Array.Empty<ISixnetField>();
        }

        /// <summary>
        /// Get insertable fields
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static IEnumerable<DataField> GetInsertableFields(DatabaseType databaseType, Type entityType)
        {
            return SixnetEntityManager.GetEntityConfig(entityType)?.EditableFields ?? new List<DataField>(0);
        }

        /// <summary>
        /// Get queryable fields
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="includeNecessaryFields">Whether include necessary fields</param>
        /// <returns></returns>
        public static IEnumerable<ISixnetField> GetQueryableFields(DatabaseType databaseType, Type entityType
            , ISixnetQueryable queryable, bool includeNecessaryFields)
        {
            var queryFields = queryable.GetFields(entityType, includeNecessaryFields);
            return GetFields(databaseType, entityType, queryFields);
        }

        /// <summary>
        /// Get all queryable fields
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static IEnumerable<DataField> GetAllQueryableFields(DatabaseType databaseType, Type entityType)
        {
            return SixnetEntityManager.GetEntityConfig(entityType)?.QueryableFields;
        }

        #endregion

        #region Isolation level

        #region Get data isolation level

        /// <summary>
        /// Get data isolation level
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns>Return data isolation level</returns>
        internal static DataIsolationLevel? GetDataIsolationLevel(DatabaseType databaseType)
        {
            return GetDataOptions().GetDefaultIsolationLevel(databaseType);
        }

        #endregion

        #region Get system isolation level by data isolation level

        /// <summary>
        /// Get system isolation level by data isolation level
        /// </summary>
        /// <param name="dataIsolationLevel">Data isolation level</param>
        /// <returns>Return system data isolation level</returns>
        internal static IsolationLevel? GetSystemIsolationLevel(DataIsolationLevel? dataIsolationLevel)
        {
            return GetDataOptions().GetSystemIsolationLevel(dataIsolationLevel);
        }

        #endregion

        #endregion

        #region Field formatter

        /// <summary>
        /// Get field formatter
        /// </summary>
        /// <param name="formatterName">Formatter name</param>
        /// <returns>Return the field formatter</returns>
        internal static ISixnetFieldFormatter GetFieldFormatter(string formatterName)
        {
            return GetDataOptions().GetFieldFormatter(formatterName);
        }

        #endregion

        #region Parameter handler

        /// <summary>
        /// Get parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">Database data type</param>
        /// <returns>Return a parameter handler</returns>
        static ISixnetDataCommandParameterHandler GetParameterHandler(DatabaseType databaseType, DbType dbType)
        {
            return GetDataOptions().GetParameterHandler(databaseType, dbType);
        }

        /// <summary>
        /// Get parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return a parameter handler</returns>
        static ISixnetDataCommandParameterHandler GetParameterHandler(DatabaseType databaseType, DataCommandParameterItem parameter)
        {
            if (parameter != null)
            {
                var dbType = parameter.DbType;
                if (!dbType.HasValue && parameter.Value != null)
                {
                    var valueType = parameter.Value.GetType();
                    if (valueType != typeof(string) && parameter.Value is IEnumerable values)
                    {
                        foreach (var val in values)
                        {
                            valueType = val.GetType();
                            break;
                        }
                    }

#pragma warning disable CS0618
                    dbType = SqlMapper.LookupDbType(valueType, parameter.Name, false, out _);
#pragma warning restore CS0618
                }
                return GetParameterHandler(databaseType, dbType.GetValueOrDefault());
            }
            return null;
        }

        /// <summary>
        /// Handle parameter
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        internal static DataCommandParameterItem HandleParameter(DatabaseType databaseType, DataCommandParameterItem parameter)
        {
            var handler = GetParameterHandler(databaseType, parameter);
            return handler?.Parse(parameter) ?? parameter;
        }

        #endregion

        #region Data command event

        /// <summary>
        /// Trigger data command starting event
        /// </summary>
        /// <param name="dataCommand">Data command</param>
        /// <returns></returns>
        internal static void TriggerDataCommandStartingEvent(SixnetDataCommand dataCommand)
        {
            SixnetDirectThrower.ThrowArgNullIf(dataCommand == null, "Data command is null");
            var startingEvent = new SixnetDataCommandStartingEvent()
            {
                Command = dataCommand
            };

            // global event handler
            var globalHandlers = GetDataOptions().GetCommandStartingEventHandlers();
            if (!globalHandlers.IsNullOrEmpty())
            {
                foreach (var handler in globalHandlers)
                {
                    handler.Handle(startingEvent);
                }
            }

            // local event handler
            dataCommand.TriggerStartingEvent(startingEvent);
        }

        /// <summary>
        /// Trigger data command callback event
        /// </summary>
        /// <param name="dataCommand"></param>
        internal static void TriggerDataCommandCallbackEvent(SixnetDataCommand dataCommand)
        {
            SixnetDirectThrower.ThrowArgNullIf(dataCommand == null, nameof(dataCommand));

            var callbackEvent = new SixnetDataCommandCallbackEvent()
            {
                Command = dataCommand
            };

            // global event handler
            var globalHandlers = GetDataOptions().GetCommandCallbackEventHandlers();
            if (!globalHandlers.IsNullOrEmpty())
            {
                foreach (var asyncHandler in globalHandlers)
                {
                    _ = asyncHandler.ExecuteAsync(callbackEvent);
                }
            }

            // local event handler
            dataCommand.TriggerCallbackEvent(callbackEvent);
        }

        #endregion

        #region Client

        /// <summary>
        /// Get a data client
        /// </summary>
        /// <param name="autoOpen">Whether auto open database connection</param>
        /// <param name="useTransaction">Whether use transaction</param>
        /// <param name="isolationLevel">Data isolation level</param>
        /// <returns></returns>
        public static ISixnetDataClient GetClient(bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
        {
            return new DefaultDataClient(autoOpen, useTransaction, null, isolationLevel);
        }

        /// <summary>
        /// Get a data client for the default database server is configured
        /// </summary>
        /// <param name="autoOpen">Whether auto open database connection</param>
        /// <param name="useTransaction">Whether use transaction</param>
        /// <param name="isolationLevel">Data isolation level</param>
        /// <returns>New data client</returns>
        public static ISixnetDataClient GetDefaultClient(bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
        {
            var defaultServers = GetDefaultDatabaseServers();
            if (defaultServers.IsNullOrEmpty())
            {
                throw new SixnetException("The default database server is not set");
            }
            return GetClient(defaultServers, autoOpen, useTransaction, isolationLevel);
        }

        /// <summary>
        /// Get a data client for the all configured databases
        /// </summary>
        /// <param name="autoOpen">Whether auto open database connection</param>
        /// <param name="useTransaction">Whether use transaction</param>
        /// <param name="isolationLevel">Data isolation level</param>
        /// <returns>New data client</returns>
        public static ISixnetDataClient GetFullClient(bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
        {
            var servers = GetAllDatabaseServers();
            if (servers.IsNullOrEmpty())
            {
                throw new SixnetException("No database server is configured");
            }
            return GetClient(servers, autoOpen, useTransaction, isolationLevel);
        }

        /// <summary>
        /// Get a data client for the special database server name
        /// </summary>
        /// <param name="serverName">Configured database server name</param>
        /// <param name="autoOpen">Whether auto open database connection</param>
        /// <param name="useTransaction">Whether use transaction</param>
        /// <param name="isolationLevel">Data isolation level</param>
        /// <returns>New data client</returns>
        public static ISixnetDataClient GetClient(string serverName, bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
        {
            if (string.IsNullOrWhiteSpace(serverName))
            {
                throw new ArgumentNullException(nameof(serverName));
            }
            var servers = GetDatabaseServers(serverName);
            return GetClient(servers, autoOpen, useTransaction, isolationLevel);
        }

        /// <summary>
        /// Get a data client for the special database server names
        /// </summary>
        /// <param name="serverNames">Configured database server name</param>
        /// <param name="autoOpen">Whether auto open database connection</param>
        /// <param name="useTransaction">Whether use transaction</param>
        /// <param name="isolationLevel">Data isolation level</param>
        /// <returns>New data client</returns>
        public static ISixnetDataClient GetClient(IEnumerable<string> serverNames, bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
        {
            if (serverNames.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(serverNames));
            }
            var servers = GetDatabaseServers(serverNames);
            return GetClient(servers, autoOpen, useTransaction, isolationLevel);
        }

        /// <summary>
        /// Get data client
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="autoOpen">Whether auto open database connection</param>
        /// <param name="useTransaction">Whether use transaction</param>
        /// <param name="isolationLevel">Data isolation level</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ISixnetDataClient GetClient(DatabaseServer server, bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
        {
            return GetClient(new DatabaseServer[1] { server }, autoOpen, useTransaction, isolationLevel);
        }

        /// <summary>
        /// Get data client
        /// </summary>
        /// <param name="servers">servers</param>
        /// <param name="autoOpen">Whether auto open database connection</param>
        /// <param name="useTransaction">Whether use transaction</param>
        /// <param name="isolationLevel">Data isolation level</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">servers is null or empty</exception>
        public static ISixnetDataClient GetClient(IEnumerable<DatabaseServer> servers, bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
        {
            if (servers.IsNullOrEmpty())
            {
                throw new ArgumentNullException($"{nameof(servers)} is null or empty");
            }
            return new DefaultDataClient(autoOpen, useTransaction, servers, isolationLevel);
        }

        /// <summary>
        /// Get data client
        /// </summary>
        /// <param name="connection">Database connection</param>
        /// <param name="onlyForSingleConnection">Only for single connection</param>
        /// <param name="autoOpen">Auto open</param>
        /// <param name="useTransaction">Use transaction</param>
        /// <param name="isolationLevel">Isolation level</param>
        /// <returns></returns>
        internal static ISixnetDataClient GetClientForConnection(DatabaseConnection connection, bool onlyForSingleConnection = true, bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
        {
            if (onlyForSingleConnection && connection.DatabaseServer.UseSingleConnection())
            {
                return new DefaultDataClient(true, connection);
            }
            return GetClient(connection.DatabaseServer, autoOpen, useTransaction, isolationLevel);
        }

        #endregion

        #region Logical delete

        /// <summary>
        /// Whether allow logical delete
        /// </summary>
        /// <param name="dataOperationOptions">Data operation options</param>
        /// <returns></returns>
        internal static bool AllowLogicalDelete(SixnetDataOperationOptions dataOperationOptions)
        {
            var dataOptions = GetDataOptions();
            return dataOperationOptions?.AllowLogicalDelete(!(dataOptions?.DisableLogicalDelete ?? false))
                   ?? !(dataOptions?.DisableLogicalDelete ?? false);
        }

        #endregion

        #region Insert increment field

        /// <summary>
        /// Whether allow insert increment field
        /// </summary>
        /// <param name="commandExecutionContext">Command execution context</param>
        /// <returns></returns>
        public static bool AllowInsertIncrementField(DataCommandExecutionContext commandExecutionContext)
        {
            return GetDataOptions().AllowInsertIncrementField(commandExecutionContext);
        }

        #endregion

        #region Get data options

        /// <summary>
        /// Get data options
        /// </summary>
        /// <returns></returns>
        public static SixnetDataOptions GetDataOptions()
        {
            return SixnetContainer.GetOptions<SixnetDataOptions>();
        }

        #endregion

        #region Get entity setting

        /// <summary>
        /// Get entity setting
        /// </summary>
        /// <returns></returns>
        public static EntitySetting GetEntitySetting(DatabaseType databaseType, Type entityType)
        {
            return GetDataOptions()?.GetEntitySetting(databaseType, entityType);
        }

        #endregion

        #region Paging

        /// <summary>
        /// Gets the paging total field name
        /// </summary>
        /// <returns></returns>
        public static string GetPagingTotalFieldName()
        {
            return GetDataOptions().PagingTotalFieldName;
        }

        /// <summary>
        /// Gets the paging total split field name
        /// </summary>
        /// <returns></returns>
        public static string GetPagingTotalSplitFieldName()
        {
            return GetDataOptions().PagingTotalSplitFieldName;
        }

        /// <summary>
        /// Gets the default paging size
        /// </summary>
        /// <returns></returns>
        public static int GetDefaultPagingSize()
        {
            return GetDataOptions().DefaultPagingSize;
        }

        #endregion

        #region Batch setting

        /// <summary>
        /// Get batch setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        public static DatabaseBatchSetting GetBatchSetting(DatabaseType databaseType)
        {
            return GetDataOptions().GetBatchSetting(databaseType);
        }

        #endregion

        #region Format database object name

        /// <summary>
        /// Format database object name
        /// </summary>
        /// <param name="databaseType">Databae type</param>
        /// <param name="objectName">Database object name value</param>
        /// <returns></returns>
        public static DatabaseObjectName FormatDatabaseObjectName(DatabaseType databaseType, DatabaseObjectName objectName)
        {
            return GetDataOptions().FormatDatabaseObjectName(databaseType, objectName);
        }

        #endregion

        #region Database update

        /// <summary>
        /// Add database update record
        /// </summary>
        /// <param name="record"></param>
        public static void AddDatabaseUpdateRecord(ISixnetDatabaseUpdateRecord record)
        {
            if (record != null)
            {
                _updateRecords.TryGetValue(record.Version, out var records);
                records ??= [];
                records.Add(record);
                _updateRecords[record.Version] = records;
            }
        }

        /// <summary>
        /// Get database update records
        /// </summary>
        /// <returns></returns>
        public static SortedDictionary<Version, SortedSet<ISixnetDatabaseUpdateRecord>> GetDatabaseUpdateRecords()
        {
            return _updateRecords;
        }

        /// <summary>
        /// Update database
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static UpdateDatabaseResult UpdateDatabase(SixnetUpdateDatabaseParameter parameter)
        {
            var reportProcess = parameter.ReportProcess;
            try
            {
                reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Begin, parameter));

                reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.BeginGettingCurrentInfo, parameter));
                var recordRes = GetDatabaseUpdateRecordsCore(parameter);
                var context = recordRes.Item1;
                reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.EndGettingCurrentInfo, parameter, null, context.CurrentVersion, context.CurrentRecordId));

                if (recordRes.Item2.IsNullOrEmpty())
                {
                    reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.NoneRecords, parameter));
                }
                else
                {
                    foreach (var record in recordRes.Item2)
                    {
                        if (recordRes.Item3)
                        {
                            reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Update, parameter, record));
                            record.UpdateAsync(context).Wait();
                            reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.UpdateFinished, parameter, record));
                        }
                        else
                        {
                            reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Rollback, parameter, record));
                            record.RollbackAsync(context).Wait();
                            reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.RollbackFinished, parameter, record));
                        }
                    }
                }
                reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.End, parameter));
            }
            catch (Exception ex)
            {
                reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Error, parameter, ex: ex));
                throw;
            }

            return UpdateDatabaseResult.SuccessResult("");
        }

        /// <summary>
        /// Get database update records
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static List<ISixnetDatabaseUpdateRecord> GetDatabaseUpdateRecords(SixnetUpdateDatabaseParameter parameter)
        {
            return GetDatabaseUpdateRecordsCore(parameter).Item2;
        }

        /// <summary>
        /// Get database update records core
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        static Tuple<SixnetUpdateDatabaseContext, List<ISixnetDatabaseUpdateRecord>, bool> GetDatabaseUpdateRecordsCore(SixnetUpdateDatabaseParameter parameter)
        {
            SixnetDirectThrower.ThrowArgErrorIf(parameter?.DatabaseServer == null, "Database server is null");
            SixnetDirectThrower.ThrowArgErrorIf(parameter?.TargetVersion == null, "Target version is null");
            var currentVersion = new Version(0, 0, 0);
            var currentRecordId = 0L;
            using (var client = GetClient(parameter.DatabaseServer, false, true))
            {
                // create table
                client.CreateTable(typeof(SixnetAppUpdateRecordEntity));
                client.Commit();
                var lastRecordQueryable = SixnetQuerier.Create<SixnetAppUpdateRecordEntity>()
                    .OrderBy(c => c.Id, true);
                var lastRecord = client.QueryFirst<SixnetAppUpdateRecordEntity>(lastRecordQueryable);
                if (lastRecord != null)
                {
                    currentVersion = Version.Parse(lastRecord.AppVersion);
                    currentRecordId = lastRecord.Id;
                }
            }
            var context = new SixnetUpdateDatabaseContext()
            {
                UpdateParameter = parameter,
                CurrentVersion = currentVersion,
                CurrentRecordId = currentRecordId
            };
            if (currentVersion <= parameter.TargetVersion)
            {
                return new Tuple<SixnetUpdateDatabaseContext, List<ISixnetDatabaseUpdateRecord>, bool>(context, GetForwardRecords(context), true);
            }
            else
            {
                return new Tuple<SixnetUpdateDatabaseContext, List<ISixnetDatabaseUpdateRecord>, bool>(context, GetRollbackRecords(context), false);
            }
        }

        /// <summary>
        /// Get forward records
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static List<ISixnetDatabaseUpdateRecord> GetForwardRecords(SixnetUpdateDatabaseContext context)
        {
            if (_updateRecords.IsNullOrEmpty())
            {
                return new List<ISixnetDatabaseUpdateRecord>(0);
            }
            var targetVersion = context.UpdateParameter.TargetVersion;
            var currentVersion = context.CurrentVersion;
            var currentRecordId = context.CurrentRecordId;
            var reportProcess = context.UpdateParameter?.ReportProcess;
            var recordVersions = new SortedSet<Version>(_updateRecords.Keys);
            var minVersion = recordVersions.Min;
            var allRecords = new List<ISixnetDatabaseUpdateRecord>();

            // lower version
            if (currentVersion >= recordVersions.Min)
            {
                var nextRecord = SixnetEmptyDatabaseUpdateRecord.Create(currentRecordId + 1);
                var lowerVersions = recordVersions.GetViewBetween(recordVersions.Min, currentVersion);
                foreach (var lowerVersion in lowerVersions)
                {
                    _updateRecords.TryGetValue(lowerVersion, out var lowerVersionRecords);
                    if (lowerVersionRecords == null || lowerVersionRecords.Count < 1)
                    {
                        continue;
                    }
                    var maxRecord = lowerVersionRecords.Max;
                    if (maxRecord.Id < nextRecord.Id)
                    {
                        continue;
                    }
                    var updatableRecords = lowerVersionRecords.GetViewBetween(nextRecord, maxRecord);
                    foreach (var record in updatableRecords)
                    {
                        if (record.Version != lowerVersion || currentVersion < record.Version || currentRecordId >= record.Id)
                        {
                            reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.ErrorRecord, context.UpdateParameter, record));
                            continue;
                        }
                        allRecords.Add(record);
                    }
                }
            }

            // abover version
            var nextVersion = new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build, currentVersion.Revision + 1);
            if (nextVersion <= targetVersion)
            {
                var aboverVersions = recordVersions.GetViewBetween(nextVersion, targetVersion);
                if (!aboverVersions.IsNullOrEmpty())
                {
                    foreach (var aboverVersion in aboverVersions)
                    {
                        _updateRecords.TryGetValue(aboverVersion, out var aboverVersionRecords);
                        if (aboverVersionRecords == null || aboverVersionRecords.Count < 1)
                        {
                            continue;
                        }
                        foreach (var record in aboverVersionRecords)
                        {
                            if (record.Version != aboverVersion || record.Version > targetVersion || record.Version <= currentVersion)
                            {
                                reportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.ErrorRecord, context.UpdateParameter, record));
                                continue;
                            }
                            allRecords.Add(record);
                        }
                    }
                }
            }
            return allRecords;
        }

        /// <summary>
        /// Get rollback records
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        static List<ISixnetDatabaseUpdateRecord> GetRollbackRecords(SixnetUpdateDatabaseContext context)
        {
            if (_updateRecords.IsNullOrEmpty())
            {
                return new List<ISixnetDatabaseUpdateRecord>(0);
            }
            var targetVersion = context.UpdateParameter.TargetVersion;
            var currentVersion = context.CurrentVersion;
            var recordVersions = new SortedSet<Version>(_updateRecords.Keys);
            if (targetVersion >= currentVersion)
            {
                return new List<ISixnetDatabaseUpdateRecord>(0);
            }
            var endVersion = new Version(targetVersion.Major, targetVersion.Minor, targetVersion.Build, targetVersion.Revision + 1);
            if (endVersion > currentVersion)
            {
                return new List<ISixnetDatabaseUpdateRecord>(0);
            }
            var lowerVersions = recordVersions.GetViewBetween(endVersion, currentVersion).Reverse();
            var allRecords = new List<ISixnetDatabaseUpdateRecord>();
            foreach (var lowerVersion in lowerVersions)
            {
                _updateRecords.TryGetValue(lowerVersion, out var lowerVersionRecords);
                if (lowerVersionRecords == null || lowerVersionRecords.Count < 1)
                {
                    continue;
                }
                var reverseRecords = lowerVersionRecords.Reverse();
                foreach (var record in reverseRecords)
                {
                    allRecords.Add(record);
                }
            }
            return allRecords;
        }

        #endregion

        #region Timeout 

        /// <summary>
        /// Get command timeout
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static int? GetCommandTimeout(SixnetDataOperationOptions options)
        {
            var timeout = options?.Timeout;
            if (!timeout.HasValue || timeout.Value < 1)
            {
                timeout = GetDataOptions()?.DefaultCommandTimeout;
            }
            return timeout;
        }

        #endregion

        #endregion
    }
}
