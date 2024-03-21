﻿using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sixnet.Cache.String.Results;

namespace Sixnet.Cache.String.Parameters
{
    /// <summary>
    /// String set bit parameter
    /// </summary>
    public class StringSetBitParameter : CacheParameter<StringSetBitResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the offset
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// Gets or sets whether set bit value
        /// </summary>
        public bool Bit { get; set; }

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
        /// <returns>Return string bit response</returns>
        protected override async Task<StringSetBitResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringSetBitAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string bit response</returns>
        protected override StringSetBitResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.StringSetBit(server, this);
        }
    }
}