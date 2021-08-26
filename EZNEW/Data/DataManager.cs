using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using Dapper;
using EZNEW.Development.DataAccess;
using EZNEW.Data.CriteriaConverter;
using EZNEW.Development.Entity;
using EZNEW.Development.Query.Translator;
using EZNEW.Serialization;
using EZNEW.Data.Configuration;
using EZNEW.DependencyInjection;
using EZNEW.Development.Command;
using EZNEW.Development.Query;
using EZNEW.Application;
using EZNEW.Exceptions;
using EZNEW.Data.ParameterHandler;
using EZNEW.Data.TypeHandler;

namespace EZNEW.Data
{
    /// <summary>
    /// Data access manager
    /// </summary>
    public static class DataManager
    {
        static DataManager()
        {
            ApplicationInitializer.Init();
            ContainerManager.Container?.Register(typeof(ICommandExecutor), typeof(DatabaseCommandExecutor));
            SqlMapper.Settings.ApplyNullValues = true;
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
            ConfigureDefaultParameterHandler();
        }

        #region Properties

        /// <summary>
        /// Gets database servers delegate
        /// </summary>
        static Func<ICommand, List<DatabaseServer>> GetDatabaseServerDelegate { get; set; }

        /// <summary>
        /// Gets or sets get connection delegate
        /// </summary>
        static Func<DatabaseServer, IDbConnection> GetDatabaseConnectionDelegate { get; set; }

        /// <summary>
        /// Database server query translators
        /// </summary>
        static readonly Dictionary<DatabaseServerType, IQueryTranslator> DatabaseServerQueryTranslators = new Dictionary<DatabaseServerType, IQueryTranslator>();

        /// <summary>
        /// Gets the database providers
        /// </summary>
        static Dictionary<DatabaseServerType, IDatabaseProvider> DatabaseProviders { get; } = new Dictionary<DatabaseServerType, IDatabaseProvider>();

        /// <summary>
        /// servertype&entity object name
        /// key:{database server type}_{entity type id}
        /// value:object name
        /// </summary>
        static readonly Dictionary<string, string> DatabaseServerEntityObjectNames = new Dictionary<string, string>();

        /// <summary>
        /// servertype&entity fields
        /// key:{database server type}_{entity type id}
        /// value:fields
        /// </summary>
        static readonly Dictionary<string, Dictionary<string, EntityField>> DatabaseServerEntityFields = new Dictionary<string, Dictionary<string, EntityField>>();

        /// <summary>
        /// Database server type&entity edit fields
        /// Key:{database server type}_{entity type id}
        /// Value:fields
        /// </summary>
        static readonly Dictionary<string, List<EntityField>> DatabaseServerEntityEditFields = new Dictionary<string, List<EntityField>>();

        /// <summary>
        /// Database server type&entity query fields
        /// Key:{database server type}_{entity type id}
        /// Value:fields
        /// </summary>
        static readonly Dictionary<string, List<EntityField>> DatabaseServerEntityQueryFields = new Dictionary<string, List<EntityField>>();

        /// <summary>
        /// Server type & entity format keys
        /// </summary>
        static readonly Dictionary<DatabaseServerType, Dictionary<Guid, string>> DatabaseServerEntityFormatKeys = new Dictionary<DatabaseServerType, Dictionary<Guid, string>>();

        /// <summary>
        /// Database server batch execute config
        /// </summary>
        static readonly Dictionary<DatabaseServerType, BatchExecutionConfiguration> DatabaseServerExecuteConfigurations = new Dictionary<DatabaseServerType, BatchExecutionConfiguration>();

        /// <summary>
        /// Database server data isolation level collection
        /// </summary>
        static readonly Dictionary<DatabaseServerType, DataIsolationLevel> DatabaseServerDataIsolationLevels = new Dictionary<DatabaseServerType, DataIsolationLevel>()
            {
                { DatabaseServerType.MySQL,DataIsolationLevel.RepeatableRead },
                { DatabaseServerType.SQLServer,DataIsolationLevel.ReadCommitted },
                { DatabaseServerType.Oracle,DataIsolationLevel.ReadCommitted }
            };

        /// <summary>
        /// System data isolation level map
        /// </summary>
        static readonly Dictionary<DataIsolationLevel, IsolationLevel> SystemDataIsolationLevelMaps = new Dictionary<DataIsolationLevel, IsolationLevel>()
            {
                { DataIsolationLevel.Chaos,IsolationLevel.Chaos },
                { DataIsolationLevel.ReadCommitted,IsolationLevel.ReadCommitted },
                { DataIsolationLevel.ReadUncommitted,IsolationLevel.ReadUncommitted },
                { DataIsolationLevel.RepeatableRead,IsolationLevel.RepeatableRead },
                { DataIsolationLevel.Serializable,IsolationLevel.Serializable },
                { DataIsolationLevel.Snapshot,IsolationLevel.Snapshot },
                { DataIsolationLevel.Unspecified,IsolationLevel.Unspecified }
            };

        /// <summary>
        /// criteria convert parse config
        /// </summary>
        static readonly Dictionary<string, Func<CriteriaConverterParseOptions, string>> CriteriaConverterParsers = new Dictionary<string, Func<CriteriaConverterParseOptions, string>>();

        /// <summary>
        /// Key=>DatabaseServerType+DbType
        /// Value=>IParameterHandler
        /// </summary>
        private static readonly Dictionary<string, IParameterHandler> ParameterHandlers = new Dictionary<string, IParameterHandler>();

        /// <summary>
        /// Gets the Paging total conut field name
        /// </summary>
        public const string PagingTotalCountFieldName = "QueryDataTotalCount";

        /// <summary>
        /// Data options
        /// </summary>
        public static DataOptions DataOptions { get; private set; } = new DataOptions();

        #endregion

        #region Methods

        #region Configure

        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="configureDelegate">Configure delegate</param>
        public static void Configure(Action<DataOptions> configureDelegate)
        {
            configureDelegate?.Invoke(DataOptions);
        }

        #endregion

        #region Configure database

        /// <summary>
        /// Configure database
        /// </summary>
        /// <param name="configuration">Database configuration</param>
        public static void ConfigureDatabase(DatabaseConfiguration configuration)
        {
            if (configuration == null || configuration.Servers == null || configuration.Servers.Count <= 0)
            {
                return;
            }
            foreach (var serverItem in configuration.Servers)
            {
                if (serverItem.Value == null)
                {
                    continue;
                }
                //register database provider
                if (!string.IsNullOrWhiteSpace(serverItem.Value.DatabaseProviderFullTypeName))
                {
                    IDatabaseProvider provider = (IDatabaseProvider)Activator.CreateInstance(Type.GetType(serverItem.Value.DatabaseProviderFullTypeName));
                    ConfigureDatabaseProvider(serverItem.Key, provider);
                }
                //configure entity
                if (!serverItem.Value.EntityConfigurations.IsNullOrEmpty())
                {
                    foreach (var entityConfig in serverItem.Value.EntityConfigurations)
                    {
                        ConfigureDatabaseServerEntity(serverItem.Key, entityConfig.Key, entityConfig.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Configure data access through json data
        /// </summary>
        /// <param name="json">Json data</param>
        public static void ConfigureDatabase(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }
            var dataConfig = JsonSerializer.Deserialize<DatabaseConfiguration>(json);
            if (dataConfig == null)
            {
                return;
            }
            ConfigureDatabase(dataConfig);
        }

        /// <summary>
        /// Configure data access through default configuration file
        /// </summary>
        /// <param name="configPath">Data access configuration root path</param>
        public static void ConfigureDatabaseByConfigFile(string configPath = "")
        {
            if (string.IsNullOrWhiteSpace(configPath))
            {
                configPath = ApplicationManager.RootPath;
            }
            InitFolderConfiguration(configPath);
        }

        /// <summary>
        /// Init folder configuration
        /// </summary>
        /// <param name="path">Folder path</param>
        static void InitFolderConfiguration(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return;
            }
            var files = Directory.GetFiles(path, "*.daconfig", SearchOption.AllDirectories);
            if (!files.IsNullOrEmpty())
            {
                foreach (var file in files)
                {
                    var fileData = File.ReadAllText(file);
                    ConfigureDatabase(fileData);
                }
            }
        }

        /// <summary>
        /// Configure server type entity
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityConfiguration">Entity configuration</param>
        static void ConfigureDatabaseServerEntity(DatabaseServerType serverType, Type entityType, DataEntityConfiguration entityConfiguration)
        {
            if (entityConfiguration == null)
            {
                return;
            }
            //Configure object name
            ConfigureEntityObjectName(serverType, entityType, entityConfiguration.TableName);
            //Configure fields
            ConfigureEntityFields(serverType, entityType, entityConfiguration.Fields);
        }

        #endregion

        #region Database server

        /// <summary>
        /// Configure database server
        /// </summary>
        /// <param name="getDatabaseServerOperation">Get database server operation</param>
        public static void ConfigureDatabaseServer(Func<ICommand, List<DatabaseServer>> getDatabaseServerOperation)
        {
            GetDatabaseServerDelegate = getDatabaseServerOperation;
        }

        /// <summary>
        /// Get database servers
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return database servers</returns>
        public static List<DatabaseServer> GetDatabaseServers(ICommand command)
        {
            return GetDatabaseServerDelegate?.Invoke(command) ?? new List<DatabaseServer>(0);
        }

        #endregion

        #region Query translator

        /// <summary>
        /// Configure query translator
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="queryTranslator"></param>
        public static void ConfigureQueryTranslator(DatabaseServerType serverType, IQueryTranslator queryTranslator)
        {
            DatabaseServerQueryTranslators[serverType] = queryTranslator;
        }

        /// <summary>
        /// Get query translator
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return query translator</returns>
        public static IQueryTranslator GetQueryTranslator(DatabaseServerType serverType)
        {
            DatabaseServerQueryTranslators.TryGetValue(serverType, out var queryTranslator);
            return queryTranslator;
        }

        #endregion

        #region Database connection

        /// <summary>
        /// Configure connection
        /// </summary>
        /// <param name="getDatabaseConnectionOperation">Get database connection operation</param>
        public static void ConfigureDatabaseConnection(Func<DatabaseServer, IDbConnection> getDatabaseConnectionOperation)
        {
            GetDatabaseConnectionDelegate = getDatabaseConnectionOperation;
        }

        /// <summary>
        /// Get database connection
        /// </summary>
        /// <param name="databaseServer">Database server</param>
        /// <returns></returns>
        public static IDbConnection GetDatabaseConnection(DatabaseServer databaseServer)
        {
            return GetDatabaseConnectionDelegate?.Invoke(databaseServer);
        }

        #endregion

        #region Database provider

        /// <summary>
        /// Configure database provider
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="databaseProvider">Database provider</param>
        public static void ConfigureDatabaseProvider(DatabaseServerType serverType, IDatabaseProvider databaseProvider)
        {
            if (databaseProvider == null)
            {
                return;
            }
            DatabaseProviders[serverType] = databaseProvider;
            InitDatabaseAllEntityConfiguration(serverType);
        }

        /// <summary>
        /// Get database provider
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return database provider</returns>
        public static IDatabaseProvider GetDatabaseProvider(DatabaseServerType serverType)
        {
            DatabaseProviders.TryGetValue(serverType, out var databaseProvider);
            return databaseProvider;
        }

        /// <summary>
        /// Init database entity configuration
        /// </summary>
        /// <param name="serverType">Server type</param>
        /// <param name="entityTypeId">Entity type</param>
        static void InitDatabaseEntityConfiguration(DatabaseServerType serverType, Guid entityTypeId)
        {
            var entityConfig = EntityManager.GetEntityConfiguration(entityTypeId);
            if (entityConfig == null)
            {
                return;
            }
            var formatKey = GetDatabaseServerEntityFormatKey(serverType, entityTypeId);
            if (!DatabaseServerEntityFields.TryGetValue(formatKey, out var serverEntityFields) || serverEntityFields.IsNullOrEmpty())
            {
                return;
            }
            //Query fields
            var queryFields = new List<EntityField>(entityConfig.QueryFields.Count);
            foreach (var field in entityConfig.QueryFields)
            {
                if (serverEntityFields.TryGetValue(field, out var serverField) && serverField != null)
                {
                    if (serverField.IsDisableQuery)
                    {
                        continue;
                    }
                    queryFields.Add(serverField);
                }
                else
                {
                    queryFields.Add(field);
                }
            }
            DatabaseServerEntityQueryFields[formatKey] = queryFields;
            //Edit fields
            var editFields = new List<EntityField>(entityConfig.EditFields.Count);
            foreach (var field in entityConfig.EditFields)
            {
                if (serverEntityFields.TryGetValue(field.PropertyName, out var serverField) && serverField != null)
                {
                    if (serverField.IsDisableEdit)
                    {
                        continue;
                    }
                    editFields.Add(serverField);
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
            foreach (var configItem in EntityManager.EntityConfigurations)
            {
                InitDatabaseEntityConfiguration(serverType, configItem.Key);
            }
        }

        #endregion

        #region Entity object name

        /// <summary>
        /// Configure entity object name
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="objectName">Object name</param>
        public static void ConfigureEntityObjectName(DatabaseServerType serverType, Type entityType, string objectName)
        {
            if (entityType == null || string.IsNullOrWhiteSpace(objectName))
            {
                return;
            }
            DatabaseServerEntityObjectNames[GetDatabaseServerEntityFormatKey(serverType, entityType)] = objectName;
        }

        /// <summary>
        /// Get entity object name
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="defaultName">Default name</param>
        /// <returns>Return Entity object name</returns>
        public static string GetEntityObjectName(DatabaseServerType serverType, Type entityType, string defaultName = "")
        {
            DatabaseServerEntityObjectNames.TryGetValue(GetDatabaseServerEntityFormatKey(serverType, entityType), out var objectName);
            if (!string.IsNullOrWhiteSpace(objectName))
            {
                return objectName;
            }
            objectName = EntityManager.GetEntityObjectName(entityType);
            return string.IsNullOrWhiteSpace(objectName) ? defaultName : objectName;
        }

        /// <summary>
        /// Get query object name
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="query">Query object</param>
        /// <returns>Return query relation entity object name</returns>
        public static string GetQueryRelationObjectName(DatabaseServerType serverType, IQuery query)
        {
            var entityType = query?.GetEntityType();
            if (query == null || entityType == null)
            {
                return string.Empty;
            }
            return GetEntityObjectName(serverType, entityType);
        }

        #endregion

        #region Entity field

        /// <summary>
        /// Configure entity fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="fields">Fields</param>
        public static void ConfigureEntityFields(DatabaseServerType serverType, Type entityType, IEnumerable<EntityField> fields)
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
                if (field == null)
                {
                    continue;
                }
                nowFields[field.PropertyName] = field;
            }
            DatabaseServerEntityFields[key] = nowFields;
            InitDatabaseEntityConfiguration(serverType, entityType.GUID);
        }

        /// <summary>
        ///  Get field
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return property relation entity field</returns>
        public static EntityField GetField(DatabaseServerType serverType, Type entityType, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }
            EntityField field = null;
            DatabaseServerEntityFields.TryGetValue(GetDatabaseServerEntityFormatKey(serverType, entityType), out var entityServerFields);
            entityServerFields?.TryGetValue(propertyName, out field);
            if (field == null)
            {
                EntityManager.GetEntityField(entityType, propertyName);
            }
            return field ?? propertyName;
        }

        /// <summary>
        ///  Get field
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="query">Query object</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return property relation entity field</returns>
        public static EntityField GetField(DatabaseServerType serverType, IQuery query, string propertyName)
        {
            var entityType = query?.GetEntityType();
            if (query == null || entityType == null)
            {
                return propertyName;
            }
            return GetField(serverType, entityType, propertyName);
        }

        /// <summary>
        /// Get fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="propertyNames">Property names</param>
        /// <returns>Return properties relation entity fields</returns>
        public static IEnumerable<EntityField> GetFields(DatabaseServerType serverType, Type entityType, IEnumerable<string> propertyNames)
        {
            if (propertyNames.IsNullOrEmpty())
            {
                return Array.Empty<EntityField>();
            }
            var allFields = EntityManager.GetEntityConfiguration(entityType)?.AllFields;
            if (allFields.IsNullOrEmpty())
            {
                return Array.Empty<EntityField>();
            }
            DatabaseServerEntityFields.TryGetValue(GetDatabaseServerEntityFormatKey(serverType, entityType), out var entityServerFields);
            List<EntityField> resultFields = new List<EntityField>();
            foreach (var propertyName in propertyNames)
            {
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    continue;
                }
                EntityField propertyField = null;
                entityServerFields?.TryGetValue(propertyName, out propertyField);
                if (propertyField == null)
                {
                    allFields.TryGetValue(propertyName, out propertyField);
                }
                if (propertyField != null)
                {
                    resultFields.Add(propertyField);
                }
            }
            return resultFields;
        }

        /// <summary>
        /// Get edit fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity edit fields</returns>
        public static IEnumerable<EntityField> GetEditFields(DatabaseServerType serverType, Type entityType)
        {
            string formatKey = GetDatabaseServerEntityFormatKey(serverType, entityType);
            if (DatabaseServerEntityEditFields.TryGetValue(formatKey, out var editFields))
            {
                return editFields;
            }
            return EntityManager.GetEntityConfiguration(entityType)?.EditFields ?? new List<EntityField>(0);
        }

        /// <summary>
        /// Get query fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="query">Query object</param>
        /// <param name="forceMustFields">Whether return the must query fields</param>
        /// <returns>Return entity fields</returns>
        public static IEnumerable<EntityField> GetQueryFields(DatabaseServerType serverType, Type entityType, IQuery query, bool forceMustFields)
        {
            var queryFieldsWithSign = query.GetActuallyQueryFieldsWithSign(entityType, forceMustFields);
            if (queryFieldsWithSign.Item1)
            {
                return GetAllQueryFields(serverType, entityType);
            }
            return GetFields(serverType, entityType, queryFieldsWithSign.Item2);
        }

        /// <summary>
        /// Get all query fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return all query fields</returns>
        public static IEnumerable<EntityField> GetAllQueryFields(DatabaseServerType serverType, Type entityType)
        {
            var formatKey = GetDatabaseServerEntityFormatKey(serverType, entityType);
            if (!DatabaseServerEntityQueryFields.TryGetValue(formatKey, out var queryFields) || queryFields.IsNullOrEmpty())
            {
                queryFields = EntityManager.GetEntityConfiguration(entityType)?.QueryEntityFields;
            }
            return queryFields;
        }

        /// <summary>
        /// Get default field
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return default field</returns>
        public static EntityField GetDefaultField(DatabaseServerType serverType, Type entityType)
        {
            var entityConfig = EntityManager.GetEntityConfiguration(entityType);
            var primaryKeys = entityConfig?.PrimaryKeys;
            if (primaryKeys.IsNullOrEmpty())
            {
                primaryKeys = entityConfig?.QueryFields;
            }
            if (primaryKeys.IsNullOrEmpty())
            {
                return null;
            }
            var defaultField = primaryKeys[0];
            return GetField(serverType, entityType, defaultField);
        }

        #endregion

        #region Batch execution configuration

        #region Configure batch execute

        /// <summary>
        /// Configure batch execution
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="batchExecuteConfig">Batch execution configuration</param>
        public static void ConfigureBatchExecution(DatabaseServerType serverType, BatchExecutionConfiguration batchExecuteConfig)
        {
            if (batchExecuteConfig == null)
            {
                return;
            }
            DatabaseServerExecuteConfigurations[serverType] = batchExecuteConfig;
        }

        #endregion

        #region Get batch execute configuration

        /// <summary>
        /// Get batch execution configuration
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return batch execution configuration</returns>
        public static BatchExecutionConfiguration GetBatchExecutionConfiguration(DatabaseServerType serverType)
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
        public static void ConfigureDataIsolationLevel(DatabaseServerType serverType, DataIsolationLevel dataIsolationLevel)
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
        public static DataIsolationLevel? GetDataIsolationLevel(DatabaseServerType serverType)
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
        public static IsolationLevel? GetSystemIsolationLevel(DataIsolationLevel? dataIsolationLevel)
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

        #region Criteria converter

        /// <summary>
        /// Configure criteria converter parser
        /// </summary>
        /// <param name="converterConfigName">Converter config name</param>
        /// <param name="converterParseOperation">Converter parse operation</param>
        public static void ConfigureCriteriaConverterParser(string converterConfigName, Func<CriteriaConverterParseOptions, string> converterParseOperation)
        {
            if (string.IsNullOrWhiteSpace(converterConfigName) || converterParseOperation == null)
            {
                return;
            }
            CriteriaConverterParsers[converterConfigName] = converterParseOperation;
        }

        /// <summary>
        /// Get criteria converter parser
        /// </summary>
        /// <param name="converterConfigName">Converter config name</param>
        /// <returns>Return convert parse operation</returns>
        public static Func<CriteriaConverterParseOptions, string> GetCriteriaConverterParser(string converterConfigName)
        {
            CriteriaConverterParsers.TryGetValue(converterConfigName, out var parse);
            return parse;
        }

        #endregion

        #region Parameter handler

        /// <summary>
        /// Add parameter handler
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="dbType">Database type</param>
        /// <param name="parameterHandler">Parameter handler</param>
        public static void AddParameterHandler(DatabaseServerType databaseServerType, DbType dbType, IParameterHandler parameterHandler)
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
        /// Add parameter handler
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="dbType">Database data type</param>
        /// <param name="parameterHandleDelegate">Parameter handle delegate</param>
        public static void AddParameterHandler(DatabaseServerType databaseServerType, DbType dbType, Func<ParameterItem, ParameterItem> parameterHandleDelegate)
        {
            IParameterHandler parameterHandler = null;
            if (parameterHandleDelegate != null)
            {
                parameterHandler = new DefaultParameterHandler(parameterHandleDelegate);
            }
            AddParameterHandler(databaseServerType, dbType, parameterHandler);
        }

        /// <summary>
        /// Remove parameter handler
        /// </summary>
        /// <param name="databaseServerType">Database server type</param>
        /// <param name="dbType">Database data type</param>
        public static void RemoveParameterHandler(DatabaseServerType databaseServerType, DbType dbType)
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
        public static IParameterHandler GetParameterHandler(DatabaseServerType databaseServerType, DbType dbType)
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
        public static IParameterHandler GetParameterHandler(DatabaseServerType databaseServerType, ParameterItem parameter)
        {
            if (parameter != null)
            {
                DbType? dbType = parameter.DbType;
                if (!dbType.HasValue && parameter.Value != null)
                {
                    dbType = SqlMapper.LookupDbType(parameter.Value.GetType(), parameter.Name, false, out _);
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
        public static ParameterItem HandleParameter(DatabaseServerType databaseServerType, ParameterItem parameter)
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

            #region MySQL

            //DateTimeOffset
            AddParameterHandler(DatabaseServerType.MySQL, DbType.DateTimeOffset, datetimeOffsetHandler);

            #endregion

            #region Oracle

            //boolean
            AddParameterHandler(DatabaseServerType.Oracle, DbType.Boolean, boolToIntegerHandler);

            #endregion

            #region SQLite

            //DateTimeOffset
            AddParameterHandler(DatabaseServerType.SQLite, DbType.DateTimeOffset, datetimeOffsetHandler);

            #endregion
        }

        #endregion

        #region Database server & Entity format key

        /// <summary>
        /// Generate servertype&entit format key
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return database server entity format key</returns>
        static string GenerateDatabaseServerEntityFormatKey(DatabaseServerType serverType, Type entityType)
        {
            if (entityType == null)
            {
                throw new EZNEWException("EntityType is null");
            }
            return GenerateDatabaseServerEntityFormatKey(serverType, entityType.GUID);
        }

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
        public static string GetDatabaseServerEntityFormatKey(DatabaseServerType serverType, Type entityType)
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
        public static string GetDatabaseServerEntityFormatKey(DatabaseServerType serverType, Guid entityTypeId)
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

        #endregion
    }
}
