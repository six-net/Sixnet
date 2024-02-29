using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.String.Response;

namespace Sixnet.Cache.String
{
    /// <summary>
    /// String decrement options
    /// </summary>
    public class StringDecrementOptions : CacheOperationOptions<StringDecrementResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public long Value { get; set; }

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
        /// <returns>Return string decrement response</returns>
        protected override async Task<StringDecrementResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringDecrementAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string decrement response</returns>
        protected override StringDecrementResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.StringDecrement(server, this);
        }
    }
}
