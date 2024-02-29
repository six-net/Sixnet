using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.SortedSet.Response;

namespace Sixnet.Cache.SortedSet.Options
{
    /// <summary>
    /// Sorted set score options
    /// </summary>
    public class SortedSetScoreOptions : CacheOperationOptions<SortedSetScoreResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the member
        /// </summary>
        public string Member { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set score response</returns>
        protected override async Task<SortedSetScoreResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetScoreAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set score response</returns>
        protected override SortedSetScoreResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SortedSetScore(server, this);
        }
    }
}
