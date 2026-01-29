// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Hash.Results;

namespace Sixnet.Cache.Hash.Parameters
{
    /// <summary>
    /// Hash get all parameter
    /// </summary>
    public class SixnetHashGetAllParameter : SixnetCacheParameter<SixnetHashGetAllResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash get all response</returns>
        protected override async Task<SixnetHashGetAllResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.HashGetAllAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash get all response</returns>
        protected override SixnetHashGetAllResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.HashGetAll(server, this);
        }
    }
}
