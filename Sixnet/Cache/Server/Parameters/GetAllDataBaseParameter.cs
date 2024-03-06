using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Cache.Server.Response;

namespace Sixnet.Cache.Server.Parameters
{
    /// <summary>
    /// Get all database parameter
    /// </summary>
    public class GetAllDataBaseParameter : CacheParameter<GetAllDataBaseResult>
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
        protected override async Task<GetAllDataBaseResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetAllDataBaseAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get all database response</returns>
        protected override GetAllDataBaseResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, CacheServer server)
        {
            return cacheProvider.GetAllDataBase(server, this);
        }
    }
}
