namespace EZNEW.Cache.Hash
{
    /// <summary>
    /// Hash increment response
    /// </summary>
    public class HashIncrementResponse : CacheResponse
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
