using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Cache.Keys.Results;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Scan parameter key
    /// </summary>
    public class ScanParameter : CacheParameter<ScanResult>
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
        protected override async Task<ScanResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyScanAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute the cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return delete key response</returns>
        protected override ScanResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.KeyScan(server, this);
        }
    }
}
