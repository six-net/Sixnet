// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Set.Results;

namespace Sixnet.Cache.Set.Parameters
{
    /// <summary>
    /// Set combine parameter
    /// </summary>
    public class SixnetSetCombineParameter : SixnetCacheParameter<SixnetSetCombineResult>
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public List<SixnetCacheKey> Keys { get; set; }

        /// <summary>
        /// Gets or sets the combine operation
        /// </summary>
        public SixnetCombineOperation CombineOperation { get; set; } = SixnetCombineOperation.Union;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override async Task<SixnetSetCombineResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SetCombineAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override SixnetSetCombineResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SetCombine(server, this);
        }
    }
}
