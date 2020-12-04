using System.Collections.Generic;

namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set range by value response
    /// </summary>
    public class SortedSetRangeByValueResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
