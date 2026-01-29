// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.List.Results;

namespace Sixnet.Cache.List.Parameters
{
    /// <summary>
    /// List length parameter
    /// </summary>
    public class SixnetListLengthParameter : SixnetCacheParameter<SixnetListLengthResult>
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
        /// <returns>Return list length response</returns>
        protected override async Task<SixnetListLengthResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.ListLengthAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list length response</returns>
        protected override SixnetListLengthResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.ListLength(server, this);
        }
    }
}
