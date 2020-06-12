using System.Threading.Tasks;
using EZNEW.Cache.String.Response;

namespace EZNEW.Cache.String.Request
{
    /// <summary>
    /// String get bit option
    /// </summary>
    public class StringGetBitOption : CacheRequestOption<StringGetBitResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the offset
        /// </summary>
        public long Offset
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string get bit response</returns>
        protected override async Task<StringGetBitResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringGetBitAsync(server, this).ConfigureAwait(false);
        }
    }
}
