using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Set.Response;

namespace Sixnet.Cache.Set.Options
{
    /// <summary>
    /// Set random member options
    /// </summary>
    public class SetRandomMemberOptions : CacheOperationOptions<SetRandomMemberResponse>
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
        /// <returns>Return set random member response</returns>
        protected override async Task<SetRandomMemberResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetRandomMemberAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set random member response</returns>
        protected override SetRandomMemberResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SetRandomMember(server, this);
        }
    }
}
