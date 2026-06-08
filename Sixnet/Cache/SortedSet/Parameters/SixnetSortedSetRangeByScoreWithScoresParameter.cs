// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Cache.SortedSet.Results;

namespace Sixnet.Cache.SortedSet.Parameters
{
    /// <summary>
    /// Sortes set range by score with scores parameter
    /// </summary>
    public class SixnetSortedSetRangeByScoreWithScoresParameter : SixnetCacheParameter<SixnetSortedSetRangeByScoreWithScoresResult>
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the start score value
        /// </summary>
        public double Start { get; set; } = double.MinValue;

        /// <summary>
        /// Gets or sets the stop score value
        /// </summary>
        public double Stop { get; set; } = double.MaxValue;

        /// <summary>
        /// Gets or sets the data offset
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Gets or sets the data count
        /// </summary>
        public int Count { get; set; } = -1;

        /// <summary>
        /// Gets or sets the order type
        /// </summary>
        public SixnetCacheOrder Order { get; set; } = SixnetCacheOrder.Ascending;

        /// <summary>
        /// Gets or sets the exclude type
        /// </summary>
        public SixnetBoundaryExclude Exclude { get; set; } = SixnetBoundaryExclude.None;

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set range by score with scores response</returns>
        protected override async Task<SixnetSortedSetRangeByScoreWithScoresResult> ExecuteCacheOperationAsync(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return await cacheProvider.SortedSetRangeByScoreWithScoresAsync(server, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="cacheProvider">Cache provider</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return sorted set range by score with scores response</returns>
        protected override SixnetSortedSetRangeByScoreWithScoresResult ExecuteCacheOperation(ISixnetCacheProvider cacheProvider, SixnetCacheServer server)
        {
            return cacheProvider.SortedSetRangeByScoreWithScores(server, this);
        }
    }
}
