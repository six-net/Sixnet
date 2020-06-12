using System.Threading.Tasks;
using EZNEW.Cache.Set.Response;

namespace EZNEW.Cache.Set.Request
{
    /// <summary>
    /// Set pop option
    /// </summary>
    public class SetPopOption : CacheRequestOption<SetPopResponse>
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
        /// <returns>Return set pop response response</returns>
        protected override async Task<SetPopResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetPopAsync(server, this).ConfigureAwait(false);
        }
    }
}
