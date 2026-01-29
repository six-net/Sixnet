// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Keys.Results;

namespace Sixnet.Cache.Keys.Parameters
{
    /// <summary>
    /// Get keys parameter
    /// </summary>
    public class SixnetGetKeysParameter : SixnetCacheParameter<SixnetGetKeysResult>
    {
        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public SixnetKeyQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public SixnetCacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get keys response</returns>
        protected override async Task<SixnetGetKeysResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.GetKeysAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get keys response</returns>
        protected override SixnetGetKeysResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.GetKeys(server, this);
        }
    }
}
