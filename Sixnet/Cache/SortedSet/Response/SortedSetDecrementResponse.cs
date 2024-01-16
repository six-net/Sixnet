namespace Sixnet.Cache.SortedSet.Response
{
    /// <summary>
    /// Sorted set decrement response
    /// </summary>
    public class SortedSetDecrementResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the new score value
        /// </summary>
        public double NewScore { get; set; }
    }
}
