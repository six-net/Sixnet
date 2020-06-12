using System.Threading.Tasks;
using EZNEW.Cache.SortedSet.Response;

namespace EZNEW.Cache.SortedSet.Request
{
    /// <summary>
    /// Sorted set remove range by rank option
    /// </summary>
    public class SortedSetRemoveRangeByRankOption : CacheRequestOption<SortedSetRemoveRangeByRankResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the start rank value
        /// </summary>
        public int Start
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the stop rank value
        /// </summary>
        public int Stop
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the exclude type
        /// </summary>
        public SortedSetExclude Exclude
        {
            get; set;
        } = SortedSetExclude.None;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set remove range by rank response</returns>
        protected override async Task<SortedSetRemoveRangeByRankResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRemoveRangeByRankAsync(server, this).ConfigureAwait(false);
        }
    }
}
