﻿using System.Collections.Generic;

namespace Sixnet.Cache.Keys.Response
{
    /// <summary>
    /// Sort response
    /// </summary>
    public class SortResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<string> Values { get; set; }
    }
}
