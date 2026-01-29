// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set increment result
    /// </summary>
    public class SixnetSortedSetIncrementResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets new score value
        /// </summary>
        public double NewScore { get; set; }
    }
}
