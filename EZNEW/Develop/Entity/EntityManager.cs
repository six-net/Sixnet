using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using EZNEW.ExpressionUtil;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Entity manager
    /// </summary>
    public static class EntityManager
    {
        static readonly Type booleanType = typeof(bool);

        #region Properties

        /// <summary>
        /// Entity configurations
        /// key:entity type guid
        /// </summary>
        static ConcurrentDictionary<Guid, EntityConfiguration> EntityConfigurations
        {
            get;
        } = new ConcurrentDictionary<Guid, EntityConfiguration>();

        #endregion

        #region Methods

        #region Entity configuration

        #region Configure entity

        /// <summary>
        /// Configure entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        internal static void ConfigureEntity<TEntity>() where TEntity : BaseEntity<TEntity>, new()
        {
            var type = typeof(TEntity);
            ConfigureEntity(type);
        }

        /// <summary>
        /// Configure entity
        /// </summary>
        /// <param name="entityType">Entity type</param>
        internal static void ConfigureEntity(Type entityType)
        {
            if (entityType == null)
            {
                return;
            }
            var typeGuid = entityType.GUID;
            if (EntityConfigurations.ContainsKey(typeGuid))
            {
                return;
            }
            var entityAttribute = (entityType.GetCustomAttributes(typeof(EntityAttribute), false)?.FirstOrDefault()) as EntityAttribute;
            if (entityAttribute == null)
            {
                return;
            }
            var propertys = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string objectName = string.IsNullOrWhiteSpace(entityAttribute.ObjectName) ? entityType.Name : entityAttribute.ObjectName;
            if (!EntityConfigurations.TryGetValue(typeGuid, out EntityConfiguration entityConfig))
            {
                entityConfig = new EntityConfiguration();
            }
            //table name
            if (string.IsNullOrWhiteSpace(entityConfig.TableName))
            {
                entityConfig.TableName = objectName;
            }
            //fields
            List<EntityField> allFields = new List<EntityField>();
            List<EntityField> primaryKeys = new List<EntityField>();

            //cache keys
            List<EntityField> cacheKeys = new List<EntityField>();
            List<EntityField> cachePrefixKeys = new List<EntityField>();
            List<EntityField> cacheIgnoreKeys = new List<EntityField>();

            List<EntityField> queryFields = new List<EntityField>();
            EntityField versionField = null;
            EntityField refreshDateField = null;

            foreach (var property in propertys)
            {
                var name = property.Name;
                var propertyName = name;
                var entityFieldAttribute = (property.GetCustomAttributes(typeof(EntityFieldAttribute), false)?.FirstOrDefault()) as EntityFieldAttribute;
                bool disableEdit = false;
                bool isVersion = false;
                bool refreshDate = false;
                bool disableQuery = false;
                bool primaryKey = false;
                EntityFieldCacheOption cacheOption = EntityFieldCacheOption.None;
                string queryFormat = string.Empty;
                if (entityFieldAttribute != null)
                {
                    if (!string.IsNullOrWhiteSpace(entityFieldAttribute.Name))
                    {
                        name = entityFieldAttribute.Name;
                    }
                    disableEdit = entityFieldAttribute.DisableEdit;
                    isVersion = entityFieldAttribute.IsVersion;
                    refreshDate = entityFieldAttribute.RefreshDate;
                    disableQuery = entityFieldAttribute.DisableQuery;
                    queryFormat = entityFieldAttribute.QueryFormat;
                    primaryKey = entityFieldAttribute.PrimaryKey;
                    cacheOption = entityFieldAttribute.CacheOption;
                }
                var propertyField = new EntityField()
                {
                    FieldName = name,
                    PropertyName = propertyName,
                    QueryFormat = queryFormat,
                    CacheOption = cacheOption,
                    IsDisableEdit = disableEdit,
                    IsDisableQuery = disableQuery,
                    IsPrimaryKey = primaryKey,
                    IsRefreshDate = refreshDate,
                    IsVersion = isVersion,
                    DataType = property.PropertyType
                };
                allFields.Add(propertyField);
                if (primaryKey)
                {
                    primaryKeys.Add(propertyField);
                }
                else
                {
                    if ((cacheOption & EntityFieldCacheOption.CacheKey) != 0)
                    {
                        cacheKeys.Add(propertyField);
                    }
                    if ((cacheOption & EntityFieldCacheOption.CacheKeyPrefix) != 0)
                    {
                        cachePrefixKeys.Add(propertyField);
                    }
                    if ((cacheOption & EntityFieldCacheOption.Ignore) != 0)
                    {
                        cacheIgnoreKeys.Add(propertyField);
                    }
                }
                if (isVersion)
                {
                    versionField = propertyField;
                }
                if (refreshDate)
                {
                    refreshDateField = propertyField;
                }
                if (!disableQuery)
                {
                    queryFields.Add(propertyField);
                }

                //relation config
                var relationAttributes = property.GetCustomAttributes(typeof(EntityRelationAttribute), false);
                if (relationAttributes.IsNullOrEmpty())
                {
                    continue;
                }
                if (entityConfig.RelationFields.IsNullOrEmpty())
                {
                    entityConfig.RelationFields = new Dictionary<Guid, Dictionary<string, string>>();
                }
                foreach (var attrObj in relationAttributes)
                {
                    EntityRelationAttribute relationAttr = attrObj as EntityRelationAttribute;
                    if (relationAttr == null || relationAttr.RelationType == null || string.IsNullOrWhiteSpace(relationAttr.RelationField))
                    {
                        continue;
                    }
                    var relationTypeId = relationAttr.RelationType.GUID;
                    entityConfig.RelationFields.TryGetValue(relationTypeId, out var values);
                    values = values ?? new Dictionary<string, string>();
                    if (values.ContainsKey(propertyName))
                    {
                        continue;
                    }
                    values.Add(propertyName, relationAttr.RelationField);
                    if (entityConfig.RelationFields.ContainsKey(relationTypeId))
                    {
                        entityConfig.RelationFields[relationTypeId] = values;
                    }
                    else
                    {
                        entityConfig.RelationFields.Add(relationTypeId, values);
                    }
                }
            }
            entityConfig.PrimaryKeys = primaryKeys;
            if (entityConfig.AllFields.IsNullOrEmpty())
            {
                entityConfig.AllFields = allFields;
            }
            if (entityConfig.VersionField == null)
            {
                entityConfig.VersionField = versionField;
            }
            if (entityConfig.RefreshDateField == null)
            {
                entityConfig.RefreshDateField = refreshDateField;
            }
            if (entityConfig.CacheKeys.IsNullOrEmpty())
            {
                entityConfig.CacheKeys = cacheKeys;
            }
            if (entityConfig.CachePrefixKeys.IsNullOrEmpty())
            {
                entityConfig.CachePrefixKeys = cachePrefixKeys;
            }
            if (entityConfig.CacheIgnoreKeys.IsNullOrEmpty())
            {
                entityConfig.CacheIgnoreKeys = cacheIgnoreKeys;
            }
            if (entityConfig.QueryFields.IsNullOrEmpty())
            {
                entityConfig.QueryFields = queryFields;
            }
            entityConfig.PredicateType = typeof(Func<,>).MakeGenericType(entityType, booleanType);
            ConfigureEntity(entityType.GUID, entityConfig);
        }

        /// <summary>
        /// Configure entity
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        internal static void ConfigureEntity(string entityTypeAssemblyQualifiedName)
        {
            var type = Type.GetType(entityTypeAssemblyQualifiedName);
            ConfigureEntity(type);
        }

        /// <summary>
        /// Configure entity
        /// </summary>
        /// <param name="typeGuid">Entity type guid</param>
        /// <param name="entityConfig">Entity configuration</param>
        static void ConfigureEntity(Guid typeGuid, EntityConfiguration entityConfig)
        {
            if (entityConfig == null)
            {
                return;
            }
            EntityConfigurations[typeGuid] = entityConfig;
        }

        #endregion

        #region Get entity configuration

        /// <summary>
        /// Get entity configuration
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity configuration</returns>
        public static EntityConfiguration GetEntityConfiguration(Type entityType)
        {
            if (entityType == null)
            {
                return null;
            }
            var typeGuid = entityType.GUID;
            EntityConfigurations.TryGetValue(typeGuid, out var entityConfig);
            if (entityConfig == null)
            {
                ConfigureEntity(entityType);
                EntityConfigurations.TryGetValue(typeGuid, out entityConfig);
            }
            return entityConfig;
        }

        /// <summary>
        /// Get entity configuration
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return entity configuration</returns>
        public static EntityConfiguration GetEntityConfiguration<TEntity>()
        {
            return GetEntityConfiguration(typeof(TEntity));
        }

        /// <summary>
        /// Get entity configuration
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <returns>Return entity configuration</returns>
        public static EntityConfiguration GetEntityConfiguration(string entityTypeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(entityTypeAssemblyQualifiedName))
            {
                return null;
            }
            return GetEntityConfiguration(Type.GetType(entityTypeAssemblyQualifiedName));
        }

        #endregion 

        #endregion

        #region Entity object name

        #region Configure object name

        /// <summary>
        /// Configure entity object name
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="objectName">Entity object name</param>
        public static void ConfigureObjectName<TEntity>(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return;
            }
            var type = typeof(TEntity);
            ConfigureObjectName(type, objectName);
        }

        /// <summary>
        /// Configure entity object name
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="objectName">Entity object name</param>
        public static void ConfigureObjectName(Type entityType, string objectName)
        {
            if (entityType == null)
            {
                return;
            }
            var entityConfiguration = GetEntityConfiguration(entityType);
            if (entityConfiguration != null)
            {
                entityConfiguration.TableName = objectName;
            }
        }

        /// <summary>
        /// Configure entity object name
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <param name="objectName">Entity object name</param>
        public static void ConfigureObjectName(string entityTypeAssemblyQualifiedName, string objectName)
        {
            var type = Type.GetType(entityTypeAssemblyQualifiedName);
            ConfigureObjectName(type, objectName);
        }

        #endregion

        #region Get object name

        /// <summary>
        /// Get entity object name
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <returns>Return entity object name</returns>
        public static string GetEntityObjectName(string entityTypeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(entityTypeAssemblyQualifiedName))
            {
                return string.Empty;
            }
            var type = Type.GetType(entityTypeAssemblyQualifiedName);
            return GetEntityObjectName(type);
        }

        /// <summary>
        /// Get entity object name
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity object name</returns>
        public static string GetEntityObjectName(Type entityType)
        {
            if (entityType == null)
            {
                return string.Empty;
            }
            var entityConfig = GetEntityConfiguration(entityType);
            return entityConfig?.TableName ?? string.Empty;
        }

        /// <summary>
        /// Get entity object name
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return entity object name</returns>
        public static string GetEntityObjectName<TEntity>()
        {
            return GetEntityObjectName(typeof(TEntity));
        }

        #endregion

        #endregion

        #region Entity fields

        #region Add fields

        /// <summary>
        /// Add fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fields">Fields</param>
        public static void AddFields(Type entityType, IEnumerable<EntityField> fields)
        {
            if (entityType == null)
            {
                return;
            }
            var entityConfig = GetEntityConfiguration(entityType);
            if (entityConfig == null)
            {
                return;
            }
            var otherFields = entityConfig.AllFields?.Except(fields);
            if (otherFields.IsNullOrEmpty())
            {
                entityConfig.AllFields = fields.ToList();
            }
            else
            {
                entityConfig.AllFields = otherFields.Union(fields).ToList();
            }
        }

        #endregion

        #region Get fields

        /// <summary>
        /// Get fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity fields</returns>
        public static List<EntityField> GetFields(Type entityType)
        {
            var entityConfig = GetEntityConfiguration(entityType);
            return entityConfig?.AllFields ?? new List<EntityField>(0);
        }

        #endregion

        #region Get query fields

        /// <summary>
        /// Get entity query fields
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <returns>Return entity query fields</returns>
        public static List<EntityField> GetQueryFields(string entityTypeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(entityTypeAssemblyQualifiedName))
            {
                return new List<EntityField>(0);
            }
            var type = Type.GetType(entityTypeAssemblyQualifiedName);
            return GetQueryFields(type);
        }

        /// <summary>
        /// Get entity query fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="queryPropertyNames">Query property names</param>
        /// <param name="forcePrimaryKey">Whether force return primary key</param>
        /// <param name="forceVersionKey">Whether force return version key</param>
        /// <returns>Return entity query fields</returns>
        public static List<EntityField> GetQueryFields(Type entityType, IEnumerable<string> queryPropertyNames = null, bool forcePrimaryKey = false, bool forceVersionKey = false)
        {
            var entityConfig = GetEntityConfiguration(entityType);
            var allQueryFields = entityConfig?.QueryFields;
            if (allQueryFields.IsNullOrEmpty())
            {
                return new List<EntityField>(0);
            }
            if (queryPropertyNames.IsNullOrEmpty())
            {
                return allQueryFields;
            }
            // special query fields
            if (forcePrimaryKey && !entityConfig.PrimaryKeys.IsNullOrEmpty())
            {
                queryPropertyNames = queryPropertyNames.Union(entityConfig.PrimaryKeys.Select(c => c.PropertyName));
            }
            if (forceVersionKey && entityConfig.VersionField != null)
            {
                queryPropertyNames = queryPropertyNames.Union(new List<string>(1) { entityConfig.VersionField.PropertyName });
            }
            allQueryFields = allQueryFields.Intersect(queryPropertyNames.Select<string, EntityField>(c => c)).ToList();
            return allQueryFields;
        }

        /// <summary>
        /// Get entity query fields
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return entity query fields</returns>
        public static List<EntityField> GetQueryFields<TEntity>()
        {
            return GetQueryFields(typeof(TEntity));
        }

        #endregion

        #endregion

        #region Primary key

        #region Add primary keys 

        /// <summary>
        /// Add primary keys
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="keyNames">Primary key names</param>
        public static void AddPrimaryKey(Type entityType, params string[] keyNames)
        {
            if (entityType == null || keyNames.IsNullOrEmpty())
            {
                return;
            }
            var entityConfig = GetEntityConfiguration(entityType);
            if (entityConfig == null)
            {
                return;
            }
            var keyFields = keyNames.Select<string, EntityField>(c => c);
            if (entityConfig.PrimaryKeys == null)
            {
                entityConfig.PrimaryKeys = keyFields.ToList();
            }
            else
            {
                entityConfig.PrimaryKeys = entityConfig.PrimaryKeys.Union(keyFields).ToList();
            }
        }

        /// <summary>
        /// Add primary key
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">Entity type full name</param>
        /// <param name="keyNames">Primary key names</param>
        public static void AddPrimaryKey(string typeAssemblyQualifiedName, params string[] keyNames)
        {
            if (string.IsNullOrWhiteSpace(typeAssemblyQualifiedName) || keyNames == null || keyNames.Length <= 0)
            {
                return;
            }
            var type = Type.GetType(typeAssemblyQualifiedName);
            AddPrimaryKey(type);
        }

        /// <summary>
        /// Add primary key
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="fields">Primary key fields</param>
        public static void AddPrimaryKey<TEntity>(params Expression<Func<TEntity, dynamic>>[] fields)
        {
            if (fields == null)
            {
                return;
            }
            AddPrimaryKey(typeof(TEntity), fields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body)).ToArray());
        }

        #endregion

        #region Get primary keys

        /// <summary>
        /// Get primary keys
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <returns>Return all primary key field</returns>
        public static List<EntityField> GetPrimaryKeys(string entityTypeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(entityTypeAssemblyQualifiedName))
            {
                return null;
            }
            var type = Type.GetType(entityTypeAssemblyQualifiedName);
            return GetPrimaryKeys(type);
        }

        /// <summary>
        /// Get primary keys
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return all primary key field</returns>
        public static List<EntityField> GetPrimaryKeys(Type entityType)
        {
            if (entityType == null)
            {
                return new List<EntityField>();
            }
            var entityConfig = GetEntityConfiguration(entityType);
            return entityConfig?.PrimaryKeys;
        }

        /// <summary>
        /// Get primary keys
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return all primary key fields</returns>
        public static List<EntityField> GetPrimaryKeys<TEntity>()
        {
            return GetPrimaryKeys(typeof(TEntity));
        }

        /// <summary>
        /// Determines whether the property is primary key
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return whether is primary key</returns>
        public static bool IsPrimaryKey<TEntity>(string propertyName)
        {
            return IsPrimaryKey(typeof(TEntity), propertyName);
        }

        /// <summary>
        /// Determines whether the property is primary key
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return whether is primary key</returns>
        public static bool IsPrimaryKey(Type entityType, string propertyName)
        {
            if (entityType == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }
            var primaryKeys = GetPrimaryKeys(entityType);
            return primaryKeys?.Exists(r => r == propertyName) ?? false;
        }

        #endregion

        #endregion

        #region Version field

        #region Set version field

        /// <summary>
        /// Set version field
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <param name="fieldName">Version field name</param>
        public static void SetVersionField(string entityTypeAssemblyQualifiedName, string fieldName)
        {
            var type = Type.GetType(entityTypeAssemblyQualifiedName);
            SetVersionField(type, fieldName);
        }

        /// <summary>
        /// Set version field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fieldName">Version field name</param>
        public static void SetVersionField(Type entityType, string fieldName)
        {
            if (entityType == null)
            {
                return;
            }
            var entityConfig = GetEntityConfiguration(entityType);
            if (entityConfig != null)
            {
                entityConfig.VersionField = fieldName;
            }
        }

        /// <summary>
        /// Set version field
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="field">Version field</param>
        public static void SetVersionField<TEntity>(Expression<Func<TEntity, dynamic>> field)
        {
            if (field == null)
            {
                return;
            }
            SetVersionField(typeof(TEntity), ExpressionHelper.GetExpressionPropertyName(field.Body));
        }

        #endregion

        #region Get version field

        /// <summary>
        /// Get version field
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <returns>Return the version field name</returns>
        public static string GetVersionField(string entityTypeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(entityTypeAssemblyQualifiedName))
            {
                return string.Empty;
            }
            var type = Type.GetType(entityTypeAssemblyQualifiedName);
            return GetVersionField(type);
        }

        /// <summary>
        /// Get version field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return the version field name</returns>
        public static string GetVersionField(Type entityType)
        {
            if (entityType == null)
            {
                return string.Empty;
            }
            var entityConfig = GetEntityConfiguration(entityType);
            return entityConfig?.VersionField;
        }

        #endregion

        #endregion

        #region Refresh date field

        #region Set refresh date field

        /// <summary>
        /// Set refresh date field
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <param name="fieldName">Refresh date field name</param>
        public static void SetRefreshDateField(string entityTypeAssemblyQualifiedName, string fieldName)
        {
            var type = Type.GetType(entityTypeAssemblyQualifiedName);
            SetRefreshDateField(type, fieldName);
        }

        /// <summary>
        /// Set refresh date field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fieldName">Refresh date field name</param>
        public static void SetRefreshDateField(Type entityType, string fieldName)
        {
            if (entityType == null)
            {
                return;
            }
            var entityConfig = GetEntityConfiguration(entityType);
            if (entityConfig != null)
            {
                entityConfig.RefreshDateField = fieldName;
            }
        }

        /// <summary>
        /// Set refresh date field
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="field">Refresh date field</param>
        public static void SetRefreshDateField<TEntity>(Expression<Func<TEntity, dynamic>> field)
        {
            if (field == null)
            {
                return;
            }
            SetRefreshDateField(typeof(TEntity), ExpressionHelper.GetExpressionPropertyName(field.Body));
        }

        #endregion

        #region Get refreshdate field

        /// <summary>
        /// Get refresh date field
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <returns>Return the refresh date field name</returns>
        public static string GetRefreshDateField(string entityTypeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(entityTypeAssemblyQualifiedName))
            {
                return string.Empty;
            }
            var type = Type.GetType(entityTypeAssemblyQualifiedName);
            return GetRefreshDateField(type);
        }

        /// <summary>
        /// Get refresh date field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return the refresh date field name</returns>
        public static string GetRefreshDateField(Type entityType)
        {
            if (entityType == null)
            {
                return string.Empty;
            }
            var entityConfig = GetEntityConfiguration(entityType);
            return entityConfig?.RefreshDateField;
        }

        #endregion 

        #endregion

        #region Relation fields

        /// <summary>
        /// Get relation fields
        /// </summary>
        /// <param name="sourceEntityType">Source entity type</param>
        /// <param name="relationEntityType">Relation entity type</param>
        /// <returns>Return all relation fields</returns>
        public static Dictionary<string, string> GetRelationFields(Type sourceEntityType, Type relationEntityType)
        {
            if (sourceEntityType == null || relationEntityType == null)
            {
                return new Dictionary<string, string>(0);
            }
            var entityConfig = GetEntityConfiguration(sourceEntityType);
            Dictionary<string, string> relationFields = null;
            entityConfig?.RelationFields?.TryGetValue(relationEntityType.GUID, out relationFields);
            return relationFields ?? new Dictionary<string, string>(0);
        }

        #endregion

        #endregion
    }
}
