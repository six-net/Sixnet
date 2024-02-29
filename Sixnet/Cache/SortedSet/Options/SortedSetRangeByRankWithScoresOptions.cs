using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.SortedSet.Response;

namespace Sixnet.Cache.SortedSet.Options
{
    /// <summary>
    /// Sorted set range by rank with scores options
    /// </summary>
    public class SortedSetRangeByRankWithScoresOptions : CacheOperationOptions<SortedSetRangeByRankWithScoresResponse>
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
        /// <returns>Return sorted set range by rank with scores response</returns>
        protected override async Task<SortedSetRangeByRankWithScoresResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRangeByRankWithScoresAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set range by rank with scores response</returns>
        protected override SortedSetRangeByRankWithScoresResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SortedSetRangeByRankWithScores(server, this);
        }
    }
}
