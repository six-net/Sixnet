using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.String.Response;

namespace Sixnet.Cache.String
{
    /// <summary>
    /// String length options
    /// </summary>
    public class StringLengthOptions : CacheOperationOptions<StringLengthResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string length response</returns>
        protected override async Task<StringLengthResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringLengthAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string length response</returns>
        protected override StringLengthResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.StringLength(server, this);
        }
    }
}
