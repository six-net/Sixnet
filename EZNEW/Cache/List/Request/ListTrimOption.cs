using System.Threading.Tasks;
using EZNEW.Cache.List.Response;

namespace EZNEW.Cache.List.Request
{
    /// <summary>
    /// List trim option
    /// </summary>
    public class ListTrimOption : CacheRequestOption<ListTrimResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the start index
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the stop index
        /// </summary>
        public int Stop { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list trim response</returns>
        protected override async Task<ListTrimResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListTrimAsync(server, this).ConfigureAwait(false);
        }
    }
}
