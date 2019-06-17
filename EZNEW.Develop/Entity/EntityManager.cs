using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using EZNEW.Framework.Extension;
using System.Linq.Expressions;
using EZNEW.Framework.ExpressionUtil;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// entity manager
    /// </summary>
    public class EntityManager
    {
        #region Propertys

        /// <summary>
        /// entity config
        /// key:entity type guid
        /// </summary>
        public static Dictionary<Guid, EntityConfig> EntityConfigs
        {
            get; private set;
        } = new Dictionary<Guid, EntityConfig>();

        /// <summary>
        /// alread config entitys
        /// </summary>
        static HashSet<Guid> AlreadConfigEntitys = new HashSet<Guid>();

        #endregion

        #region Methods

        #region config entity

        /// <summary>
        /// config entity
        /// </summary>
        /// <typeparam name="T">entty type</typeparam>
        public static void ConfigEntity<T>()
        {
            var type = typeof(T);
            ConfigEntity(type);
        }

        /// <summary>
        /// config entity
        /// </summary>
        /// <param name="type">entity type</param>
        public static void ConfigEntity(Type type)
        {
            if (type == null)
            {
                return;
            }
            var typeGuid = type.GUID;
            if (AlreadConfigEntitys.Contains(typeGuid))
            {
                return;
            }
            AlreadConfigEntitys.Add(typeGuid);
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
            List<EntityField> editFields = new List<EntityField>();
            List<EntityField> queryFields = new List<EntityField>();
            List<EntityField> allFields = new List<EntityField>();
            List<EntityField> primaryKeys = new List<EntityField>();
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
                }
                var propertyField = new EntityField()
                {
                    FieldName = name,
                    PropertyName = propertyName,
                    QueryFormat = queryFormat
                };
                allFields.Add(propertyField);
                if (primaryKey)
                {
                    primaryKeys.Add(propertyField);
                }
                if (!disableQuery)
                {
                    queryFields.Add(propertyField);
                }
                if (!disableEdit)
                {
                    editFields.Add(propertyField);
                }
                if (isVersion)
                {
                    versionField = propertyField;
                }
                if (refreshDate)
                {
                    refreshDateField = propertyField;
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
            if (entityConfig.EditFields.IsNullOrEmpty())
            {
                entityConfig.EditFields = editFields;
            }
            if (entityConfig.QueryFields.IsNullOrEmpty())
            {
                entityConfig.QueryFields = queryFields;
            }
            if (entityConfig.VersionField == null)
            {
                entityConfig.VersionField = versionField;
            }
            if (entityConfig.RefreshDateField == null)
            {
                entityConfig.RefreshDateField = refreshDateField;
            }
            ConfigEntity(type.GUID, entityConfig);
        }

        /// <summary>
        /// config entity
        /// </summary>
        /// <param name="entityConfig">entity config</param>
        public static void ConfigEntity(string typeAssemblyQualifiedName)
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
            if (EntityConfigs.ContainsKey(typeGuid))
            {
                EntityConfigs[typeGuid] = entityConfig;
            }
            else
            {
                EntityConfigs.Add(typeGuid, entityConfig);
            }
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
            EntityConfigs.TryGetValue(typeGuid, out EntityConfig entityConfig);
            if (entityConfig == null && !AlreadConfigEntitys.Contains(typeGuid))
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
            ConfigObjectName(type.GUID, objectName);
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

        /// <summary>
        /// config entity object name
        /// </summary>
        /// <param name="typeGuid">type guid</param>
        /// <param name="objectName">object name</param>
        static void ConfigObjectName(Guid typeGuid, string objectName)
        {
            if (objectName.IsNullOrEmpty())
            {
                return;
            }
            if (EntityConfigs.TryGetValue(typeGuid, out EntityConfig entityConfig))
            {
                entityConfig = new EntityConfig();
            }
            entityConfig.TableName = objectName;
            ConfigEntity(typeGuid, entityConfig);
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

        #region config edit fields

        /// <summary>
        /// config edit fields
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="fields">edit fields</param>
        public static void ConfigEntityEditFields<T>(IEnumerable<EntityField> fields)
        {
            ConfigEntityEditFields(typeof(T), fields);
        }

        /// <summary>
        /// config edit fields
        /// </summary>
        /// <param name="type">entity type</param>
        /// <param name="fields">edit fields</param>
        public static void ConfigEntityEditFields(Type type, IEnumerable<EntityField> fields)
        {
            if (type == null)
            {
                return;
            }
            ConfigEntityEditFields(type.GUID, fields);
        }

        /// <summary>
        /// config edit fields
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">type full name</param>
        /// <param name="fields">edit fields</param>
        public static void ConfigEntityEditFields(string typeAssemblyQualifiedName, IEnumerable<EntityField> fields)
        {
            var type = Type.GetType(typeAssemblyQualifiedName);
            ConfigEntityEditFields(type, fields);
        }

        /// <summary>
        /// config entity edit fields
        /// </summary>
        /// <param name="typeGuid">type guid</param>
        /// <param name="fields">fields</param>
        static void ConfigEntityEditFields(Guid typeGuid, IEnumerable<EntityField> fields)
        {
            if (fields.IsNullOrEmpty())
            {
                return;
            }
            if (EntityConfigs.TryGetValue(typeGuid, out EntityConfig entityConfig))
            {
                entityConfig = new EntityConfig();
            }
            entityConfig.EditFields = fields.ToList();
            ConfigEntity(typeGuid, entityConfig);
        }

        #endregion

        #region get edit fields

        /// <summary>
        /// get entity edit fields
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">entity type full name</param>
        /// <returns></returns>
        public static List<EntityField> GetEntityEditFields(string typeAssemblyQualifiedName)
        {
            if (typeAssemblyQualifiedName.IsNullOrEmpty())
            {
                return new List<EntityField>(0);
            }
            var type = Type.GetType(typeAssemblyQualifiedName);
            return GetEntityEditFields(type);
        }

        /// <summary>
        /// get entity edit fields
        /// </summary>
        /// <param name="type">entity type</param>
        /// <returns></returns>
        public static List<EntityField> GetEntityEditFields(Type type)
        {
            if (type == null)
            {
                return new List<EntityField>(0);
            }
            var entityConfig = GetEntityConfig(type);
            return entityConfig.EditFields ?? new List<EntityField>(0);
        }

        /// <summary>
        /// get entity edit fields
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns></returns>
        public static List<EntityField> GetEntityEditFields<T>()
        {
            return GetEntityEditFields(typeof(T));
        }

        #endregion

        #region config query fields

        /// <summary>
        /// config query fields
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <param name="fields">query fields</param>
        public static void ConfigEntityQueryFields<T>(IEnumerable<EntityField> fields)
        {
            ConfigEntityQueryFields(typeof(T), fields);
        }

        /// <summary>
        /// config query fields
        /// </summary>
        /// <param name="type">entity type</param>
        /// <param name="fields">query fields</param>
        public static void ConfigEntityQueryFields(Type type, IEnumerable<EntityField> fields)
        {
            if (type == null)
            {
                return;
            }
            ConfigEntityQueryFields(type.GUID, fields);
        }

        /// <summary>
        /// config query fields
        /// </summary>
        /// <param name="typeAssemblyQualifiedName">type full name</param>
        /// <param name="fields">query fields</param>
        public static void ConfigEntityQueryFields(string typeAssemblyQualifiedName, IEnumerable<EntityField> fields)
        {
            if (typeAssemblyQualifiedName.IsNullOrEmpty())
            {
                return;
            }
            var type = Type.GetType(typeAssemblyQualifiedName);
            ConfigEntityQueryFields(type, fields);
        }

        /// <summary>
        /// config entity query fields
        /// </summary>
        /// <param name="typeGuid">type guid</param>
        /// <param name="fields">fields</param>
        static void ConfigEntityQueryFields(Guid typeGuid, IEnumerable<EntityField> fields)
        {
            if (fields.IsNullOrEmpty())
            {
                return;
            }
            if (EntityConfigs.TryGetValue(typeGuid, out EntityConfig entityConfig))
            {
                entityConfig = new EntityConfig();
            }
            entityConfig.QueryFields = fields.ToList();
            ConfigEntity(typeGuid, entityConfig);
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
        public static List<EntityField> GetEntityQueryFields(Type type)
        {
            if (type == null)
            {
                return new List<EntityField>(0);
            }
            var entityConfig = GetEntityConfig(type);
            return entityConfig?.QueryFields ?? new List<EntityField>(0);
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
            SetPrimaryKey(type.GUID, keyNames);
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

        /// <summary>
        /// set primary keys
        /// </summary>
        /// <param name="typeGuid">type guid</param>
        /// <param name="keyNames">primary key names</param>
        static void SetPrimaryKey(Guid typeGuid, params string[] keyNames)
        {
            if (keyNames.IsNullOrEmpty())
            {
                return;
            }
            if (EntityConfigs.TryGetValue(typeGuid, out EntityConfig entityConfig))
            {
                entityConfig = new EntityConfig();
            }
            if (entityConfig.PrimaryKeys == null)
            {
                entityConfig.PrimaryKeys = new List<EntityField>(keyNames.Select<string, EntityField>(c => c));
            }
            foreach (string key in keyNames)
            {
                entityConfig.PrimaryKeys.Add(key);
            }
            ConfigEntity(typeGuid, entityConfig);
        }

        #endregion

        #region Get Primary Keys

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

        #endregion

        #region Set Version Field

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
            SetVersionField(type.GUID, fieldName);
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

        /// <summary>
        /// Set Version Field
        /// </summary>
        /// <param name="typeGuid">type guid</param>
        /// <param name="fieldName">field name</param>
        static void SetVersionField(Guid typeGuid, string fieldName)
        {
            if (!EntityConfigs.TryGetValue(typeGuid, out EntityConfig entityConfig))
            {
                entityConfig = new EntityConfig();
            }
            entityConfig.VersionField = fieldName;
            ConfigEntity(typeGuid, entityConfig);
        }

        #endregion

        #region Get Version Field

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

        #region Set RefreshDate Field

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
            SetRefreshDateField(type.GUID, fieldName);
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

        /// <summary>
        /// Set RefreshDate Field
        /// </summary>
        /// <param name="typeGuid">type guid</param>
        /// <param name="fieldName">field name</param>
        static void SetRefreshDateField(Guid typeGuid, string fieldName)
        {
            if (!EntityConfigs.TryGetValue(typeGuid, out EntityConfig entityConfig))
            {
                entityConfig = new EntityConfig();
            }
            entityConfig.RefreshDateField = fieldName;
            ConfigEntity(typeGuid, entityConfig);
        }

        #endregion

        #region Get RefreshDate Field

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

        #region get field

        /// <summary>
        /// get field
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <param name="propertyName">property name</param>
        /// <returns></returns>
        public static EntityField GetField(Type entityType, string propertyName)
        {
            if (entityType == null || propertyName.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var entityConfig = GetEntityConfig(entityType);
            var allFields = entityConfig?.AllFields;
            if (allFields.IsNullOrEmpty())
            {
                return propertyName;
            }
            return allFields.FirstOrDefault(c => c.PropertyName == propertyName) ?? propertyName;
        }

        #endregion

        #region fields

        /// <summary>
        /// fields
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <param name="propertyNames">property names</param>
        /// <returns></returns>
        public static List<EntityField> GetFields(Type entityType, IEnumerable<string> propertyNames)
        {
            if (entityType == null || propertyNames.IsNullOrEmpty())
            {
                return new List<EntityField>(0);
            }
            var entityConfig = GetEntityConfig(entityType);
            var allFields = entityConfig?.AllFields;
            if (allFields.IsNullOrEmpty())
            {
                return new List<EntityField>(0);
            }
            return allFields.Intersect(propertyNames.Select<string, EntityField>(c => c)).ToList();
        }

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
