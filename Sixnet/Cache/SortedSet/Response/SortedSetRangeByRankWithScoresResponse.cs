﻿using System.Collections.Generic;

namespace Sixnet.Cache.SortedSet.Response
{
    /// <summary>
    /// Sorted set range by rank with scores response
    /// </summary>
    public class SortedSetRangeByRankWithScoresResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the members
        /// </summary>
        public List<SortedSetMember> Members { get; set; }
    }
}
