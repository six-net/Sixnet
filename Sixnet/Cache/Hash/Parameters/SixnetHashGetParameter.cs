// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Hash.Results;

namespace Sixnet.Cache.Hash.Parameters
{
    /// <summary>
    /// Hash get parameter
    /// </summary>
    public class SixnetHashGetParameter : SixnetCacheParameter<SixnetHashGetResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets he hash field
        /// </summary>
        public string HashField { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash get response</returns>
        protected override async Task<SixnetHashGetResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.HashGetAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash get response</returns>
        protected override SixnetHashGetResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.HashGet(server, this);
        }
    }
}
