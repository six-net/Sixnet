using System;
using System.Collections.Generic;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Sixnet sms options
    /// </summary>
    public class SixnetSmsOptions
    {
        /// <summary>
        /// Whether use the same account.
        /// Default is true
        /// </summary>
        public bool UseSameAccount { get; set; } = true;

        /// <summary>
        /// Gets or sets the sms account
        /// </summary>
        public SmsAccount Account { get; set; }

        /// <summary>
        /// Gets or sets the get sms account
        /// </summary>
        public Func<SmsParameter, SmsAccount> GetSmsAccount { get; set; }

        /// <summary>
        /// Gets or sets the send sms callback
        /// </summary>
        public Action<IEnumerable<SendSmsResult>> SendCallback { get; set; }
    }
}
