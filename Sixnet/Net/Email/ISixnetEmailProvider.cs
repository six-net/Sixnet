using System.Collections.Generic;
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
        /// <param name="emailInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        Task<SendEmailResult[]> SendAsync(EmailAccount account, params EmailInfo[] emailInfos);

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emailInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        Task<SendEmailResult[]> SendAsync(EmailAccount account, IEnumerable<EmailInfo> emailInfos);

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        SendEmailResult[] Send(EmailAccount account, params EmailInfo[] emailInfos);

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        SendEmailResult[] Send(EmailAccount account, IEnumerable<EmailInfo> emailInfos);
    }
}
