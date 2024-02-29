using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Set.Response;

namespace Sixnet.Cache.Set.Options
{
    /// <summary>
    /// Set remove options
    /// </summary>
    public class SetRemoveOptions : CacheOperationOptions<SetRemoveResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the remove members
        /// </summary>
        public List<string> RemoveMembers { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set remove response</returns>
        protected override async Task<SetRemoveResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetRemoveAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set remove response</returns>
        protected override SetRemoveResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SetRemove(server, this);
        }
    }
}
