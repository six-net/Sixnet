// "Company © 2025. All rights reserved."

using System.Text;

using Sixnet.Cache.Provider.Memory;

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache options
    /// </summary>
    public class SixnetCacheOptions
    {
        #region Fields

        readonly Dictionary<SixnetCacheServerType, ISixnetCacheProvider> _providers = new();
        internal static SixnetCacheServer DefaultInMemoryServer = new()
        {
            Name = "SIXNET_DEFAULT_IN_MEMORY_SERVER_NAME",
            Type = SixnetCacheServerType.InMemory
        };
        internal static MemoryProvider _defaultMemoryProvider = new();

        /// <summary>
        /// Get cache servers func
        /// </summary>
        Func<ISixnetCacheParameter, SixnetCacheServer> _getCacheServersFunc;

        /// <summary>
        /// Get global cache key prefixs func
        /// </summary>
        Func<List<string>> _getGlobalCacheKeyPrefixsFunc;

        /// <summary>
        /// Get cache object prefixs func
        /// </summary>
        Func<SixnetCacheObject, List<string>> _getCacheObjectPrefixsFunc;

        #endregion

        #region Properties

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
        public SixnetCacheServer Server { get; set; }

        /// <summary>
        /// Whether lowercase key
        /// </summary>
        public bool LowercaseKey { get; set; } = true;

        /// <summary>
        /// Whether use in memory for default
        /// </summary>
        public bool UseInMemoryForDefault { get; set; } = true;

        #endregion

        #region Methods

        #region Provider

        /// <summary>
        /// Add cache provider
        /// </summary>
        /// <param name="serverType">Cache server type</param>
        /// <param name="cacheProvider">Cache provider</param>
        public void AddCacheProvider(SixnetCacheServerType serverType, ISixnetCacheProvider cacheProvider)
        {
            if (cacheProvider != null)
            {
                _providers[serverType] = cacheProvider;
            }
        }

        /// <summary>
        /// Get cache provider
        /// </summary>
        /// <param name="serverType">Server type</param>
        /// <returns>Return cache provider</returns>
        public ISixnetCacheProvider GetCacheProvider(SixnetCacheServerType serverType)
        {
            _providers.TryGetValue(serverType, out var provider);
            if (provider == null && serverType == SixnetCacheServerType.InMemory)
            {
                provider = _defaultMemoryProvider;
            }
            return provider;
        }

        #endregion

        #region Cache server

        /// <summary>
        /// Configure cache server
        /// </summary>
        /// <param name="configure">Configure</param>
        public void ConfigureCacheServer(Func<ISixnetCacheParameter, SixnetCacheServer> configure)
        {
            _getCacheServersFunc = configure;
        }

        /// <summary>
        /// Get cache server
        /// </summary>
        /// <param name="parameter">Cache parameter</param>
        /// <returns>Return cache server</returns>
        public SixnetCacheServer GetCacheServer<T>(SixnetCacheParameter<T> parameter) where T : SixnetCacheResult, new()
        {
            return _getCacheServersFunc?.Invoke(parameter) ?? Server;
        }

        #endregion

        #region Key prefixs

        /// <summary>
        /// Configure global prefixs
        /// </summary>
        /// <param name="configure"></param>
        public void ConfigureGlobalPrefixs(Func<List<string>> configure)
        {
            _getGlobalCacheKeyPrefixsFunc = configure;
        }

        /// <summary>
        /// Get global cache key prefixs
        /// </summary>
        /// <returns>Return global cache key prefixs</returns>
        public List<string> GetGlobalPrefixs()
        {
            return _getGlobalCacheKeyPrefixsFunc?.Invoke() ?? new List<string>(0);
        }

        /// <summary>
        /// Configure cache object prefixs
        /// </summary>
        /// <param name="configure"></param>
        public void ConfigureCacheObjectPrefixs(Func<SixnetCacheObject, List<string>> configure)
        {
            _getCacheObjectPrefixsFunc = configure;
        }

        /// <summary>
        /// Get cache object prefixs
        /// </summary>
        /// <param name="cacheObject">Cache object</param>
        /// <returns>Return cache object prefixs</returns>
        public List<string> GetCacheObjectPrefixs(SixnetCacheObject cacheObject)
        {
            return _getCacheObjectPrefixsFunc?.Invoke(cacheObject) ?? new List<string>(0);
        }

        #endregion

        #endregion
    }
}
