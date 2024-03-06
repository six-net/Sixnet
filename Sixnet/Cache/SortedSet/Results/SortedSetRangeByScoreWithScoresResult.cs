using System.Collections.Generic;

namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set range by score with scores result
    /// </summary>
    public class SortedSetRangeByScoreWithScoresResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<SortedSetMember> Members { get; set; }
    }
}
