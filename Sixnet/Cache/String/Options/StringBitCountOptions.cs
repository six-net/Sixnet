using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.String.Response;

namespace Sixnet.Cache.String
{
    /// <summary>
    /// String bit count operations
    /// </summary>
    public class StringBitCountOptions : CacheOptions<StringBitCountResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the start
        /// </summary>
        public long Start { get; set; } = 0;

        /// <summary>
        /// Gets or sets the end
        /// </summary>
        public long End { get; set; } = -1;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string bit count response</returns>
        protected override async Task<StringBitCountResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringBitCountAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string bit count response</returns>
        protected override StringBitCountResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.StringBitCount(server, this);
        }
    }
}
