using EZNEW.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EZNEW.Email
{
    /// <summary>
    /// Net email provider
    /// </summary>
    [Serializable]
    internal class NetEmailProvider : IEmailProvider
    {
        /// <summary>
        /// Send
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="sendInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        public async Task<List<SendEmailResult>> SendAsync(EmailAccount account, params SendEmailOptions[] sendInfos)
        {
            if (account == null || sendInfos.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            var sendTasks = new Task<SendEmailResult>[sendInfos.Length];
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
        async Task<SendEmailResult> ExecuteSendAsync(SendEmailOptions sendInfo, EmailAccount account)
        {
            SendEmailResult result = null;
            try
            {
                var toEmailAddress = sendInfo.Emails.Where(email => email.IsEmail());//clear out irregular email addresses
                if (toEmailAddress.IsNullOrEmpty())
                {
                    return SendEmailResult.Empty;
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
                result = SendEmailResult.SuccessResult(sendInfo);
            }
            catch (Exception ex)
            {
                LogManager.LogError<NetEmailProvider>(ex.Message, ex);
                result = SendEmailResult.FailResult(ex.Message, ex, sendInfo);
            }
            if (result != null)
            {
                result.EmailAccount = account;
                result.SendInfo = sendInfo;
            }
            return result;
        }
    }
}
