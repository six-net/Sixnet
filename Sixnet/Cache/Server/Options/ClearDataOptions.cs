using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Server.Response;

namespace Sixnet.Cache.Server.Options
{
    /// <summary>
    /// Clear data options
    /// </summary>
    public class ClearDataOptions : CacheOptions<ClearDataResponse>
    {
        ///// <summary>
        ///// Gets or sets the clear data databases
        ///// </summary>
        //public List<CacheDatabase> Databases { get; set; }

        /// <summary>
        /// Gets or sets the end point
        /// </summary>
        public CacheEndPoint EndPoint { get; set; }

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

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return clear data response</returns>
        protected override ClearDataResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.ClearData(server, this);
        }
    }
}
