using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Cache.SortedSet.Response;

namespace EZNEW.Cache.SortedSet.Request
{
    /// <summary>
    /// Sorted set combine and store
    /// </summary>
    public class SortedSetCombineAndStoreOption : CacheRequestOption<SortedSetCombineAndStoreResponse>
    {
        /// <summary>
        /// Gets or sets the source keys
        /// </summary>
        public List<CacheKey> SourceKeys
        {
            get; set;
        }

        /// <summary>
        /// gets or sets the destination key
        /// </summary>
        public CacheKey DestinationKey
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the set operation
        /// </summary>
        public SetOperationType SetOperationType
        {
            get; set;
        } = SetOperationType.Union;

        /// <summary>
        /// Gets or sets the weights
        /// </summary>
        public double[] Weights
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the set aggregate
        /// </summary>
        public SetAggregate Aggregate
        {
            get; set;
        } = SetAggregate.Sum;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set combine and store response</returns>
        protected override async Task<SortedSetCombineAndStoreResponse> ExecuteCacheOperationAsync(ICacheProvider cacheProvider, CacheServer server)
        {
            return await cacheProvider.SortedSetCombineAndStoreAsync(server, this).ConfigureAwait(false);
        }
    }
}
