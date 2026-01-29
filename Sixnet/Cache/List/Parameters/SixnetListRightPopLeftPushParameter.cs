// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.List.Results;

namespace Sixnet.Cache.List.Parameters
{
    /// <summary>
    /// List right pop left push parameter
    /// </summary>
    public class SixnetListRightPopLeftPushParameter : SixnetCacheParameter<SixnetListRightPopLeftPushResult>
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
        /// Gets or sets the cache entry expiration
        /// When the specified cache item is not found, the cache item is created with the change expiration information 
        /// </summary>
        public SixnetCacheExpiration Expiration { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return right pop left push response</returns>
        protected override async Task<SixnetListRightPopLeftPushResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.ListRightPopLeftPushAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return right pop left push response</returns>
        protected override SixnetListRightPopLeftPushResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.ListRightPopLeftPush(server, this);
        }
    }
}
