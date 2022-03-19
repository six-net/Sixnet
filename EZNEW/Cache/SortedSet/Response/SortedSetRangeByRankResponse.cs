﻿using System.Collections.Generic;

namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set range by rank response
    /// </summary>
    public class SortedSetRangeByRankResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<string> Members { get; set; }
    }
}
