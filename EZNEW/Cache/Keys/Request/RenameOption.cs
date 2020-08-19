using System.Threading.Tasks;
using EZNEW.Cache.Keys.Response;

namespace EZNEW.Cache.Keys.Request
{
    /// <summary>
    /// Rename option
    /// </summary>
    public class RenameOption : CacheRequestOption<RenameResponse>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the new key
        /// </summary>
        public CacheKey NewKey { get; set; }

        /// <summary>
        /// Gets or sets whether only set new key when not exists
        /// </summary>
        public bool WhenNewKeyNotExists { get; set; }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return rename response</returns>
        protected override async Task<RenameResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.KeyRenameAsync(server, this).ConfigureAwait(false);
        }
    }
}
