// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Keys.Results;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Exists key parameter
    /// </summary>
    public class SixnetExistParameter : SixnetCacheParameter<SixnetExistResult>
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public List<SixnetCacheKey> Keys { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return exists key response</returns>
        protected override async Task<SixnetExistResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.KeyExistAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return exists key response</returns>
        protected override SixnetExistResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.KeyExist(server, this);
        }
    }
}
