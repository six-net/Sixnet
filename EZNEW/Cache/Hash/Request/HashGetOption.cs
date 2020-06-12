using System.Threading.Tasks;
using EZNEW.Cache.Hash.Response;

namespace EZNEW.Cache.Hash.Request
{
    /// <summary>
    /// Hash get option
    /// </summary>
    public class HashGetOption : CacheRequestOption<HashGetResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets he hash field
        /// </summary>
        public string HashField
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash get response</returns>
        protected override async Task<HashGetResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashGetAsync(server, this).ConfigureAwait(false);
        }
    }
}
