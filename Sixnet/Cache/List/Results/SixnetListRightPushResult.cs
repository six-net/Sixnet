// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.List.Results
{
    /// <summary>
    /// List right push result
    /// </summary>
    public class SixnetListRightPushResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the length of the list after the push operation.
        /// </summary>
        public long NewListLength { get; set; }
    }
}
