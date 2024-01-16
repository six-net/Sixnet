using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Hash.Response;

namespace Sixnet.Cache.Hash.Options
{
    /// <summary>
    /// Hash values options
    /// </summary>
    public class HashValuesOptions : CacheOptions<HashValuesResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash values response</returns>
        protected override async Task<HashValuesResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.HashValuesAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash values response</returns>
        protected override HashValuesResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.HashValues(server, this);
        }
    }
}
