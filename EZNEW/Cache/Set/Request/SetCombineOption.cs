using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Cache.Set.Response;

namespace EZNEW.Cache.Set.Request
{
    /// <summary>
    /// Set combine option
    /// </summary>
    public class SetCombineOption : CacheRequestOption<SetCombineResponse>
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public List<CacheKey> Keys
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the set operation
        /// </summary>
        public SetOperationType SetOperationType
        {
            get; set;
        } = SetOperationType.Union;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override async Task<SetCombineResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetCombineAsync(server, this).ConfigureAwait(false);
        }
    }
}
