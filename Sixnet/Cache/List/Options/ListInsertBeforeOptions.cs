using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.List.Response;

namespace Sixnet.Cache.List.Options
{
    /// <summary>
    /// List insert before options
    /// </summary>
    public class ListInsertBeforeOptions : CacheOperationOptions<ListInsertBeforeResponse>
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
        protected override async Task<ListInsertBeforeResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListInsertBeforeAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return insert before response</returns>
        protected override ListInsertBeforeResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.ListInsertBefore(server, this);
        }
    }
}
