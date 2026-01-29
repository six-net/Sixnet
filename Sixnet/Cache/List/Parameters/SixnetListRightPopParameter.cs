// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.List.Results;

namespace Sixnet.Cache.List.Parameters
{
    /// <summary>
    /// List right pop parameter
    /// </summary>
    public class SixnetListRightPopParameter : SixnetCacheParameter<SixnetListRightPopResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list right pop response</returns>
        protected override async Task<SixnetListRightPopResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.ListRightPopAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list right pop response</returns>
        protected override SixnetListRightPopResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.ListRightPop(server, this);
        }
    }
}
