using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.List.Response;

namespace Sixnet.Cache.List.Options
{
    /// <summary>
    /// List left pop options
    /// </summary>
    public class ListLeftPopOptions : CacheOperationOptions<ListLeftPopResponse>
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
        /// <returns>Return list left pop response</returns>
        protected override async Task<ListLeftPopResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListLeftPopAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list left pop response</returns>
        protected override ListLeftPopResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.ListLeftPop(server, this);
        }
    }
}
