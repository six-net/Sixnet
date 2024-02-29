using System;
using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Get sms report options
    /// </summary>
    public class GetSmsReportOptions : SmsExecutionOptions
    {
        /// <summary>
        /// Gets or sets the mobile number
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the send date
        /// </summary>
        public DateTimeOffset SendDate { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the page
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the message id
        /// </summary>
        public string MessageId { get; set; }

        public override SmsExecutionOptions Clone()
        {
            return new GetSmsReportOptions()
            {
                Tag = Tag,
                Parameters = Parameters?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
                Mobile = Mobile,
                SendDate = SendDate,
                MessageId = MessageId,
                Page = Page,
                PageSize = PageSize
            };
        }
    }
}
