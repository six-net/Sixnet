using System;
using System.Collections.Generic;
using System.Linq;
using static EZNEW.Dapper.SqlMapper;
using EZNEW.Develop.Command;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Base entity
    /// </summary>
    public abstract class BaseEntity<T> where T : BaseEntity<T>, new()
    {
        protected Dictionary<string, dynamic> valueDict = new Dictionary<string, dynamic>();//field values
        string identityValue = string.Empty;
        bool loadedIdentityValue = false;
        protected static Type entityType = typeof(T);

        static BaseEntity()
        {
            EntityManager.ConfigureEntity<T>();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the query data total count
        /// </summary>
        protected int QueryDataTotalCount
        {
            get; set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the modifed values
        /// </summary>
        /// <returns>Return the modify values</returns>
        internal Dictionary<string, dynamic> GetModifyValues(T oldValue)
        {
            if (oldValue == null)
            {
                return valueDict;
            }
            var oldValues = oldValue.GetAllPropertyValues();
            if (oldValues.IsNullOrEmpty())
            {
                return valueDict;
            }
            Dictionary<string, dynamic> modifyValues = new Dictionary<string, dynamic>(valueDict.Count);
            var versionField = EntityManager.GetVersionField(typeof(T));
            foreach (var valueItem in valueDict)
            {
                if (valueItem.Key != versionField && (!oldValues.ContainsKey(valueItem.Key) || oldValues[valueItem.Key] != valueItem.Value))
                {
                    modifyValues.Add(valueItem.Key, valueItem.Value);
                }
            }
            return modifyValues;
        }

        /// <summary>
        /// Gets the object primary key value
        /// </summary>
        /// <returns>Return the primary key values</returns>
        internal Dictionary<string, dynamic> GetPrimaryKeyValues()
        {
            var primaryKeys = EntityManager.GetPrimaryKeys(typeof(T));
            if (primaryKeys == null || primaryKeys.Count <= 0)
            {
                return new Dictionary<string, dynamic>(0);
            }
            Dictionary<string, dynamic> values = new Dictionary<string, dynamic>(primaryKeys.Count);
            foreach (var key in primaryKeys)
            {
                if (valueDict.ContainsKey(key))
                {
                    values.Add(key, valueDict[key]);
                }
            }
            return values;
        }

        /// <summary>
        /// Compare two objects determine whether is equal
        /// </summary>
        /// <param name="obj">Compare object</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            BaseEntity<T> targetObj = obj as BaseEntity<T>;
            if (targetObj == null)
            {
                return false;
            }
            var myIdentity = GetIdentityValue();
            var targetIdentity = targetObj.GetIdentityValue();
            return myIdentity == targetIdentity;
        }

        /// <summary>
        /// Gets the identity value
        /// </summary>
        /// <returns>Return identity value</returns>
        public virtual string GetIdentityValue()
        {
            if (loadedIdentityValue)
            {
                return identityValue;
            }
            var primaryValues = GetPrimaryKeyValues();
            identityValue = primaryValues.IsNullOrEmpty() ? Guid.NewGuid().ToString() : string.Join("_", primaryValues.Values.OrderBy(c => c?.ToString() ?? string.Empty));
            loadedIdentityValue = true;
            return identityValue;
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Return the hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets data total count
        /// </summary>
        /// <returns>Return total count</returns>
        public int GetTotalCount()
        {
            return QueryDataTotalCount;
        }

        /// <summary>
        /// Gets the property name
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return property value</returns>
        public dynamic GetPropertyValue(string propertyName)
        {
            if (valueDict.ContainsKey(propertyName))
            {
                return valueDict[propertyName];
            }
            throw new Exception("error property");
        }

        /// <summary>
        /// Gets the property value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return the property value</returns>
        public TValue GetPropertyValue<TValue>(string propertyName)
        {
            return valueDict.GetValue<TValue>(propertyName);
        }

        /// <summary>
        /// Sets the property value
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value</param>
        public void SetPropertyValue(string propertyName, dynamic value)
        {
            IEnumerableExtensions.SetValue(valueDict, propertyName, value);
            if (loadedIdentityValue && EntityManager.IsPrimaryKey(entityType, propertyName))
            {
                loadedIdentityValue = false;
            }
        }

        /// <summary>
        /// Gets all property values
        /// </summary>
        /// <returns>Return all property values</returns>
        public Dictionary<string, dynamic> GetAllPropertyValues()
        {
            return valueDict;
        }

        /// <summary>
        /// Get cmd parameters
        /// </summary>
        /// <returns></returns>
        public CommandParameters GetCommandParameters()
        {
            var parameters = new CommandParameters();
            var propertys = EntityManager.GetFields(entityType);
            foreach (var property in propertys)
            {
                var value = GetPropertyValue(property.PropertyName);
                var dbType = LookupDbType(property.DataType, property.PropertyName, false, out ITypeHandler handler);
                parameters.Add(property.PropertyName, value, dbType: dbType);
            }
            return parameters;
        }

        /// <summary>
        /// Copy new object by identity
        /// </summary>
        /// <returns>Return a new entity object</returns>
        public T CopyOnlyWithIdentity()
        {
            var newData = new T();
            var primaryKeys = EntityManager.GetPrimaryKeys<T>();
            if (primaryKeys.IsNullOrEmpty())
            {
                return newData;
            }
            foreach (var pk in primaryKeys)
            {
                var value = GetPropertyValue(pk.PropertyName);
                newData.SetPropertyValue(pk.PropertyName, value);
            }
            return newData;
        }

        /// <summary>
        /// Copy a new object
        /// </summary>
        /// <returns>Return a new entity object</returns>
        public T Copy()
        {
            var newData = new T();
            newData.valueDict = valueDict.Select(c => c).ToDictionary(c => c.Key, c => c.Value);
            newData.QueryDataTotalCount = QueryDataTotalCount;
            newData.loadedIdentityValue = loadedIdentityValue;
            newData.identityValue = identityValue;
            return newData;
        }

        #endregion
    }
}
