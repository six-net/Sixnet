namespace Sixnet.Cache.Hash.Results
{
    /// <summary>
    /// Hash decrement result
    /// </summary>
    public class HashDecrementResult : CacheResult
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
