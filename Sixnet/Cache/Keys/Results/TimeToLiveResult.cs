using System;

namespace Sixnet.Cache.Keys.Results
{
    /// <summary>
    /// Time to live result
    /// </summary>
    public class TimeToLiveResult : CacheResult
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
