// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Keys.Results;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Sort and store parameter
    /// </summary>
    public class SixnetSortAndStoreParameter : SixnetCacheParameter<SixnetSortAndStoreResult>
    {
        /// <summary>
        /// Gets or sets the destination key
        /// </summary>
        public SixnetCacheKey DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the source key
        /// </summary>
        public SixnetCacheKey SourceKey { get; set; }

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
        public SixnetCacheOrder Order { get; set; } = SixnetCacheOrder.Ascending;

        /// <summary>
        /// Gets or sets sort type
        /// </summary>
        public SixnetCacheSortType SortType { get; set; } = SixnetCacheSortType.Numeric;

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
        public SixnetCacheExpiration Expiration { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sort and store response</returns>
        protected override async Task<SixnetSortAndStoreResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SortAndStoreAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sort and store response</returns>
        protected override SixnetSortAndStoreResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SortAndStore(server, this);
        }
    }
}
