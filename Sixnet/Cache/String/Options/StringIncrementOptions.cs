using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.String.Response;

namespace Sixnet.Cache.String
{
    /// <summary>
    /// String increment options
    /// </summary>
    public class StringIncrementOptions : CacheOptions<StringIncrementResponse>
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
        /// <returns>Return string increment response</returns>
        protected override async Task<StringIncrementResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringIncrementAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string increment response</returns>
        protected override StringIncrementResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.StringIncrement(server, this);
        }
    }
}
