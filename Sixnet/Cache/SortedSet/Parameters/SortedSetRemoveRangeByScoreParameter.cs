﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.SortedSet.Results;

namespace Sixnet.Cache.SortedSet.Parameters
{
    /// <summary>
    /// Sorted set remove range by score parameter
    /// </summary>
    public class SortedSetRemoveRangeByScoreParameter : CacheParameter<SortedSetRemoveRangeByScoreResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the start score value
        /// </summary>
        public double Start { get; set; }

        /// <summary>
        /// Gets or sets the stop score value
        /// </summary>
        public double Stop { get; set; }

        /// <summary>
        /// Gets or sets the exclude type
        /// </summary>
        public BoundaryExclude Exclude { get; set; } = BoundaryExclude.None;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set remove range by score response</returns>
        protected override async Task<SortedSetRemoveRangeByScoreResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRemoveRangeByScoreAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set remove range by score response</returns>
        protected override SortedSetRemoveRangeByScoreResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SortedSetRemoveRangeByScore(server, this);
        }
    }
}