using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Sort options
    /// </summary>
    public class SortOptions : CacheOptions<SortResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the data offset
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the data count
        /// </summary>
        public int Count { get; set; } = -1;

        /// <summary>
        /// Gets or sets order
        /// </summary>
        public CacheOrder Order { get; set; } = CacheOrder.Ascending;

        /// <summary>
        /// Gets or sets the sort type
        /// </summary>
        public CacheSortType SortType { get; set; } = CacheSortType.Numeric;

        /// <summary>
        /// Gets or sets the sort by value
        /// </summary>
        public string By { get; set; }

        /// <summary>
        /// Gets or sets the get values
        /// </summary>
        public List<string> Gets { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sort response</returns>
        protected override async Task<IEnumerable<SortResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortAsync(server, this).ConfigureAwait(false);
        }
    }
}
