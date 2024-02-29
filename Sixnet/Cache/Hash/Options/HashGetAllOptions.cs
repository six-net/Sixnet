using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Hash.Response;

namespace Sixnet.Cache.Hash.Options
{
    /// <summary>
    /// Hash get all options
    /// </summary>
    public class HashGetAllOptions : CacheOperationOptions<HashGetAllResponse>
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
        protected override async Task<HashGetAllResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashGetAllAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash get all response</returns>
        protected override HashGetAllResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.HashGetAll(server, this);
        }
    }
}
