using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Set
{
    /// <summary>
    /// Set members options
    /// </summary>
    public class SetMembersOptions : CacheOptions<SetMembersResponse>
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
        protected override async Task<IEnumerable<SetMembersResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetMembersAsync(server, this).ConfigureAwait(false);
        }
    }
}
