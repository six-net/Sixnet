namespace Sixnet.Cache.SortedSet.Response
{
    /// <summary>
    /// Sorted set rank response
    /// </summary>
    public class SortedSetRankResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the rank
        /// </summary>
        public long? Rank { get; set; }
    }
}
