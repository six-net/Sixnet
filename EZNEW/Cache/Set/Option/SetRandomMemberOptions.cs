using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Set
{
    /// <summary>
    /// Set random member options
    /// </summary>
    public class SetRandomMemberOptions : CacheOptions<SetRandomMemberResponse>
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
        protected override async Task<IEnumerable<SetRandomMemberResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetRandomMemberAsync(server, this).ConfigureAwait(false);
        }
    }
}
