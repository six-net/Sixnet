using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.SortedSet.Response;

namespace Sixnet.Cache.SortedSet.Options
{
    /// <summary>
    /// Sorted set remove range by value options
    /// </summary>
    public class SortedSetRemoveRangeByValueOptions : CacheOptions<SortedSetRemoveRangeByValueResponse>
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
        /// Gets or sets the exclude type
        /// </summary>
        public BoundaryExclude Exclude { get; set; } = BoundaryExclude.None;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set remove range by value response</returns>
        protected override async Task<SortedSetRemoveRangeByValueResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRemoveRangeByValueAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set remove range by value response</returns>
        protected override SortedSetRemoveRangeByValueResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SortedSetRemoveRangeByValue(server, this);
        }
    }
}
