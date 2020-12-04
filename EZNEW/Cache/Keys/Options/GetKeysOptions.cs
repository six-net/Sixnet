using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Get keys options
    /// </summary>
    public class GetKeysOptions : CacheOptions<GetKeysResponse>
    {
        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public KeyQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public List<CacheEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get keys response</returns>
        protected override async Task<IEnumerable<GetKeysResponse>> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetKeysAsync(server, this).ConfigureAwait(false);
        }
    }
}
