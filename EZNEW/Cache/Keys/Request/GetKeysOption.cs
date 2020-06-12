using System.Threading.Tasks;
using EZNEW.Cache.Keys.Response;

namespace EZNEW.Cache.Keys.Request
{
    /// <summary>
    /// Get keys options
    /// </summary>
    public class GetKeysOption : CacheRequestOption<GetKeysResponse>
    {
        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public KeyQuery Query
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return get keys response</returns>
        protected override async Task<GetKeysResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.GetKeysAsync(server, this).ConfigureAwait(false);
        }
    }
}
