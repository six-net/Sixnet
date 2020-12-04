namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set remove range by value response
    /// </summary>
    public class SortedSetRemoveRangeByValueResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the remove count
        /// </summary>
        public long RemoveCount { get; set; }
    }
}
