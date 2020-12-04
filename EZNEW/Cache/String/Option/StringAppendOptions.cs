using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.String
{
    /// <summary>
    /// String append options
    /// </summary>
    public class StringAppendOptions : CacheOptions<StringAppendResponse>
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
        /// Gets or sets the cache entry expiration
        /// When the specified cache item is not found, the cache item is created with the change expiration information 
        /// </summary>
        public CacheExpiration Expiration { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string append response</returns>
        protected override async Task<IEnumerable<StringAppendResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringAppendAsync(server, this).ConfigureAwait(false);
        }
    }
}
