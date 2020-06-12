using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Email
{
    /// <summary>
    /// Email send engine
    /// </summary>
    public interface IEmailEngine
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="sendInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        Task<List<EmailSendResult>> SendAsync(EmailAccount account, params EmailSendInfo[] sendInfos);
    }
}
