using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Cache.SortedSet.Response;

namespace EZNEW.Cache.SortedSet.Request
{
    /// <summary>
    /// Sorted set remove option
    /// </summary>
    public class SortedSetRemoveOption : CacheRequestOption<SortedSetRemoveResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the remove members
        /// </summary>
        public List<string> RemoveMembers { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set remove response</returns>
        protected override async Task<SortedSetRemoveResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRemoveAsync(server, this).ConfigureAwait(false);
        }
    }
}
