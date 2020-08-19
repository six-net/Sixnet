namespace EZNEW.Cache.SortedSet.Response
{
    /// <summary>
    /// Sorted set remove response
    /// </summary>
    public class SortedSetRemoveResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the remove count
        /// </summary>
        public long RemoveCount { get; set; }
    }
}
