// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Keys.Results
{
    /// <summary>
    /// Get keys result
    /// </summary>
    public class SixnetGetKeysResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the cache keys
        /// </summary>
        public SixnetCachePaging<SixnetCacheKey> Keys { get; set; }
    }
}
