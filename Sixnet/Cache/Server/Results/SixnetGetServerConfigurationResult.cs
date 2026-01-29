// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Server.Response
{
    /// <summary>
    /// Get server configuration result
    /// </summary>
    public class SixnetGetServerConfigurationResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the server configuration
        /// </summary>
        public SixnetCacheServerConfiguration ServerConfiguration { get; set; }
    }
}
