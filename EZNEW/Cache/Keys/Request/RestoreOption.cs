using System.Threading.Tasks;
using EZNEW.Cache.Keys.Response;

namespace EZNEW.Cache.Keys.Request
{
    /// <summary>
    /// Restore option
    /// </summary>
    public class RestoreOption : CacheRequestOption<RestoreResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the cache value
        /// </summary>
        public byte[] Value
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return restore response</returns>
        protected override async Task<RestoreResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyRestoreAsync(server, this).ConfigureAwait(false);
        }
    }
}
