using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.SortedSet.Response;

namespace Sixnet.Cache.SortedSet.Options
{
    /// <summary>
    /// Sorted set length options
    /// </summary>
    public class SortedSetLengthOptions : CacheOperationOptions<SortedSetLengthResponse>
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
        /// <returns>Return sorted set length response</returns>
        protected override async Task<SortedSetLengthResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetLengthAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set length response</returns>
        protected override SortedSetLengthResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SortedSetLength(server, this);
        }
    }
}
