using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.String
{
    /// <summary>
    /// String get range options
    /// </summary>
    public class StringGetRangeOptions : CacheOptions<StringGetRangeResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the start
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the end
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get range response</returns>
        protected override async Task<IEnumerable<StringGetRangeResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringGetRangeAsync(server, this).ConfigureAwait(false);
        }
    }
}
