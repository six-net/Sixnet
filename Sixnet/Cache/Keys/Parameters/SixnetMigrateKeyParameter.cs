// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Keys.Results;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Migrate key parameter
    /// </summary>
    public class SixnetMigrateKeyParameter : SixnetCacheParameter<SixnetMigrateKeyResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the destination
        /// </summary>
        public SixnetCacheEndPoint Destination { get; set; }

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
        protected override async Task<SixnetMigrateKeyResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.KeyMigrateAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return migrate key response</returns>
        protected override SixnetMigrateKeyResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.KeyMigrate(server, this);
        }
    }
}
