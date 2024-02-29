using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Set.Response;

namespace Sixnet.Cache.Set.Options
{
    /// <summary>
    /// Set members options
    /// </summary>
    public class SetMembersOptions : CacheOperationOptions<SetMembersResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set members response</returns>
        protected override async Task<SetMembersResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetMembersAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set members response</returns>
        protected override SetMembersResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SetMembers(server, this);
        }
    }
}
