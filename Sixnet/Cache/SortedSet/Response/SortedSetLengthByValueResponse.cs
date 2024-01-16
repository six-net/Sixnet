namespace Sixnet.Cache.SortedSet.Response
{
    /// <summary>
    /// Sorted set length by value response 
    /// </summary>
    public class SortedSetLengthByValueResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public long Length { get; set; }
    }
}
