using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Keys.Response;

namespace Sixnet.Cache.Keys.Options
{
    /// <summary>
    /// Get key detail options
    /// </summary>
    public class GetDetailOptions : CacheOperationOptions<GetDetailResponse>
    {
        /// <summary>
        /// Gets or sets cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public CacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get key detail response</returns>
        protected override async Task<GetDetailResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetKeyDetailAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get key detail response</returns>
        protected override GetDetailResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.GetKeyDetail(server, this);
        }
    }
}
