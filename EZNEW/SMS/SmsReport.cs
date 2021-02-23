using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
{
    /// <summary>
    /// Sms report
    /// </summary>
    public class SmsReport
    {
        /// <summary>
        /// Gets or sets the message id
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Get or sets the mobile number
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the sms send status
        /// </summary>
        public SmsSendStatus Status { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the gateway code
        /// </summary>
        public string GatewayCode { get; set; }

        /// <summary>
        /// Gets or sets the report time
        /// </summary>
        public DateTimeOffset ReportTime { get; set; }
    }
}
