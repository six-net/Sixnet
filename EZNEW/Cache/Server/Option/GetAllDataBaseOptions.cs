using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Server
{
    /// <summary>
    /// Get all database options
    /// </summary>
    public class GetAllDataBaseOptions : CacheOptions<GetAllDataBaseResponse>
    {
        /// <summary>
        /// Gets or sets the endpoints
        /// </summary>
        public List<CacheEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get all database response</returns>
        protected override async Task<IEnumerable<GetAllDataBaseResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetAllDataBaseAsync(server, this).ConfigureAwait(false);
        }
    }
}
