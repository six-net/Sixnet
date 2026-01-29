// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Set.Results;

namespace Sixnet.Cache.Set.Parameters
{
    /// <summary>
    /// Set move parameter
    /// </summary>
    public class SixnetSetMoveParameter : SixnetCacheParameter<SixnetSetMoveResult>
    {
        /// <summary>
        /// Gets or sets the source key
        /// </summary>
        public SixnetCacheKey SourceKey { get; set; }

        /// <summary>
        /// Gets or sets the destination key
        /// </summary>
        public SixnetCacheKey DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the move member
        /// </summary>
        public string MoveMember { get; set; }

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
        /// <returns>Return set move response</returns>
        protected override async Task<SixnetSetMoveResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SetMoveAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set move response</returns>
        protected override SixnetSetMoveResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SetMove(server, this);
        }
    }
}
