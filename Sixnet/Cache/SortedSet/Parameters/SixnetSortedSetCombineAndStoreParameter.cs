// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.SortedSet.Results;

namespace Sixnet.Cache.SortedSet.Parameters
{
    /// <summary>
    /// Sorted set combine and store parameter
    /// </summary>
    public class SixnetSortedSetCombineAndStoreParameter : SixnetCacheParameter<SixnetSortedSetCombineAndStoreResult>
    {
        /// <summary>
        /// Gets or sets the source keys
        /// </summary>
        public List<SixnetCacheKey> SourceKeys { get; set; }

        /// <summary>
        /// gets or sets the destination key
        /// </summary>
        public SixnetCacheKey DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the set operation
        /// </summary>
        public CombineOperation CombineOperation { get; set; } = CombineOperation.Union;

        /// <summary>
        /// Gets or sets the weights
        /// </summary>
        public double[] Weights { get; set; }

        /// <summary>
        /// Gets or sets the set aggregate
        /// </summary>
        public SetAggregate Aggregate { get; set; } = SetAggregate.Sum;

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
        /// <returns>Return sorted set combine and store response</returns>
        protected override async Task<SixnetSortedSetCombineAndStoreResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SortedSetCombineAndStoreAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set combine and store response</returns>
        protected override SixnetSortedSetCombineAndStoreResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SortedSetCombineAndStore(server, this);
        }
    }
}
