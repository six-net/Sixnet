using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Cache.Hash.Response;

namespace EZNEW.Cache.Hash.Request
{
    /// <summary>
    /// Hash delete option
    /// </summary>
    public class HashDeleteOption : CacheRequestOption<HashDeleteResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the hash fields
        /// </summary>
        public List<string> HashFields { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash delete response</returns>
        protected override async Task<HashDeleteResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashDeleteAsync(server, this).ConfigureAwait(false);
        }
    }
}
