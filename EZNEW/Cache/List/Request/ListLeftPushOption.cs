using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Cache.List.Response;

namespace EZNEW.Cache.List.Request
{
    /// <summary>
    /// List left push option
    /// </summary>
    public class ListLeftPushOption : CacheRequestOption<ListLeftPushResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<string> Values
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return list left push response</returns>
        protected override async Task<ListLeftPushResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.ListLeftPushAsync(server, this).ConfigureAwait(false);
        }
    }
}
