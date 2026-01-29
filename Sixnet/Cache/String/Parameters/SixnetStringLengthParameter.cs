// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.String.Results;

namespace Sixnet.Cache.String.Parameters
{
    /// <summary>
    /// String length parameter
    /// </summary>
    public class SixnetStringLengthParameter : SixnetCacheParameter<SixnetStringLengthResult>
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
        /// <returns>Return string length response</returns>
        protected override async Task<SixnetStringLengthResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.StringLengthAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string length response</returns>
        protected override SixnetStringLengthResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.StringLength(server, this);
        }
    }
}
