// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set range by score result
    /// </summary>
    public class SixnetSortedSetRangeByScoreResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
