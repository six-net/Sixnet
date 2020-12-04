using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Hash
{
    /// <summary>
    /// Hash length options
    /// </summary>
    public class HashLengthOptions : CacheOptions<HashLengthResponse>
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
        /// <returns>Return hash length response</returns>
        protected override async Task<IEnumerable<HashLengthResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashLengthAsync(server, this).ConfigureAwait(false);
        }
    }
}
