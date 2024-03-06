﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Set.Results;

namespace Sixnet.Cache.Set.Parameters
{
    /// <summary>
    /// Set combine and store parameter
    /// </summary>
    public class SetCombineAndStoreParameter : CacheParameter<SetCombineAndStoreResult>
    {
        /// <summary>
        /// Gets or sets the source keys
        /// </summary>
        public List<CacheKey> SourceKeys { get; set; }

        /// <summary>
        /// Gets or sets the destination key
        /// </summary>
        public CacheKey DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the set combine operation
        /// </summary>
        public CombineOperation CombineOperation { get; set; } = CombineOperation.Union;

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
        /// <returns>Return set combine and store response</returns>
        protected override async Task<SetCombineAndStoreResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetCombineAndStoreAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set combine and store response</returns>
        protected override SetCombineAndStoreResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SetCombineAndStore(server, this);
        }
    }
}
