using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Cache.Server.Response;

namespace EZNEW.Cache.Server.Request
{
    /// <summary>
    /// Clear data option
    /// </summary>
    public class ClearDataOption : CacheRequestOption<ClearDataResponse>
    {
        /// <summary>
        /// Gets or sets the clear data databases
        /// </summary>
        public List<CacheDatabase> Databases
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return clear data response</returns>
        protected override async Task<ClearDataResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ClearDataAsync(server, this).ConfigureAwait(false);
        }
    }
}
