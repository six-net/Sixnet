using Sixnet.App;
using Sixnet.Cache;
using Sixnet.Cache.Keys.Options;
using Sixnet.Cache.Set.Options;
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
using Sixnet.Development.Data.ParameterHandler.Handler;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Serialization;
using Sixnet.Threading.Locking;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
            ConfigureDefaultParameterHandler();
            SubscribeDefaultDataCommandStartingEvent();
            Configure(SixnetContainer.GetOptions<DataOptions>());
        }

        #endregion

        #region Fields

        /// <summary>
        /// Get data command database servers func
        /// </summary>
        static Func<SixnetDataCommand, List<SixnetDatabaseServer>> GetDataCommandDatabaseServerFunc { get; set; } = null;

        /// <summary>
        /// Gets or sets get connection func
        /// </summary>
        static Func<SixnetDatabaseServer, IDbConnection> GetDatabaseConnectionFunc { get; set; } = null;

        /// <summary>
        /// Gets the database providers
        /// </summary>
        static Dictionary<DatabaseServerType, ISixnetDatabaseProvider> DatabaseProviders { get; } = new();

        /// <summary>
        /// servertype&entity table name
        /// key:{database server type}_{entity type id}
        /// value:table name
        /// </summary>
        static readonly Dictionary<string, string> DatabaseServerDefaultEntityTableNames = new();

        /// <summary>
        /// servertype&entity fields
        /// key:{database server type}_{entity type id}
        /// value:fields
        /// </summary>
        static readonly Dictionary<string, Dictionary<string, EntityField>> DatabaseServerEntityFields = new();

        /// <summary>
        /// Database server type&entity edit fields
        /// Key:{database server type}_{entity type id}
        /// Value:fields
        /// </summary>
        static readonly Dictionary<string, List<EntityField>> DatabaseServerEntityEditFields = new();

        /// <summary>
        /// Database server type&entity query fields
        /// Key:{database server type}_{entity type id}
        /// Value:fields
        /// </summary>
        static readonly Dictionary<string, List<EntityField>> DatabaseServerEntityQueryFields = new();

        /// <summary>
        /// Server type & entity format keys
        /// </summary>
        static readonly Dictionary<DatabaseServerType, Dictionary<Guid, string>> DatabaseServerEntityFormatKeys = new();

        /// <summary>
        /// Database server batch execute config
        /// </summary>
        static readonly Dictionary<DatabaseServerType, DatabaseBatchExecutionOptions> DatabaseServerExecuteConfigurations = new();

        /// <summary>
        /// Database server data isolation level collection
        /// </summary>
        static readonly Dictionary<DatabaseServerType, DataIsolationLevel> DatabaseServerDataIsolationLevels = new()
        {
            { DatabaseServerType.MySQL, DataIsolationLevel.RepeatableRead },
            { DatabaseServerType.SQLServer, DataIsolationLevel.ReadCommitted },
            { DatabaseServerType.Oracle, DataIsolationLevel.ReadCommitted }
        };

        /// <summary>
        /// System data isolation level map
        /// </summary>
        static readonly Dictionary<DataIsolationLevel, IsolationLevel> SystemDataIsolationLevelMaps = new()
        {
            { DataIsolationLevel.Chaos, IsolationLevel.Chaos },
            { DataIsolationLevel.ReadCommitted, IsolationLevel.ReadCommitted },
            { DataIsolationLevel.ReadUncommitted, IsolationLevel.ReadUncommitted },
            { DataIsolationLevel.RepeatableRead, IsolationLevel.RepeatableRead },
            { DataIsolationLevel.Serializable, IsolationLevel.Serializable },
            { DataIsolationLevel.Snapshot, IsolationLevel.Snapshot },
            { DataIsolationLevel.Unspecified, IsolationLevel.Unspecified }
        };

        /// <summary>
        /// Key: Field conversion name
        /// Field converters
        /// </summary>
        static readonly Dictionary<string, ISixnetFieldFormatter> FieldFormatterDict = new();

        /// <summary>
        /// Key=>DatabaseServerType+DbType
        /// Value=>IParameterHandler
        /// </summary>
        static readonly Dictionary<string, ISixnetDataCommandParameterHandler> ParameterHandlers = new();

        /// <summary>
        /// Get entity table name func collection
        /// </summary>
        static readonly Dictionary<Guid, Func<DataCommandExecutionContext, string>> GetEntityTableNameFuncCollection = new();

        /// <summary>
        /// Config database servers
        /// </summary>
        static readonly Dictionary<string, SixnetDatabaseServer> ConfigDatabaseServers = new();

        /// <summary>
        /// Gets the Paging total count field name
        /// </summary>
        public const string PagingTotalCountFieldName = "PagingTotalDataCount";

        /// <summary>
        /// Gets the paging total count split field name
        /// </summary>
        public const string PagingTotalCountSplitFieldName = "PagingTotalDataCountSplit";

        /// <summary>
        /// Default paging size
        /// </summary>
        public static int DefaultPagingSize = 20;

        /// <summary>
        /// Default data command database server source
        /// </summary>
        public static DefaultDataCommandDatabaseServerSource DefaultDataCommandDatabaseServerSource { get; set; } = DefaultDataCommandDatabaseServerSource.Default;

        /// <summary>
        /// Global data command starting event handlers
        /// </summary>
        static List<ISixnetDataCommandStartingEventHandler> GlobalDataCommandStartingEventHandlers;

        /// <summary>
        /// Global data command callback event handler
        /// </summary>
        static List<ISixnetDataCommandCallbackEventHandler> GlobalDataCommandCallbackEventHandlers;

        /// <summary>
        /// Whether disabled logical delete
        /// </summary>
        static bool DisabledLogicalDelete = false;

        /// <summary>
        /// Database tables
        /// Key: database server identity
        /// Value: database tables
        /// </summary>
        static readonly ConcurrentDictionary<string, SixnetDataTable> DatabaseTables = new();

        /// <summary>
        /// Key: split table provider name
        /// Value: Split table provider
        /// </summary>
        static readonly Dictionary<string, ISixnetSplitTableProvider> SplitTableProviders = new();

        /// <summary>
        /// Auto create split table
        /// </summary>
        static bool AutoCreateSplitTable = true;

        /// <summary>
        /// Date split table provider
        /// </summary>
        static readonly DefaultDateSplitTableProvider DateSplitTableProvider = new();

        /// <summary>
        /// All table split behavior
        /// </summary>
        static readonly SplitTableBehavior AllTableSplitBehavior = new();

        /// <summary>
        /// Default database table name comparer
        /// </summary>
        static readonly SixnetDataTableNameComparer DefaultDatabaseTableNameComparer = new();

        #endregion

        #region Methods

        #region Configure

        /// <summary>
        /// Configure database
        /// </summary>
        /// <param name="configure">Configure</param>
        static void Configure(Action<DataOptions> configure)
        {
            var dataOptions = new DataOptions();
            configure?.Invoke(dataOptions);
            Configure(dataOptions);
        }

        /// <summary>
        /// Configure data through json config
        /// </summary>
        /// <param name="jsonValues">Json values</param>
        static void Configure(params string[] jsonValues)
        {
            if (!jsonValues.IsNullOrEmpty())
            {
                foreach (var value in jsonValues)
                {
                    var dataOptions = SixnetJsonSerializer.Deserialize<DataOptions>(value);
                    Configure(dataOptions);
                }
            }
        }

        /// <summary>
        /// Configure data access through default configuration file
        /// </summary>
        /// <param name="configPath">Data access configuration root path</param>
        /// <param name="searchPattern">Config file extension</param>
        static void Configure(string configPath, string searchPattern = "*.sndbc")
        {
            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                return;
            }
            var rootPath = SixnetApplication.RootPath;
            if (string.IsNullOrWhiteSpace(configPath))
            {
                configPath = rootPath;
            }
            if (!Path.IsPathRooted(configPath))
            {
                configPath = Path.Combine(rootPath, configPath);
            }
            var files = Directory.GetFiles(configPath, searchPattern, SearchOption.AllDirectories);
            var jsonArray = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                var json = File.ReadAllText(files[i]);
                jsonArray[i] = json;
            }
            Configure(jsonArray);
        }

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="dataOptions">Data options</param>
        internal static void Configure(DataOptions dataOptions)
        {
            if (dataOptions == null)
            {
                return;
            }

            // servers
            if (!dataOptions.Servers.IsNullOrEmpty())
            {
                AddDatabaseServers(dataOptions.Servers);
            }

            // database
            if (!dataOptions.Databases.IsNullOrEmpty())
            {
                foreach (var database in dataOptions.Databases)
                {
                    if (database.Value == null)
                    {
                        continue;
                    }
                    // register database provider
                    if (!string.IsNullOrWhiteSpace(database.Value.DatabaseProviderFullTypeName))
                    {
                        var provider = (ISixnetDatabaseProvider)Activator.CreateInstance(Type.GetType(database.Value.DatabaseProviderFullTypeName));
                        AddDatabaseProvider(database.Key, provider);
                    }
                    // configure entity
                    if (!database.Value.Entities.IsNullOrEmpty())
                    {
                        foreach (var entityConfig in database.Value.Entities)
                        {
                            ConfigureDatabaseServerEntity(database.Key, entityConfig.Key, entityConfig.Value);
                        }
                    }
                    // batch execution
                    ConfigureBatchExecution(database.Key, database.Value.BatchExecution);

                    // data isolation level
                    if (database.Value.DataIsolationLevel.HasValue)
                    {
                        ConfigureDataIsolationLevel(database.Key, database.Value.DataIsolationLevel.Value);
                    }

                    // parameter hander
                    if (!database.Value.ParameterHandlers.IsNullOrEmpty())
                    {
                        foreach (var parameterHandlerItem in database.Value.ParameterHandlers)
                        {
                            AddParameterHandler(database.Key, parameterHandlerItem.Key, parameterHandlerItem.Value);
                        }
                    }
                    if (!database.Value.RemovedParameterHandlerDbTypes.IsNullOrEmpty())
                    {
                        foreach (var dbType in database.Value.RemovedParameterHandlerDbTypes)
                        {
                            RemoveParameterHandler(database.Key, dbType);
                        }
                    }
                }
            }

            // providers
            if (!dataOptions.DatabaseProviders.IsNullOrEmpty())
            {
                foreach (var provider in dataOptions.DatabaseProviders)
                {
                    AddDatabaseProvider(provider.Key, provider.Value);
                }
            }

            // field formatter
            if (!dataOptions.FieldFormatters.IsNullOrEmpty())
            {
                foreach (var formatterItem in dataOptions.FieldFormatters)
                {
                    AddFieldFormatter(formatterItem.Key, formatterItem.Value);
                }
            }

            // data command event
            if (!dataOptions.DataCommandStartingEventHandlers.IsNullOrEmpty())
            {
                SubscribeDataCommandStartingEvent(dataOptions.DataCommandStartingEventHandlers);
            }
            if (!dataOptions.DataCommandCallbackEventHandlers.IsNullOrEmpty())
            {
                SubscribeDataCommandCallbackEvent(dataOptions.DataCommandCallbackEventHandlers);
            }

            // logical delete
            if (dataOptions.DisableLogicalDelete)
            {
                DisableLogicalDelete();
            }

            // command server
            ConfigureDataCommandDatabaseServer(dataOptions.GetDataCommandDatabaseServerFunc);

            // database connection
            ConfigureDatabaseConnection(dataOptions.GetDatabaseConnectionFunc);
        }

        /// <summary>
        /// Configure server type entity
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityOptions">Entity options</param>
        static void ConfigureDatabaseServerEntity(DatabaseServerType serverType, Type entityType, DataEntityOptions entityOptions)
        {
            if (entityOptions != null)
            {
                //Configure object name
                ConfigureTableName(serverType, entityType, entityOptions.TableName);
                //Configure fields
                ConfigureFields(serverType, entityType, entityOptions.Fields);
            }
        }

        #endregion

        #region Database server

        /// <summary>
        /// Add database servers
        /// </summary>
        /// <param name="servers">Database servers</param>
        static void AddDatabaseServers(IEnumerable<SixnetDatabaseServer> servers)
        {
            if (!servers.IsNullOrEmpty())
            {
                foreach (var server in servers)
                {
                    if (server != null)
                    {
                        ConfigDatabaseServers[server.Name] = server;
                    }
                }
            }
        }

        /// <summary>
        /// Configure data command database server
        /// </summary>
        /// <param name="getDataCommandServerFunc"></param>
        static void ConfigureDataCommandDatabaseServer(Func<SixnetDataCommand, List<SixnetDatabaseServer>> getDataCommandServerFunc)
        {
            GetDataCommandDatabaseServerFunc = getDataCommandServerFunc;
        }

        /// <summary>
        /// Get data command database servers
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return database servers</returns>
        internal static List<SixnetDatabaseServer> GetDataCommandDatabaseServers(SixnetDataCommand command)
        {
            List<SixnetDatabaseServer> servers = null;
            if (GetDataCommandDatabaseServerFunc == null)
            {
                switch (DefaultDataCommandDatabaseServerSource)
                {
                    case DefaultDataCommandDatabaseServerSource.Default:
                        servers = GetDefaultDatabaseServers();
                        break;
                    case DefaultDataCommandDatabaseServerSource.All:
                        servers = GetAllDatabaseServers();
                        break;
                }
            }
            else
            {
                servers = GetDataCommandDatabaseServerFunc.Invoke(command);
            }
            return servers ?? new List<SixnetDatabaseServer>(0);
        }

        /// <summary>
        /// Get default database servers
        /// </summary>
        /// <returns></returns>
        public static List<SixnetDatabaseServer> GetDefaultDatabaseServers()
        {
            var allServers = ConfigDatabaseServers?.Values;
            if (allServers.IsNullOrEmpty())
            {
                return new List<SixnetDatabaseServer>(0);
            }
            var defaultServers = ConfigDatabaseServers?.Values.Where(c => c != null && c.Role == DatabaseServerRole.Default);
            if (defaultServers.IsNullOrEmpty() && allServers.Count == 1)
            {
                return new List<SixnetDatabaseServer>(1) { allServers.First() };
            }
            return defaultServers?.ToList() ?? new List<SixnetDatabaseServer>(0);
        }

        /// <summary>
        /// Get all config database servers
        /// </summary>
        /// <returns></returns>
        public static List<SixnetDatabaseServer> GetAllDatabaseServers()
        {
            return ConfigDatabaseServers?.Values.ToList() ?? new List<SixnetDatabaseServer>(0);
        }

        /// <summary>
        /// Get config database server by names
        /// </summary>
        /// <param name="databaseServerNames">Database server names</param>
        /// <returns></returns>
        public static List<SixnetDatabaseServer> GetDatabaseServers(params string[] databaseServerNames)
        {
            IEnumerable<string> serverNames = databaseServerNames;
            return GetDatabaseServers(serverNames);
        }

        /// <summary>
        /// Get config database server by names
        /// </summary>
        /// <param name="databaseServerNames">Database server names</param>
        /// <returns></returns>
        public static List<SixnetDatabaseServer> GetDatabaseServers(IEnumerable<string> databaseServerNames)
        {
            if (databaseServerNames.IsNullOrEmpty())
            {
                return new List<SixnetDatabaseServer>(0);
            }
            List<SixnetDatabaseServer> servers = new(databaseServerNames.GetCount());
            foreach (var name in databaseServerNames)
            {
                if (!ConfigDatabaseServers.TryGetValue(name, out var server))
                {
                    throw new SixnetException($"Database server {name} not found");
                }
                servers.Add(server);
            }
            return servers;
        }

        #endregion

        #region Database connection

        /// <summary>
        /// Configure database connection
        /// </summary>
        /// <param name="getDatabaseConnectionFunc">Get database server connection func</param>
        static void ConfigureDatabaseConnection(Func<SixnetDatabaseServer, IDbConnection> getDatabaseConnectionFunc)
        {
            GetDatabaseConnectionFunc = getDatabaseConnectionFunc;
        }

        /// <summary>
        /// Get database connection
        /// </summary>
        /// <param name="databaseServer">Database server</param>
        /// <returns></returns>
        public static IDbConnection GetDatabaseConnection(SixnetDatabaseServer databaseServer)
        {
            return GetDatabaseConnectionFunc?.Invoke(databaseServer);
        }

        #endregion

        #region Database provider

        /// <summary>
        /// Add database provider
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="databaseProvider">Database provider</param>
        static void AddDatabaseProvider(DatabaseServerType serverType, ISixnetDatabaseProvider databaseProvider)
        {
            if (databaseProvider != null)
            {
                DatabaseProviders[serverType] = databaseProvider;
                InitDatabaseAllEntityConfiguration(serverType);
            }
        }

        /// <summary>
        /// Get database provider
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return database provider</returns>
        internal static ISixnetDatabaseProvider GetDatabaseProvider(DatabaseServerType serverType)
        {
            DatabaseProviders.TryGetValue(serverType, out var databaseProvider);
            if (databaseProvider == null)
            {
                throw new SixnetException($"Not configured IDatabaseProvider for {serverType}");
            }
            return databaseProvider;
        }

        /// <summary>
        /// Init database entity configuration
        /// </summary>
        /// <param name="serverType">Server type</param>
        /// <param name="entityTypeId">Entity type</param>
        static void InitDatabaseEntityConfiguration(DatabaseServerType serverType, Guid entityTypeId)
        {
            var entityConfig = SixnetEntityManager.GetEntityConfiguration(entityTypeId);
            if (entityConfig == null)
            {
                return;
            }
            var formatKey = GetDatabaseServerEntityFormatKey(serverType, entityTypeId);
            if (!DatabaseServerEntityFields.TryGetValue(formatKey, out var serverEntityFields) || serverEntityFields.IsNullOrEmpty())
            {
                return;
            }
            //Queryable fields
            var queryableFields = new List<EntityField>(entityConfig.QueryableFields.Count);
            foreach (var field in entityConfig.QueryableFields)
            {
                if (serverEntityFields.TryGetValue(field, out var serverField) && serverField != null)
                {
                    if (!serverField.AllowBehavior(FieldBehavior.NotQuery))
                    {
                        queryableFields.Add(serverField);
                    }
                }
                else
                {
                    queryableFields.Add(field);
                }
            }
            DatabaseServerEntityQueryFields[formatKey] = queryableFields;
            //Edit fields
            var editFields = new List<EntityField>(entityConfig.EditableFields.Count);
            foreach (var field in entityConfig.EditableFields)
            {
                if (serverEntityFields.TryGetValue(field.PropertyName, out var serverField) && serverField != null)
                {
                    if (!serverField.AllowBehavior(FieldBehavior.NotInsert))
                    {
                        editFields.Add(serverField);
                    }
                }
                else
                {
                    editFields.Add(field);
                }
            }
            DatabaseServerEntityEditFields[formatKey] = editFields;
        }

        /// <summary>
        /// Init database all entity configuration
        /// </summary>
        /// <param name="serverType">Server type</param>
        static void InitDatabaseAllEntityConfiguration(DatabaseServerType serverType)
        {
            if (DatabaseServerEntityQueryFields.Any(c => c.Key.StartsWith(((int)serverType).ToString())))
            {
                return;
            }
            foreach (var configItem in SixnetEntityManager.EntityConfigurations)
            {
                InitDatabaseEntityConfiguration(serverType, configItem.Key);
            }
        }

        #endregion

        #region Table name

        /// <summary>
        /// Configure table name
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="tableName">Table name</param>
        static void ConfigureTableName(DatabaseServerType serverType, Type entityType, string tableName)
        {
            if (entityType == null || string.IsNullOrWhiteSpace(tableName))
            {
                return;
            }
            DatabaseServerDefaultEntityTableNames[GetDatabaseServerEntityFormatKey(serverType, entityType)] = tableName;
        }

        /// <summary>
        /// Get default table name
        /// </summary>
        /// <param name="serverType">Server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        static string GetDefaultTableName(DatabaseServerType? serverType, Type entityType)
        {
            SixnetDirectThrower.ThrowArgNullIf(entityType == null, nameof(entityType));

            var entityTableName = string.Empty;
            if (serverType.HasValue)
            {
                DatabaseServerDefaultEntityTableNames.TryGetValue(GetDatabaseServerEntityFormatKey(serverType.Value, entityType), out entityTableName);
            }
            if (string.IsNullOrWhiteSpace(entityTableName))
            {
                entityTableName = SixnetEntityManager.GetTableName(entityType);
            }
            return entityTableName;
        }

        /// <summary>
        /// Gets data command table names
        /// </summary>
        /// <param name="context">Data command execution context</param>
        /// <returns></returns>
        internal static List<string> GetTableNames(DataCommandExecutionContext context)
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
                return GetSplitTableNames(context, entityConfig);
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
        internal static List<string> GetSplitTableNames(DataCommandExecutionContext context, EntityConfiguration entityConfig)
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));
            SixnetDirectThrower.ThrowArgNullIf(entityConfig == null, nameof(entityConfig));
            SixnetDirectThrower.ThrowNotSupportIf(entityConfig.SplitTableType == SplitTableType.None, $"{entityConfig.EntityType.Name} not support split table.");
            var splitProviderName = entityConfig.SplitTableProviderName ?? string.Empty;
            if (!SplitTableProviders.TryGetValue(splitProviderName, out var provider))
            {
                provider = GetDefaultSplitTableProvider(entityConfig.SplitTableType);
            }
            SixnetDirectThrower.ThrowSixnetExceptionIf(provider == null, $"Not set split table provider for {entityConfig.SplitTableProviderName}");

            var splitBehavior = context.GetSplitTableBehavior() ?? AllTableSplitBehavior;
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

            var existOptions = new ExistOptions()
            {
                Keys = new List<CacheKey>() { serverTableKey }
            };
            HandleSplitTableCacheOptions(existOptions);
            var hasGotTables = SixnetCacher.Keys.Exist(existOptions)?.KeyCount == 1;

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
                var diffTables = splitTableNames.Except(allTableNames, DefaultDatabaseTableNameComparer);
                if (!diffTables.IsNullOrEmpty() && !hasGotTables)
                {
                    allTableNames = RefreshTables(context, rootTableName, serverTableKey);
                    diffTables = splitTableNames.Except(allTableNames, DefaultDatabaseTableNameComparer);
                }
                if (!diffTables.IsNullOrEmpty() && AutoCreateSplitTable)
                {
                    var createTableLock = SixnetLocker.GetCreateTableLock(entityConfig.EntityType);
                    if (createTableLock != null)
                    {
                        try
                        {
                            allTableNames = GetCachedTableNames(serverTableKey);
                            diffTables = splitTableNames.Except(allTableNames, DefaultDatabaseTableNameComparer);
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
                var diffTables = splitTableNames.Except(allTableNames, DefaultDatabaseTableNameComparer);
                if (!diffTables.IsNullOrEmpty() && hasGotTables)
                {
                    allTableNames = RefreshTables(context, rootTableName, serverTableKey);
                    diffTables = splitTableNames.Except(allTableNames, DefaultDatabaseTableNameComparer);
                }
                if (!diffTables.IsNullOrEmpty())
                {
                    splitTableNames = splitTableNames.Except(diffTables, DefaultDatabaseTableNameComparer).ToList();
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
            var setMembers = new SetMembersOptions()
            {
                Key = serverTableKey
            };
            HandleSplitTableCacheOptions(setMembers);
            return SixnetCacher.Set.Members(setMembers)?.Members ?? new List<string>(0);
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
                allTableNames = dataClient.GetTables()?.Select(c => c.Name).ToList();
            }
            allTableNames = allTableNames?.Where(t => t.ToLower().StartsWith(rootTableName.ToLower())).ToList() ?? new List<string>(0);
            if (!allTableNames.IsNullOrEmpty())
            {
                var setAddOptions = new SetAddOptions()
                {
                    Key = serverTableKey,
                    Members = allTableNames
                };
                HandleSplitTableCacheOptions(setAddOptions);
                SixnetCacher.Set.Add(setAddOptions);
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
        static void AutoCreateTables(DataCommandExecutionContext context, string rootTableName, string serverTableKey, EntityConfiguration entityConfig, IEnumerable<string> newTableNames)
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
                    return DateSplitTableProvider;
                case SplitTableType.None:
                    break;
                case SplitTableType.Custom:
                    break;
            }
            return null;
        }

        /// <summary>
        /// Handle split table cache options
        /// </summary>
        /// <param name="cacheOptions"></param>
        static void HandleSplitTableCacheOptions(ISixnetCacheOperationOptions cacheOptions)
        {
            cacheOptions.CacheObject = new CacheObject()
            {
                ObjectName = SixnetCacher.SplitTableCacheObjectName
            };
            cacheOptions.UseInMemoryForDefault = true;
        }

        #endregion

        #region Entity fields

        /// <summary>
        /// Configure entity fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="fields">Fields</param>
        static void ConfigureFields(DatabaseServerType serverType, Type entityType, IEnumerable<EntityField> fields)
        {
            if (entityType == null || fields.IsNullOrEmpty())
            {
                return;
            }
            var key = GetDatabaseServerEntityFormatKey(serverType, entityType);
            DatabaseServerEntityFields.TryGetValue(key, out var nowFields);
            nowFields ??= new Dictionary<string, EntityField>();
            foreach (var field in fields)
            {
                if (field != null)
                {
                    nowFields[field.PropertyName] = field;
                }
            }
            DatabaseServerEntityFields[key] = nowFields;
            InitDatabaseEntityConfiguration(serverType, entityType.GUID);
        }

        /// <summary>
        ///  Get entity field
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="field">Field</param>
        /// <returns></returns>
        public static ISixnetDataField GetField(DatabaseServerType serverType, Type entityType, ISixnetDataField field)
        {
            if (field is not PropertyField)
            {
                return field;
            }
            var fieldEntityType = field.GetModelType();
            if (fieldEntityType != null)
            {
                entityType = fieldEntityType;
            }
            var regularField = field as PropertyField;
            DatabaseServerEntityFields.TryGetValue(GetDatabaseServerEntityFormatKey(serverType, entityType), out var entityServerFields);
            EntityField serverField = null;
            entityServerFields?.TryGetValue(field.PropertyName, out serverField);
            serverField ??= SixnetEntityManager.GetField(entityType, field.PropertyName);

            regularField.FieldName = serverField.FieldName;
            regularField.FormatOption ??= serverField.FormatOption;

            return regularField;
        }

        /// <summary>
        ///  Get field
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="field">Field</param>
        /// <returns></returns>
        public static ISixnetDataField GetField(DatabaseServerType serverType, ISixnetQueryable queryable, ISixnetDataField field)
        {
            return GetField(serverType, queryable?.GetModelType(), field);
        }

        /// <summary>
        /// Get fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="fields">Original fields</param>
        /// <returns></returns>
        public static IEnumerable<ISixnetDataField> GetFields(DatabaseServerType serverType, Type entityType, IEnumerable<ISixnetDataField> fields)
        {
            if (fields.IsNullOrEmpty())
            {
                return Array.Empty<ISixnetDataField>();
            }
            var resultFields = new List<ISixnetDataField>();
            foreach (var field in fields)
            {
                var realField = GetField(serverType, entityType, field);
                if (realField != null)
                {
                    resultFields.Add(realField);
                }
            }
            return resultFields;
        }

        /// <summary>
        /// Get insertable fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static IEnumerable<EntityField> GetInsertableFields(DatabaseServerType serverType, Type entityType)
        {
            string formatKey = GetDatabaseServerEntityFormatKey(serverType, entityType);
            if (DatabaseServerEntityEditFields.TryGetValue(formatKey, out var editFields))
            {
                return editFields;
            }
            return SixnetEntityManager.GetEntityConfiguration(entityType)?.EditableFields ?? new List<EntityField>(0);
        }

        /// <summary>
        /// Get queryable fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="forceNecessaryFields">Whether include necessary fields</param>
        /// <returns></returns>
        public static IEnumerable<ISixnetDataField> GetQueryableFields(DatabaseServerType serverType, Type entityType, ISixnetQueryable queryable, bool forceNecessaryFields)
        {
            var queryFields = queryable.GetSelectedFields(entityType, forceNecessaryFields);
            return GetFields(serverType, entityType, queryFields);
        }

        /// <summary>
        /// Get all queryable fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static IEnumerable<PropertyField> GetAllQueryableFields(DatabaseServerType serverType, Type entityType)
        {
            var formatKey = GetDatabaseServerEntityFormatKey(serverType, entityType);
            if (!DatabaseServerEntityQueryFields.TryGetValue(formatKey, out var queryableFields) || queryableFields.IsNullOrEmpty())
            {
                queryableFields = SixnetEntityManager.GetEntityConfiguration(entityType)?.QueryableFields;
            }
            return queryableFields;
        }

        #endregion

        #region Batch execution

        #region Configure batch execution

        /// <summary>
        /// Configure batch execution
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="batchExecutionConfiguration">Batch execution configuration</param>
        static void ConfigureBatchExecution(DatabaseServerType serverType, DatabaseBatchExecutionOptions batchExecutionConfiguration)
        {
            if (batchExecutionConfiguration == null)
            {
                return;
            }
            DatabaseServerExecuteConfigurations[serverType] = batchExecutionConfiguration;
        }

        #endregion

        #region Get batch execute configuration

        /// <summary>
        /// Get batch execution options
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return batch execution configuration</returns>
        public static DatabaseBatchExecutionOptions GetBatchExecutionOptions(DatabaseServerType serverType)
        {
            DatabaseServerExecuteConfigurations.TryGetValue(serverType, out var config);
            return config;
        }

        #endregion

        #endregion

        #region Isolation level

        #region Configure data isolation level

        /// <summary>
        /// Configure data isolation level
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="dataIsolationLevel">Data isolation level</param>
        static void ConfigureDataIsolationLevel(DatabaseServerType serverType, DataIsolationLevel dataIsolationLevel)
        {
            DatabaseServerDataIsolationLevels[serverType] = dataIsolationLevel;
        }

        #endregion

        #region Get data isolation level

        /// <summary>
        /// Get data isolation level
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return data isolation level</returns>
        internal static DataIsolationLevel? GetDataIsolationLevel(DatabaseServerType serverType)
        {
            if (DatabaseServerDataIsolationLevels.ContainsKey(serverType))
            {
                return DatabaseServerDataIsolationLevels[serverType];
            }
            return null;
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
            IsolationLevel? isolationLevel = null;
            if (dataIsolationLevel.HasValue && SystemDataIsolationLevelMaps.ContainsKey(dataIsolationLevel.Value))
            {
                isolationLevel = SystemDataIsolationLevelMaps[dataIsolationLevel.Value];
            }
            return isolationLevel;
        }

        #endregion

        #endregion

        #region Field formatter

        /// <summary>
        /// Add field formatter
        /// </summary>
        /// <param name="formatterName">Formatter name</param>
        /// <param name="fieldFormatter">Field formatter</param>
        static void AddFieldFormatter(string formatterName, ISixnetFieldFormatter fieldFormatter)
        {
            if (string.IsNullOrWhiteSpace(formatterName) || fieldFormatter == null)
            {
                return;
            }
            FieldFormatterDict[formatterName] = fieldFormatter;
        }

        /// <summary>
        /// Get field formatter
        /// </summary>
        /// <param name="formatterName">Formatter name</param>
        /// <returns>Return the field formatter</returns>
        internal static ISixnetFieldFormatter GetFieldFormatter(string formatterName)
        {
            FieldFormatterDict.TryGetValue(formatterName, out var formatter);
            return formatter;
        }

        #endregion

        #region Parameter handler

        /// <summary>
        /// Add parameter handler
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="dbType">Database type</param>
        /// <param name="parameterHandler">Parameter handler</param>
        static void AddParameterHandler(DatabaseServerType databaseServerType, DbType dbType, ISixnetDataCommandParameterHandler parameterHandler)
        {
            var databaseTypeKey = GetDatabaseTypeFormatKey(databaseServerType, dbType);
            if (ParameterHandlers.ContainsKey(databaseTypeKey) && parameterHandler == null)
            {
                ParameterHandlers.Remove(databaseTypeKey);
            }
            else if (parameterHandler != null)
            {
                ParameterHandlers[databaseTypeKey] = parameterHandler;
            }
        }

        /// <summary>
        /// Remove parameter handler
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="dbType">Database data type</param>
        static void RemoveParameterHandler(DatabaseServerType databaseServerType, DbType dbType)
        {
            var databaseTypeKey = GetDatabaseTypeFormatKey(databaseServerType, dbType);
            if (ParameterHandlers.ContainsKey(databaseTypeKey))
            {
                ParameterHandlers.Remove(databaseTypeKey);
            }
        }

        /// <summary>
        /// Get parameter handler
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="dbType">Database data type</param>
        /// <returns>Return a parameter handler</returns>
        static ISixnetDataCommandParameterHandler GetParameterHandler(DatabaseServerType databaseServerType, DbType dbType)
        {
            var databaseTypeKey = GetDatabaseTypeFormatKey(databaseServerType, dbType);
            ParameterHandlers.TryGetValue(databaseTypeKey, out var handler);
            return handler;
        }

        /// <summary>
        /// Get parameter handler
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return a parameter handler</returns>
        static ISixnetDataCommandParameterHandler GetParameterHandler(DatabaseServerType databaseServerType, DataCommandParameterItem parameter)
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
                return GetParameterHandler(databaseServerType, dbType.GetValueOrDefault());
            }
            return null;
        }

        /// <summary>
        /// Handle parameter
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        internal static DataCommandParameterItem HandleParameter(DatabaseServerType databaseServerType, DataCommandParameterItem parameter)
        {
            var handler = GetParameterHandler(databaseServerType, parameter);
            return handler?.Parse(parameter) ?? parameter;
        }

        /// <summary>
        /// Get database type format key
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="dbType">Database type</param>
        /// <returns></returns>
        internal static string GetDatabaseTypeFormatKey(DatabaseServerType serverType, DbType dbType)
        {
            return string.Format("{0}_{1}", (int)serverType, (int)dbType);
        }

        /// <summary>
        /// Configure default parameter handler
        /// </summary>
        static void ConfigureDefaultParameterHandler()
        {
            var datetimeOffsetHandler = new DateTimeOffsetParameterHandler();
            var boolToIntegerHandler = new BooleanToIntegerParameterHandler();
            var guidHandler = new GuidFormattingParameterHandler();
            var sbyteHandler = new SByteToShortParameterHandler();
            var uintHandler = new UIntToLongParameterHandler();
            var ushortHandler = new UShortToIntParameterHandler();
            var ulongHandler = new ULongToDecimalParameterHandler();
            var timespanHandler = new TimeSpanParameterHandler();
            var charHandler = new CharToStringParameterHandler();
            var ulongToStringHandler = new ULongToStringParameterHandler();
            var nullCharHandler = new NullCharacterParameterHandler();

            #region MySQL

            //DateTimeOffset
            AddParameterHandler(DatabaseServerType.MySQL, DbType.DateTimeOffset, datetimeOffsetHandler);

            #endregion

            #region Oracle

            //boolean
            AddParameterHandler(DatabaseServerType.Oracle, DbType.Boolean, boolToIntegerHandler);
            //Guid
            AddParameterHandler(DatabaseServerType.Oracle, DbType.Guid, guidHandler);
            //SByte
            AddParameterHandler(DatabaseServerType.Oracle, DbType.SByte, sbyteHandler);
            //UInt
            AddParameterHandler(DatabaseServerType.Oracle, DbType.UInt32, uintHandler);
            //UShort
            AddParameterHandler(DatabaseServerType.Oracle, DbType.UInt16, ushortHandler);
            //ULong
            AddParameterHandler(DatabaseServerType.Oracle, DbType.UInt64, ulongHandler);
            //Timespan
            AddParameterHandler(DatabaseServerType.Oracle, DbType.Time, timespanHandler);
            //Char
            AddParameterHandler(DatabaseServerType.Oracle, DbType.StringFixedLength, charHandler);

            #endregion

            #region SQLite

            //DateTimeOffset
            AddParameterHandler(DatabaseServerType.SQLite, DbType.DateTimeOffset, datetimeOffsetHandler);
            //ULong
            AddParameterHandler(DatabaseServerType.SQLite, DbType.UInt64, ulongToStringHandler);

            #endregion

            #region SQL Server

            //SByte
            AddParameterHandler(DatabaseServerType.SQLServer, DbType.SByte, sbyteHandler);
            //UInt
            AddParameterHandler(DatabaseServerType.SQLServer, DbType.UInt32, uintHandler);
            //UShort
            AddParameterHandler(DatabaseServerType.SQLServer, DbType.UInt16, ushortHandler);
            //ULong
            AddParameterHandler(DatabaseServerType.SQLServer, DbType.UInt64, ulongHandler);

            #endregion

            #region PostgreSQL

            //UInt
            AddParameterHandler(DatabaseServerType.PostgreSQL, DbType.UInt32, uintHandler);
            //ULong
            AddParameterHandler(DatabaseServerType.PostgreSQL, DbType.UInt64, ulongHandler);
            //UShort
            AddParameterHandler(DatabaseServerType.PostgreSQL, DbType.UInt16, ushortHandler);
            //null char
            AddParameterHandler(DatabaseServerType.PostgreSQL, DbType.AnsiString, nullCharHandler);
            AddParameterHandler(DatabaseServerType.PostgreSQL, DbType.AnsiStringFixedLength, nullCharHandler);
            AddParameterHandler(DatabaseServerType.PostgreSQL, DbType.String, nullCharHandler);
            AddParameterHandler(DatabaseServerType.PostgreSQL, DbType.StringFixedLength, nullCharHandler);

            #endregion
        }

        #endregion

        #region Database server & Entity format key

        /// <summary>
        /// Generate servertype&entit format key
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityTypeId">Entity type</param>
        /// <returns>Return database server entity format key</returns>
        static string GenerateDatabaseServerEntityFormatKey(DatabaseServerType serverType, Guid entityTypeId)
        {
            return string.Format("{0}_{1}", (int)serverType, entityTypeId);
        }

        /// <summary>
        /// Get servertype&entity format key
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return database server entity format key</returns>
        static string GetDatabaseServerEntityFormatKey(DatabaseServerType serverType, Type entityType)
        {
            if (entityType == null)
            {
                return string.Empty;
            }
            return GetDatabaseServerEntityFormatKey(serverType, entityType.GUID);
        }

        /// <summary>
        /// Get servertype&entity format key
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityTypeId">Entity type id</param>
        /// <returns></returns>
        static string GetDatabaseServerEntityFormatKey(DatabaseServerType serverType, Guid entityTypeId)
        {
            string key = string.Empty;
            DatabaseServerEntityFormatKeys.TryGetValue(serverType, out var entityKeys);
            entityKeys?.TryGetValue(entityTypeId, out key);
            if (string.IsNullOrWhiteSpace(key))
            {
                key = GenerateDatabaseServerEntityFormatKey(serverType, entityTypeId);
                if (entityKeys == null)
                {
                    entityKeys = new Dictionary<Guid, string>();
                }
                entityKeys[entityTypeId] = key;
                DatabaseServerEntityFormatKeys[serverType] = entityKeys;
            }
            return key ?? string.Empty;
        }

        #endregion

        #region Data command event

        /// <summary>
        /// Subscribe default data command starting event
        /// </summary>
        static void SubscribeDefaultDataCommandStartingEvent()
        {
            GlobalDataCommandStartingEventHandlers ??= new List<ISixnetDataCommandStartingEventHandler>();
            GlobalDataCommandStartingEventHandlers.Add(new InterceptParameterDataCommandStartingHandler());
            GlobalDataCommandStartingEventHandlers.Add(new HandleQueryableDataCommandStartingEventHandler());
        }

        /// <summary>
        /// Subscribe data command starting event
        /// </summary>
        /// <param name="dataCommandStartingEventHandler">Data command starting event handler</param>
        static void SubscribeDataCommandStartingEvent(ISixnetDataCommandStartingEventHandler dataCommandStartingEventHandler)
        {
            if (dataCommandStartingEventHandler != null)
            {
                GlobalDataCommandStartingEventHandlers ??= new List<ISixnetDataCommandStartingEventHandler>();
                GlobalDataCommandStartingEventHandlers.Add(dataCommandStartingEventHandler);
            }
        }

        /// <summary>
        /// Subscribe data command starting event
        /// </summary>
        /// <param name="dataCommandStartingEventHandlers">Data command starting event handlers</param>
        static void SubscribeDataCommandStartingEvent(IEnumerable<ISixnetDataCommandStartingEventHandler> dataCommandStartingEventHandlers)
        {
            if (dataCommandStartingEventHandlers.IsNullOrEmpty())
            {
                return;
            }
            foreach (var handler in dataCommandStartingEventHandlers)
            {
                SubscribeDataCommandStartingEvent(handler);
            }
        }

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

            // global sync event handler
            if (!GlobalDataCommandStartingEventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in GlobalDataCommandStartingEventHandlers)
                {
                    handler.Handle(startingEvent);
                }
            }

            // local event handler
            dataCommand.TriggerStartingEvent(startingEvent);
        }

        /// <summary>
        /// Subscribe data command callback event
        /// </summary>
        /// <param name="dataCommandCallbackEventHandler">Data command callback event hander</param>
        static void SubscribeDataCommandCallbackEvent(ISixnetDataCommandCallbackEventHandler dataCommandCallbackEventHandler)
        {
            if (dataCommandCallbackEventHandler != null)
            {
                GlobalDataCommandCallbackEventHandlers ??= new List<ISixnetDataCommandCallbackEventHandler>();
                GlobalDataCommandCallbackEventHandlers.Add(dataCommandCallbackEventHandler);
            }
        }

        /// <summary>
        /// Subscribe data command callback event
        /// </summary>
        /// <param name="dataCommandCallbackEventHandlers">Data command callback event handlers</param>
        static void SubscribeDataCommandCallbackEvent(IEnumerable<ISixnetDataCommandCallbackEventHandler> dataCommandCallbackEventHandlers)
        {
            if (!dataCommandCallbackEventHandlers.IsNullOrEmpty())
            {
                foreach (var handler in dataCommandCallbackEventHandlers)
                {
                    SubscribeDataCommandCallbackEvent(handler);
                }
            }
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
            if (!GlobalDataCommandCallbackEventHandlers.IsNullOrEmpty())
            {
                foreach (var asyncHandler in GlobalDataCommandCallbackEventHandlers)
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
        public static ISixnetDataClient GetClient(SixnetDatabaseServer server, bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
        {
            return GetClient(new SixnetDatabaseServer[1] { server }, autoOpen, useTransaction, isolationLevel);
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
        public static ISixnetDataClient GetClient(IEnumerable<SixnetDatabaseServer> servers, bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
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
        internal static ISixnetDataClient GetClientForConnection(SixnetDatabaseConnection connection, bool onlyForSingleConnection = true, bool autoOpen = false, bool useTransaction = false, DataIsolationLevel? isolationLevel = null)
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
        /// Disable logical delete
        /// </summary>
        static void DisableLogicalDelete()
        {
            DisabledLogicalDelete = true;
        }

        /// <summary>
        /// Whether allow logical delete
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="dataOperationOptions">Data operation options</param>
        /// <returns></returns>
        internal static bool AllowLogicalDelete(DataOperationOptions dataOperationOptions)
        {
            return !DisabledLogicalDelete && !(dataOperationOptions?.DisableLogicalDelete ?? false);
        }

        #endregion

        #endregion
    }
}
