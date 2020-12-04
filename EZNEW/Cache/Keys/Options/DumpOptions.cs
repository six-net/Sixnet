using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Dump options
    /// </summary>
    public class DumpOptions : CacheOptions<DumpResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Execute cache key
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return dump key response</returns>
        protected override async Task<IEnumerable<DumpResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyDumpAsync(server, this).ConfigureAwait(false);
        }
    }
}
