﻿namespace Sixnet.Cache.String.Response
{
    /// <summary>
    /// String decrement response
    /// </summary>
    public class StringDecrementResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the new value
        /// </summary>
        public decimal NewValue { get; set; }
    }
}
