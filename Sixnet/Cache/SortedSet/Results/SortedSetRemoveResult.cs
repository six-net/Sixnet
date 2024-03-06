namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set remove result
    /// </summary>
    public class SortedSetRemoveResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the remove count
        /// </summary>
        public long RemoveCount { get; set; }
    }
}
