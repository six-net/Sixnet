﻿using System.Collections.Generic;

namespace Sixnet.Cache.Hash.Response
{
    /// <summary>
    /// Hash scan response
    /// </summary>
    public class HashScanResponse : CacheResponse
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
