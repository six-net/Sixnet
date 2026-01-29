// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.String.Results;

namespace Sixnet.Cache.String.Parameters
{
    /// <summary>
    /// String get with expiry parameter
    /// </summary>
    public class SixnetStringGetWithExpiryParameter : SixnetCacheParameter<SixnetStringGetWithExpiryResult>
    {
        /// <summary>
        /// Gets or sets the cahce key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get with expiry response</returns>
        protected override async Task<SixnetStringGetWithExpiryResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.StringGetWithExpiryAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get with expiry response</returns>
        protected override SixnetStringGetWithExpiryResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.StringGetWithExpiry(server, this);
        }
    }
}
