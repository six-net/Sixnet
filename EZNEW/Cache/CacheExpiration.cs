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
        public bool SlidingExpiration { get; set; } = false;

        /// <summary>
        /// Gets a absolute time expiration
        /// </summary>
        /// <param name="absoluteExpirationTime">Absolute expiration time</param>
        /// <returns></returns>
        public static CacheExpiration GetAbsoluteExpiration(DateTimeOffset absoluteExpirationTime)
        {
            return new CacheExpiration()
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
        public static CacheExpiration GetRelativeToNowExpiration(TimeSpan timeSpan, bool sliding = true)
        {
            return new CacheExpiration()
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
        public static CacheExpiration GetRelativeToNowExpiration(int seconds, bool sliding = true)
        {
            return GetRelativeToNowExpiration(TimeSpan.FromSeconds(seconds), sliding);
        }
    }
}
