using System.Collections.Generic;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Receive sms result
    /// </summary>
    public class ReceiveSmsResult : SmsResult
    {
        /// <summary>
        /// Gets or sets the received sms
        /// </summary>
        public List<ReceiveSmsInfo> SmsInfos { get; set; }
    }
}
