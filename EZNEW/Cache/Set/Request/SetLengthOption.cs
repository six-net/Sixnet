using System.Threading.Tasks;
using EZNEW.Cache.Set.Response;

namespace EZNEW.Cache.Set.Request
{
    /// <summary>
    /// Set length option
    /// </summary>
    public class SetLengthOption : CacheRequestOption<SetLengthResponse>
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
        /// <returns>Return set length response</returns>
        protected override async Task<SetLengthResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetLengthAsync(server, this).ConfigureAwait(false);
        }
    }
}
