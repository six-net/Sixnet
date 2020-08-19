using System.Threading.Tasks;
using EZNEW.Cache.List.Response;

namespace EZNEW.Cache.List.Request
{
    /// <summary>
    /// List right pop option
    /// </summary>
    public class ListRightPopOption : CacheRequestOption<ListRightPopResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list right pop response</returns>
        protected override async Task<ListRightPopResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListRightPopAsync(server, this).ConfigureAwait(false);
        }
    }
}
