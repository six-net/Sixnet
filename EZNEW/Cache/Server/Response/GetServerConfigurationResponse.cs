namespace EZNEW.Cache.Server
{
    /// <summary>
    /// Get server configuration response
    /// </summary>
    public class GetServerConfigurationResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the server configuration
        /// </summary>
        public CacheServerConfiguration ServerConfiguration { get; set; }
    }
}
