// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Hash.Results;

namespace Sixnet.Cache.Hash.Parameters
{
    /// <summary>
    /// Hash set parameter
    /// </summary>
    public class SixnetHashSetParameter : SixnetCacheParameter<SixnetHashSetResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the value items
        /// </summary>
        public Dictionary<string, dynamic> Items { get; set; }

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
        /// <returns>Return hash set response</returns>
        protected override async Task<SixnetHashSetResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.HashSetAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash set response</returns>
        protected override SixnetHashSetResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.HashSet(server, this);
        }
    }
}
