using System.Threading.Tasks;
using EZNEW.Cache.Keys.Response;

namespace EZNEW.Cache.Keys.Request
{
    /// <summary>
    /// Dump option
    /// </summary>
    public class DumpOption : CacheRequestOption<DumpResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Execute cache key
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return dump key response</returns>
        protected override async Task<DumpResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyDumpAsync(server, this).ConfigureAwait(false);
        }
    }
}
