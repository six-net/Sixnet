using System.Collections.Generic;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Get sms report result
    /// </summary>
    public class GetSmsReportResult : SmsResult
    {
        /// <summary>
        /// Gets or sets the reports
        /// </summary>
        public List<SmsReport> Reports { get; set; }

        /// <summary>
        /// Gets or sets the total count
        /// </summary>
        public int TotalCount { get; set; }
    }
}
