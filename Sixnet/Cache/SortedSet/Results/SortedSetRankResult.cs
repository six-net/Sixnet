namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set rank result
    /// </summary>
    public class SortedSetRankResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the rank
        /// </summary>
        public long? Rank { get; set; }
    }
}
