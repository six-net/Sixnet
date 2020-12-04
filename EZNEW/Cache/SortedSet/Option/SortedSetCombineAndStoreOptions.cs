using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set combine and store options
    /// </summary>
    public class SortedSetCombineAndStoreOptions : CacheOptions<SortedSetCombineAndStoreResponse>
    {
        /// <summary>
        /// Gets or sets the source keys
        /// </summary>
        public List<CacheKey> SourceKeys { get; set; }

        /// <summary>
        /// gets or sets the destination key
        /// </summary>
        public CacheKey DestinationKey { get; set; }

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
        public CacheExpiration Expiration { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set combine and store response</returns>
        protected override async Task<IEnumerable<SortedSetCombineAndStoreResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetCombineAndStoreAsync(server, this).ConfigureAwait(false);
        }
    }
}
