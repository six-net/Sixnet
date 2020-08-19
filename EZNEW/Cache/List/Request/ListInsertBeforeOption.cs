using System.Threading.Tasks;
using EZNEW.Cache.List.Response;

namespace EZNEW.Cache.List.Request
{
    /// <summary>
    /// List insert before option
    /// </summary>
    public class ListInsertBeforeOption : CacheRequestOption<ListInsertBeforeResponse>
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
        /// <returns>Return insert before response</returns>
        protected override async Task<ListInsertBeforeResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListInsertBeforeAsync(server, this).ConfigureAwait(false);
        }
    }
}
