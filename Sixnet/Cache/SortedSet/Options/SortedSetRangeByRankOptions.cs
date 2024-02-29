using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.SortedSet.Response;

namespace Sixnet.Cache.SortedSet.Options
{
    /// <summary>
    /// Sorted set range by rank options
    /// </summary>
    public class SortedSetRangeByRankOptions : CacheOperationOptions<SortedSetRangeByRankResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the start value
        /// </summary>
        public int Start { get; set; } = 0;

        /// <summary>
        /// Gets or sets the stop value
        /// </summary>
        public int Stop { get; set; } = -1;

        /// <summary>
        /// Gets or sets the order type
        /// </summary>
        public CacheOrder Order { get; set; } = CacheOrder.Ascending;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set range by rank response</returns>
        protected override async Task<SortedSetRangeByRankResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRangeByRankAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set range by rank response</returns>
        protected override SortedSetRangeByRankResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SortedSetRangeByRank(server, this);
        }
    }
}
