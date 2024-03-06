using System.Collections.Generic;

namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set range by rank with scores result
    /// </summary>
    public class SortedSetRangeByRankWithScoresResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<SortedSetMember> Members { get; set; }
    }
}
