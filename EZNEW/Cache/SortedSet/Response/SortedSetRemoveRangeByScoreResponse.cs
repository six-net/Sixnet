﻿namespace EZNEW.Cache.SortedSet
{
    /// <summary>
    /// Sorted set remove range by score response
    /// </summary>
    public class SortedSetRemoveRangeByScoreResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the remove count
        /// </summary>
        public long RemoveCount { get; set; }
    }
}
