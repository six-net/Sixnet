using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Email options
    /// </summary>
    public class EmailOptions
    {
        /// <summary>
        /// Whether use same account
        /// </summary>
        public bool UseSameAccount { get; set; } = true;

        /// <summary>
        /// Get email account func
        /// </summary>
        public Func<EmailInfo, EmailAccount> GetEmailAccountFunc{ get; set; }

        /// <summary>
        /// Send callback
        /// </summary>
        public Action<IEnumerable<SendEmailResult>> SendCallback { get; set; }
    }
}
