using Sixnet.Exceptions;
using Sixnet.MQ;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Net.Email
{
    public static partial class SixnetEmailer
    {
        #region Send

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        public static Task<List<SendEmailResult>> SendAsync(IEnumerable<EmailInfo> emailInfos)
        {
            if (emailInfos.IsNullOrEmpty())
            {
                return Task.FromResult(new List<SendEmailResult>(0));
            }
            var syncInfos = new List<EmailInfo>();
            var asyncInfos = new List<EmailInfo>();
            foreach (var emailInfo in emailInfos)
            {
                SetAdditional(emailInfo);
                if (emailInfo == null)
                {
                    continue;
                }
                if (emailInfo.Asynchronously)
                {
                    asyncInfos.Add(emailInfo);
                }
                else
                {
                    syncInfos.Add(emailInfo);
                }
            }
            if (!asyncInfos.IsNullOrEmpty())
            {
                var emailMessage = new InProcessQueueEmailMessage()
                {
                    EmailInfos = asyncInfos
                };
                _ = SixnetMQ.SendAsync(emailMessage);
            }
            if (syncInfos.IsNullOrEmpty())
            {
                return Task.FromResult(new List<SendEmailResult>(0));
            }
            return ExecuteSendAsync(syncInfos);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailInfo">Send email options</param>
        /// <returns>Return send result</returns>
        public static async Task<SendEmailResult> SendAsync(EmailInfo emailInfo)
        {
            return (await SendAsync(new EmailInfo[1] { emailInfo }).ConfigureAwait(false))?.FirstOrDefault();
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static Task<SendEmailResult> SendAsync(string categoryName, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(subject), nameof(subject));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(receiveAddresses.IsNullOrEmpty(), nameof(receiveAddresses));

            return SendAsync(new EmailInfo()
            {
                Asynchronously = asynchronously,
                Category = categoryName,
                Subject = subject,
                Content = content,
                Emails = receiveAddresses,
            });
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static Task<SendEmailResult> SendAsync(string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            return SendAsync(string.Empty, subject, content, asynchronously, receiveAddresses);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static Task<SendEmailResult> SendAsync(string subject, string content, params string[] receiveAddresses)
        {
            return SendAsync(string.Empty, subject, content, true, receiveAddresses);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emailInfos">Email send options</param>
        /// <returns>Return the email send result</returns>
        public static Task<List<SendEmailResult>> SendAsync(EmailAccount account, params EmailInfo[] emailInfos)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));
            if (emailInfos.IsNullOrEmpty())
            {
                return Task.FromResult(new List<SendEmailResult>(0));
            }
            var syncInfos = new List<EmailInfo>();
            var asyncInfos = new List<EmailInfo>();
            foreach (var emailInfo in emailInfos)
            {
                SetAdditional(emailInfo);
                if (emailInfo == null)
                {
                    continue;
                }
                if (emailInfo.Asynchronously)
                {
                    asyncInfos.Add(emailInfo);
                }
                else
                {
                    syncInfos.Add(emailInfo);
                }
            }
            if (!asyncInfos.IsNullOrEmpty())
            {
                var emailMessage = new InProcessQueueEmailMessage()
                {
                    EmailInfos = asyncInfos,
                    EmailAccount = account
                };
                _ = SixnetMQ.SendAsync(emailMessage);
            }
            if (syncInfos.IsNullOrEmpty())
            {
                return Task.FromResult(new List<SendEmailResult>(0));
            }
            return ExecuteSendAsync(account, syncInfos);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="categoryName">Category name</param>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static async Task<SendEmailResult> SendAsync(EmailAccount account, string categoryName, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(subject), nameof(subject));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(receiveAddresses.IsNullOrEmpty(), nameof(receiveAddresses));

            var results = await SendAsync(account, new EmailInfo()
            {
                Asynchronously = asynchronously,
                Category = categoryName,
                Subject = subject,
                Content = content,
                Emails = receiveAddresses,
            }).ConfigureAwait(false);
            return results?.FirstOrDefault();
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static Task<SendEmailResult> SendAsync(EmailAccount account, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            return SendAsync(account, string.Empty, subject, content, asynchronously, receiveAddresses);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static Task<SendEmailResult> SendAsync(EmailAccount account, string subject, string content, params string[] receiveAddresses)
        {
            return SendAsync(account, string.Empty, subject, content, true, receiveAddresses);
        }

        #endregion

        #region Util

        /// <summary>
        /// Execute send emails
        /// </summary>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        internal static async Task<List<SendEmailResult>> ExecuteSendAsync(IEnumerable<EmailInfo> emailInfos)
        {
            if (emailInfos.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }

            var emailProvider = GetEmailProvider();
            var emailInfoGroups = new Dictionary<EmailAccount, List<EmailInfo>>();

            #region Gets email account

            foreach (var emailInfo in emailInfos)
            {
                var account = GetAccount(emailInfo);
                if (account == null)
                {
                    continue;
                }
                if (_useSameEmailAccount)
                {
                    emailInfoGroups[account] = emailInfos.ToList();
                    break;
                }
                if (emailInfoGroups.ContainsKey(account))
                {
                    emailInfoGroups[account].Add(emailInfo);
                }
                else
                {
                    emailInfoGroups.Add(account, new List<EmailInfo>() { emailInfo });
                }
            }

            #endregion

            #region Execute send

            IEnumerable<SendEmailResult> sendResults = null;

            //Single email account
            if (emailInfoGroups.Count == 1)
            {
                var firstGroup = emailInfoGroups.First();
                var account = firstGroup.Key;
                sendResults = await emailProvider.SendAsync(account, firstGroup.Value);
            }
            else
            {
                //Multiple email account
                var emailTasks = new Task<SendEmailResult[]>[emailInfoGroups.Count];
                var groupIndex = 0;
                foreach (var emailGroup in emailInfoGroups)
                {
                    var account = emailGroup.Key;
                    emailTasks[groupIndex] = emailProvider.SendAsync(account, emailGroup.Value);
                    groupIndex++;
                }
                sendResults = (await Task.WhenAll(emailTasks).ConfigureAwait(false)).SelectMany(c => c);
            }

            #endregion

            #region Callback

            ThreadPool.QueueUserWorkItem(s =>
            {
                _sendEmailCallback?.Invoke(sendResults?.Select(c => c.Clone()).ToList() ?? new List<SendEmailResult>(0));
            });

            #endregion

            return sendResults?.ToList() ?? new List<SendEmailResult>(0);
        }

        /// <summary>
        /// Execute send emails
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        internal static async Task<List<SendEmailResult>> ExecuteSendAsync(EmailAccount emailAccount, IEnumerable<EmailInfo> emailInfos)
        {
            var emailProvider = GetEmailProvider();
            var results = await emailProvider.SendAsync(emailAccount, emailInfos).ConfigureAwait(false);

            #region Callback

            ThreadPool.QueueUserWorkItem(s =>
            {
                _sendEmailCallback?.Invoke(results?.Select(c => c.Clone()).ToList() ?? new List<SendEmailResult>(0));
            });

            #endregion

            return results?.ToList() ?? new List<SendEmailResult>(0);
        }

        #endregion
    }
}
