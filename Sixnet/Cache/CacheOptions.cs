using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache options
    /// </summary>
    public class CacheOptions
    {
        #region Fields

        readonly Dictionary<CacheServerType, ISixnetCacheProvider> _providers = new();
        internal CacheServer DefaultInMemoryServer = new()
        {
            Name = "SIXNET_DEFAULT_IN_MEMORY_SERVER_NAME",
            Type = CacheServerType.InMemory
        };

        #endregion

        #region Properties

        /// <summary>
        /// Get cache servers operation func
        /// </summary>
        public Func<ISixnetCacheParameter, CacheServer> GetCacheServersFunc { get; set; }

        /// <summary>
        /// Get global cache key prefixs func
        /// </summary>
        public Func<List<string>> GetGlobalCacheKeyPrefixsFunc { get; set; }

        /// <summary>
        /// Get cache object prefixs func
        /// </summary>
        public Func<CacheObject, List<string>> GetCacheObjectPrefixsFunc { get; set; }

        /// <summary>
        /// Gets or sets the each key name split char
        /// </summary>
        public string KeyNameSplitChar { get; set; } = ":";

        /// <summary>
        /// Gets or sets the name&value split char
        /// </summary>
        public string NameValueSplitChar { get; set; } = "$";

        /// <summary>
        /// Gets or sets the encoding
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Whether throw exception when not get any database
        /// </summary>
        public bool ThrowOnMissingDatabase { get; set; } = false;

        /// <summary>
        /// Gets or sets the default cache server
        /// </summary>
        public CacheServer Server { get; set; }

        #endregion

        #region Methods

        #region Provider

        /// <summary>
        /// Add cache provider
        /// </summary>
        /// <param name="databaseType">Cache server type</param>
        /// <param name="cacheProvider">Cache provider</param>
        public void AddCacheProvider(CacheServerType databaseType, ISixnetCacheProvider cacheProvider)
        {
            if (cacheProvider != null)
            {
                _providers[databaseType] = cacheProvider;
            }
        }

        /// <summary>
        /// Get cache provider
        /// </summary>
        /// <param name="databaseType">Server type</param>
        /// <returns>Return cache provider</returns>
        public ISixnetCacheProvider GetCacheProvider(CacheServerType databaseType)
        {
            _providers.TryGetValue(databaseType, out var provider);
            return provider;
        }

        #endregion

        #region Cache server

        /// <summary>
        /// Get cache server
        /// </summary>
        /// <param name="operationOptions">Cache operation options</param>
        /// <returns>Return cache server</returns>
        public CacheServer GetCacheServer<T>(CacheParameter<T> operationOptions) where T : CacheResult, new()
        {
            return GetCacheServersFunc?.Invoke(operationOptions) ?? Server;
        }

        #endregion

        #region Key prefixs

        /// <summary>
        /// Get global cache key prefixs
        /// </summary>
        /// <returns>Return global cache key prefixs</returns>
        public List<string> GetGlobalPrefixs()
        {
            return GetGlobalCacheKeyPrefixsFunc?.Invoke() ?? new List<string>(0);
        }

        /// <summary>
        /// Get cache object prefixs
        /// </summary>
        /// <param name="cacheObject">Cache object</param>
        /// <returns>Return cache object prefixs</returns>
        public List<string> GetObjectPrefixs(CacheObject cacheObject)
        {
            return GetCacheObjectPrefixsFunc?.Invoke(cacheObject) ?? new List<string>(0);
        }

        #endregion

        #endregion
    }
}
