using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EZNEW.DependencyInjection;
using EZNEW.Fault;
using EZNEW.Internal.MessageQueue;

namespace EZNEW.Email
{
    /// <summary>
    /// Email manager
    /// </summary>
    public static class EmailManager
    {
        /// <summary>
        /// Email engine
        /// </summary>
        static readonly IEmailEngine EmailEngine = null;

        static EmailManager()
        {
            EmailEngine = ContainerManager.Resolve<IEmailEngine>();
            if (EmailEngine == null)
            {
                EmailEngine = new NetEmailEngine();
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the method to gets email account
        /// </summary>
        static Func<EmailSendInfo, EmailAccount> GetEmailAccount;

        /// <summary>
        /// Determines whether use the same email account
        /// </summary>
        static bool UseSameEmailAccount = false;

        /// <summary>
        /// Email sent callback
        /// </summary>
        static Action<IEnumerable<EmailSendResult>> EmailSentCallback;

        #endregion

        #region Send

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="sendInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        public static async Task<List<EmailSendResult>> SendAsync(params EmailSendInfo[] sendInfos)
        {
            if (sendInfos.IsNullOrEmpty())
            {
                return new List<EmailSendResult>(0);
            }
            List<EmailSendInfo> SyncInfos = new List<EmailSendInfo>();
            List<EmailSendInfo> AsyncInfos = new List<EmailSendInfo>();
            foreach (var sendInfo in sendInfos)
            {
                if (sendInfo == null)
                {
                    continue;
                }
                if (sendInfo.Asynchronously)
                {
                    AsyncInfos.Add(sendInfo);
                }
                else
                {
                    SyncInfos.Add(sendInfo);
                }
            }
            if (!AsyncInfos.IsNullOrEmpty())
            {
                SendEmailInternalMessageCommand emailInternalMessageCommand = new SendEmailInternalMessageCommand()
                {
                    SendInfos = AsyncInfos
                };
                InternalMessageQueue.Enqueue(emailInternalMessageCommand);
            }
            if (SyncInfos.IsNullOrEmpty())
            {
                return new List<EmailSendResult>(1) { EmailSendResult.SuccessResult(null) };
            }
            return await ExecuteSendAsync(SyncInfos).ConfigureAwait(false);
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
        public static async Task<EmailSendResult> SendAsync(string categoryName, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(content) || receiveAddresses.IsNullOrEmpty())
            {
                return EmailSendResult.Empty;
            }
            var results = await SendAsync(new EmailSendInfo()
            {
                Asynchronously = asynchronously,
                Category = categoryName,
                Subject = subject,
                Content = content,
                EmailAddress = receiveAddresses,
            }).ConfigureAwait(false);
            return results?.FirstOrDefault() ?? EmailSendResult.Empty;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static async Task<EmailSendResult> SendAsync(string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            return await SendAsync(string.Empty, subject, content, asynchronously, receiveAddresses).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static async Task<EmailSendResult> SendAsync(string subject, string content, params string[] receiveAddresses)
        {
            return await SendAsync(string.Empty, subject, content, true, receiveAddresses).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute send emails
        /// </summary>
        /// <param name="sendInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        internal static async Task<List<EmailSendResult>> ExecuteSendAsync(IEnumerable<EmailSendInfo> sendInfos)
        {
            if (sendInfos.IsNullOrEmpty())
            {
                return new List<EmailSendResult>(0);
            }
            if (EmailEngine == null)
            {
                throw new EZNEWException("No mail delivery execution engine is configured");
            }

            Dictionary<string, List<EmailSendInfo>> emailInfoGroups = new Dictionary<string, List<EmailSendInfo>>();
            Dictionary<string, EmailAccount> accounts = new Dictionary<string, EmailAccount>();

            #region Gets email account

            foreach (var sendInfo in sendInfos)
            {
                var account = GetAccount(sendInfo);
                if (account == null)
                {
                    continue;
                }
                string accountKey = account.IdentityKey;
                if (UseSameEmailAccount)
                {
                    emailInfoGroups[accountKey] = sendInfos.ToList();
                    accounts[accountKey] = account;
                    break;
                }
                if (accounts.ContainsKey(accountKey))
                {
                    emailInfoGroups[accountKey].Add(sendInfo);
                }
                else
                {
                    emailInfoGroups.Add(accountKey, new List<EmailSendInfo>() { sendInfo });
                    accounts.Add(accountKey, account);
                }
            }

            #endregion

            #region Execute send

            //Single email account
            if (emailInfoGroups.Count == 1)
            {
                var firstGroup = emailInfoGroups.First();
                var account = accounts[firstGroup.Key];
                return await EmailEngine.SendAsync(account, firstGroup.Value.ToArray());
            }

            //Multiple email account
            var emailTasks = new Task<List<EmailSendResult>>[emailInfoGroups.Count];
            var groupIndex = 0;
            foreach (var optionGroup in emailInfoGroups)
            {
                var account = accounts[optionGroup.Key];
                emailTasks[groupIndex] = EmailEngine.SendAsync(account, optionGroup.Value.ToArray());
            }
            var sendResults = (await Task.WhenAll(emailTasks).ConfigureAwait(false)).SelectMany(c => c);

            #endregion

            //callback
            ThreadPool.QueueUserWorkItem(s =>
            {
                EmailSentCallback?.Invoke(sendResults);
            });
            return sendResults.ToList();
        }

        /// <summary>
        /// Execute send emails
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="sendInfos">Email send infos</param>
        /// <returns>Return the email send results</returns>
        internal static async Task<List<EmailSendResult>> ExecuteSendAsync(EmailAccount emailAccount, IEnumerable<EmailSendInfo> sendInfos)
        {
            var results = await EmailEngine.SendAsync(emailAccount, sendInfos.ToArray()).ConfigureAwait(false);

            //callback
            ThreadPool.QueueUserWorkItem(s =>
            {
                EmailSentCallback?.Invoke(new List<EmailSendResult>(results));
            });
            return results;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="sendInfos">Email send infos</param>
        /// <returns>Return the email send result</returns>
        public static async Task<List<EmailSendResult>> SendAsync(EmailAccount emailAccount, params EmailSendInfo[] sendInfos)
        {
            if (emailAccount == null || sendInfos.IsNullOrEmpty())
            {
                return new List<EmailSendResult>(0);
            }
            List<EmailSendInfo> SyncInfos = new List<EmailSendInfo>();
            List<EmailSendInfo> AsyncInfos = new List<EmailSendInfo>();
            foreach (var sendInfo in sendInfos)
            {
                if (sendInfo == null)
                {
                    continue;
                }
                if (sendInfo.Asynchronously)
                {
                    AsyncInfos.Add(sendInfo);
                }
                else
                {
                    SyncInfos.Add(sendInfo);
                }
            }
            if (!AsyncInfos.IsNullOrEmpty())
            {
                SendEmailInternalMessageCommand emailInternalMessageCommand = new SendEmailInternalMessageCommand()
                {
                    SendInfos = AsyncInfos,
                    EmailAccount = emailAccount
                };
                InternalMessageQueue.Enqueue(emailInternalMessageCommand);
            }
            if (SyncInfos.IsNullOrEmpty())
            {
                return new List<EmailSendResult>(1) { EmailSendResult.SuccessResult(null) };
            }
            return await ExecuteSendAsync(emailAccount, SyncInfos).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="categoryName">Category name</param>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static async Task<EmailSendResult> SendAsync(EmailAccount emailAccount, string categoryName, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(content) || receiveAddresses.IsNullOrEmpty())
            {
                return EmailSendResult.Empty;
            }
            var results = await SendAsync(emailAccount, new EmailSendInfo()
            {
                Asynchronously = asynchronously,
                Category = categoryName,
                Subject = subject,
                Content = content,
                EmailAddress = receiveAddresses,
            }).ConfigureAwait(false);
            return results?.FirstOrDefault() ?? EmailSendResult.Empty;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static async Task<EmailSendResult> SendAsync(EmailAccount emailAccount, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            return await SendAsync(emailAccount, string.Empty, subject, content, asynchronously, receiveAddresses).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static async Task<EmailSendResult> SendAsync(EmailAccount emailAccount, string subject, string content, params string[] receiveAddresses)
        {
            return await SendAsync(emailAccount, string.Empty, subject, content, true, receiveAddresses).ConfigureAwait(false);
        }

        #endregion

        #region Email account

        #region Configure email account

        /// <summary>
        /// Configure email account
        /// </summary>
        /// <param name="getEmailAccountOperation">Get email account operation</param>
        /// <param name="useSameEmailAccount">
        /// Whether enable use the same email account.
        /// It will use the first email account for all email infos if set true.
        /// </param>
        public static void ConfigureEmailAccount(Func<EmailSendInfo, EmailAccount> getEmailAccountOperation, bool useSameEmailAccount = false)
        {
            GetEmailAccount = getEmailAccountOperation;
            UseSameEmailAccount = useSameEmailAccount;
        }

        #endregion

        #region Get email account

        /// <summary>
        /// Get email account
        /// </summary>
        /// <param name="sendInfo">Email send info</param>
        /// <returns>Return the email accounts</returns>
        static EmailAccount GetAccount(EmailSendInfo sendInfo)
        {
            if (sendInfo == null)
            {
                return null;
            }
            var emailAccount = GetEmailAccount?.Invoke(sendInfo);
            if (emailAccount == null)
            {
                throw new EZNEWException("No mail sending account was specified");
            }
            return emailAccount;
        }

        #endregion

        #endregion

        #region Callback

        /// <summary>
        /// Add email sent callback operation
        /// </summary>
        /// <param name="callback">Callback operation</param>
        public static void AddEmailSentCallback(Action<IEnumerable<EmailSendResult>> callback)
        {
            if (callback == null)
            {
                return;
            }
            if (EmailSentCallback == null)
            {
                EmailSentCallback = null;
            }
            else
            {
                EmailSentCallback += callback;
            }
        }

        #endregion
    }
}
