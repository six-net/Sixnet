namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set increment result
    /// </summary>
    public class SortedSetIncrementResult : CacheResult
    {
        /// <summary>
        /// Gets or sets new score value
        /// </summary>
        public double NewScore { get; set; }
    }
}
