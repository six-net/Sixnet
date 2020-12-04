using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.String
{
    /// <summary>
    /// String get options
    /// </summary>
    public class StringGetOptions : CacheOptions<StringGetResponse>
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
        protected override async Task<IEnumerable<StringGetResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringGetAsync(server, this).ConfigureAwait(false);
        }
    }
}
