using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Get key detail options
    /// </summary>
    public class GetDetailOptions : CacheOptions<GetDetailResponse>
    {
        /// <summary>
        /// Gets or sets cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public List<CacheEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get key detail response</returns>
        protected override async Task<IEnumerable<GetDetailResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetKeyDetailAsync(server, this).ConfigureAwait(false);
        }
    }
}
