using System.Threading.Tasks;
using EZNEW.Cache.Keys.Response;

namespace EZNEW.Cache.Keys.Request
{
    /// <summary>
    /// Migrate option
    /// </summary>
    public class MigrateOption : CacheRequestOption<MigrateResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the destination server
        /// </summary>
        public CacheServer DestinationServer
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the time out milliseconds
        /// </summary>
        public int TimeOutMilliseconds
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the migrate options
        /// </summary>
        public CacheMigrateOptions MigrateOptions
        {
            get; set;
        } = CacheMigrateOptions.None;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return migrate key response</returns>
        protected override async Task<MigrateResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyMigrateAsync(server, this).ConfigureAwait(false);
        }
    }
}
