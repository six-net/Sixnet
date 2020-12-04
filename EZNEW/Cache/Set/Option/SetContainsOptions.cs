using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Set
{
    /// <summary>
    /// Set contains options
    /// </summary>
    public class SetContainsOptions : CacheOptions<SetContainsResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the member value
        /// </summary>
        public string Member { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set contains response</returns>
        protected override async Task<IEnumerable<SetContainsResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetContainsAsync(server, this).ConfigureAwait(false);
        }
    }
}
