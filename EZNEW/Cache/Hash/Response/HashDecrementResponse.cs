namespace EZNEW.Cache.Hash
{
    /// <summary>
    /// Hash decrement response
    /// </summary>
    public class HashDecrementResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the hash field
        /// </summary>
        public string HashField { get; set; }

        /// <summary>
        /// Gets or sets the new value
        /// </summary>
        public dynamic NewValue { get; set; }
    }
}
