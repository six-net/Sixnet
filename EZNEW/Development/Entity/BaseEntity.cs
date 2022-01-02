using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EZNEW.Development.Command;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Query;
using EZNEW.Exceptions;
using EZNEW.Model;
using EZNEW.Serialization;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Base entity
    /// </summary>
    public abstract class BaseEntity<T> : IEntity, IQueryModel<T> where T : BaseEntity<T>, new()
    {
        protected string identityRealValue = string.Empty;
        protected bool loadedIdentityValue = false;
        protected static Type entityType = typeof(T);

        #region Properties

        /// <summary>
        /// Gets or sets the query data total count
        /// </summary>
        [NonData]
        protected int QueryDataTotalCount { get; set; }

        #endregion

        #region Methods

        IEntityPropertyValueProvider GetValueProvider(string propertyName)
        {
            var valueProvider = EntityManager.GetEntityField(entityType, propertyName)?.ValueProvider;
            if (valueProvider == null)
            {
                throw new EZNEWException($"{entityType.FullName} => {propertyName}'s value provider is null");
            }
            return valueProvider;
        }

        /// <summary>
        /// Gets the modified values
        /// </summary>
        /// <returns>Return the modify values</returns>
        internal Dictionary<string, dynamic> GetModifiedValues(T oldData)
        {
            var valueDict = GetAllValues();
            if (oldData == null)
            {
                return valueDict;
            }
            var oldValues = oldData.GetAllValues();
            if (oldValues.IsNullOrEmpty())
            {
                return valueDict;
            }
            Dictionary<string, dynamic> modificationValues = new Dictionary<string, dynamic>(valueDict.Count);
            var versionField = EntityManager.GetVersionField(typeof(T));
            foreach (var valueItem in valueDict)
            {
                var field = EntityManager.GetEntityField(entityType, valueItem.Key);
                if (field?.DataType == null)
                {
                    continue;
                }
                if (valueItem.Key != versionField)
                {
                    var oldValue = oldValues[valueItem.Key];
                    var newValue = valueItem.Value;
                    bool isValueType = field.DataType.IsValueType || typeof(string).IsAssignableFrom(field.DataType);
                    bool modification = !oldValues.ContainsKey(valueItem.Key)
                        || (isValueType ? oldValue != newValue
                        : field.DataType.IsSerializable ? BinarySerializer.SerializeObjectToString(oldValue) != BinarySerializer.SerializeObjectToString(newValue) : oldValue != newValue);
                    if (modification)
                    {
                        modificationValues.Add(valueItem.Key, valueItem.Value);
                    }
                }
            }
            return modificationValues;
        }

        /// <summary>
        /// Gets the object primary key value
        /// </summary>
        /// <returns>Return the primary key values</returns>
        internal Dictionary<string, dynamic> GetPrimaryKeyValues()
        {
            var primaryKeys = EntityManager.GetPrimaryKeys(typeof(T));
            var keysCount = primaryKeys.GetCount();
            if (primaryKeys == null || keysCount <= 0)
            {
                return new Dictionary<string, dynamic>(0);
            }
            Dictionary<string, dynamic> values = new Dictionary<string, dynamic>(keysCount);
            foreach (var key in primaryKeys)
            {
                values.Add(key, GetValue(key));
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
            if (this == obj)
            {
                return true;
            }
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
                return identityRealValue;
            }
            var primaryValues = GetPrimaryKeyValues();
            identityRealValue = primaryValues.IsNullOrEmpty() ? Guid.NewGuid().ToString() : string.Join("_", primaryValues.Values.OrderBy(c => c?.ToString() ?? string.Empty));
            loadedIdentityValue = true;
            return identityRealValue;
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
        /// Gets the property or field name
        /// </summary>
        /// <param name="name">Property or field name</param>
        /// <returns>Return the value</returns>
        public dynamic GetValue(string name)
        {
            var valueProvider = GetValueProvider(name);
            return valueProvider.Get(this);
        }

        /// <summary>
        /// Gets the property or field name
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="name">Property or field name</param>
        /// <returns>Return the value</returns>
        public TValue GetValue<TValue>(string name)
        {
            var value = GetValue(name);
            if (value is TValue)
            {
                return value;
            }
            return DataConverter.Convert<TValue>(value);
        }

        /// <summary>
        /// Sets the property or field value
        /// </summary>
        /// <param name="name">Property or field name</param>
        /// <param name="value">Value</param>
        public void SetValue(string name, dynamic value)
        {
            var valueProvider = GetValueProvider(name);
            valueProvider.Set(this, value);
            if (loadedIdentityValue && EntityManager.IsPrimaryKey(entityType, name))
            {
                loadedIdentityValue = false;
            }
        }

        /// <summary>
        /// Gets all property or field values
        /// </summary>
        /// <returns>Return all property values</returns>
        public Dictionary<string, dynamic> GetAllValues()
        {
            var entityConfig = EntityManager.GetEntityConfiguration(entityType);
            if (entityConfig == null)
            {
                throw new EZNEWException($"{entityType.FullName}'s configuration is null");
            }
            var allValues = new Dictionary<string, dynamic>(entityConfig.AllFields.Count);
            foreach (var field in entityConfig.AllFields)
            {
                if (field.Value?.ValueProvider == null)
                {
                    throw new EZNEWException($"{entityType.FullName} => {field.Key}'s value provider is null");
                }
                allValues[field.Key] = field.Value.ValueProvider.Get(this);
            }
            return allValues;
        }

        /// <summary>
        /// Get cmd parameters
        /// </summary>
        /// <returns></returns>
        public CommandParameters GetCommandParameters()
        {
            var parameters = new CommandParameters();
            var entityConfig = EntityManager.GetEntityConfiguration(entityType);
            if (entityConfig == null)
            {
                return parameters;
            }
            foreach (var fieldItem in entityConfig.AllFields)
            {
                var value = GetValue(fieldItem.Key);
                parameters.Add(fieldItem.Key, value);
            }
            return parameters;
        }

        /// <summary>
        /// Clone new object by identity
        /// </summary>
        /// <returns>Return a new entity object</returns>
        public T CloneOnlyWithIdentity()
        {
            var newData = new T();
            var primaryKeys = EntityManager.GetPrimaryKeys<T>();
            if (primaryKeys.IsNullOrEmpty())
            {
                return newData;
            }
            foreach (var key in primaryKeys)
            {
                var value = GetValue(key);
                newData.SetValue(key, value);
            }
            return newData;
        }

        /// <summary>
        /// Clone a new object
        /// </summary>
        /// <returns>Return a new entity object</returns>
        public T Clone()
        {
            var newData = new T
            {
                QueryDataTotalCount = QueryDataTotalCount,
                loadedIdentityValue = loadedIdentityValue,
                identityRealValue = identityRealValue
            };
            var allValues = GetAllValues();
            foreach (var item in allValues)
            {
                newData.SetValue(item.Key, item.Value);
            }
            return newData;
        }

        #endregion
    }
}
