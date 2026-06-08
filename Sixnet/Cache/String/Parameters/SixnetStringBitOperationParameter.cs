// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.String.Results;

namespace Sixnet.Cache.String.Parameters
{
    /// <summary>
    /// String bit operation parameter
    /// </summary>
    public class SixnetStringBitOperationParameter : SixnetCacheParameter<SixnetStringBitOperationResult>
    {
        /// <summary>
        /// Gets or sets the bit wise
        /// </summary>
        public SixnetCacheBitwise Bitwise { get; set; }

        /// <summary>
        /// Gets or sets the destination key for store
        /// </summary>
        public SixnetCacheKey DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public List<SixnetCacheKey> Keys { get; set; }

        /// <summary>
        /// Gets or sets the cache entry expiration
        /// When the specified cache item is not found, the cache item is created with the change expiration information 
        /// </summary>
        public SixnetCacheExpiration Expiration { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string bit operation response</returns>
        protected override async Task<SixnetStringBitOperationResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.StringBitOperationAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string bit operation response</returns>
        protected override SixnetStringBitOperationResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.StringBitOperation(server, this);
        }
    }
}
