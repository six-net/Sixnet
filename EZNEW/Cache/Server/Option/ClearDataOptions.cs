using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Server
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
        /// Gets or sets the end points
        /// </summary>
        public List<CacheEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return clear data response</returns>
        protected override async Task<IEnumerable<ClearDataResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ClearDataAsync(server, this).ConfigureAwait(false);
        }
    }
}
