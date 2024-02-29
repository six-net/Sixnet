using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Sms options
    /// </summary>
    public class SmsOptions
    {
        /// <summary>
        /// Get sms account func
        /// </summary>
        public Func<SmsExecutionOptions, SmsAccount> GetSmsAccountFunc { get; set; }

        /// <summary>
        /// Send sms callbacl
        /// </summary>
        public Action<IEnumerable<SendSmsResult>> SendCallback { get; set; }
    }
}
