using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.List
{
    /// <summary>
    /// List length options
    /// </summary>
    public class ListLengthOptions : CacheOptions<ListLengthResponse>
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
        /// <returns>Return list length response</returns>
        protected override async Task<IEnumerable<ListLengthResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListLengthAsync(server, this).ConfigureAwait(false);
        }
    }
}
