namespace EZNEW.Cache.SortedSet
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
