using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Hash.Response;

namespace Sixnet.Cache.Hash.Options
{
    /// <summary>
    /// Hash key options
    /// </summary>
    public class HashKeysOptions : CacheOptions<HashKeysResponse>
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
        /// <returns>Return hash keys response</returns>
        protected override async Task<HashKeysResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashKeysAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash keys response</returns>
        protected override HashKeysResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.HashKeys(server, this);
        }
    }
}
