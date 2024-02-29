using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Keys.Response;

namespace Sixnet.Cache.Keys.Options
{
    /// <summary>
    /// Delete key options
    /// </summary>
    public class DeleteOptions : CacheOperationOptions<DeleteResponse>
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public List<CacheKey> Keys { get; set; }

        /// <summary>
        /// Execute the cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return delete key response</returns>
        protected override async Task<DeleteResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyDeleteAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute the cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return delete key response</returns>
        protected override DeleteResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.KeyDelete(server, this);
        }
    }
}
