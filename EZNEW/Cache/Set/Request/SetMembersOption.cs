using System.Threading.Tasks;
using EZNEW.Cache.Set.Response;

namespace EZNEW.Cache.Set.Request
{
    /// <summary>
    /// Set members option
    /// </summary>
    public class SetMembersOption : CacheRequestOption<SetMembersResponse>
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
        /// <returns>Return set members response</returns>
        protected override async Task<SetMembersResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetMembersAsync(server, this).ConfigureAwait(false);
        }
    }
}
