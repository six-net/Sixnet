// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Set.Results;

namespace Sixnet.Cache.Set.Parameters
{
    /// <summary>
    /// Set remove parameter
    /// </summary>
    public class SixnetSetRemoveParameter : SixnetCacheParameter<SixnetSetRemoveResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the remove members
        /// </summary>
        public List<string> Members { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set remove response</returns>
        protected override async Task<SixnetSetRemoveResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SetRemoveAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set remove response</returns>
        protected override SixnetSetRemoveResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SetRemove(server, this);
        }
    }
}
