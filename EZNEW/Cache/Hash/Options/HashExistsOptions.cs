using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Hash
{
    /// <summary>
    /// Hash exists options
    /// </summary>
    public class HashExistsOptions : CacheOptions<HashExistsResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the hash field
        /// </summary>
        public string HashField { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash exists response</returns>
        protected override async Task<IEnumerable<HashExistsResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashExistAsync(server, this).ConfigureAwait(false);
        }
    }
}
