// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Set.Results;

namespace Sixnet.Cache.Set.Parameters
{
    /// <summary>
    /// Set length parameter
    /// </summary>
    public class SixnetSetLengthParameter : SixnetCacheParameter<SixnetSetLengthResult>
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
        /// <returns>Return set length response</returns>
        protected override async Task<SixnetSetLengthResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SetLengthAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set length response</returns>
        protected override SixnetSetLengthResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SetLength(server, this);
        }
    }
}
