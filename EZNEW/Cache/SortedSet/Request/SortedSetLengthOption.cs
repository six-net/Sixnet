using System.Threading.Tasks;
using EZNEW.Cache.SortedSet.Response;

namespace EZNEW.Cache.SortedSet.Request
{
    /// <summary>
    /// Sorted set length option
    /// </summary>
    public class SortedSetLengthOption : CacheRequestOption<SortedSetLengthResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the minimum
        /// </summary>
        public double Minimum
        {
            get; set;
        } = double.NegativeInfinity;

        /// <summary>
        /// Gets or sets the maximum
        /// </summary>
        public double Maximum
        {
            get; set;
        } = double.PositiveInfinity;

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
        /// <returns>Return sorted set length response</returns>
        protected override async Task<SortedSetLengthResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetLengthAsync(server, this).ConfigureAwait(false);
        }
    }
}
