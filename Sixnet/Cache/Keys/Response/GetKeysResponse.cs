﻿namespace Sixnet.Cache.Keys.Response
{
    /// <summary>
    /// Get keys response
    /// </summary>
    public class GetKeysResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public CachePaging<CacheKey> Keys { get; set; }
    }
}
