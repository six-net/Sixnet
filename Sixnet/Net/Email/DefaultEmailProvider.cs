using Sixnet.Exceptions;
using Sixnet.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Default email provider
    /// </summary>
    [Serializable]
    internal class DefaultEmailProvider : ISixnetEmailProvider
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Email infos</param>
        /// <returns></returns>
        public async Task<List<SendEmailResult>> SendAsync(EmailAccount account, IEnumerable<EmailInfo> emails)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));

            if (emails.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            var sendTasks = new List<Task<SendEmailResult>>();
            foreach (var email in emails)
            {
                sendTasks.Add(ExecuteSendAsync(account, email));
            }
            return new List<SendEmailResult>(await Task.WhenAll(sendTasks).ConfigureAwait(false));
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public List<SendEmailResult> Send(EmailAccount account, IEnumerable<EmailInfo> emails)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));

            if (emails.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            var results = new List<SendEmailResult>();
            foreach (var email in emails)
            {
                if (email != null)
                {
                    results.Add(ExecuteSend(account, email));
                }
            }
            return results;
        }

        /// <summary>
        /// Execute send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="email">Email info</param>
        /// <returns></returns>
        async Task<SendEmailResult> ExecuteSendAsync(EmailAccount account, EmailInfo email)
        {
            SendEmailResult result;
            try
            {
                var mailMessage = GetMailMessage(account, email);
                if (mailMessage == null)
                {
                    result = SendEmailResult.FailResult("Convert to mail message failed", null, email);
                }
                else
                {
                    var smtpClient = GetSmtpClient(account);
                    await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                    result = SendEmailResult.SuccessResult(email);
                }
            }
            catch (Exception ex)
            {
                SixnetLogger.LogError<DefaultEmailProvider>(SixnetLogEvents.Framework.Email, ex, ex.Message);
                result = SendEmailResult.FailResult(ex.Message, ex, email);
            }
            if (result != null)
            {
                result.EmailAccount = account;
                result.EmailInfo = email;
            }
            return result;
        }

        /// <summary>
        /// Execute send email
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="account">Email account</param>
        /// <returns></returns>
        SendEmailResult ExecuteSend(EmailAccount account, EmailInfo email)
        {
            SendEmailResult result;
            try
            {
                var mailMessage = GetMailMessage(account, email);
                if (mailMessage == null)
                {
                    return SendEmailResult.FailResult("Convert to mail message failed", null, email);
                }
                else
                {
                    var smtpClient = GetSmtpClient(account);
                    smtpClient.Send(mailMessage);
                    result = SendEmailResult.SuccessResult(email);
                }
            }
            catch (Exception ex)
            {
                SixnetLogger.LogError<DefaultEmailProvider>(SixnetLogEvents.Framework.Email, ex, ex.Message);
                result = SendEmailResult.FailResult(ex.Message, ex, email);
            }
            if (result != null)
            {
                result.EmailAccount = account;
                result.EmailInfo = email;
            }
            return result;
        }

        /// <summary>
        /// Get mail message
        /// </summary>
        /// <param name="account"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        MailMessage GetMailMessage(EmailAccount account, EmailInfo email)
        {
            //clear out irregular email addresses
            var toEmailAddress = email.Emails.Where(email => email.IsEmail());
            if (toEmailAddress.IsNullOrEmpty())
            {
                return null;
            }
            //sender
            var fromMailAddress = new MailAddress(account.SendEmailAddress, account.SendPersonName);
            var mailMessage = new MailMessage
            {
                Sender = fromMailAddress,
                From = fromMailAddress
            };
            foreach (string emailAddress in toEmailAddress)
            {
                mailMessage.To.Add(new MailAddress(emailAddress));
            }
            mailMessage.Subject = email.Title;
            mailMessage.SubjectEncoding = email.SubjectEncoding;
            mailMessage.Body = email.Content;
            mailMessage.BodyEncoding = email.BodyEncoding;
            mailMessage.IsBodyHtml = email.BodyIsHtml;
            return mailMessage;
        }

        /// <summary>
        /// Get smtp client
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        SmtpClient GetSmtpClient(EmailAccount account)
        {
            return new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = account.EnableSsl,
                Host = account.SmtpAddress,
                Port = int.Parse(account.Port),
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(account.UserName, account.Password),
            };
        }
    }
}
