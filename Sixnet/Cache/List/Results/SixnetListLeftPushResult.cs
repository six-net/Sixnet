// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.List.Results
{
    /// <summary>
    /// List left push result
    /// </summary>
    public class SixnetListLeftPushResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the the length of the list after the push operations.
        /// </summary>
        public long NewListLength { get; set; }
    }
}
