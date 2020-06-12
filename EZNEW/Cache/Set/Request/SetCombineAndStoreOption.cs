using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Cache.Set.Response;

namespace EZNEW.Cache.Set.Request
{
    /// <summary>
    /// Set combine and store option
    /// </summary>
    public class SetCombineAndStoreOption : CacheRequestOption<SetCombineAndStoreResponse>
    {
        /// <summary>
        /// Gets or sets the source keys
        /// </summary>
        public List<CacheKey> SourceKeys
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the destination key
        /// </summary>
        public CacheKey DestinationKey
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the set operation type
        /// </summary>
        public SetOperationType SetOperationType
        {
            get; set;
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return set combine and store response</returns>
        protected override async Task<SetCombineAndStoreResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SetCombineAndStoreAsync(server, this).ConfigureAwait(false);
        }
    }
}
