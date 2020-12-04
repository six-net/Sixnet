using System;

namespace EZNEW.Cache.Keys
{
    /// <summary>
    /// Time to live response
    /// </summary>
    public class TimeToLiveResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the to live time
        /// </summary>
        public long TimeToLiveSeconds { get; set; }

        /// <summary>
        /// Gets or sets whether key exist
        /// </summary>
        public bool KeyExist { get; set; }

        /// <summary>
        /// Gets or sets the key live to for ever
        /// </summary>
        public bool Perpetual { get; set; }
    }
}
