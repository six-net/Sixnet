using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Framework.ExpressionUtil;
using EZNEW.Framework.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// Command Entity
    /// </summary>
    public abstract class BaseEntity<T> where T : BaseEntity<T>
    {
        protected Dictionary<string, dynamic> valueDict = new Dictionary<string, dynamic>();//field values
        string identityValue = string.Empty;
        bool loadedIdentityValue = false;
        protected static Type entityType = typeof(T);

        static BaseEntity()
        {
            EntityManager.ConfigEntity<T>();
        }

        #region Propertys

        /// <summary>
        /// Page Count
        /// </summary>
        protected int PagingTotalCount
        {
            get; set;
        }

        #endregion

        #region methods

        /// <summary>
        /// get has modifed values
        /// </summary>
        /// <returns>values</returns>
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
        /// get object primary key value
        /// </summary>
        /// <returns></returns>
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
        /// compare two objects determine whether is equal
        /// </summary>
        /// <param name="obj">compare object</param>
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
        /// get identity value
        /// </summary>
        /// <returns></returns>
        public virtual string GetIdentityValue()
        {
            if (loadedIdentityValue)
            {
                return identityValue;
            }
            var primaryValues = GetPrimaryKeyValues();
            identityValue = primaryValues.IsNullOrEmpty() ? Guid.NewGuid().ToString() : string.Join("_", primaryValues.Values.OrderBy(c => c?.ToString()??string.Empty).ToArray());
            loadedIdentityValue = true;
            return identityValue;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// get primary keys with cache
        /// </summary>
        /// <returns></returns>
        public string GetPrimaryCacheKey(bool includeObjectName = true)
        {
            List<EntityField> primaryKeys = EntityManager.GetPrimaryKeys(typeof(T));
            return GenerateCacheKey(primaryKeys, includeObjectName);
        }

        /// <summary>
        /// get cache keys
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="includeObjectName">whether include object name</param>
        /// <param name="propertys"></param>
        /// <returns></returns>
        public string GetCacheKey(bool includeObjectName = true, params Expression<Func<T, dynamic>>[] propertys)
        {
            if (propertys == null)
            {
                return string.Empty;
            }
            return GetCacheKey(includeObjectName, propertys.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body)).ToArray());
        }

        /// <summary>
        /// get chache keys
        /// </summary>
        /// <param name="includeObjectName">whether include object name</param>
        /// <param name="keys">cache keys</param>
        /// <returns></returns>
        public string GetCacheKey(bool includeObjectName = true, params string[] keys)
        {
            if (keys == null || keys.Length <= 0)
            {
                return string.Empty;
            }
            SortedSet<string> sortedKeys = new SortedSet<string>();
            foreach (string key in keys)
            {
                sortedKeys.Add(key);
            }
            return GenerateCacheKey(keys.Select<string, EntityField>(c => c), includeObjectName);
        }

        /// <summary>
        /// get cache keys
        /// </summary>
        /// <param name="keyAndValues">key and values</param>
        /// <param name="includeObjectName">whether include object name</param>
        /// <returns></returns>
        public static string GetCacheKey(IDictionary<string, dynamic> keyAndValues, bool includeObjectName = true)
        {
            List<string> keys = new List<string>();
            if (includeObjectName)
            {
                Type type = typeof(T);
                string objectName = EntityManager.GetEntityObjectName(type);
                if (!string.IsNullOrWhiteSpace(objectName))
                {
                    keys.Add(objectName);
                }
            }
            if (keyAndValues != null && keyAndValues.Count > 0)
            {
                SortedDictionary<string, dynamic> sortedValues = new SortedDictionary<string, dynamic>();
                foreach (var valItem in keyAndValues)
                {
                    sortedValues.Add(valItem.Key, valItem.Value);
                }
                foreach (var sortValItem in sortedValues)
                {
                    keys.Add(string.Format("{0}${1}", sortValItem.Key, sortValItem.Value));
                }
            }
            return string.Join(":", keys);
        }

        /// <summary>
        /// generate cache key
        /// </summary>
        /// <param name="keys">keys</param>
        /// <param name="includeObjectName">whether include object name</param>
        /// <returns></returns>
        string GenerateCacheKey(IEnumerable<EntityField> keys, bool includeObjectName = true)
        {
            List<string> keyValues = new List<string>();
            var type = typeof(T);
            if (includeObjectName)
            {
                string objectName = EntityManager.GetEntityObjectName(type);
                if (!string.IsNullOrWhiteSpace(objectName))
                {
                    keyValues.Add(objectName);
                }
            }
            if (keys != null)
            {
                foreach (string key in keys)
                {
                    keyValues.Add(string.Format("{0}${1}", key, valueDict[key]));
                }
            }
            return string.Join(":", keyValues);
        }

        /// <summary>
        /// get paging count
        /// </summary>
        /// <returns></returns>
        public int GetPagingTotalCount()
        {
            return PagingTotalCount;
        }

        /// <summary>
        /// get object name
        /// </summary>
        /// <returns></returns>
        public static string GetObjectName()
        {
            Type type = typeof(T);
            return EntityManager.GetEntityObjectName(type);
        }

        /// <summary>
        /// get property name
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns></returns>
        public dynamic GetPropertyValue(string propertyName)
        {
            if (valueDict.ContainsKey(propertyName))
            {
                return valueDict[propertyName];
            }
            throw new Exception("error property");
        }

        /// <summary>
        /// get property value
        /// </summary>
        /// <typeparam name="VT"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public VT GetPropertyValue<VT>(string propertyName)
        {
            return valueDict.GetValue<VT>(propertyName);
        }

        /// <summary>
        /// set property value
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <param name="value">value</param>
        public void SetPropertyValue(string propertyName, dynamic value)
        {
            IEnumerableExtension.SetValue(valueDict, propertyName, value);
            if (loadedIdentityValue && EntityManager.IsPrimaryKey(entityType, propertyName))
            {
                loadedIdentityValue = false;
            }
        }

        /// <summary>
        /// get entity edit fields
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns></returns>
        public static List<EntityField> GetEditFields()
        {
            return EntityManager.GetEntityEditFields<T>();
        }

        /// <summary>
        /// get entity query fields
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <returns></returns>
        public static List<EntityField> GetQueryFields()
        {
            return EntityManager.GetEntityQueryFields<T>();
        }

        /// <summary>
        /// get all property values
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, dynamic> GetAllPropertyValues()
        {
            return valueDict;
        }

        /// <summary>
        /// copy new by identity
        /// </summary>
        /// <returns></returns>
        public T CopyNewByIdentity()
        {
            var newData = (T)this.DeepClone();
            newData.valueDict.Clear();
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

        #endregion
    }
}
