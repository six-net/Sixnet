using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.List
{
    /// <summary>
    /// List remove options
    /// </summary>
    public class ListRemoveOptions : CacheOptions<ListRemoveResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the value count
        /// </summary>
        public long Count { get; set; } = 0;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list remove response</returns>
        protected override async Task<IEnumerable<ListRemoveResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListRemoveAsync(server, this).ConfigureAwait(false);
        }
    }
}
