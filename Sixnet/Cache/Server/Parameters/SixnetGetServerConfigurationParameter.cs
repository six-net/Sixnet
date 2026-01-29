// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Server.Response;

namespace Sixnet.Cache.Server.Parameters
{
    /// <summary>
    /// Get server configuration parameter
    /// </summary>
    public class SixnetGetServerConfigurationParameter : SixnetCacheParameter<SixnetGetServerConfigurationResult>
    {
        /// <summary>
        /// Gets or sets the end point
        /// </summary>
        public SixnetCacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get server configuration response</returns>
        protected override async Task<SixnetGetServerConfigurationResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.GetServerConfigurationAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get server configuration response</returns>
        protected override SixnetGetServerConfigurationResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.GetServerConfiguration(server, this);
        }
    }
}
