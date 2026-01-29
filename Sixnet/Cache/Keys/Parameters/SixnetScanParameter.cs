// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Keys.Results;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Scan parameter key
    /// </summary>
    public class SixnetScanParameter : SixnetCacheParameter<SixnetScanResult>
    {
        /// <summary>
        /// Gets or sets the cursor
        /// </summary>
        public long Cursor { get; set; }

        /// <summary>
        /// Gets or sets the pattern
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Execute the cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return delete key response</returns>
        protected override async Task<SixnetScanResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.KeyScanAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute the cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return delete key response</returns>
        protected override SixnetScanResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.KeyScan(server, this);
        }
    }
}
