// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.SortedSet.Results;

namespace Sixnet.Cache.SortedSet.Parameters
{
    /// <summary>
    /// Sorted set remove range by rank parameter
    /// </summary>
    public class SixnetSortedSetRemoveRangeByRankParameter : SixnetCacheParameter<SixnetSortedSetRemoveRangeByRankResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

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
        protected override async Task<SixnetSortedSetRemoveRangeByRankResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SortedSetRemoveRangeByRankAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set remove range by rank response</returns>
        protected override SixnetSortedSetRemoveRangeByRankResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SortedSetRemoveRangeByRank(server, this);
        }
    }
}
