using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.List
{
    /// <summary>
    /// List left push options
    /// </summary>
    public class ListLeftPushOptions : CacheOptions<ListLeftPushResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<string> Values { get; set; }

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
        /// <returns>Return list left push response</returns>
        protected override async Task<IEnumerable<ListLeftPushResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListLeftPushAsync(server, this).ConfigureAwait(false);
        }
    }
}
