// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Server.Response;

namespace Sixnet.Cache.Server.Parameters
{
    /// <summary>
    /// Get all database parameter
    /// </summary>
    public class SixnetGetAllDataBaseParameter : SixnetCacheParameter<SixnetGetAllDataBaseResult>
    {
        /// <summary>
        /// Gets or sets the endpoint
        /// </summary>
        public SixnetCacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get all database response</returns>
        protected override async Task<SixnetGetAllDataBaseResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.GetAllDataBaseAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get all database response</returns>
        protected override SixnetGetAllDataBaseResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.GetAllDataBase(server, this);
        }
    }
}
