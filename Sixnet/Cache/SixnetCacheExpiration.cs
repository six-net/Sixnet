// "Company © 2025. All rights reserved."

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache entry expiration
    /// </summary>
    public class SixnetCacheExpiration
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
        public bool SlidingExpiration { get; set; } = false;

        /// <summary>
        /// Gets a absolute time expiration
        /// </summary>
        /// <param name="absoluteExpirationTime">Absolute expiration time</param>
        /// <returns></returns>
        public static SixnetCacheExpiration GetAbsoluteExpiration(DateTimeOffset absoluteExpirationTime)
        {
            return new SixnetCacheExpiration()
            {
                AbsoluteExpiration = absoluteExpirationTime,
                SlidingExpiration = false
            };
        }

        /// <summary>
        /// Gets a relative expiration
        /// </summary>
        /// <param name="timeSpan">Relative expiration time</param>
        /// <param name="sliding">Whether allow sliding expiration</param>
        /// <returns></returns>
        public static SixnetCacheExpiration GetRelativeToNowExpiration(TimeSpan timeSpan, bool sliding = true)
        {
            return new SixnetCacheExpiration()
            {
                SlidingExpiration = sliding,
                AbsoluteExpirationRelativeToNow = timeSpan
            };
        }

        /// <summary>
        /// Gets a relative expiration
        /// </summary>
        /// <param name="seconds">Expiration seconds</param>
        /// <param name="sliding">Whether allow sliding expiration</param>
        /// <returns></returns>
        public static SixnetCacheExpiration GetRelativeToNowExpiration(int seconds, bool sliding = true)
        {
            return GetRelativeToNowExpiration(TimeSpan.FromSeconds(seconds), sliding);
        }
    }
}
