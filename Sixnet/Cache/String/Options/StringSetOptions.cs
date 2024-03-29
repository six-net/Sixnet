﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixnet.Cache.String.Response;

namespace Sixnet.Cache.String
{
    /// <summary>
    /// String set options
    /// </summary>
    public class StringSetOptions : CacheOptions<StringSetResponse>
    {
        /// <summary>
        /// Gets or sets the data items
        /// </summary>
        public List<CacheEntry> Items { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string set response</returns>
        protected override async Task<StringSetResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringSetAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string set response</returns>
        protected override StringSetResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.StringSet(server, this);
        }
    }
}
