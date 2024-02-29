using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Set.Response;

namespace Sixnet.Cache.Set.Options
{
    /// <summary>
    /// Set combine options
    /// </summary>
    public class SetCombineOptions : CacheOperationOptions<SetCombineResponse>
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public List<CacheKey> Keys { get; set; }

        /// <summary>
        /// Gets or sets the combine operation
        /// </summary>
        public CombineOperation CombineOperation { get; set; } = CombineOperation.Union;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override async Task<SetCombineResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetCombineAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override SetCombineResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SetCombine(server, this);
        }
    }
}
