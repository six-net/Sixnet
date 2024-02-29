using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Hash.Response;

namespace Sixnet.Cache.Hash.Options
{
    /// <summary>
    /// Hash set options
    /// </summary>
    public class HashSetOptions : CacheOperationOptions<HashSetResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the value items
        /// </summary>
        public Dictionary<string, dynamic> Items { get; set; }

        /// <summary>
        /// Gets or sets the cache entry expiration
        /// When the specified cache item is not found, the cache item is created with the change expiration information 
        /// </summary>
        public CacheExpiration Expiration { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash set response</returns>
        protected override async Task<HashSetResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashSetAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash set response</returns>
        protected override HashSetResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.HashSet(server, this);
        }
    }
}
