using System.Threading.Tasks;
using EZNEW.Cache.SortedSet.Response;

namespace EZNEW.Cache.SortedSet.Request
{
    /// <summary>
    /// Sorted set length by value option
    /// </summary>
    public class SortedSetLengthByValueOption : CacheRequestOption<SortedSetLengthByValueResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the min value
        /// </summary>
        public string MinValue { get; set; }

        /// <summary>
        /// Gets or sets the max value
        /// </summary>
        public string MaxValue { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set length by value response</returns>
        protected override async Task<SortedSetLengthByValueResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetLengthByValueAsync(server, this).ConfigureAwait(false);
        }
    }
}
