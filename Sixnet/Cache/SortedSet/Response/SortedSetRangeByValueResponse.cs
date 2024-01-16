using System.Collections.Generic;

namespace Sixnet.Cache.SortedSet.Response
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
