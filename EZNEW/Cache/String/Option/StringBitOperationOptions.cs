using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.String
{
    /// <summary>
    /// String bit operation options
    /// </summary>
    public class StringBitOperationOptions : CacheOptions<StringBitOperationResponse>
    {
        /// <summary>
        /// Gets or sets the bit wise
        /// </summary>
        public CacheBitwise Bitwise { get; set; }

        /// <summary>
        /// Gets or sets the destination key for store
        /// </summary>
        public CacheKey DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public List<CacheKey> Keys { get; set; }

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
        /// <returns>Return string bit operation response</returns>
        protected override async Task<IEnumerable<StringBitOperationResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringBitOperationAsync(server, this).ConfigureAwait(false);
        }
    }
}
