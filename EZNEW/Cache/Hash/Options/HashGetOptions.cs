using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Hash
{
    /// <summary>
    /// Hash get options
    /// </summary>
    public class HashGetOptions : CacheOptions<HashGetResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets he hash field
        /// </summary>
        public string HashField { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash get response</returns>
        protected override async Task<IEnumerable<HashGetResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashGetAsync(server, this).ConfigureAwait(false);
        }
    }
}
