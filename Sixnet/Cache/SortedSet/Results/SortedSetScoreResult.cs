namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set score result
    /// </summary>
    public class SortedSetScoreResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the score value
        /// </summary>
        public double? Score { get; set; }
    }
}
