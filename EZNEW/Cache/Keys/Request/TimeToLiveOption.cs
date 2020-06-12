using System.Threading.Tasks;
using EZNEW.Cache.Keys.Response;

namespace EZNEW.Cache.Keys.Request
{
    /// <summary>
    /// Time to live option
    /// </summary>
    public class TimeToLiveOption : CacheRequestOption<TimeToLiveResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return time to live response</returns>
        protected override async Task<TimeToLiveResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyTimeToLiveAsync(server, this).ConfigureAwait(false);
        }
    }
}
