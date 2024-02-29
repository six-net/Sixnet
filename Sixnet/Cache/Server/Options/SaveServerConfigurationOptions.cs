using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Server.Response;

namespace Sixnet.Cache.Server.Options
{
    /// <summary>
    /// Save server configuration options
    /// </summary>
    public class SaveServerConfigurationOptions : CacheOperationOptions<SaveServerConfigurationResponse>
    {
        /// <summary>
        /// Gets or sets the configuration
        /// </summary>
        public CacheServerConfiguration ServerConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the end point
        /// </summary>
        public CacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override async Task<SaveServerConfigurationResponse> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SaveServerConfigurationAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override SaveServerConfigurationResponse ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.SaveServerConfiguration(server, this);
        }
    }
}
