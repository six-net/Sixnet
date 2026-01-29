// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set range by rank with scores result
    /// </summary>
    public class SixnetSortedSetRangeByRankWithScoresResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<SixnetSortedSetMember> Members { get; set; }
    }
}
