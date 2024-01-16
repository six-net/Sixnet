﻿using System;

namespace Sixnet.Cache.String.Response
{
    /// <summary>
    /// String get with expiry response
    /// </summary>
    public class StringGetWithExpiryResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the expiry time
        /// </summary>
        public TimeSpan? Expiry { get; set; }
    }
}
