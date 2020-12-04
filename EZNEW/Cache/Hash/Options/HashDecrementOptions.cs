using System.Threading.Tasks;
using System.Collections.Generic;

namespace EZNEW.Cache.Hash
{
    /// <summary>
    /// Hash decrement options
    /// </summary>
    public class HashDecrementOptions : CacheOptions<HashDecrementResponse>
    {
        /// <summary>
        /// Gets or sets the key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the hash field
        /// </summary>
        public string HashField { get; set; }

        /// <summary>
        /// Gets or sets the decrement value
        /// </summary>
        public dynamic DecrementValue { get; set; }

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
        /// <returns>Return hash decrement response</returns>
        protected override async Task<IEnumerable<HashDecrementResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashDecrementAsync(server, this).ConfigureAwait(false);
        }
    }
}
