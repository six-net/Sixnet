// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Server.Response;

namespace Sixnet.Cache.Server.Parameters
{
    /// <summary>
    /// Save server configuration parameter
    /// </summary>
    public class SixnetSaveServerConfigurationParameter : SixnetCacheParameter<SixnetSaveServerConfigurationResult>
    {
        /// <summary>
        /// Gets or sets the configuration
        /// </summary>
        public SixnetCacheServerConfiguration ServerConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the end point
        /// </summary>
        public SixnetCacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override async Task<SixnetSaveServerConfigurationResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SaveServerConfigurationAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override SixnetSaveServerConfigurationResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SaveServerConfiguration(server, this);
        }
    }
}
