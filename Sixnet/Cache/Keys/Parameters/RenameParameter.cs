﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Keys.Results;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Rename parameter
    /// </summary>
    public class RenameParameter : CacheParameter<RenameResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the new key
        /// </summary>
        public CacheKey NewKey { get; set; }

        /// <summary>
        /// Gets or sets whether only set new key when not exists
        /// </summary>
        public bool WhenNewKeyNotExists { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return rename response</returns>
        protected override async Task<RenameResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyRenameAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return rename response</returns>
        protected override RenameResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.KeyRename(server, this);
        }
    }
}
