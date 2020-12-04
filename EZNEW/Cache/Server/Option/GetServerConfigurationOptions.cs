using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Server
{
    /// <summary>
    /// Get server configuration options
    /// </summary>
    public class GetServerConfigurationOptions : CacheOptions<GetServerConfigurationResponse>
    {
        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public List<CacheEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get server configuration response</returns>
        protected override async Task<IEnumerable<GetServerConfigurationResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetServerConfigurationAsync(server, this).ConfigureAwait(false);
        }
    }
}
