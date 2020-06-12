using System.Threading.Tasks;
using EZNEW.Cache.SortedSet.Response;

namespace EZNEW.Cache.SortedSet.Request
{
    /// <summary>
    /// Sorted set decrement option
    /// </summary>
    public class SortedSetDecrementOption : CacheRequestOption<SortedSetDecrementResponse>
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
        public double DecrementScore
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set decrement response</returns>
        protected override async Task<SortedSetDecrementResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetDecrementAsync(server, this).ConfigureAwait(false);
        }
    }
}
