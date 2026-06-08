// "Company © 2025. All rights reserved."

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Sixnet email options
    /// </summary>
    public class SixnetEmailOptions
    {
        /// <summary>
        /// Whether use same account
        /// </summary>
        public bool UseSameAccount { get; set; } = true;

        /// <summary>
        /// Gets or set the email account
        /// </summary>
        public SixnetEmailAccount Account { get; set; }

        /// <summary>
        /// Get email account
        /// </summary>
        public Func<SixnetEmailInfo, SixnetEmailAccount> GetEmailAccount { get; set; }

        /// <summary>
        /// Send callback
        /// </summary>
        public Action<IEnumerable<SixnetSendEmailResult>> SendCallback { get; set; }
    }
}
