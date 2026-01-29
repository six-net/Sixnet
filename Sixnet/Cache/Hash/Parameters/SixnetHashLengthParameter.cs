// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Hash.Results;

namespace Sixnet.Cache.Hash.Parameters
{
    /// <summary>
    /// Hash length parameter
    /// </summary>
    public class SixnetHashLengthParameter : SixnetCacheParameter<SixnetHashLengthResult>
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
        /// <returns>Return hash length response</returns>
        protected override async Task<SixnetHashLengthResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.HashLengthAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash length response</returns>
        protected override SixnetHashLengthResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.HashLength(server, this);
        }
    }
}
