using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.String
{
    /// <summary>
    /// String get with expiry options
    /// </summary>
    public class StringGetWithExpiryOptions : CacheOptions<StringGetWithExpiryResponse>
    {
        /// <summary>
        /// Gets or sets the cahce key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get with expiry response</returns>
        protected override async Task<IEnumerable<StringGetWithExpiryResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringGetWithExpiryAsync(server, this).ConfigureAwait(false);
        }
    }
}
