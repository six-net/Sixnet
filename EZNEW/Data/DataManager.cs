using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Collections.Concurrent;
using System.IO;
using EZNEW.Dapper;
using EZNEW.Develop.DataAccess;
using EZNEW.Data.CriteriaConverter;
using EZNEW.Develop.Entity;
using EZNEW.Develop.CQuery.Translator;
using EZNEW.Serialize;
using EZNEW.Data.Configuration;
using EZNEW.Fault;
using EZNEW.DependencyInjection;
using EZNEW.Develop.Command;
using EZNEW.Develop.CQuery;

namespace EZNEW.Data
{
    /// <summary>
    /// Data access manager
    /// </summary>
    public static class DataManager
    {
        static DataManager()
        {
            ContainerManager.Container?.Register(typeof(ICommandEngine), typeof(DatabaseCommandEngine));
            SqlMapper.Settings.ApplyNullValues = true;
        }

        #region Properties

        /// <summary>
        /// Gets database servers operation proxy
        /// </summary>
        static Func<ICommand, List<DatabaseServer>> GetDatabaseServerProxy { get; set; }

        /// <summary>
        /// Database server query translators
        /// </summary>
        static Dictionary<DatabaseServerType, IQueryTranslator> DatabaseServerQueryTranslators = new Dictionary<DatabaseServerType, IQueryTranslator>();

        /// <summary>
        /// Gets or sets get connection operation proxy
        /// </summary>
        static Func<DatabaseServer, IDbConnection> GetDatabaseConnectionProxy { get; set; }

        /// <summary>
        /// Gets the database engines
        /// </summary>
        static Dictionary<DatabaseServerType, IDatabaseEngine> DatabaseEngines { get; } = new Dictionary<DatabaseServerType, IDatabaseEngine>();

        /// <summary>
        /// servertype&entity object name
        /// key:{database server type}_{entity type id}
        /// value:object name
        /// </summary>
        static ConcurrentDictionary<string, string> DatabaseServerEntityObjectNames = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// servertype&entity fields
        /// key:{database server type}_{entity type id}
        /// value:fields
        /// </summary>
        static ConcurrentDictionary<string, ConcurrentDictionary<string, EntityField>> DatabaseServerEntityFields = new ConcurrentDictionary<string, ConcurrentDictionary<string, EntityField>>();

        /// <summary>
        /// servertype&entity query fields
        /// key:{database server type}_{entity type id}
        /// value:fieds
        /// </summary>
        static ConcurrentDictionary<string, List<EntityField>> DatabaseServerEntityQueryFields = new ConcurrentDictionary<string, List<EntityField>>();

        /// <summary>
        /// servertype&entity edit fields
        /// key:{database server type}_{entity type id}
        /// value:fields
        /// </summary>
        static ConcurrentDictionary<string, List<EntityField>> DatabaseServerEntityEditFields = new ConcurrentDictionary<string, List<EntityField>>();

        /// <summary>
        /// Server type & entity format keys
        /// </summary>
        static ConcurrentDictionary<DatabaseServerType, ConcurrentDictionary<Guid, string>> DatabaseServerEntityFormatKeys = new ConcurrentDictionary<DatabaseServerType, ConcurrentDictionary<Guid, string>>();

        /// <summary>
        /// Already configure servertype entity
        /// </summary>
        static ConcurrentDictionary<string, bool> AlreadyConfigureDatabaseServerEntities = new ConcurrentDictionary<string, bool>();

        /// <summary>
        /// Database server batch execute config
        /// </summary>
        static Dictionary<DatabaseServerType, BatchExecuteConfiguration> DatabaseServerExecuteConfigurations = new Dictionary<DatabaseServerType, BatchExecuteConfiguration>();

        /// <summary>
        /// Database server data isolation level collection
        /// </summary>
        static Dictionary<DatabaseServerType, DataIsolationLevel> DatabaseServerDataIsolationLevels = new Dictionary<DatabaseServerType, DataIsolationLevel>()
        {
            { DatabaseServerType.MySQL,DataIsolationLevel.RepeatableRead },
            { DatabaseServerType.SQLServer,DataIsolationLevel.ReadCommitted },
            { DatabaseServerType.Oracle,DataIsolationLevel.ReadCommitted }
        };

        /// <summary>
        /// System data isolation level map
        /// </summary>
        static Dictionary<DataIsolationLevel, IsolationLevel> SystemDataIsolationLevelMaps = new Dictionary<DataIsolationLevel, IsolationLevel>()
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
        static Dictionary<string, Func<CriteriaConverterParseOption, string>> CriteriaConverterParseDictionary = new Dictionary<string, Func<CriteriaConverterParseOption, string>>();

        /// <summary>
        /// Gets or sets the default page size
        /// </summary>
        public static int DefaultPageSize { get; set; } = 20;

        #endregion

        #region Methods

        #region Configure

        /// <summary>
        /// Configure data access
        /// </summary>
        /// <param name="configuration">Data configuration</param>
        public static void Configure(DataConfiguration configuration)
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
                //register database engine
                if (!string.IsNullOrWhiteSpace(serverItem.Value.EngineFullTypeName))
                {
                    IDatabaseEngine engine = (IDatabaseEngine)Activator.CreateInstance(Type.GetType(serverItem.Value.EngineFullTypeName));
                    ConfigureDatabaseEngine(serverItem.Key, engine);
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
        /// <param name="jsonConfiguration">Json value</param>
        public static void Configure(string jsonConfiguration)
        {
            if (string.IsNullOrWhiteSpace(jsonConfiguration))
            {
                return;
            }
            var dataConfig = JsonSerializeHelper.JsonToObject<DataConfiguration>(jsonConfiguration);
            if (dataConfig == null)
            {
                return;
            }
            Configure(dataConfig);
        }

        /// <summary>
        /// Configure data access through default configuration file
        /// </summary>
        /// <param name="configRootPath">Data access configuration root path</param>
        public static void ConfigureByFile(string configRootPath = "")
        {
            if (string.IsNullOrWhiteSpace(configRootPath))
            {
                configRootPath = Path.Combine(Directory.GetCurrentDirectory(), "Configuration", "DataAccess");
            }
            if (!Directory.Exists(configRootPath))
            {
                return;
            }
            InitFolderConfiguration(configRootPath);
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
            var files = Directory.GetFiles(path).Where(c => Path.GetExtension(c).Trim('.').ToLower() == "daconfig").ToArray();
            if (!files.IsNullOrEmpty())
            {
                foreach (var file in files)
                {
                    var fileData = File.ReadAllText(file);
                    Configure(fileData);
                }
            }
            var childFolders = new DirectoryInfo(path).GetDirectories();
            foreach (var folder in childFolders)
            {
                InitFolderConfiguration(folder.FullName);
            }
        }

        /// <summary>
        /// Configure server type entity
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="entityConfiguration">Entity configuration</param>
        static void ConfigureDatabaseServerEntity(DatabaseServerType serverType, Type entityType, DataEntityConfiguration entityConfiguration, bool cover = true)
        {
            if (entityConfiguration == null)
            {
                return;
            }
            string key = serverType.GetDatabaseServerEntityFormatKey(entityType);
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }
            //Configure object name
            ConfigureEntityObjectName(key, entityConfiguration.TableName, cover);
            //Configure fields
            ConfigureEntityFields(key, entityType, entityConfiguration.Fields, cover);
        }

        #endregion

        #region Configure entity

        /// <summary>
        /// Configure entity
        /// </summary>
        /// <param name="databaseServerEntityFormatKey">Database server entity format key</param>
        /// <param name="entityType">Entity type</param>
        static void ConfigureEntity(string databaseServerEntityFormatKey, Type entityType)
        {
            if (string.IsNullOrWhiteSpace(databaseServerEntityFormatKey) || entityType == null || AlreadyConfigureDatabaseServerEntities.ContainsKey(databaseServerEntityFormatKey))
            {
                return;
            }
            var entityConfig = EntityManager.GetEntityConfiguration(entityType);
            if (entityConfig == null)
            {
                return;
            }
            AlreadyConfigureDatabaseServerEntities[databaseServerEntityFormatKey] = true;
            //Configure object name
            ConfigureEntityObjectName(databaseServerEntityFormatKey, entityConfig.TableName, false);
            //Configure fields
            ConfigureEntityFields(databaseServerEntityFormatKey, entityType, entityConfig.AllFields, false);
        }

        #endregion

        #region Database server

        /// <summary>
        /// Configure database server
        /// </summary>
        /// <param name="getDatabaseServerOperation">Get database server operation</param>
        public static void ConfigureDatabaseServer(Func<ICommand, List<DatabaseServer>> getDatabaseServerOperation)
        {
            GetDatabaseServerProxy = getDatabaseServerOperation;
        }

        /// <summary>
        /// Get database servers
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return database servers</returns>
        public static List<DatabaseServer> GetDatabaseServers(ICommand command)
        {
            return GetDatabaseServerProxy?.Invoke(command) ?? new List<DatabaseServer>(0);
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
            GetDatabaseConnectionProxy = getDatabaseConnectionOperation;
        }

        /// <summary>
        /// Get database connection
        /// </summary>
        /// <param name="databaseServer">Database server</param>
        /// <returns></returns>
        public static IDbConnection GetDatabaseConnection(DatabaseServer databaseServer)
        {
            return GetDatabaseConnectionProxy?.Invoke(databaseServer);
        }

        #endregion

        #region Database engine

        /// <summary>
        /// Configure database engine
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="databaseEngine">Database engine</param>
        public static void ConfigureDatabaseEngine(DatabaseServerType serverType, IDatabaseEngine databaseEngine)
        {
            if (databaseEngine == null)
            {
                return;
            }
            DatabaseEngines[serverType] = databaseEngine;
        }

        /// <summary>
        /// Get database engine
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return database engine</returns>
        public static IDatabaseEngine GetDatabaseEngine(DatabaseServerType serverType)
        {
            DatabaseEngines.TryGetValue(serverType, out var databaseEngine);
            return databaseEngine;
        }

        #endregion

        #region Entity object name

        /// <summary>
        /// Configure entity object name
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="objectName">Object name</param>
        /// <param name="cover">Whether cover current object name</param>
        public static void ConfigureEntityObjectName(DatabaseServerType serverType, Type entityType, string objectName, bool cover = true)
        {
            if (entityType == null)
            {
                return;
            }
            var key = serverType.GetDatabaseServerEntityFormatKey(entityType);
            ConfigureEntityObjectName(key, objectName, cover);
        }

        /// <summary>
        /// Configure entity object name
        /// </summary>
        /// <param name="databaseServerFormatEntityKey">Database server format entity key</param>
        /// <param name="objectName">Object name</param>
        /// <param name="cover">Whether cover current object name</param>
        static void ConfigureEntityObjectName(string databaseServerFormatEntityKey, string objectName, bool cover = true)
        {
            if (string.IsNullOrWhiteSpace(databaseServerFormatEntityKey) || string.IsNullOrWhiteSpace(objectName))
            {
                return;
            }
            if (cover || !DatabaseServerEntityObjectNames.ContainsKey(databaseServerFormatEntityKey))
            {
                DatabaseServerEntityObjectNames[databaseServerFormatEntityKey] = objectName;
            }
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
            string key = serverType.GetDatabaseServerEntityFormatKey(entityType);
            if (string.IsNullOrWhiteSpace(key))
            {
                return defaultName;
            }
            DatabaseServerEntityObjectNames.TryGetValue(key, out var cacheObjectName);
            if (string.IsNullOrWhiteSpace(cacheObjectName))
            {
                ConfigureEntity(key, entityType);
                DatabaseServerEntityObjectNames.TryGetValue(key, out cacheObjectName);
                cacheObjectName = string.IsNullOrWhiteSpace(cacheObjectName) ? defaultName : cacheObjectName;
            }
            return cacheObjectName ?? string.Empty;
        }

        /// <summary>
        /// Get entity object name
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="defaultName">Default name</param>
        /// <returns>Return entity object name</returns>
        public static string GetEntityObjectName<T>(DatabaseServerType serverType, string defaultName = "")
        {
            return GetEntityObjectName(serverType, typeof(T), defaultName);
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
        /// <param name="cover">Whether conver current fields</param>
        public static void ConfigureEntityFields(DatabaseServerType serverType, Type entityType, IEnumerable<EntityField> fields, bool cover = true)
        {
            if (entityType == null)
            {
                return;
            }
            var key = serverType.GetDatabaseServerEntityFormatKey(entityType);
            ConfigureEntityFields(key, entityType, fields, cover);
        }

        /// <summary>
        /// Configure entity fields
        /// </summary>
        /// <param name="serverTypeFormatEntityKey">Database server format entity key</param>
        /// <param name="fields">Fields</param>
        /// <param name="cover">Whether conver current fields</param>
        static void ConfigureEntityFields(string serverTypeFormatEntityKey, Type entityType, IEnumerable<EntityField> fields, bool cover = true)
        {
            if (string.IsNullOrWhiteSpace(serverTypeFormatEntityKey) || fields.IsNullOrEmpty())
            {
                return;
            }
            if (!AlreadyConfigureDatabaseServerEntities.ContainsKey(serverTypeFormatEntityKey))
            {
                ConfigureEntity(serverTypeFormatEntityKey, entityType);
            }
            DatabaseServerEntityFields.TryGetValue(serverTypeFormatEntityKey, out var nowFields);
            nowFields = nowFields ?? new ConcurrentDictionary<string, EntityField>();
            List<EntityField> queryFields = new List<EntityField>();
            List<EntityField> editFields = new List<EntityField>();

            #region new fields

            foreach (var newField in fields)
            {
                if (string.IsNullOrWhiteSpace(newField?.PropertyName))
                {
                    continue;
                }
                nowFields.TryGetValue(newField.PropertyName, out var nowField);
                if (nowField == null || cover)
                {
                    nowField = newField;
                    nowFields[newField.PropertyName] = nowField;
                }
            }

            #endregion

            #region query&edit fields

            foreach (var field in nowFields.Values.OrderByDescending(c => c.IsPrimaryKey).ThenByDescending(c => c.CacheOption))
            {
                if (!field.IsDisableQuery)
                {
                    queryFields.Add(field);
                }
                if (!field.IsDisableEdit)
                {
                    editFields.Add(field);
                }
            }

            #endregion

            //query fields
            DatabaseServerEntityQueryFields[serverTypeFormatEntityKey] = queryFields;
            //edit fields
            DatabaseServerEntityEditFields[serverTypeFormatEntityKey] = editFields;
            //all fields
            DatabaseServerEntityFields[serverTypeFormatEntityKey] = nowFields;
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
                return string.Empty;
            }
            string key = serverType.GetDatabaseServerEntityFormatKey(entityType);
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }
            DatabaseServerEntityFields.TryGetValue(key, out var allFields);
            if (allFields.IsNullOrEmpty())
            {
                //configure entity
                ConfigureEntity(key, entityType);
                DatabaseServerEntityFields.TryGetValue(key, out allFields);
            }
            EntityField field = null;
            allFields?.TryGetValue(propertyName, out field);
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
        public static List<EntityField> GetFields(DatabaseServerType serverType, Type entityType, IEnumerable<string> propertyNames)
        {
            if (propertyNames.IsNullOrEmpty())
            {
                return new List<EntityField>(0);
            }
            var propertyFields = propertyNames.Select<string, EntityField>(p => p).ToList();
            var key = serverType.GetDatabaseServerEntityFormatKey(entityType);
            if (string.IsNullOrWhiteSpace(key))
            {
                return propertyFields;
            }
            DatabaseServerEntityFields.TryGetValue(key, out var allFields);
            if (allFields.IsNullOrEmpty())
            {
                ConfigureEntity(key, entityType);
                DatabaseServerEntityFields.TryGetValue(key, out allFields);
                if (allFields.IsNullOrEmpty())
                {
                    return propertyFields;
                }
            }
            for (var p = 0; p < propertyFields.Count; p++)
            {
                var propertyField = propertyFields[p];
                if (allFields.TryGetValue(propertyField.PropertyName, out var nowField) && nowField != null)
                {
                    propertyFields[p] = nowField;
                }
            }
            return propertyFields;
        }

        /// <summary>
        /// Get edit fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity edit fields</returns>
        public static List<EntityField> GetEditFields(DatabaseServerType serverType, Type entityType)
        {
            string key = serverType.GetDatabaseServerEntityFormatKey(entityType);
            if (string.IsNullOrWhiteSpace(key))
            {
                return new List<EntityField>(0);
            }
            DatabaseServerEntityEditFields.TryGetValue(key, out var fields);
            if (fields.IsNullOrEmpty())
            {
                ConfigureEntity(key, entityType);
                DatabaseServerEntityEditFields.TryGetValue(key, out fields);
            }
            return fields ?? new List<EntityField>(0);
        }

        /// <summary>
        /// Get query fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="query">Query object</param>
        /// <returns>Return entity fields</returns>
        public static List<EntityField> GetQueryFields(DatabaseServerType serverType, Type entityType, IQuery query)
        {
            string key = serverType.GetDatabaseServerEntityFormatKey(entityType);
            if (string.IsNullOrWhiteSpace(key))
            {
                return new List<EntityField>(0);
            }
            DatabaseServerEntityQueryFields.TryGetValue(key, out var fields);
            if (fields.IsNullOrEmpty())
            {
                ConfigureEntity(key, entityType);
                DatabaseServerEntityQueryFields.TryGetValue(key, out fields);
            }
            if (fields.IsNullOrEmpty())
            {
                throw new Exception("empty fields");
            }
            if (query.QueryFields.IsNullOrEmpty() && query.NotQueryFields.IsNullOrEmpty())
            {
                return fields;
            }
            var queryFields = query.GetActuallyQueryFields(entityType, true, true);
            return fields.Intersect(queryFields).ToList();
        }

        /// <summary>
        /// Get default field
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return default field</returns>
        public static EntityField GetDefaultField(DatabaseServerType serverType, Type entityType)
        {
            string key = serverType.GetDatabaseServerEntityFormatKey(entityType);
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }
            DatabaseServerEntityQueryFields.TryGetValue(key, out var fields);
            if (fields.IsNullOrEmpty())
            {
                ConfigureEntity(key, entityType);
                DatabaseServerEntityQueryFields.TryGetValue(key, out fields);
            }
            EntityField field = null;
            if (fields?.Count > 0)
            {
                field = fields[0];
            }
            field = field ?? string.Empty;
            return field;
        }

        #endregion

        #region Batch execute configuration

        #region Configure batch execute

        /// <summary>
        /// Configure batch execute
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="batchExecuteConfig">Batch execute configuration</param>
        public static void ConfigureBatchExecute(DatabaseServerType serverType, BatchExecuteConfiguration batchExecuteConfig)
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
        /// Get batch execute configuration
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return batch execute configuration</returns>
        public static BatchExecuteConfiguration GetBatchExecuteConfiguration(DatabaseServerType serverType)
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
        public static void ConfigureCriteriaConverterParser(string converterConfigName, Func<CriteriaConverterParseOption, string> converterParseOperation)
        {
            if (string.IsNullOrWhiteSpace(converterConfigName) || converterParseOperation == null)
            {
                return;
            }
            CriteriaConverterParseDictionary[converterConfigName] = converterParseOperation;
        }

        /// <summary>
        /// Get criteria converter parser
        /// </summary>
        /// <param name="converterConfigName">Converter config name</param>
        /// <returns>Return convert parse operation</returns>
        public static Func<CriteriaConverterParseOption, string> GetCriteriaConverterParser(string converterConfigName)
        {
            CriteriaConverterParseDictionary.TryGetValue(converterConfigName, out var parse);
            return parse;
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
            return string.Format("{0}_{1}", (int)serverType, entityType.GUID);
        }

        /// <summary>
        /// Get servertype&entity format key
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return database server entity format key</returns>
        public static string GetDatabaseServerEntityFormatKey(this DatabaseServerType serverType, Type entityType)
        {
            if (entityType == null)
            {
                return string.Empty;
            }
            var entityId = entityType.GUID;
            string key = string.Empty;
            DatabaseServerEntityFormatKeys.TryGetValue(serverType, out var entityKeys);
            entityKeys?.TryGetValue(entityId, out key);
            if (string.IsNullOrWhiteSpace(key))
            {
                key = GenerateDatabaseServerEntityFormatKey(serverType, entityType);
                if (entityKeys == null)
                {
                    entityKeys = new ConcurrentDictionary<Guid, string>();
                }
                entityKeys[entityId] = key;
                DatabaseServerEntityFormatKeys[serverType] = entityKeys;
            }
            return key;
        }

        #endregion

        #endregion
    }
}
