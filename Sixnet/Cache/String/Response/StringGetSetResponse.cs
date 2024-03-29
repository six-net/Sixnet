﻿namespace Sixnet.Cache.String.Response
{
    /// <summary>
    /// String get set response
    /// </summary>
    public class StringGetSetResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the old value
        /// </summary>
        public string OldValue { get; set; }
    }
}
