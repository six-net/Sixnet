// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.SortedSet.Results;

namespace Sixnet.Cache.SortedSet.Parameters
{
    /// <summary>
    /// Sorted set increment parameter
    /// </summary>
    public class SixnetSortedSetIncrementParameter : SixnetCacheParameter<SixnetSortedSetIncrementResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the member
        /// </summary>
        public string Member { get; set; }

        /// <summary>
        /// Gets or sets the score value
        /// </summary>
        public double IncrementScore { get; set; }

        /// <summary>
        /// Gets or sets the cache entry expiration
        /// When the specified cache item is not found, the cache item is created with the change expiration information 
        /// </summary>
        public SixnetCacheExpiration Expiration { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set increment response</returns>
        protected override async Task<SixnetSortedSetIncrementResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SortedSetIncrementAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set increment response</returns>
        protected override SixnetSortedSetIncrementResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SortedSetIncrement(server, this);
        }
    }
}
