using System.Collections.Generic;

namespace Sixnet.Cache.SortedSet.Results
{
    /// <summary>
    /// Sorted set range by value result
    /// </summary>
    public class SortedSetRangeByValueResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
