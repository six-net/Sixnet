using System.Threading.Tasks;
using EZNEW.Cache.List.Response;

namespace EZNEW.Cache.List.Request
{
    /// <summary>
    /// List insert after option
    /// </summary>
    public class ListInsertAfterOption : CacheRequestOption<ListInsertAfterResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the pivot value
        /// </summary>
        public string PivotValue { get; set; }

        /// <summary>
        /// Gets or sets the insert value
        /// </summary>
        public string InsertValue { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list insert after response</returns>
        protected override async Task<ListInsertAfterResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListInsertAfterAsync(server, this).ConfigureAwait(false);
        }
    }
}
