using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Keys.Response;

namespace Sixnet.Cache.Keys.Options
{
    /// <summary>
    /// Time to live options
    /// </summary>
    public class TimeToLiveOptions : CacheOperationOptions<TimeToLiveResponse>
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
        /// <returns>Return time to live response</returns>
        protected override async Task<TimeToLiveResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyTimeToLiveAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return time to live response</returns>
        protected override TimeToLiveResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.KeyTimeToLive(server, this);
        }
    }
}
