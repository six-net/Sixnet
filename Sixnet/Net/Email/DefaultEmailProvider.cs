using Sixnet.Exceptions;
using Sixnet.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Net email provider
    /// </summary>
    [Serializable]
    internal class DefaultEmailProvider : ISixnetEmailProvider
    {
        /// <summary>
        /// Send
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        public Task<SendEmailResult[]> SendAsync(EmailAccount account, params EmailInfo[] emailInfos)
        {
            IEnumerable<EmailInfo> emailInfoCollection = emailInfos;
            return SendAsync(account, emailInfoCollection);
        }

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        public Task<SendEmailResult[]> SendAsync(EmailAccount account, IEnumerable<EmailInfo> emailInfos)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));

            if (emailInfos.IsNullOrEmpty())
            {
                return Task.FromResult(new SendEmailResult[0]);
            }
            var sendTasks = new List<Task<SendEmailResult>>();
            foreach (var emailInfo in emailInfos)
            {
                sendTasks.Add(ExecuteSendAsync(emailInfo, account));
            }
            return Task.WhenAll(sendTasks);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        public SendEmailResult[] Send(EmailAccount account, params EmailInfo[] emailInfos)
        {
            IEnumerable<EmailInfo> emailInfoCollection = emailInfos;
            return Send(account, emailInfoCollection);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        public SendEmailResult[] Send(EmailAccount account, IEnumerable<EmailInfo> emailInfos)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));

            if (emailInfos.IsNullOrEmpty())
            {
                return new SendEmailResult[0];
            }
            var results = new List<SendEmailResult>();
            foreach (var sendInfo in emailInfos)
            {
                if (sendInfo != null)
                {
                    results.Add(ExecuteSend(sendInfo, account));
                }
            }
            return results.ToArray();
        }

        /// <summary>
        /// Execute send email
        /// </summary>
        /// <param name="emailInfo">Email info</param>
        /// <param name="account">Email account</param>
        /// <returns>Return email send result</returns>
        async Task<SendEmailResult> ExecuteSendAsync(EmailInfo emailInfo, EmailAccount account)
        {
            SendEmailResult result = null;
            try
            {
                var mailMessage = GetMailMessage(emailInfo, account);
                if (mailMessage == null)
                {
                    result = SendEmailResult.FailResult("Convert to mail message failed", null, emailInfo);
                }
                else
                {
                    var smtpClient = GetSmtpClient(account);
                    await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                    result = SendEmailResult.SuccessResult(emailInfo);
                }
            }
            catch (Exception ex)
            {
                SixnetLogger.LogError<DefaultEmailProvider>(SixnetLogEvents.Framework.Email, ex, ex.Message);
                result = SendEmailResult.FailResult(ex.Message, ex, emailInfo);
            }
            if (result != null)
            {
                result.EmailAccount = account;
                result.EmailInfo = emailInfo;
            }
            return result;
        }

        /// <summary>
        /// Execute send email
        /// </summary>
        /// <param name="emailInfo">Email info</param>
        /// <param name="account">Email account</param>
        /// <returns>Return email send result</returns>
        SendEmailResult ExecuteSend(EmailInfo emailInfo, EmailAccount account)
        {
            SendEmailResult result = null;
            try
            {
                var mailMessage = GetMailMessage(emailInfo, account);
                if (mailMessage == null)
                {
                    return SendEmailResult.FailResult("Convert to mail message failed", null, emailInfo);
                }
                else
                {
                    var smtpClient = GetSmtpClient(account);
                    smtpClient.Send(mailMessage);
                    result = SendEmailResult.SuccessResult(emailInfo);
                }
            }
            catch (Exception ex)
            {
                SixnetLogger.LogError<DefaultEmailProvider>(SixnetLogEvents.Framework.Email, ex, ex.Message);
                result = SendEmailResult.FailResult(ex.Message, ex, emailInfo);
            }
            if (result != null)
            {
                result.EmailAccount = account;
                result.EmailInfo = emailInfo;
            }
            return result;
        }

        /// <summary>
        /// Get mail message
        /// </summary>
        /// <param name="emailOptions"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        MailMessage GetMailMessage(EmailInfo emailOptions, EmailAccount account)
        {
            var toEmailAddress = emailOptions.Emails.Where(email => email.IsEmail());//clear out irregular email addresses
            if (toEmailAddress.IsNullOrEmpty())
            {
                return null;
            }
            var fromMailAddress = new MailAddress(account.SendEmailAddress, account.SendPersonName);//sender
            var mailMessage = new MailMessage
            {
                Sender = fromMailAddress,
                From = fromMailAddress
            };
            foreach (string email in toEmailAddress)
            {
                mailMessage.To.Add(new MailAddress(email));
            }
            mailMessage.Subject = emailOptions.Subject;
            mailMessage.SubjectEncoding = emailOptions.SubjectEncoding;
            mailMessage.Body = emailOptions.Content;
            mailMessage.BodyEncoding = emailOptions.BodyEncoding;
            mailMessage.IsBodyHtml = emailOptions.BodyIsHtml;
            return mailMessage;
        }

        /// <summary>
        /// Get smtp client
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        SmtpClient GetSmtpClient(EmailAccount account)
        {
            var smtpClient = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = account.EnableSsl,
                Host = account.SmtpAddress,
                Port = int.Parse(account.Port),
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(account.UserName, account.Password),
            };
            return smtpClient;
        }
    }
}
