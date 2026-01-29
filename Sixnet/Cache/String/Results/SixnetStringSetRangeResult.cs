// "Company © 2025. All rights reserved."

namespace Sixnet.Cache.String.Results
{
    /// <summary>
    /// String set range response
    /// </summary>
    public class SixnetStringSetRangeResult : SixnetCacheResult
    {
        /// <summary>
        /// Gets or sets the value length after modified
        /// </summary>
        public long NewValueLength { get; set; }
    }
}
