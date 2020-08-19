using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Cache.String.Response;

namespace EZNEW.Cache.String.Request
{
    /// <summary>
    /// String get option
    /// </summary>
    public class StringGetOption : CacheRequestOption<StringGetResponse>
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public IEnumerable<CacheKey> Keys { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get response</returns>
        protected override async Task<StringGetResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringGetAsync(server, this).ConfigureAwait(false);
        }
    }
}
