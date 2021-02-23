using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
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
