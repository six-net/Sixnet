namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set combine and store result
    /// </summary>
    public class SortedSetCombineAndStoreResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the new set length
        /// </summary>
        public long NewSetLength { get; set; }
    }
}
