using System.Threading.Tasks;
using EZNEW.Cache.Server.Response;


namespace EZNEW.Cache.Server.Request
{
    /// <summary>
    /// Save server configuration option
    /// </summary>
    public class SaveServerConfigurationOption : CacheRequestOption<SaveServerConfigurationResponse>
    {
        /// <summary>
        /// Gets or sets the configuration
        /// </summary>
        public CacheServerConfiguration ServerConfiguration
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return save server configuration response</returns>
        protected override async Task<SaveServerConfigurationResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SaveServerConfigurationAsync(server, this).ConfigureAwait(false);
        }
    }
}
