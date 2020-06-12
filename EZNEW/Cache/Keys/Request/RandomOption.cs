using System.Threading.Tasks;
using EZNEW.Cache.Keys.Response;

namespace EZNEW.Cache.Keys.Request
{
    /// <summary>
    /// Random option
    /// </summary>
    public class RandomOption : CacheRequestOption<RandomResponse>
    {
        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return random response</returns>
        protected override async Task<RandomResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyRandomAsync(server, this).ConfigureAwait(false);
        }
    }
}
