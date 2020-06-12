using System;

namespace EZNEW.Cache
{
    /// <summary>
    /// Cache entry expiration
    /// </summary>
    public class CacheExpiration
    {
        /// <summary>
        /// Gets or sets an absolute expiration date for the cache entry.
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }

        /// <summary>
        /// Gets or sets an absolute expiration time, relative to now.
        /// </summary>
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

        /// <summary>
        /// Gets or sets whether open sliding expiration
        /// </summary>
        public bool SlidingExpiration { get; set; } = true;
    }
}
