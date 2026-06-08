// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Set.Results;

namespace Sixnet.Cache.Set.Parameters
{
    /// <summary>
    /// Set combine and store parameter
    /// </summary>
    public class SixnetSetCombineAndStoreParameter : SixnetCacheParameter<SixnetSetCombineAndStoreResult>
    {
        /// <summary>
        /// Gets or sets the source keys
        /// </summary>
        public List<SixnetCacheKey> SourceKeys { get; set; }

        /// <summary>
        /// Gets or sets the destination key
        /// </summary>
        public SixnetCacheKey DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the set combine operation
        /// </summary>
        public SixnetCombineOperation CombineOperation { get; set; } = SixnetCombineOperation.Union;

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
        /// <returns>Return set combine and store response</returns>
        protected override async Task<SixnetSetCombineAndStoreResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SetCombineAndStoreAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set combine and store response</returns>
        protected override SixnetSetCombineAndStoreResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SetCombineAndStore(server, this);
        }
    }
}
