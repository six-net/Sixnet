// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.String.Results;

namespace Sixnet.Cache.String.Parameters
{
    /// <summary>
    /// String bit position parameter
    /// </summary>
    public class SixnetStringBitPositionParameter : SixnetCacheParameter<SixnetStringBitPositionResult>
    {
        /// <summary>
        /// Gets or sets cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

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
        protected override async Task<SixnetStringBitPositionResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.StringBitPositionAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return string bit position response</returns>
        protected override SixnetStringBitPositionResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.StringBitPosition(server, this);
        }
    }
}
