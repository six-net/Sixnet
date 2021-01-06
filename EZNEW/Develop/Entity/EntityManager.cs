using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using EZNEW.ExpressionUtil;
using EZNEW.Configuration;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Entity manager
    /// </summary>
    public static class EntityManager
    {
        #region Get entity configuration

        /// <summary>
        /// Get entity configuration
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity configuration</returns>
        public static EntityConfiguration GetEntityConfiguration(Type entityType)
        {
            return ConfigurationManager.Entity.GetEntityConfiguration(entityType);
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

        /// <summary>
        /// Get entity configuration
        /// </summary>
        /// <param name="entityTypeId">Entity type id</param>
        /// <returns>Return entity configuration</returns>
        public static EntityConfiguration GetEntityConfiguration(Guid entityTypeId)
        {
            return ConfigurationManager.Entity.GetEntityConfiguration(entityTypeId);
        }

        /// <summary>
        /// Get all entity configurations
        /// </summary>
        /// <returns>Return all entity configurations</returns>
        public static IEnumerable<EntityConfiguration> GetAllEntityConfigurations()
        {
            return ConfigurationManager.Entity.GetAllEntityConfigurations();
        }

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
            ConfigurationManager.Entity.ConfigureObjectName(entityType, objectName);
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
            return ConfigurationManager.Entity.GetEntityObjectName(entityType);
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

        #region Get all fields

        /// <summary>
        /// Get fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity fields</returns>
        public static IEnumerable<string> GetAllFields(Type entityType)
        {
            return ConfigurationManager.Entity.GetAllFields(entityType);
        }

        #endregion

        #region Get query fields

        /// <summary>
        /// Get entity query fields
        /// </summary>
        /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
        /// <returns>Return entity query fields</returns>
        public static IEnumerable<string> GetQueryFields(string entityTypeAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(entityTypeAssemblyQualifiedName))
            {
                return Array.Empty<string>();
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
        public static IEnumerable<string> GetQueryFields(Type entityType)
        {
            return ConfigurationManager.Entity.GetQueryFields(entityType);
        }

        /// <summary>
        /// Get entity query fields
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return entity query fields</returns>
        public static IEnumerable<string> GetQueryFields<TEntity>()
        {
            return GetQueryFields(typeof(TEntity));
        }

        #endregion

        #region Get must query fields

        /// <summary>
        /// Get must query fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity must query fields</returns>
        public static IEnumerable<string> GetMustQueryFields(Type entityType)
        {
            return ConfigurationManager.Entity.GetMustQueryFields(entityType);
        }

        #endregion

        #region Get entity field

        /// <summary>
        /// Get entity field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return entity field</returns>
        internal static EntityField GetEntityField(Type entityType, string propertyName)
        {
            return ConfigurationManager.Entity.GetEntityField(entityType, propertyName);
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
            ConfigurationManager.Entity.AddPrimaryKey(entityType, keyNames);
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
        public static IEnumerable<string> GetPrimaryKeys(string entityTypeAssemblyQualifiedName)
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
        public static IEnumerable<string> GetPrimaryKeys(Type entityType)
        {
            return ConfigurationManager.Entity.GetPrimaryKeys(entityType);
        }

        /// <summary>
        /// Get primary keys
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return all primary key fields</returns>
        public static IEnumerable<string> GetPrimaryKeys<TEntity>()
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
            return ConfigurationManager.Entity.IsPrimaryKey(entityType, propertyName);
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
            ConfigurationManager.Entity.SetVersionField(entityType, fieldName);
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
            return ConfigurationManager.Entity.GetVersionField(entityType);
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
            ConfigurationManager.Entity.SetVersionField(entityType, fieldName);
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
            return ConfigurationManager.Entity.GetRefreshDateField(entityType);
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
            return ConfigurationManager.Entity.GetRelationFields(sourceEntityType, relationEntityType);
        }

        #endregion
    }
}
