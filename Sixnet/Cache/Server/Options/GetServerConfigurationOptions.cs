using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Server.Response;

namespace Sixnet.Cache.Server.Options
{
    /// <summary>
    /// Get server configuration options
    /// </summary>
    public class GetServerConfigurationOptions : CacheOperationOptions<GetServerConfigurationResponse>
    {
        /// <summary>
        /// Gets or sets the end point
        /// </summary>
        public CacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get server configuration response</returns>
        protected override async Task<GetServerConfigurationResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetServerConfigurationAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get server configuration response</returns>
        protected override GetServerConfigurationResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.GetServerConfiguration(server, this);
        }
    }
}
