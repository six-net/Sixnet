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
using EZNEW.Cache;
using EZNEW.Configuration;

namespace EZNEW.Data
{
    /// <summary>
    /// Data access manager
    /// </summary>
    public static class DataManager
    {
        static DataManager()
        {
            ContainerManager.Container?.Register(typeof(ICommandExecutor), typeof(DatabaseCommandExecutor));
            SqlMapper.Settings.ApplyNullValues = true;
        }

        #region Properties

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
            ConfigurationManager.Data.ConfigureDatabaseServer(getDatabaseServerOperation);
        }

        /// <summary>
        /// Get database servers
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return database servers</returns>
        public static List<DatabaseServer> GetDatabaseServers(ICommand command)
        {
            return ConfigurationManager.Data.GetDatabaseServers(command);
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
            ConfigurationManager.Data.ConfigureQueryTranslator(serverType, queryTranslator);
        }

        /// <summary>
        /// Get query translator
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return query translator</returns>
        public static IQueryTranslator GetQueryTranslator(DatabaseServerType serverType)
        {
            return ConfigurationManager.Data.GetQueryTranslator(serverType);
        }

        #endregion

        #region Database connection

        /// <summary>
        /// Configure connection
        /// </summary>
        /// <param name="getDatabaseConnectionOperation">Get database connection operation</param>
        public static void ConfigureDatabaseConnection(Func<DatabaseServer, IDbConnection> getDatabaseConnectionOperation)
        {
            ConfigurationManager.Data.ConfigureDatabaseConnection(getDatabaseConnectionOperation);
        }

        /// <summary>
        /// Get database connection
        /// </summary>
        /// <param name="databaseServer">Database server</param>
        /// <returns></returns>
        public static IDbConnection GetDatabaseConnection(DatabaseServer databaseServer)
        {
            return ConfigurationManager.Data.GetDatabaseConnection(databaseServer);
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
            ConfigurationManager.Data.ConfigureDatabaseProvider(serverType, databaseProvider);
        }

        /// <summary>
        /// Get database provider
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <returns>Return database provider</returns>
        public static IDatabaseProvider GetDatabaseProvider(DatabaseServerType serverType)
        {
            return ConfigurationManager.Data.GetDatabaseProvider(serverType);
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
            ConfigurationManager.Data.ConfigureEntityObjectName(serverType, entityType, objectName);
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
            return ConfigurationManager.Data.GetEntityObjectName(serverType, entityType, defaultName);
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
        public static void ConfigureEntityFields(DatabaseServerType serverType, Type entityType, IEnumerable<EntityField> fields)
        {
            ConfigurationManager.Data.ConfigureEntityFields(serverType, entityType, fields);
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
            return ConfigurationManager.Data.GetField(serverType, entityType, propertyName);
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
            return ConfigurationManager.Data.GetFields(serverType, entityType, propertyNames);
        }

        /// <summary>
        /// Get edit fields
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity edit fields</returns>
        public static IEnumerable<EntityField> GetEditFields(DatabaseServerType serverType, Type entityType)
        {
            return ConfigurationManager.Data.GetEditFields(serverType, entityType);
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
            return ConfigurationManager.Data.GetQueryFields(serverType, entityType, query, forceMustFields);
        }

        /// <summary>
        /// Get default field
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return default field</returns>
        public static EntityField GetDefaultField(DatabaseServerType serverType, Type entityType)
        {
            return ConfigurationManager.Data.GetDefaultField(serverType, entityType);
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
            ConfigurationManager.Data.ConfigureBatchExecute(serverType, batchExecuteConfig);
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
            return ConfigurationManager.Data.GetBatchExecuteConfiguration(serverType);
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
            ConfigurationManager.Data.ConfigureDataIsolationLevel(serverType, dataIsolationLevel);
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
            return ConfigurationManager.Data.GetDataIsolationLevel(serverType);
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
            return ConfigurationManager.Data.GetSystemIsolationLevel(dataIsolationLevel);
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
            ConfigurationManager.Data.ConfigureCriteriaConverterParser(converterConfigName, converterParseOperation);
        }

        /// <summary>
        /// Get criteria converter parser
        /// </summary>
        /// <param name="converterConfigName">Converter config name</param>
        /// <returns>Return convert parse operation</returns>
        public static Func<CriteriaConverterParseOptions, string> GetCriteriaConverterParser(string converterConfigName)
        {
            return ConfigurationManager.Data.GetCriteriaConverterParser(converterConfigName);
        }

        #endregion

        #region Database server & Entity format key

        /// <summary>
        /// Get servertype&entity format key
        /// </summary>
        /// <param name="serverType">Database server type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return database server entity format key</returns>
        public static string GetDatabaseServerEntityFormatKey(this DatabaseServerType serverType, Type entityType)
        {
            return ConfigurationManager.Data.GetDatabaseServerEntityFormatKey(serverType, entityType);
        }

        #endregion

        #endregion
    }
}
