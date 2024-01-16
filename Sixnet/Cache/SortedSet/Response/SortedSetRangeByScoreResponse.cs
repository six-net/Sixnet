﻿using System.Collections.Generic;

namespace Sixnet.Cache.SortedSet.Response
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
