// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.Server.Response;

namespace Sixnet.Cache.Server.Parameters
{
    /// <summary>
    /// Clear data parameter
    /// </summary>
    public class SixnetClearDataParameter : SixnetCacheParameter<SixnetClearDataResult>
    {
        ///// <summary>
        ///// Gets or sets the clear data databases
        ///// </summary>
        //public List<CacheDatabase> Databases { get; set; }

        /// <summary>
        /// Gets or sets the end point
        /// </summary>
        public SixnetCacheEndPoint EndPoint { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return clear data response</returns>
        protected override async Task<SixnetClearDataResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.ClearDataAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return clear data response</returns>
        protected override SixnetClearDataResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.ClearData(server, this);
        }
    }
}
