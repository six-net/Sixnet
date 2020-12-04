using System.Collections.Generic;

namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set range by score response
    /// </summary>
    public class SortedSetRangeByScoreResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
