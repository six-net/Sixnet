using System;

namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String get with expiry response
    /// </summary>
    public class StringGetWithExpiryResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the expiry time
        /// </summary>
        public TimeSpan? Expiry { get; set; }
    }
}
