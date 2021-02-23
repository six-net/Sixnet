using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
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
