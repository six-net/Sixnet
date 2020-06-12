using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EZNEW.Email
{
    /// <summary>
    /// Net email engine
    /// </summary>
    [Serializable]
    internal class NetEmailEngine : IEmailEngine
    {
        /// <summary>
        /// Send
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="sendInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        public async Task<List<EmailSendResult>> SendAsync(EmailAccount account, params EmailSendInfo[] sendInfos)
        {
            if (account == null || sendInfos.IsNullOrEmpty())
            {
                return new List<EmailSendResult>(0);
            }
            var sendTasks = new Task<EmailSendResult>[sendInfos.Length];
            var infoIndex = 0;
            foreach (var sendInfo in sendInfos)
            {
                if (sendInfo == null)
                {
                    continue;
                }
                sendTasks[infoIndex] = ExecuteSendAsync(sendInfo, account);
            }
            return (await Task.WhenAll(sendTasks).ConfigureAwait(false)).ToList();
        }

        /// <summary>
        /// Execute send email
        /// </summary>
        /// <param name="sendInfo">Email send info</param>
        /// <param name="account">Email account</param>
        /// <returns>Return email send result</returns>
        async Task<EmailSendResult> ExecuteSendAsync(EmailSendInfo sendInfo, EmailAccount account)
        {
            try
            {
                var toEmailAddress = sendInfo.EmailAddress.Where(email => email.IsEmail());//clear out irregular email addresses
                if (toEmailAddress.IsNullOrEmpty())
                {
                    return EmailSendResult.Empty;
                }
                MailAddress fromMailAddress = new MailAddress(account.SendEmailAddress, account.SendPersonName);//sender
                MailMessage nowMessage = new MailMessage
                {
                    Sender = fromMailAddress,
                    From = fromMailAddress
                };
                foreach (string email in toEmailAddress)
                {
                    nowMessage.To.Add(new MailAddress(email));
                }
                nowMessage.Subject = sendInfo.Subject;
                nowMessage.SubjectEncoding = sendInfo.SubjectEncoding;
                nowMessage.Body = sendInfo.Content;
                nowMessage.BodyEncoding = sendInfo.BodyEncoding;
                nowMessage.IsBodyHtml = sendInfo.BodyIsHtml;
                SmtpClient smtpClient = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = account.EnableSsl,
                    Host = account.SmtpAddress,
                    Port = int.Parse(account.Port),
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(account.UserName, account.Password)
                };
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                await smtpClient.SendMailAsync(nowMessage).ConfigureAwait(false);
                return EmailSendResult.SuccessResult(sendInfo);
            }
            catch (Exception ex)
            {
                return EmailSendResult.FailResult(ex.Message, ex, sendInfo);
            }
        }
    }
}
