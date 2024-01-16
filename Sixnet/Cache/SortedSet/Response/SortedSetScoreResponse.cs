﻿namespace Sixnet.Cache.SortedSet.Response
{
    /// <summary>
    /// Sorted set score response
    /// </summary>
    public class SortedSetScoreResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the score value
        /// </summary>
        public double? Score { get; set; }
    }
}
