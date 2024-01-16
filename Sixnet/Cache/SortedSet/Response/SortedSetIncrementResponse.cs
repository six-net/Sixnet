﻿namespace Sixnet.Cache.SortedSet.Response
{
    /// <summary>
    /// Sorted set increment response
    /// </summary>
    public class SortedSetIncrementResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets new score value
        /// </summary>
        public double NewScore { get; set; }
    }
}
