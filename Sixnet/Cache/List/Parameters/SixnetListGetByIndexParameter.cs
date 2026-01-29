// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.List.Results;

namespace Sixnet.Cache.List.Parameters
{
    /// <summary>
    /// List get by index parameter
    /// </summary>
    public class SixnetListGetByIndexParameter : SixnetCacheParameter<SixnetListGetByIndexResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get by index response</returns>
        protected override async Task<SixnetListGetByIndexResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.ListGetByIndexAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get by index response</returns>
        protected override SixnetListGetByIndexResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.ListGetByIndex(server, this);
        }
    }
}
