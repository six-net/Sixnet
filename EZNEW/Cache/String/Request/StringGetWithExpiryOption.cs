using System.Threading.Tasks;
using EZNEW.Cache.String.Response;

namespace EZNEW.Cache.String.Request
{
    /// <summary>
    /// String get with expiry option
    /// </summary>
    public class StringGetWithExpiryOption : CacheRequestOption<StringGetWithExpiryResponse>
    {
        /// <summary>
        /// Gets or sets the cahce key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get with expiry response</returns>
        protected override async Task<StringGetWithExpiryResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringGetWithExpiryAsync(server, this).ConfigureAwait(false);
        }
    }
}
