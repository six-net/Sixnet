using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.String.Response;

namespace Sixnet.Cache.String
{
    /// <summary>
    /// String get with expiry options
    /// </summary>
    public class StringGetWithExpiryOptions : CacheOperationOptions<StringGetWithExpiryResponse>
    {
        /// <summary>
        /// Gets or sets the cahce key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get with expiry response</returns>
        protected override async Task<StringGetWithExpiryResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringGetWithExpiryAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get with expiry response</returns>
        protected override StringGetWithExpiryResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.StringGetWithExpiry(server, this);
        }
    }
}
