using System.Collections.Generic;

namespace Sixnet.Cache.Hash.Results
{
    /// <summary>
    /// Hash scan result
    /// </summary>
    public class HashScanResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the cursor
        /// </summary>
        public long Cursor { get; set; }

        /// <summary>
        /// Gets or sets the hash values
        /// </summary>
        public Dictionary<string, dynamic> HashValues { get; set; }
    }
}
