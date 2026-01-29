// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String value set response
    /// </summary>
    public class SixnetStringSetResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the set results
        /// </summary>
        public List<SixnetStringEntrySetResult> Results { get; set; }
    }
}
