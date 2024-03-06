namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set remove range by score result
    /// </summary>
    public class SortedSetRemoveRangeByScoreResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the remove count
        /// </summary>
        public long RemoveCount { get; set; }
    }
}
