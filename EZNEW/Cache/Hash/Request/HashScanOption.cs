using System.Threading.Tasks;
using EZNEW.Cache.Hash.Response;

namespace EZNEW.Cache.Hash.Request
{
    /// <summary>
    /// Hash scan option
    /// </summary>
    public class HashScanOption : CacheRequestOption<HashScanResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the pattern
        /// </summary>
        public string Pattern
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the pattern type
        /// </summary>
        public PatternType PatternType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize
        {
            get; set;
        } = int.MaxValue;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash scan response</returns>
        protected override async Task<HashScanResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashScanAsync(server, this).ConfigureAwait(false);
        }
    }
}
