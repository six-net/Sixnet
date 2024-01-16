using System.Collections.Generic;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Check black result
    /// </summary>
    public class CheckBlackResult : SmsResult
    {
        /// <summary>
        /// Gets or sets the blacklist
        /// </summary>
        public List<string> Blacklist { get; set; }
    }
}
