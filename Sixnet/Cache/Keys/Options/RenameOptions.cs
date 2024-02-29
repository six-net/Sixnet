using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Keys.Response;

namespace Sixnet.Cache.Keys.Options
{
    /// <summary>
    /// Rename options
    /// </summary>
    public class RenameOptions : CacheOperationOptions<RenameResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the new key
        /// </summary>
        public CacheKey NewKey { get; set; }

        /// <summary>
        /// Gets or sets whether only set new key when not exists
        /// </summary>
        public bool WhenNewKeyNotExists { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return rename response</returns>
        protected override async Task<RenameResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyRenameAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return rename response</returns>
        protected override RenameResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.KeyRename(server, this);
        }
    }
}
