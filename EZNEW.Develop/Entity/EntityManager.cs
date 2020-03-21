using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using EZNEW.Framework.Extension;
using System.Linq.Expressions;
using EZNEW.Framework.ExpressionUtil;
using System.Collections.Concurrent;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// entity manager
    /// </summary>
    public static class EntityManager
    {
        #region propertys

        /// <summary>
        /// entity config
        /// key:entity type guid
        /// </summary>
        public static ConcurrentDictionary<Guid, EntityConfig> EntityConfigs
        {
            get; private set;
        } = new ConcurrentDictionary<Guid, EntityConfig>();

        #endregion

        #region methods

        #region entity config

        #region config entity

        /// <summary>
        /// config entity
        /// </summary>
        /// <typeparam name="T">entty type</typeparam>
        internal static void ConfigEntity<T>() where T : BaseEntity<T>, new()
        {
            var type = typeof(T);
            ConfigEntity(type);
        }

        /// <summary>
        /// config entity
        /// </summary>
        /// <param name="type">entity type</param>
        internal static void ConfigEntity(Type type)
        {
            if (type == null)
            {
                return;
            }
            var typeGuid = type.GUID;
            if (EntityConfigs.ContainsKey(typeGuid))
            {
                return;
            }
            var entityAttribute = (type.GetCustomAttributes(typeof(EntityAttribute), false)?.FirstOrDefault()) as EntityAttribute;
            if (entityAttribute == null)
            {
                return;
            }
            var propertys = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            string objectName = entityAttribute.ObjectName.IsNullOrEmpty() ? type.Name : entityAttribute.ObjectName;
            if (!EntityConfigs.TryGetValue(typeGuid, out EntityConfig entityConfig))
            {
                entityConfig = new EntityConfig();
            }
            //table name
            if (entityConfig.TableName.IsNullOrEmpty())
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
                    if (!entityFieldAttribute.Name.IsNullOrEmpty())
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
                    IsVersion = isVersion
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
            ConfigEntity(type.GUID, entityConfig);
        }

        /// <summary>
        /// config entity
        /// </summary>
        /// <param name="entityConfig">entity config</param>
        internal static void ConfigEntity(string typeAssemblyQualifiedName)
        {
            var type = Type.GetType(typeAssemblyQualifiedName);
            ConfigEntity(type);
        }

        /// <summary>
        /// config entity
        /// </summary>
        /// <param name="typeGuid">type guid</param>
        /// <param name="entityConfig">entity config</param>
        static void ConfigEntity(Guid typeGuid, EntityConfig entityConfig)
        {
            if (entityConfig == null)
            {
                return;
            }
            EntityConfigs[typeGuid] = entityConfig;
        }

        #endregion

        #region get entity config

        /// <summary>
        /// get entity config
        /// </summary>
        /// <param name="type">entity type</param>
        /// <returns></returns>
        public static EntityConfig GetEntityConfig(Type type)
        {
            if (type == null)
            {
                return null;
            }
            var typeGuid = type.GUID;
            EntityConfigs.TryGetValue(typeGuid, out var entityConfig);
            if (entityConfig == null)
            {
                ConfigEntity(type);
                EntityConfigs.TryGetValue(typeGuid, out entityConfig);
            }
            return entityConfig;
        }

        /// <summary>
        /// get entity config
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static EntityConfig GetEntityConfig<T>()
        {
            return GetEntityConfig(typeof(T));
        }

        /// <summary>
        /// get entity config
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">AssemblyQualifiedName</param>
        /// <returns></returns>
        public static EntityConfig GetEntityConfig(string typeAssemblyQualifiedName)
        {
            if (typeAssemblyQualifiedName.IsNullOrEmpty())
            {
                return null;
            }
            return GetEntityConfig(Type.GetType(typeAssemblyQualifiedName));
        }

        #endregion 

        #endregion

        #region object name

        #region config object name

        /// <summary>
        /// config entity object name
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="objectName"></param>
        public static void ConfigObjectName<T>(string objectName)
        {
            if (objectName.IsNullOrEmpty())
            {
                return;
            }
            var type = typeof(T);
            ConfigObjectName(type, objectName);
        }

        /// <summary>
        /// config entity object name
        /// </summary>
        /// <param name="type">entity type</param>
        /// <param name="objectName">object name</param>
        public static void ConfigObjectName(Type type, string objectName)
        {
            if (type == null)
            {
                return;
            }
            var entityConfig = GetEntityConfig(type);
            if (entityConfig != null)
            {
                entityConfig.TableName = objectName;
            }
        }

        /// <summary>
        /// config entity object name
        /// </summary>
        /// <param name="type">entity type</param>
        /// <param name="objectName">object name</param>
        public static void ConfigObjectName(string typeAssemblyQualifiedName, string objectName)
        {
            var type = Type.GetType(typeAssemblyQualifiedName);
            ConfigObjectName(type, objectName);
        }

        #endregion

        #region get object name

        /// <summary>
        /// get entity object name
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">entity type full name</param>
        /// <returns></returns>
        public static string GetEntityObjectName(string typeAssemblyQualifiedName)
        {
            if (typeAssemblyQualifiedName.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var type = Type.GetType(typeAssemblyQualifiedName);
            return GetEntityObjectName(type);
        }

        /// <summary>
        /// get entity object name
        /// </summary>
        /// <param name="type">entity type</param>
        /// <returns></returns>
        public static string GetEntityObjectName(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            var entityConfig = GetEntityConfig(type);
            return entityConfig?.TableName ?? string.Empty;
        }

        /// <summary>
        /// get entity object name
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns></returns>
        public static string GetEntityObjectName<T>()
        {
            return GetEntityObjectName(typeof(T));
        }

        #endregion

        #endregion

        #region fields

        #region set fields

        /// <summary>
        /// set fields
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="fields">fields</param>
        public static void SetFields(Type type, IEnumerable<EntityField> fields)
        {
            if (type == null)
            {
                return;
            }
            var entityConfig = GetEntityConfig(type);
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

        #region get fields

        /// <summary>
        /// get fields
        /// </summary>
        /// <param name="type">entity type</param>
        /// <returns></returns>
        public static List<EntityField> GetFields(Type type)
        {
            var entityConfig = GetEntityConfig(type);
            return entityConfig?.AllFields ?? new List<EntityField>(0);
        }

        #endregion

        #region get query fields

        /// <summary>
        /// get entity query fields
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">entity type full name</param>
        /// <returns></returns>
        public static List<EntityField> GetEntityQueryFields(string typeAssemblyQualifiedName)
        {
            if (typeAssemblyQualifiedName.IsNullOrEmpty())
            {
                return new List<EntityField>(0);
            }
            var type = Type.GetType(typeAssemblyQualifiedName);
            return GetEntityQueryFields(type);
        }

        /// <summary>
        /// get entity query fields
        /// </summary>
        /// <param name="type">entity type</param>
        /// <returns></returns>
        public static List<EntityField> GetEntityQueryFields(Type type, IEnumerable<string> queryPropertyNames = null, bool forcePrimaryKey = false, bool forceVersionKey = false)
        {
            var entityConfig = GetEntityConfig(type);
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
        /// get entity query fields
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns></returns>
        public static List<EntityField> GetEntityQueryFields<T>()
        {
            return GetEntityQueryFields(typeof(T));
        }

        #endregion

        #endregion

        #region primary key

        #region set primary keys 

        /// <summary>
        /// Set Primary Key
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="keyNames">key name</param>
        public static void SetPrimaryKey(Type type, params string[] keyNames)
        {
            if (type == null || keyNames.IsNullOrEmpty())
            {
                return;
            }
            var entityConfig = GetEntityConfig(type);
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
        /// Set Primary Key
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">type full name</param>
        /// <param name="keyNames">key name</param>
        public static void SetPrimaryKey(string typeAssemblyQualifiedName, params string[] keyNames)
        {
            if (string.IsNullOrWhiteSpace(typeAssemblyQualifiedName) || keyNames == null || keyNames.Length <= 0)
            {
                return;
            }
            var type = Type.GetType(typeAssemblyQualifiedName);
            SetPrimaryKey(type);
        }

        /// <summary>
        /// Set Primary Key
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="fields">field expression</param>
        public static void SetPrimaryKey<T>(params Expression<Func<T, dynamic>>[] fields)
        {
            if (fields == null)
            {
                return;
            }
            SetPrimaryKey(typeof(T), fields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body)).ToArray());
        }

        #endregion

        #region get primary keys

        /// <summary>
        /// Get Primary Key
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">type name</param>
        /// <returns></returns>
        public static List<EntityField> GetPrimaryKeys(string typeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(typeAssemblyQualifiedName))
            {
                return null;
            }
            var type = Type.GetType(typeAssemblyQualifiedName);
            return GetPrimaryKeys(type);
        }

        /// <summary>
        /// Get Primary Key
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public static List<EntityField> GetPrimaryKeys(Type type)
        {
            if (type == null)
            {
                return new List<EntityField>();
            }
            var entityConfig = GetEntityConfig(type);
            return entityConfig?.PrimaryKeys;
        }

        /// <summary>
        /// Get Primary Key
        /// </summary>
        /// <returns></returns>
        public static List<EntityField> GetPrimaryKeys<ET>()
        {
            return GetPrimaryKeys(typeof(ET));
        }

        /// <summary>
        /// property is primary key
        /// </summary>
        /// <typeparam name="ET">entity type</typeparam>
        /// <param name="propertyName">property name</param>
        /// <returns></returns>
        public static bool IsPrimaryKey<ET>(string propertyName)
        {
            return IsPrimaryKey(typeof(ET), propertyName);
        }

        /// <summary>
        /// property is primary key
        /// </summary>
        /// <param name="type">entity type</param>
        /// <param name="propertyName">property name</param>
        /// <returns></returns>
        public static bool IsPrimaryKey(Type type, string propertyName)
        {
            if (type == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }
            var primaryKeys = GetPrimaryKeys(type);
            return primaryKeys?.Exists(r => r == propertyName) ?? false;
        }

        #endregion

        #endregion

        #region version field

        #region set version field

        /// <summary>
        /// Set Version Field
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">type name</param>
        /// <param name="fieldName">field name</param>
        public static void SetVersionField(string typeAssemblyQualifiedName, string fieldName)
        {
            var type = Type.GetType(typeAssemblyQualifiedName);
            SetVersionField(type, fieldName);
        }

        /// <summary>
        /// Set Version Field
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="fieldName">field name</param>
        public static void SetVersionField(Type type, string fieldName)
        {
            if (type == null)
            {
                return;
            }
            var entityConfig = GetEntityConfig(type);
            if (entityConfig != null)
            {
                entityConfig.VersionField = fieldName;
            }
        }

        /// <summary>
        /// Set Version Field
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="field">field expression</param>
        public static void SetVersionField<T>(Expression<Func<T, dynamic>> field)
        {
            if (field == null)
            {
                return;
            }
            SetVersionField(typeof(T), ExpressionHelper.GetExpressionPropertyName(field.Body));
        }

        #endregion

        #region get version field

        /// <summary>
        /// Get Version Field
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">type name</param>
        /// <returns></returns>
        public static string GetVersionField(string typeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(typeAssemblyQualifiedName))
            {
                return string.Empty;
            }
            var type = Type.GetType(typeAssemblyQualifiedName);
            return GetVersionField(type);
        }

        /// <summary>
        /// Get Version Field
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public static string GetVersionField(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            var entityConfig = GetEntityConfig(type);
            return entityConfig?.VersionField;
        }

        #endregion

        #endregion

        #region refreshdate field

        #region set refreshdate field

        /// <summary>
        /// Set RefreshDate Field
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">type name</param>
        /// <param name="fieldName">field name</param>
        public static void SetRefreshDateField(string typeAssemblyQualifiedName, string fieldName)
        {
            var type = Type.GetType(typeAssemblyQualifiedName);
            SetRefreshDateField(type, fieldName);
        }

        /// <summary>
        /// Set RefreshDate Field
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="fieldName">field name</param>
        public static void SetRefreshDateField(Type type, string fieldName)
        {
            if (type == null)
            {
                return;
            }
            var entityConfig = GetEntityConfig(type);
            if (entityConfig != null)
            {
                entityConfig.RefreshDateField = fieldName;
            }
        }

        /// <summary>
        /// Set RefreshDate Field
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="field">field expression</param>
        public static void SetRefreshDateField<T>(Expression<Func<T, dynamic>> field)
        {
            if (field == null)
            {
                return;
            }
            SetRefreshDateField(typeof(T), ExpressionHelper.GetExpressionPropertyName(field.Body));
        }

        #endregion

        #region get refreshdate field

        /// <summary>
        /// Get RefreshDate Field
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">type name</param>
        /// <returns></returns>
        public static string GetRefreshDateField(string typeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(typeAssemblyQualifiedName))
            {
                return string.Empty;
            }
            var type = Type.GetType(typeAssemblyQualifiedName);
            return GetRefreshDateField(type);
        }

        /// <summary>
        /// Get RefreshDate Field
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public static string GetRefreshDateField(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            var entityConfig = GetEntityConfig(type);
            return entityConfig?.RefreshDateField;
        }

        #endregion 

        #endregion

        #region relation fields

        /// <summary>
        /// get relation fields
        /// </summary>
        /// <param name="sourceType">source type</param>
        /// <param name="relationType">relation type</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetRelationFields(Type sourceType, Type relationType)
        {
            if (sourceType == null || relationType == null)
            {
                return new Dictionary<string, string>(0);
            }
            var entityConfig = GetEntityConfig(sourceType);
            Dictionary<string, string> relationFields = null;
            entityConfig?.RelationFields?.TryGetValue(relationType.GUID, out relationFields);
            return relationFields ?? new Dictionary<string, string>(0);
        }

        #endregion

        #endregion
    }
}
