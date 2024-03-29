﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Hash.Response;

namespace Sixnet.Cache.Hash.Options
{
    /// <summary>
    /// Hash scan options
    /// </summary>
    public class HashScanOptions : CacheOptions<HashScanResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the pattern
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Gets or sets the pattern type
        /// </summary>
        public KeyMatchPattern PatternType { get; set; }

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
        protected override async Task<HashScanResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashScanAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash scan response</returns>
        protected override HashScanResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.HashScan(server, this);
        }
    }
}
