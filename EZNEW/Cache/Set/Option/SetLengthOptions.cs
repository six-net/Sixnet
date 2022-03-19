﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Set
{
    /// <summary>
    /// Set length options
    /// </summary>
    public class SetLengthOptions : CacheOptions<SetLengthResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set length response</returns>
        protected override async Task<IEnumerable<SetLengthResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetLengthAsync(server, this).ConfigureAwait(false);
        }
    }
}
