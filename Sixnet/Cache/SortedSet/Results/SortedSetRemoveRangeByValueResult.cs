namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set remove range by value result
    /// </summary>
    public class SortedSetRemoveRangeByValueResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the remove count
        /// </summary>
        public long RemoveCount { get; set; }
    }
}
