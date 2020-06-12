using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Cache.Command.Result;

namespace EZNEW.Cache
{
    /// <summary>
    /// Cache request option
    /// </summary>
    public abstract class CacheRequestOption<TResponse> : ICacheRequestOption where TResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the cache object
        /// </summary>
        public CacheObject CacheObject
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the command flags
        /// </summary>
        public CacheCommandFlags CommandFlags
        {
            get; set;
        } = CacheCommandFlags.None;

        /// <summary>
        /// Gets or sets the cache entry expiration
        /// When the specified cache item is not found, the cache item is created with the change expiration information 
        /// </summary>
        public CacheExpiration Expiration
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <returns>Return cache result</returns>
        internal async Task<CacheResult<TResponse>> ExecuteAsync()
        {
            var servers = GetCacheServers();
            if (servers.IsNullOrEmpty())
            {
                return CacheResult<TResponse>.EmptyResult();
            }
            CacheResult<TResponse> result = new CacheResult<TResponse>();

            //Single cache server
            if (servers.Count == 1)
            {
                var firstServer = servers.First();
                var provider = CacheManager.Configuration.GetCacheProvider(firstServer.ServerType);
                if (provider != null)
                {
                    result.AddResponse(await ExecuteCacheOperationAsync(provider, firstServer).ConfigureAwait(false));
                }
                return result;
            }

            //Multiple cache server
            Task<TResponse>[] cacheTasks = new Task<TResponse>[servers.Count];
            var serverIndex = 0;
            foreach (var server in servers)
            {
                if (server == null)
                {
                    continue;
                }
                var provider = CacheManager.Configuration.GetCacheProvider(server.ServerType);
                if (provider == null)
                {
                    continue;
                }
                cacheTasks[serverIndex] = ExecuteCacheOperationAsync(provider, server);
                serverIndex++;
            }
            result.AddResponse(await Task.WhenAll(cacheTasks).ConfigureAwait(false));
            return result;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <returns>Return cache result</returns>
        internal async Task<CacheResult<TResponse>> ExecuteAsync(CacheServer server)
        {
            if (server == null)
            {
                return CacheResult<TResponse>.EmptyResult();
            }
            var provider = CacheManager.Configuration.GetCacheProvider(server.ServerType);
            if (provider == null)
            {
                return CacheResult<TResponse>.EmptyResult();
            }
            CacheResult<TResponse> result = new CacheResult<TResponse>();
            var response = await ExecuteCacheOperationAsync(provider, server).ConfigureAwait(false);
            if (response != null)
            {
                result.AddResponse(response);
            }
            return result;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="CacheServer">Cache server</param>
        /// <returns>Return cache operation response</returns>
        protected abstract Task<TResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server);

        /// <summary>
        /// Get cache servers
        /// </summary>
        /// <returns>Return cache servers</returns>
        protected virtual List<CacheServer> GetCacheServers()
        {
            return CacheManager.Configuration.GetCacheServers(this);
        }
    }
}
