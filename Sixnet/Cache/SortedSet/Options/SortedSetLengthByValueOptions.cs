using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.SortedSet.Response;

namespace Sixnet.Cache.SortedSet.Options
{
    /// <summary>
    /// Sorted set length by value options
    /// </summary>
    public class SortedSetLengthByValueOptions : CacheOperationOptions<SortedSetLengthByValueResponse>
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
        protected override async Task<SortedSetLengthByValueResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetLengthByValueAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set length by value response</returns>
        protected override SortedSetLengthByValueResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SortedSetLengthByValue(server, this);
        }
    }
}
