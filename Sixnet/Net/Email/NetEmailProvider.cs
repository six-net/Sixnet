using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Sixnet.Logging;

namespace Sixnet.Net.Email
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
        /// <param name="emailOptionses">Email optionss</param>
        /// <returns>Return the email send results</returns>
        public async Task<List<SendEmailResult>> SendAsync(EmailAccount account, params SendEmailOptions[] emailOptionses)
        {
            if (account == null || emailOptionses.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            var sendTasks = new Task<SendEmailResult>[emailOptionses.Length];
            var infoIndex = 0;
            foreach (var sendInfo in emailOptionses)
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
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emailOptionses">Email options</param>
        /// <returns>Return the email send results</returns>
        public List<SendEmailResult> Send(EmailAccount account, params SendEmailOptions[] emailOptionses)
        {
            if (account == null || emailOptionses.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            var results = new List<SendEmailResult>(emailOptionses.Length);
            foreach (var sendInfo in emailOptionses)
            {
                if (sendInfo == null)
                {
                    continue;
                }
                results.Add(ExecuteSend(sendInfo, account));
            }
            return results;
        }

        /// <summary>
        /// Execute send email
        /// </summary>
        /// <param name="sendInfo">Email send info</param>
        /// <param name="account">Email account</param>
        /// <returns>Return email send result</returns>
        async Task<SendEmailResult> ExecuteSendAsync(SendEmailOptions emailOptions, EmailAccount account)
        {
            SendEmailResult result = null;
            try
            {
                var mailMessage = GetMailMessage(emailOptions, account);
                if (mailMessage == null)
                {
                    return SendEmailResult.Empty();
                }
                var smtpClient = GetSmtpClient(account);
                await smtpClient.SendMailAsync(mailMessage).ConfigureAwait(false);
                result = SendEmailResult.SuccessResult(emailOptions);
            }
            catch (Exception ex)
            {
                LogManager.LogError<NetEmailProvider>(FrameworkLogEvents.Framework.Email, ex, ex.Message);
                result = SendEmailResult.FailResult(ex.Message, ex, emailOptions);
            }
            if (result != null)
            {
                result.EmailAccount = account;
                result.SendInfo = emailOptions;
            }
            return result;
        }

        /// <summary>
        /// Execute send email
        /// </summary>
        /// <param name="sendInfo">Email send info</param>
        /// <param name="account">Email account</param>
        /// <returns>Return email send result</returns>
        SendEmailResult ExecuteSend(SendEmailOptions emailOptions, EmailAccount account)
        {
            SendEmailResult result = null;
            try
            {
                var mailMessage = GetMailMessage(emailOptions, account);
                if (mailMessage == null)
                {
                    return SendEmailResult.Empty();
                }
                var smtpClient = GetSmtpClient(account);
                smtpClient.Send(mailMessage);
                result = SendEmailResult.SuccessResult(emailOptions);
            }
            catch (Exception ex)
            {
                LogManager.LogError<NetEmailProvider>(FrameworkLogEvents.Framework.Email, ex, ex.Message);
                result = SendEmailResult.FailResult(ex.Message, ex, emailOptions);
            }
            if (result != null)
            {
                result.EmailAccount = account;
                result.SendInfo = emailOptions;
            }
            return result;
        }

        MailMessage GetMailMessage(SendEmailOptions emailOptions, EmailAccount account)
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
