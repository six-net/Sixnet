using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Keys.Results;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Migrate key parameter
    /// </summary>
    public class MigrateKeyParameter : CacheParameter<MigrateKeyResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the destination
        /// </summary>
        public CacheEndPoint Destination { get; set; }

        /// <summary>
        /// Gets or sets the time out milliseconds
        /// </summary>
        public int TimeOutMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets whether migrate by copy
        /// </summary>
        public bool CopyCurrent { get; set; }

        /// <summary>
        /// Gets or sets whether migrate by replace
        /// </summary>
        public bool ReplaceDestination { get; set; } = true;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return migrate key response</returns>
        protected override async Task<MigrateKeyResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyMigrateAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return migrate key response</returns>
        protected override MigrateKeyResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.KeyMigrate(server, this);
        }
    }
}
