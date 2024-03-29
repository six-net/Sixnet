﻿namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Query sms balance result
    /// </summary>
    public class QuerySmsBalanceResult : SmsResult
    {
        /// <summary>
        /// Gets or sets the amount
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the sms number
        /// </summary>
        public long Number { get; set; }

        /// <summary>
        /// Gets or sets the freeze amount
        /// </summary>
        public decimal FreezeAmount { get; set; }
    }
}
