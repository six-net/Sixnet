using System.Collections.Generic;
using System.Threading.Tasks;


namespace EZNEW.Cache.Server
{
    /// <summary>
    /// Save server configuration options
    /// </summary>
    public class SaveServerConfigurationOptions : CacheOptions<SaveServerConfigurationResponse>
    {
        /// <summary>
        /// Gets or sets the configuration
        /// </summary>
        public CacheServerConfiguration ServerConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public List<CacheEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override async Task<IEnumerable<SaveServerConfigurationResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SaveServerConfigurationAsync(server, this).ConfigureAwait(false);
        }
    }
}
