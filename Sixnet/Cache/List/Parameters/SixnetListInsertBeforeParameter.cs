// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.List.Results;

namespace Sixnet.Cache.List.Parameters
{
    /// <summary>
    /// List insert before parameter
    /// </summary>
    public class SixnetListInsertBeforeParameter : SixnetCacheParameter<SixnetListInsertBeforeResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

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
        protected override async Task<SixnetListInsertBeforeResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.ListInsertBeforeAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return insert before response</returns>
        protected override SixnetListInsertBeforeResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.ListInsertBefore(server, this);
        }
    }
}
