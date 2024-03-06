using System.Collections.Generic;

namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set range by score result
    /// </summary>
    public class SortedSetRangeByScoreResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
