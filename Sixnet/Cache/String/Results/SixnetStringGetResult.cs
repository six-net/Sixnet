// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String get response
    /// </summary>
    public class SixnetStringGetResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<SixnetCacheEntry> Values { get; set; }
    }
}
