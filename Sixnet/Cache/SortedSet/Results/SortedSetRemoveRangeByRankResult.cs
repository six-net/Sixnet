namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set remove range by rank result
    /// </summary>
    public class SortedSetRemoveRangeByRankResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the remove count
        /// </summary>
        public long RemoveCount { get; set; }
    }
}
