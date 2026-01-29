// "Company © 2025. All rights reserved."

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache entry
    /// </summary>
    public class SixnetCacheEntry
    {
        /// <summary>
        /// Gets or sets the cache key
        /// </summary>
        public SixnetCacheKey Key { get; set; }

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
        public SixnetCacheExpiration Expiration { get; set; }

        /// <summary>
        /// Gets or sets set value condition
        /// </summary>
        public CacheSetWhen When { get; set; } = CacheSetWhen.Always;
    }
}
