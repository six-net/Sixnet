using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set remove range by rank options
    /// </summary>
    public class SortedSetRemoveRangeByRankOptions : CacheOptions<SortedSetRemoveRangeByRankResponse>
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
        protected override async Task<IEnumerable<SortedSetRemoveRangeByRankResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRemoveRangeByRankAsync(server, this).ConfigureAwait(false);
        }
    }
}
