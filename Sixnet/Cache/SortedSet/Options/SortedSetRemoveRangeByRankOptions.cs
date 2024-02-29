using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.SortedSet.Response;

namespace Sixnet.Cache.SortedSet.Options
{
    /// <summary>
    /// Sorted set remove range by rank options
    /// </summary>
    public class SortedSetRemoveRangeByRankOptions : CacheOperationOptions<SortedSetRemoveRangeByRankResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the start rank value
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the stop rank value
        /// </summary>
        public int Stop { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set remove range by rank response</returns>
        protected override async Task<SortedSetRemoveRangeByRankResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRemoveRangeByRankAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set remove range by rank response</returns>
        protected override SortedSetRemoveRangeByRankResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SortedSetRemoveRangeByRank(server, this);
        }
    }
}
