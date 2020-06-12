namespace EZNEW.Cache.SortedSet.Response
{
    /// <summary>
    /// Sorted set length response
    /// </summary>
    public class SortedSetLengthResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public long Length
        {
            get; set;
        }
    }
}
