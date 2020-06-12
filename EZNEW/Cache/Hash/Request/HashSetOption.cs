using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Cache.Hash.Response;

namespace EZNEW.Cache.Hash.Request
{
    /// <summary>
    /// Hash set option
    /// </summary>
    public class HashSetOption : CacheRequestOption<HashSetResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the hash values
        /// </summary>
        public Dictionary<string, dynamic> HashValues
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash set response</returns>
        protected override async Task<HashSetResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashSetAsync(server, this).ConfigureAwait(false);
        }
    }
}
