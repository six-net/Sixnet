using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Email provider contract
    /// </summary>
    public interface IEmailProvider
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="sendInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        Task<List<SendEmailResult>> SendAsync(EmailAccount account, params SendEmailOptions[] sendInfos);

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="sendInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        List<SendEmailResult> Send(EmailAccount account, params SendEmailOptions[] sendInfos);
    }
}
