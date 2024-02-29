using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Keys.Response;

namespace Sixnet.Cache.Keys.Options
{
    /// <summary>
    /// Dump options
    /// </summary>
    public class DumpOptions : CacheOperationOptions<DumpResponse>
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
        protected override async Task<DumpResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyDumpAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache key
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return dump key response</returns>
        protected override DumpResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.KeyDump(server, this);
        }
    }
}
