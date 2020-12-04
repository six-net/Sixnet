using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.List
{
    /// <summary>
    /// List right pop left push options
    /// </summary>
    public class ListRightPopLeftPushOptions : CacheOptions<ListRightPopLeftPushResponse>
    {
        /// <summary>
        /// Gets or sets the source key
        /// </summary>
        public CacheKey SourceKey { get; set; }

        /// <summary>
        /// Gets or sets the destination key
        /// </summary>
        public CacheKey DestinationKey { get; set; }

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
        /// <returns>Return right pop left push response</returns>
        protected override async Task<IEnumerable<ListRightPopLeftPushResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListRightPopLeftPushAsync(server, this).ConfigureAwait(false);
        }
    }
}
