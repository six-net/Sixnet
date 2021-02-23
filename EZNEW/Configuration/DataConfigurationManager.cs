using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EZNEW.Data;
using EZNEW.Data.Configuration;
using EZNEW.Data.CriteriaConverter;
using EZNEW.Develop.Command;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.CQuery.Translator;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Entity;
using EZNEW.Fault;

namespace EZNEW.Configuration
{
    public partial class ConfigurationManager
    {
        internal static class Data
        {
            static Data()
            {
                Init();
            }

            #region Properties

            /// <summary>
            /// Gets database servers operation proxy
            /// </summary>
            static Func<ICommand, List<DatabaseServer>> GetDatabaseServerProxy { get; set; }

            /// <summary>
            /// Gets or sets get connection operation proxy
            /// </summary>
            static Func<DatabaseServer, IDbConnection> GetDatabaseConnectionProxy { get; set; }

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
            static readonly Dictionary<DatabaseServerType, BatchExecuteConfiguration> DatabaseServerExecuteConfigurations = new Dictionary<DatabaseServerType, BatchExecuteConfiguration>();

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

            #endregion

            #region Methods

            #region Database server

            /// <summary>
            /// Configure database server
            /// </summary>
            /// <param name="getDatabaseServerOperation">Get database server operation</param>
            internal static void ConfigureDatabaseServer(Func<ICommand, List<DatabaseServer>> getDatabaseServerOperation)
            {
                GetDatabaseServerProxy = getDatabaseServerOperation;
            }

            /// <summary>
            /// Get database servers
            /// </summary>
            /// <param name="command">Command</param>
            /// <returns>Return database servers</returns>
            internal static List<DatabaseServer> GetDatabaseServers(ICommand command)
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
            internal static void ConfigureQueryTranslator(DatabaseServerType serverType, IQueryTranslator queryTranslator)
            {
                DatabaseServerQueryTranslators[serverType] = queryTranslator;
            }

            /// <summary>
            /// Get query translator
            /// </summary>
            /// <param name="serverType">Database server type</param>
            /// <returns>Return query translator</returns>
            internal static IQueryTranslator GetQueryTranslator(DatabaseServerType serverType)
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
            internal static void ConfigureDatabaseConnection(Func<DatabaseServer, IDbConnection> getDatabaseConnectionOperation)
            {
                GetDatabaseConnectionProxy = getDatabaseConnectionOperation;
            }

            /// <summary>
            /// Get database connection
            /// </summary>
            /// <param name="databaseServer">Database server</param>
            /// <returns></returns>
            internal static IDbConnection GetDatabaseConnection(DatabaseServer databaseServer)
            {
                return GetDatabaseConnectionProxy?.Invoke(databaseServer);
            }

            #endregion

            #region Database provider

            /// <summary>
            /// Configure database provider
            /// </summary>
            /// <param name="serverType">Database server type</param>
            /// <param name="databaseProvider">Database provider</param>
            internal static void ConfigureDatabaseProvider(DatabaseServerType serverType, IDatabaseProvider databaseProvider)
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
            internal static IDatabaseProvider GetDatabaseProvider(DatabaseServerType serverType)
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
                foreach (var configItem in Entity.EntityConfigurations)
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
            internal static void ConfigureEntityObjectName(DatabaseServerType serverType, Type entityType, string objectName)
            {
                if (entityType == null || string.IsNullOrWhiteSpace(objectName))
                {
                    return;
                }
                DatabaseServerEntityObjectNames[serverType.GetDatabaseServerEntityFormatKey(entityType)] = objectName;
            }

            /// <summary>
            /// Get entity object name
            /// </summary>
            /// <param name="serverType">Database server type</param>
            /// <param name="entityType">Entity type</param>
            /// <param name="defaultName">Default name</param>
            /// <returns>Return Entity object name</returns>
            internal static string GetEntityObjectName(DatabaseServerType serverType, Type entityType, string defaultName = "")
            {
                DatabaseServerEntityObjectNames.TryGetValue(GetDatabaseServerEntityFormatKey(serverType, entityType), out var objectName);
                if (!string.IsNullOrWhiteSpace(objectName))
                {
                    return objectName;
                }
                objectName = EntityManager.GetEntityObjectName(entityType);
                return string.IsNullOrWhiteSpace(objectName) ? defaultName : objectName;
            }

            #endregion

            #region Entity field

            /// <summary>
            /// Configure entity fields
            /// </summary>
            /// <param name="serverType">Database server type</param>
            /// <param name="entityType">Entity type</param>
            /// <param name="fields">Fields</param>
            internal static void ConfigureEntityFields(DatabaseServerType serverType, Type entityType, IEnumerable<EntityField> fields)
            {
                if (entityType == null || fields.IsNullOrEmpty())
                {
                    return;
                }
                var key = serverType.GetDatabaseServerEntityFormatKey(entityType);
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
            internal static EntityField GetField(DatabaseServerType serverType, Type entityType, string propertyName)
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
            /// Get fields
            /// </summary>
            /// <param name="serverType">Database server type</param>
            /// <param name="entityType">Entity type</param>
            /// <param name="propertyNames">Property names</param>
            /// <returns>Return properties relation entity fields</returns>
            internal static IEnumerable<EntityField> GetFields(DatabaseServerType serverType, Type entityType, IEnumerable<string> propertyNames)
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
                DatabaseServerEntityFields.TryGetValue(serverType.GetDatabaseServerEntityFormatKey(entityType), out var entityServerFields);
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
            internal static IEnumerable<EntityField> GetEditFields(DatabaseServerType serverType, Type entityType)
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
            internal static IEnumerable<EntityField> GetQueryFields(DatabaseServerType serverType, Type entityType, IQuery query, bool forceMustFields)
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
            internal static IEnumerable<EntityField> GetAllQueryFields(DatabaseServerType serverType, Type entityType)
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
            internal static EntityField GetDefaultField(DatabaseServerType serverType, Type entityType)
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

            #region Batch execute configuration

            #region Configure batch execute

            /// <summary>
            /// Configure batch execute
            /// </summary>
            /// <param name="serverType">Database server type</param>
            /// <param name="batchExecuteConfig">Batch execute configuration</param>
            internal static void ConfigureBatchExecute(DatabaseServerType serverType, BatchExecuteConfiguration batchExecuteConfig)
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
            internal static BatchExecuteConfiguration GetBatchExecuteConfiguration(DatabaseServerType serverType)
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
            internal static void ConfigureDataIsolationLevel(DatabaseServerType serverType, DataIsolationLevel dataIsolationLevel)
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

            #region Criteria converter

            /// <summary>
            /// Configure criteria converter parser
            /// </summary>
            /// <param name="converterConfigName">Converter config name</param>
            /// <param name="converterParseOperation">Converter parse operation</param>
            internal static void ConfigureCriteriaConverterParser(string converterConfigName, Func<CriteriaConverterParseOptions, string> converterParseOperation)
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
            internal static Func<CriteriaConverterParseOptions, string> GetCriteriaConverterParser(string converterConfigName)
            {
                CriteriaConverterParsers.TryGetValue(converterConfigName, out var parse);
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
            internal static string GetDatabaseServerEntityFormatKey(DatabaseServerType serverType, Type entityType)
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
            internal static string GetDatabaseServerEntityFormatKey(DatabaseServerType serverType, Guid entityTypeId)
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
}
