using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Sort and store options
    /// </summary>
    public class SortAndStoreOptions : CacheOptions<SortAndStoreResponse>
    {
        /// <summary>
        /// Gets or sets the destination key
        /// </summary>
        public CacheKey DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the source key
        /// </summary>
        public CacheKey SourceKey { get; set; }

        /// <summary>
        /// Gets or sets the data offset
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Gets or sets take count
        /// </summary>
        public int Count { get; set; } = -1;

        /// <summary>
        /// Gets or sets order type
        /// </summary>
        public CacheOrder Order { get; set; } = CacheOrder.Ascending;

        /// <summary>
        /// Gets or sets sort type
        /// </summary>
        public CacheSortType SortType { get; set; } = CacheSortType.Numeric;

        /// <summary>
        /// Gets or sets sort by value
        /// </summary>
        public string By { get; set; }

        /// <summary>
        /// Gets or sets the get values
        /// </summary>
        public List<string> Gets { get; set; }

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
        /// <returns>Return sort and store response</returns>
        protected override async Task<IEnumerable<SortAndStoreResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortAndStoreAsync(server, this).ConfigureAwait(false);
        }
    }
}
