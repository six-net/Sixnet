using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set remove options
    /// </summary>
    public class SortedSetRemoveOptions : CacheOptions<SortedSetRemoveResponse>
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
        protected override async Task<IEnumerable<SortedSetRemoveResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRemoveAsync(server, this).ConfigureAwait(false);
        }
    }
}
