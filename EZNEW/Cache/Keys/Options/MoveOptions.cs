using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Move options
    /// </summary>
    public class MoveOptions : CacheOptions<MoveResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the database
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return move response</returns>
        protected override async Task<IEnumerable<MoveResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyMoveAsync(server, this).ConfigureAwait(false);
        }
    }
}
