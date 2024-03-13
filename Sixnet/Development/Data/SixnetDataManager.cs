using Sixnet.Cache;
using Sixnet.Cache.Keys.Parameters;
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
            List<DatabaseServer> servers = null;
            var dataOptions = GetDataOptions();
            if (dataOptions.GetDataCommandDatabaseServers == null)
            {
                switch (dataOptions.DatabaseServerMatchPattern)
                {
                    case DatabaseServerMatchPattern.Default:
                        servers = GetDefaultDatabaseServers();
                        break;
                    case DatabaseServerMatchPattern.All:
                        servers = GetAllDatabaseServers();
                        break;
                }
            }
            else
            {
                servers = dataOptions.GetDataCommandDatabaseServers.Invoke(command);
            }
            return servers ?? new List<DatabaseServer>(0);
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
            var options = GetDataOptions();
            return options.GetDatabaseConnection?.Invoke(server);
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
        /// Get default table name
        /// </summary>
        /// <param name="databaseType">Server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        static string GetDefaultTableName(DatabaseType? databaseType, Type entityType)
        {
            SixnetDirectThrower.ThrowArgNullIf(entityType == null, nameof(entityType));

            var tableName = databaseType.HasValue
                ? GetEntitySetting(databaseType.Value, entityType)?.TableName
                : string.Empty;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = SixnetEntityManager.GetTableName(entityType);
            }
            return tableName;
        }

        /// <summary>
        /// Gets data command table names
        /// </summary>
        /// <param name="context">Data command execution context</param>
        /// <returns></returns>
        internal static List<string> GetTableNames(DataCommandExecutionContext context)
        {
            var entityType = context.ActivityQueryable.GetModelType();
            entityType ??= (context?.Command?.GetEntityType());

            SixnetDirectThrower.ThrowArgNullIf(entityType == null, $"Entity type is null");

            var entityConfig = SixnetEntityManager.GetEntityConfig(entityType);
            if (entityConfig != null && entityConfig.SplitTableType != SplitTableType.None) // split table name
            {
                return GetSplitTableNames(context, entityConfig);
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
        internal static List<string> GetSplitTableNames(DataCommandExecutionContext context, EntityConfiguration entityConfig)
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));
            SixnetDirectThrower.ThrowArgNullIf(entityConfig == null, nameof(entityConfig));
            SixnetDirectThrower.ThrowNotSupportIf(entityConfig.SplitTableType == SplitTableType.None, $"{entityConfig.EntityType.Name} not support split table.");

            var splitProviderName = entityConfig.SplitTableProviderName ?? string.Empty;
            var dataOptions = GetDataOptions();
            var provider = dataOptions.GetSplitTableProvider(splitProviderName);
            provider ??= GetDefaultSplitTableProvider(entityConfig.SplitTableType);

            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"Not set split table provider for {entityConfig.SplitTableProviderName}");

            var splitBehavior = context.GetSplitTableBehavior() ?? _defaultSplitTableBehavior;
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

            var existParameter = new ExistParameter()
            {
                Keys = new List<CacheKey>() { serverTableKey }
            };
            HandleSplitTableCacheParameter(existParameter);
            var hasGotTables = SixnetCacher.Keys.Exist(existParameter)?.KeyCount == 1;

            List<string> allTableNames;
            if (!hasGotTables)
            {
                allTableNames = RefreshTables(context, rootTableName, serverTableKey);
            }
            else
            {
                allTableNames = GetCachedTableNames(serverTableKey);
            }

            if (context.Command?.OperationType == DataOperationType.Insert)
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(splitTableNames.IsNullOrEmpty(), $"Not assign split table for {entityConfig.EntityType.Name}");
                var diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                if (!diffTables.IsNullOrEmpty() && !hasGotTables)
                {
                    allTableNames = RefreshTables(context, rootTableName, serverTableKey);
                    diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                }
                if (!diffTables.IsNullOrEmpty() && dataOptions.AutoCreateSplitTable)
                {
                    var createTableLock = SixnetLocker.GetCreateTableLock(entityConfig.EntityType);
                    if (createTableLock != null)
                    {
                        try
                        {
                            allTableNames = GetCachedTableNames(serverTableKey);
                            diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                            if (!diffTables.IsNullOrEmpty())
                            {
                                AutoCreateTables(context, rootTableName, serverTableKey, entityConfig, diffTables);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
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
                    return allTableNames;
                }
                var diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                if (!diffTables.IsNullOrEmpty() && hasGotTables)
                {
                    allTableNames = RefreshTables(context, rootTableName, serverTableKey);
                    diffTables = splitTableNames.Except(allTableNames, _defaultDataTableNameComparer);
                }
                if (!diffTables.IsNullOrEmpty())
                {
                    splitTableNames = splitTableNames.Except(diffTables, _defaultDataTableNameComparer).ToList();
                }
                splitTableNames = provider.GetFinallySplitTableNames(splitBehavior, allTableNames, splitTableNames);
            }
            return splitTableNames;
        }

        /// <summary>
        /// Get cached table names
        /// </summary>
        /// <param name="serverTableKey">Server table key</param>
        /// <returns></returns>
        static List<string> GetCachedTableNames(string serverTableKey)
        {
            var setMembersParameter = new SetMembersParameter()
            {
                Key = serverTableKey
            };
            HandleSplitTableCacheParameter(setMembersParameter);
            return SixnetCacher.Set.Members(setMembersParameter)?.Members ?? new List<string>(0);
        }

        /// <summary>
        /// Refresh tables
        /// </summary>
        /// <param name="context"></param>
        /// <param name="rootTableName"></param>
        /// <param name="serverTableKey"></param>
        /// <returns></returns>
        static List<string> RefreshTables(DataCommandExecutionContext context, string rootTableName, string serverTableKey)
        {
            List<string> allTableNames;
            using (var dataClient = GetClientForConnection(context.DatabaseConnection, true, true, false, context.DatabaseConnection.DataIsolationLevel))
            {
                allTableNames = dataClient.GetTables()?.Select(c => c.TableName).ToList();
            }
            allTableNames = allTableNames?.Where(t => t.ToLower().StartsWith(rootTableName.ToLower()))
                .ToList() ?? new List<string>(0);
            if (!allTableNames.IsNullOrEmpty())
            {
                var setAddParameter = new SetAddParameter()
                {
                    Key = serverTableKey,
                    Members = allTableNames
                };
                HandleSplitTableCacheParameter(setAddParameter);
                SixnetCacher.Set.Add(setAddParameter);
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
        static void AutoCreateTables(DataCommandExecutionContext context, string rootTableName, string serverTableKey
            , EntityConfiguration entityConfig, IEnumerable<string> newTableNames)
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
            RefreshTables(context, rootTableName, serverTableKey);
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
        internal static bool AllowLogicalDelete(DataOperationOptions dataOperationOptions)
        {
            var dataOptijons = GetDataOptions();
            return !dataOptijons.DisableLogicalDelete && !(dataOperationOptions?.DisableLogicalDelete ?? false);
        }

        #endregion

        #region Get data options

        /// <summary>
        /// Get data options
        /// </summary>
        /// <returns></returns>
        public static DataOptions GetDataOptions()
        {
            return SixnetContainer.GetOptions<DataOptions>();
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

        #endregion
    }
}
