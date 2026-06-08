// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Hash.Results;

namespace Sixnet.Cache.Hash.Parameters
{
    /// <summary>
    /// Hash scan parameter
    /// </summary>
    public class SixnetHashScanParameter : SixnetCacheParameter<SixnetHashScanResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the pattern
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the pattern type
        /// </summary>
        public SixnetKeyMatchPattern PatternType { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the cursor
        /// </summary>
        public long Cursor { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash scan response</returns>
        protected override async Task<SixnetHashScanResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.HashScanAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash scan response</returns>
        protected override SixnetHashScanResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.HashScan(server, this);
        }
    }
}
