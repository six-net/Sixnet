using System;

namespace EZNEW.Cache.Keys.Response
{
    /// <summary>
    /// Time to live response
    /// </summary>
    public class TimeToLiveResponse : CacheResponse
    {
        /// <summary>
        /// Gets or sets the to live time
        /// </summary>
        public TimeSpan? TimeToLive
        {
            get; set;
        }
    }
}
