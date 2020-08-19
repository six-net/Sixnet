using System.Threading.Tasks;
using EZNEW.Cache.List.Response;

namespace EZNEW.Cache.List.Request
{
    /// <summary>
    /// List get by index option
    /// </summary>
    public class ListGetByIndexOption : CacheRequestOption<ListGetByIndexResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the index
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get by index response</returns>
        protected override async Task<ListGetByIndexResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListGetByIndexAsync(server, this).ConfigureAwait(false);
        }
    }
}
