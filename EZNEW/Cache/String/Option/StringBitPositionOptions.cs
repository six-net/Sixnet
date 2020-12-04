using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.String
{
    /// <summary>
    /// String bit position options
    /// </summary>
    public class StringBitPositionOptions : CacheOptions<StringBitPositionResponse>
    {
        /// <summary>
        /// Gets or sets cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets whether set bit
        /// </summary>
        public bool Bit { get; set; }

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
        /// <returns>Return string bit position response</returns>
        protected override async Task<IEnumerable<StringBitPositionResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.StringBitPositionAsync(server, this).ConfigureAwait(false);
        }
    }
}
