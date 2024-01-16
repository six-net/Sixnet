﻿using System.Collections.Generic;

namespace Sixnet.Cache.Hash.Response
{
    /// <summary>
    /// Hash values response
    /// </summary>
    public class HashValuesResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public List<dynamic> Values { get; set; }
    }
}
