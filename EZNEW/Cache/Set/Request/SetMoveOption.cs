using System.Threading.Tasks;
using EZNEW.Cache.Set.Response;

namespace EZNEW.Cache.Set.Request
{
    /// <summary>
    /// Set move option
    /// </summary>
    public class SetMoveOption : CacheRequestOption<SetMoveResponse>
    {
        /// <summary>
        /// Gets or sets the source key
        /// </summary>
        public CacheKey SourceKey { get; set; }

        /// <summary>
        /// Gets or sets the destination key
        /// </summary>
        public CacheKey DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the move member
        /// </summary>
        public string MoveMember { get; set; }

        /// <summary>
        /// Gets or sets the cache entry expiration
        /// When the specified cache item is not found, the cache item is created with the change expiration information 
        /// </summary>
        public CacheExpiration Expiration { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set move response</returns>
        protected override async Task<SetMoveResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetMoveAsync(server, this).ConfigureAwait(false);
        }
    }
}
