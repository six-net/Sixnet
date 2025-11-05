// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String get response
    /// </summary>
    public class StringGetResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<CacheEntry> Values { get; set; }
    }
}
