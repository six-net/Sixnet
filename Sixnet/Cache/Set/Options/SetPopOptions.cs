using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Set.Response;

namespace Sixnet.Cache.Set.Options
{
    /// <summary>
    /// Set pop options
    /// </summary>
    public class SetPopOptions : CacheOptions<SetPopResponse>
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
        /// <returns>Return set pop response response</returns>
        protected override async Task<SetPopResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetPopAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set pop response response</returns>
        protected override SetPopResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SetPop(server, this);
        }
    }
}
