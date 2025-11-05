// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Server.Response
{
    /// <summary>
    /// Get server configuration result
    /// </summary>
    public class GetServerConfigurationResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the server configuration
        /// </summary>
        public CacheServerConfiguration ServerConfiguration { get; set; }
    }
}
