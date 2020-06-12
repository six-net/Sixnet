using System.Threading.Tasks;
using EZNEW.Cache.String.Response;

namespace EZNEW.Cache.String.Request
{
    /// <summary>
    /// String bit count operation
    /// </summary>
    public class StringBitCountOption : CacheRequestOption<StringBitCountResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the start
        /// </summary>
        public long Start
        {
            get; set;
        } = 0;

        /// <summary>
        /// Gets or sets the end
        /// </summary>
        public long End
        {
            get; set;
        } = -1;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string bit count response</returns>
        protected override async Task<StringBitCountResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringBitCountAsync(server, this).ConfigureAwait(false);
        }
    }
}
