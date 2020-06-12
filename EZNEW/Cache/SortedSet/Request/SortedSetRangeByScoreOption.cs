using System.Threading.Tasks;
using EZNEW.Cache.SortedSet.Response;

namespace EZNEW.Cache.SortedSet.Request
{
    /// <summary>
    /// Sorted set range by score option
    /// </summary>
    public class SortedSetRangeByScoreOption : CacheRequestOption<SortedSetRangeByScoreResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the start score value
        /// </summary>
        public double Start
        {
            get; set;
        } = double.MinValue;

        /// <summary>
        /// Gets or sets the stop score value
        /// </summary>
        public double Stop
        {
            get; set;
        } = double.MaxValue;

        /// <summary>
        /// Gets or sets the skip count
        /// </summary>
        public int Skip
        {
            get; set;
        } = 0;

        /// <summary>
        /// Gets or sets the take count
        /// </summary>
        public int Take
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
        /// <returns>Return sorted set range by score response</returns>
        protected override async Task<SortedSetRangeByScoreResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRangeByScoreAsync(server, this).ConfigureAwait(false);
        }
    }
}
