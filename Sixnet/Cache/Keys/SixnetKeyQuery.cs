// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.Keys
{
    /// <summary>
    /// Cache key query
    /// </summary>
    public class SixnetKeyQuery : SixnetCacheQuery
    {
        /// <summary>
        /// Gets or sets the mate key
        /// </summary>
        public string MateKey { get; set; }

        /// <summary>
        /// Gets or sets the key pattern type
        /// </summary>
        public SixnetKeyMatchPattern Type { get; set; }
    }
}
