using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.List
{
    /// <summary>
    /// List trim options
    /// </summary>
    public class ListTrimOptions : CacheOptions<ListTrimResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the start index
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the stop index
        /// </summary>
        public int Stop { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list trim response</returns>
        protected override async Task<IEnumerable<ListTrimResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListTrimAsync(server, this).ConfigureAwait(false);
        }
    }
}
