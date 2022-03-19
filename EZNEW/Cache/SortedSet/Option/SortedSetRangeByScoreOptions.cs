﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set range by score options
    /// </summary>
    public class SortedSetRangeByScoreOptions : CacheOptions<SortedSetRangeByScoreResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the start score value
        /// </summary>
        public double Start { get; set; } = double.MinValue;

        /// <summary>
        /// Gets or sets the stop score value
        /// </summary>
        public double Stop { get; set; } = double.MaxValue;

        /// <summary>
        /// Gets or sets the data offset
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the data count
        /// </summary>
        public int Count { get; set; } = -1;

        /// <summary>
        /// Gets or sets the order type
        /// </summary>
        public CacheOrder Order { get; set; } = CacheOrder.Ascending;

        /// <summary>
        /// Gets or sets the exclude type
        /// </summary>
        public BoundaryExclude Exclude { get; set; } = BoundaryExclude.None;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set range by score response</returns>
        protected override async Task<IEnumerable<SortedSetRangeByScoreResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRangeByScoreAsync(server, this).ConfigureAwait(false);
        }
    }
}
