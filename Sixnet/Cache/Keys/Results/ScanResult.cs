using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Cache.Keys.Results
{
    /// <summary>
    /// Scan key result
    /// </summary>
    public class ScanResult : CacheResult
    {
        /// <summary>
        /// Gets or sets the cursor
        /// </summary>
        public long Cursor { get; set; }

        /// <summary>
        /// Gets or sets the keys
        /// </summary>
        public List<CacheKey> Keys { get; set; }
    }
}
