using System.Threading.Tasks;
using EZNEW.Cache.Server.Response;

namespace EZNEW.Cache.Server.Request
{
    /// <summary>
    /// Get server configuration option
    /// </summary>
    public class GetServerConfigurationOption : CacheRequestOption<GetServerConfigurationResponse>
    {
        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get server configuration response</returns>
        protected override async Task<GetServerConfigurationResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetServerConfigurationAsync(server, this).ConfigureAwait(false);
        }
    }
}
