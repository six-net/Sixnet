using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EZNEW.Cache
{
    /// <summary>
    /// Cache key
    /// </summary>
    public class CacheKey
    {
        /// <summary>
        /// Create cache key
        /// </summary>
        /// <param name="previousKeys">Previous keys</param>
        public CacheKey(params CacheKey[] previousKeys) : this(null, previousKeys)
        {
        }

        /// <summary>
        /// Create cache key
        /// </summary>
        /// <param name="cacheObject">Cache object</param>
        /// <param name="previousKeys">Previous keys</param>
        public CacheKey(CacheObject cacheObject, params CacheKey[] previousKeys) : this(cacheObject, (IEnumerable<CacheKey>)previousKeys)
        {
        }

        /// <summary>
        /// Create cache key
        /// </summary>
        /// <param name="previousKeys"></param>
        public CacheKey(IEnumerable<CacheKey> previousKeys) : this(null, previousKeys)
        {
        }

        /// <summary>
        /// Create cache keys
        /// </summary>
        /// <param name="cacheObject">Cache object</param>
        /// <param name="previousKeys">Previous keys</param>
        public CacheKey(CacheObject cacheObject, IEnumerable<CacheKey> previousKeys)
        {
            this.cacheObject = cacheObject;
            if (!previousKeys.IsNullOrEmpty())
            {
                foreach (var prevKey in previousKeys)
                {
                    foreach (var prevKeyItem in prevKey.nameValues)
                    {
                        AddName(prevKeyItem.Key, prevKeyItem.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Create cache key
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        public CacheKey(string name, string value = "") : this(null, name, value)
        {
        }

        /// <summary>
        /// Create cache key
        /// </summary>
        /// <param name="cacheObject">Cache object</param>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        public CacheKey(CacheObject cacheObject, string name, string value = "")
        {
            this.cacheObject = cacheObject;
            AddName(name, value);
        }

        #region Fields

        /// <summary>
        /// Key values
        /// </summary>
        readonly SortedDictionary<string, string> nameValues = new SortedDictionary<string, string>();

        /// <summary>
        /// Actual cache key value
        /// </summary>
        protected string actualCacheKey = string.Empty;

        /// <summary>
        /// Whether is generated actual key
        /// </summary>
        protected bool generatedActualKey = false;

        /// <summary>
        /// Cache object
        /// </summary>
        protected CacheObject cacheObject = null;

        #endregion

        #region Methods

        /// <summary>
        /// add name
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>Return the newest cache key</returns>
        public CacheKey AddName(string name, string value = "")
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return this;
            }
            nameValues[name] = value;
            generatedActualKey = false;
            return this;
        }

        /// <summary>
        /// Remove names
        /// </summary>
        /// <param name="names">Cache key names</param>
        /// <returns>Return the newest cache key</returns>
        public CacheKey RemoveNames(params string[] names)
        {
            if (names.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var name in names)
            {
                nameValues.Remove(name);
            }
            generatedActualKey = false;
            return this;
        }

        /// <summary>
        /// Get actual key
        /// </summary>
        /// <returns>Return the actual cache key</returns>
        public string GetActualKey()
        {
            if (generatedActualKey)
            {
                return actualCacheKey;
            }
            List<string> allKeys = new List<string>();

            //global keys
            var globalPrefixs = CacheManager.Configuration.GetGlobalPrefixs();
            if (!globalPrefixs.IsNullOrEmpty())
            {
                allKeys.AddRange(globalPrefixs);
            }
            //object keys
            var objectPrefixs = CacheManager.Configuration.GetObjectPrefixs(cacheObject);
            if (!objectPrefixs.IsNullOrEmpty())
            {
                allKeys.AddRange(objectPrefixs);
            }
            if (!nameValues.IsNullOrEmpty())
            {
                allKeys.AddRange(nameValues.Select(c => string.IsNullOrWhiteSpace(c.Value) ? c.Key : string.Format("{0}{1}{2}", c.Key, CacheManager.Configuration.NameValueSplitWord, c.Value)));
            }
            actualCacheKey = string.Join(CacheManager.Configuration.KeyNameSplitWord, allKeys);
            generatedActualKey = true;
            return actualCacheKey;
        }

        /// <summary>
        /// Implicit convert to string
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        public static implicit operator string(CacheKey cacheKey)
        {
            return cacheKey?.GetActualKey() ?? string.Empty;
        }

        /// <summary>
        /// Implicit convert to cache key
        /// </summary>
        /// <param name="key">key</param>
        public static implicit operator CacheKey(string key)
        {
            var cacheKey = new CacheKey();
            if (string.IsNullOrWhiteSpace(key))
            {
                return cacheKey;
            }
            var keyNameItems = key.LSplit(CacheManager.Configuration.KeyNameSplitWord);
            var globalPrefixs = CacheManager.Configuration.GetGlobalPrefixs();
            if (!globalPrefixs.IsNullOrEmpty())
            {
                keyNameItems = keyNameItems.Except(globalPrefixs).ToArray();
            }
            cacheKey.AddName(key);
            return cacheKey;
        }

        /// <summary>
        /// Get cache key actual value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetActualKey();
        }

        public override bool Equals(object obj)
        {
            var targetObj = obj as CacheKey;
            var targetKey = targetObj?.ToString();
            return ToString() == targetKey;
        }

        public override int GetHashCode()
        {
            return GetActualKey().GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Constant cache key
    /// </summary>
    public class ConstantCacheKey : CacheKey
    {
        /// <summary>
        /// Initialize constant cache key
        /// </summary>
        /// <param name="keyString">key value</param>
        private ConstantCacheKey(string keyString)
        {
            actualCacheKey = keyString;
            generatedActualKey = true;
        }

        /// <summary>
        /// Create a constant cache key
        /// </summary>
        /// <param name="keyString">key value</param>
        /// <returns>Return constant cache key</returns>
        public static ConstantCacheKey Create(string keyString)
        {
            return new ConstantCacheKey(keyString);
        }
    }
}
