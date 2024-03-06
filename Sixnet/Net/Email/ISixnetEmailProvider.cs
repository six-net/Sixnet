using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Email provider contract
    /// </summary>
    public interface ISixnetEmailProvider
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        Task<List<SendEmailResult>> SendAsync(EmailAccount account, IEnumerable<EmailInfo> emails);

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns>Return the email send results</returns>
        List<SendEmailResult> Send(EmailAccount account, IEnumerable<EmailInfo> emails);
    }
}
