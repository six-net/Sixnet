using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Server.Response;

namespace Sixnet.Cache.Server.Options
{
    /// <summary>
    /// Get all database options
    /// </summary>
    public class GetAllDataBaseOptions : CacheOptions<GetAllDataBaseResponse>
    {
        /// <summary>
        /// Gets or sets the endpoint
        /// </summary>
        public CacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get all database response</returns>
        protected override async Task<GetAllDataBaseResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetAllDataBaseAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get all database response</returns>
        protected override GetAllDataBaseResponse ExecuteCacheOperation(ICacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.GetAllDataBase(server, this);
        }
    }
}
