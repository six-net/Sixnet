using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.String.Response;

namespace Sixnet.Cache.String
{
    /// <summary>
    /// String get bit options
    /// </summary>
    public class StringGetBitOptions : CacheOperationOptions<StringGetBitResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the offset
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get bit response</returns>
        protected override async Task<StringGetBitResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringGetBitAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get bit response</returns>
        protected override StringGetBitResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.StringGetBit(server, this);
        }
    }
}
