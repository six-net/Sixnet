// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String append result
    /// </summary>
    public class SixnetStringAppendResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the length of the string after the append operation.
        /// </summary>
        public long NewValueLength { get; set; }
    }
}
