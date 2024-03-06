namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set decrement result
    /// </summary>
    public class SortedSetDecrementResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the new score value
        /// </summary>
        public double NewScore { get; set; }
    }
}
