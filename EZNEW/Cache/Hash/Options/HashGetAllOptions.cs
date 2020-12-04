using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Hash
{
    /// <summary>
    /// Hash get all options
    /// </summary>
    public class HashGetAllOptions : CacheOptions<HashGetAllResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash get all response</returns>
        protected override async Task<IEnumerable<HashGetAllResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashGetAllAsync(server, this).ConfigureAwait(false);
        }
    }
}
