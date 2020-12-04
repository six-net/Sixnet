namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set combine and store response
    /// </summary>
    public class SortedSetCombineAndStoreResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the new set length
        /// </summary>
        public long NewSetLength { get; set; }
    }
}
