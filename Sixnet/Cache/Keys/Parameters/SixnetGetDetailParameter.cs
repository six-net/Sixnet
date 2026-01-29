// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Keys.Results;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Get key detail parameter
    /// </summary>
    public class SixnetGetDetailParameter : SixnetCacheParameter<SixnetGetDetailResult>
    {
        /// <summary>
        /// Gets or sets cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public SixnetCacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get key detail response</returns>
        protected override async Task<SixnetGetDetailResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.GetKeyDetailAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get key detail response</returns>
        protected override SixnetGetDetailResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.GetKeyDetail(server, this);
        }
    }
}
