using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Delete key options
    /// </summary>
    public class DeleteOptions : CacheOptions<DeleteResponse>
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public List<CacheKey> Keys { get; set; }

        /// <summary>
        /// Execute the cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return delete key response</returns>
        protected override async Task<IEnumerable<DeleteResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyDeleteAsync(server, this).ConfigureAwait(false);
        }
    }
}
