using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.List.Response;

namespace Sixnet.Cache.List.Options
{
    /// <summary>
    /// List right pop options
    /// </summary>
    public class ListRightPopOptions : CacheOperationOptions<ListRightPopResponse>
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
        /// <returns>Return list right pop response</returns>
        protected override async Task<ListRightPopResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListRightPopAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list right pop response</returns>
        protected override ListRightPopResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.ListRightPop(server, this);
        }
    }
}
