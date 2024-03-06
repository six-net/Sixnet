﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.SortedSet.Results;

namespace Sixnet.Cache.SortedSet.Parameters
{
    /// <summary>
    /// Sorted set decrement parameter
    /// </summary>
    public class SortedSetDecrementParameter : CacheParameter<SortedSetDecrementResult>
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
        /// Gets or sets the score value
        /// </summary>
        public double DecrementScore { get; set; }

        /// <summary>
        /// Gets or sets the cache entry expiration
        /// When the specified cache item is not found, the cache item is created with the change expiration information 
        /// </summary>
        public CacheExpiration Expiration { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set decrement response</returns>
        protected override async Task<SortedSetDecrementResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetDecrementAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set decrement response</returns>
        protected override SortedSetDecrementResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SortedSetDecrement(server, this);
        }
    }
}
