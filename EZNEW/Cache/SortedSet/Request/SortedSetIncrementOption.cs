using System.Threading.Tasks;
using EZNEW.Cache.SortedSet.Response;

namespace EZNEW.Cache.SortedSet.Request
{
    /// <summary>
    /// Sorted set increment option
    /// </summary>
    public class SortedSetIncrementOption : CacheRequestOption<SortedSetIncrementResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the member
        /// </summary>
        public string Member
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the score value
        /// </summary>
        public double IncrementScore
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set increment response</returns>
        protected override async Task<SortedSetIncrementResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetIncrementAsync(server, this).ConfigureAwait(false);
        }
    }
}
