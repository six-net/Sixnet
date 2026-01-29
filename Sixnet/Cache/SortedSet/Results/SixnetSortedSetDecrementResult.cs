// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set decrement result
    /// </summary>
    public class SixnetSortedSetDecrementResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the new score value
        /// </summary>
        public double NewScore { get; set; }
    }
}
