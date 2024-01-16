namespace Sixnet.Cache
{
    /// <summary>
    /// Cache entry
    /// </summary>
    public class CacheEntry
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public CacheKey Key { get; set; }

        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        public CacheKeyType Type { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the expiration
        /// </summary>
        public CacheExpiration Expiration { get; set; }

        /// <summary>
        /// Gets or sets set value condition
        /// </summary>
        public CacheSetWhen When { get; set; } = CacheSetWhen.Always;
    }
}
