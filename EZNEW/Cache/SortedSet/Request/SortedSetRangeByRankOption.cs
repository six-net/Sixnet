using System.Threading.Tasks;
using EZNEW.Cache.SortedSet.Response;

namespace EZNEW.Cache.SortedSet.Request
{
    /// <summary>
    /// Sorted set range by rank option
    /// </summary>
    public class SortedSetRangeByRankOption : CacheRequestOption<SortedSetRangeByRankResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the start value
        /// </summary>
        public int Start
        {
            get; set;
        } = 0;

        /// <summary>
        /// Gets or sets the stop value
        /// </summary>
        public int Stop
        {
            get; set;
        } = -1;

        /// <summary>
        /// Gets or sets the order type
        /// </summary>
        public SortedOrder Order
        {
            get; set;
        } = SortedOrder.Ascending;

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
        /// <returns>Return sorted set range by rank response</returns>
        protected override async Task<SortedSetRangeByRankResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRangeByRankAsync(server, this).ConfigureAwait(false);
        }
    }
}
