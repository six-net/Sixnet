using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.String.Response;

namespace Sixnet.Cache.String
{
    /// <summary>
    /// String get set options
    /// </summary>
    public class StringGetSetOptions : CacheOperationOptions<StringGetSetResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the new value
        /// </summary>
        public string NewValue { get; set; }

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
        /// <returns>Return string get set response</returns>
        protected override async Task<StringGetSetResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringGetSetAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get set response</returns>
        protected override StringGetSetResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.StringGetSet(server, this);
        }
    }
}
