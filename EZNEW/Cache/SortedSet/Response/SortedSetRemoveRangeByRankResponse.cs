namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set remove range by rank response
    /// </summary>
    public class SortedSetRemoveRangeByRankResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the remove count
        /// </summary>
        public long RemoveCount { get; set; }
    }
}
