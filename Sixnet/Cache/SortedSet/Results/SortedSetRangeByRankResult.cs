using System.Collections.Generic;

namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set range by rank result
    /// </summary>
    public class SortedSetRangeByRankResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
