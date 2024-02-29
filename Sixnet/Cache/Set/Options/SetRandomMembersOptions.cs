using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Set.Response;

namespace Sixnet.Cache.Set.Options
{
    /// <summary>
    /// Set random members options
    /// </summary>
    public class SetRandomMembersOptions : CacheOperationOptions<SetRandomMembersResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set random members response</returns>
        protected override async Task<SetRandomMembersResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetRandomMembersAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set random members response</returns>
        protected override SetRandomMembersResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SetRandomMembers(server, this);
        }
    }
}
