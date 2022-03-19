﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set rank options
    /// </summary>
    public class SortedSetRankOptions : CacheOptions<SortedSetRankResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the member
        /// </summary>
        public string Member { get; set; }

        /// <summary>
        /// Gets or sets the order type
        /// </summary>
        public CacheOrder Order { get; set; } = CacheOrder.Ascending;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set rank response</returns>
        protected override async Task<IEnumerable<SortedSetRankResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetRankAsync(server, this).ConfigureAwait(false);
        }
    }
}
