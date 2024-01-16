using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Keys.Response;

namespace Sixnet.Cache.Keys.Options
{
    /// <summary>
    /// Get keys options
    /// </summary>
    public class GetKeysOptions : CacheOptions<GetKeysResponse>
    {
        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public KeyQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public CacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get keys response</returns>
        protected override async Task<GetKeysResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetKeysAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get keys response</returns>
        protected override GetKeysResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.GetKeys(server, this);
        }
    }
}
