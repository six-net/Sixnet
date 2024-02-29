using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Hash.Response;

namespace Sixnet.Cache.Hash.Options
{
    /// <summary>
    /// Hash length options
    /// </summary>
    public class HashLengthOptions : CacheOperationOptions<HashLengthResponse>
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
        protected override async Task<HashLengthResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashLengthAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash length response</returns>
        protected override HashLengthResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.HashLength(server, this);
        }
    }
}
