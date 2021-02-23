using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EZNEW.DependencyInjection;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Diagnostics;
using EZNEW.Fault;
using EZNEW.Queue;

namespace EZNEW.Email
{
    /// <summary>
    /// Email manager
    /// </summary>
    public static class EmailManager
    {
        /// <summary>
        /// Email provider
        /// </summary>
        static readonly IEmailProvider EmailProvider = null;

        static EmailManager()
        {
            EmailProvider = ContainerManager.Resolve<IEmailProvider>();
            if (EmailProvider == null)
            {
                EmailProvider = new NetEmailProvider();
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the method to gets email account
        /// </summary>
        static Func<SendEmailOptions, EmailAccount> GetEmailAccount;

        /// <summary>
        /// Determines whether use the same email account
        /// </summary>
        static bool UseSameEmailAccount = false;

        /// <summary>
        /// Email sent callback
        /// </summary>
        static Action<IEnumerable<SendEmailResult>> EmailSentCallback;

        #endregion

        #region Send

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="sendOptions">Send email options</param>
        /// <returns>Return the email send results</returns>
        public static async Task<List<SendEmailResult>> SendAsync(IEnumerable<SendEmailOptions> sendOptions)
        {
            if (sendOptions.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            List<SendEmailOptions> syncInfos = new List<SendEmailOptions>();
            List<SendEmailOptions> asyncInfos = new List<SendEmailOptions>();
            foreach (var sendOption in sendOptions)
            {
                //Set additional
                SetAdditional(sendOption);

                if (sendOption == null)
                {
                    continue;
                }
                if (sendOption.Asynchronously)
                {
                    asyncInfos.Add(sendOption);
                }
                else
                {
                    syncInfos.Add(sendOption);
                }
            }
            if (!asyncInfos.IsNullOrEmpty())
            {
                InternalQueueSendEmailItem emailInternalMessageCommand = new InternalQueueSendEmailItem()
                {
                    Datas = asyncInfos
                };
                InternalQueueManager.GetQueue(EZNEWConstants.InternalQueueNames.Message).Enqueue(emailInternalMessageCommand);
            }
            if (syncInfos.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(1) { SendEmailResult.SuccessResult(null) };
            }
            return await ExecuteSendAsync(syncInfos).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="sendOptions">Send email options</param>
        /// <returns>Return the email send results</returns>
        public static List<SendEmailResult> Send(IEnumerable<SendEmailOptions> sendOptions)
        {
            return SendAsync(sendOptions).Result;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="sendEmailOptions">Send email options</param>
        /// <returns>Return send result</returns>
        public static async Task<SendEmailResult> SendAsync(SendEmailOptions sendEmailOptions)
        {
            return (await SendAsync(new SendEmailOptions[1] { sendEmailOptions }).ConfigureAwait(false))?.FirstOrDefault() ?? SendEmailResult.Empty;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="sendEmailOptions">Send email options</param>
        /// <returns>Return send result</returns>
        public static SendEmailResult Send(SendEmailOptions sendEmailOptions)
        {
            return SendAsync(sendEmailOptions).Result;
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
        public static async Task<SendEmailResult> SendAsync(string categoryName, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(content) || receiveAddresses.IsNullOrEmpty())
            {
                return SendEmailResult.Empty;
            }
            return await SendAsync(new SendEmailOptions()
            {
                Asynchronously = asynchronously,
                Category = categoryName,
                Subject = subject,
                Content = content,
                Emails = receiveAddresses,
            }).ConfigureAwait(false);
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
        public static SendEmailResult Send(string categoryName, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            return SendAsync(categoryName, subject, content, asynchronously, receiveAddresses).Result;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static async Task<SendEmailResult> SendAsync(string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            return await SendAsync(string.Empty, subject, content, asynchronously, receiveAddresses).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static SendEmailResult Send(string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            return SendAsync(subject, content, asynchronously, receiveAddresses).Result;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static async Task<SendEmailResult> SendAsync(string subject, string content, params string[] receiveAddresses)
        {
            return await SendAsync(string.Empty, subject, content, true, receiveAddresses).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Email subject</param>
        /// <param name="content">Email content</param>
        /// <param name="receiveAddresses">Receive addresses</param>
        /// <returns>Return the email send result</returns>
        public static SendEmailResult Send(string subject, string content, params string[] receiveAddresses)
        {
            return SendAsync(subject, content, receiveAddresses).Result;
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="sendOptions">Email send options</param>
        /// <returns>Return the email send result</returns>
        public static async Task<List<SendEmailResult>> SendAsync(EmailAccount emailAccount, params SendEmailOptions[] sendOptions)
        {
            if (emailAccount == null || sendOptions.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            List<SendEmailOptions> syncInfos = new List<SendEmailOptions>();
            List<SendEmailOptions> asyncInfos = new List<SendEmailOptions>();
            foreach (var sendOption in sendOptions)
            {
                //Set additional
                SetAdditional(sendOption);

                if (sendOption == null)
                {
                    continue;
                }
                if (sendOption.Asynchronously)
                {
                    asyncInfos.Add(sendOption);
                }
                else
                {
                    syncInfos.Add(sendOption);
                }
            }
            if (!asyncInfos.IsNullOrEmpty())
            {
                InternalQueueSendEmailItem emailInternalMessageCommand = new InternalQueueSendEmailItem()
                {
                    Datas = asyncInfos,
                    EmailAccount = emailAccount
                };
                InternalQueueManager.GetQueue(EZNEWConstants.InternalQueueNames.Message).Enqueue(emailInternalMessageCommand);
            }
            if (syncInfos.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(1) { SendEmailResult.SuccessResult(null) };
            }
            return await ExecuteSendAsync(emailAccount, syncInfos).ConfigureAwait(false);
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
        public static async Task<SendEmailResult> SendAsync(EmailAccount emailAccount, string categoryName, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(content) || receiveAddresses.IsNullOrEmpty())
            {
                return SendEmailResult.Empty;
            }
            var results = await SendAsync(emailAccount, new SendEmailOptions()
            {
                Asynchronously = asynchronously,
                Category = categoryName,
                Subject = subject,
                Content = content,
                Emails = receiveAddresses,
            }).ConfigureAwait(false);
            return results?.FirstOrDefault() ?? SendEmailResult.Empty;
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
        public static async Task<SendEmailResult> SendAsync(EmailAccount emailAccount, string subject, string content, bool asynchronously = true, params string[] receiveAddresses)
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
        public static async Task<SendEmailResult> SendAsync(EmailAccount emailAccount, string subject, string content, params string[] receiveAddresses)
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
        public static void ConfigureEmailAccount(Func<SendEmailOptions, EmailAccount> getEmailAccountOperation, bool useSameEmailAccount = false)
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
        static EmailAccount GetAccount(SendEmailOptions sendInfo)
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
        public static void AddEmailSentCallback(Action<IEnumerable<SendEmailResult>> callback)
        {
            if (callback == null)
            {
                return;
            }
            if (EmailSentCallback == null)
            {
                EmailSentCallback = callback;
            }
            else
            {
                EmailSentCallback += callback;
            }
        }

        #endregion

        #region Util

        /// <summary>
        /// Execute send emails
        /// </summary>
        /// <param name="sendOptions">Email send options</param>
        /// <returns>Return the email send results</returns>
        internal static async Task<List<SendEmailResult>> ExecuteSendAsync(IEnumerable<SendEmailOptions> sendOptions)
        {
            if (sendOptions.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            if (EmailProvider == null)
            {
                throw new EZNEWException("No mail provider is configured");
            }

            Dictionary<string, List<SendEmailOptions>> emailInfoGroups = new Dictionary<string, List<SendEmailOptions>>();
            Dictionary<string, EmailAccount> accounts = new Dictionary<string, EmailAccount>();

            #region Gets email account

            foreach (var sendInfo in sendOptions)
            {
                var account = GetAccount(sendInfo);
                if (account == null)
                {
                    continue;
                }
                string accountKey = account.IdentityKey;
                if (UseSameEmailAccount)
                {
                    emailInfoGroups[accountKey] = sendOptions.ToList();
                    accounts[accountKey] = account;
                    break;
                }
                if (accounts.ContainsKey(accountKey))
                {
                    emailInfoGroups[accountKey].Add(sendInfo);
                }
                else
                {
                    emailInfoGroups.Add(accountKey, new List<SendEmailOptions>() { sendInfo });
                    accounts.Add(accountKey, account);
                }
            }

            #endregion

            #region Execute send

            IEnumerable<SendEmailResult> sendResults = null;

            //Single email account
            if (emailInfoGroups.Count == 1)
            {
                var firstGroup = emailInfoGroups.First();
                var account = accounts[firstGroup.Key];
                sendResults = await EmailProvider.SendAsync(account, firstGroup.Value.ToArray());
            }
            else
            {
                //Multiple email account
                var emailTasks = new Task<List<SendEmailResult>>[emailInfoGroups.Count];
                var groupIndex = 0;
                foreach (var optionGroup in emailInfoGroups)
                {
                    var account = accounts[optionGroup.Key];
                    emailTasks[groupIndex] = EmailProvider.SendAsync(account, optionGroup.Value.ToArray());
                    groupIndex++;
                }
                sendResults = (await Task.WhenAll(emailTasks).ConfigureAwait(false)).SelectMany(c => c);
            }

            #endregion

            //callback
            ThreadPool.QueueUserWorkItem(s =>
            {
                EmailSentCallback?.Invoke(sendResults?.Select(c => c.Clone()).ToList() ?? new List<SendEmailResult>(0));
            });
            return sendResults.ToList();
        }

        /// <summary>
        /// Execute send emails
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="sendOptions">Email send options</param>
        /// <returns>Return the email send results</returns>
        internal static async Task<List<SendEmailResult>> ExecuteSendAsync(EmailAccount emailAccount, IEnumerable<SendEmailOptions> sendOptions)
        {
            var results = await EmailProvider.SendAsync(emailAccount, sendOptions.ToArray()).ConfigureAwait(false);

            //callback
            ThreadPool.QueueUserWorkItem(s =>
            {
                EmailSentCallback?.Invoke(results?.Select(c => c.Clone()).ToList() ?? new List<SendEmailResult>(0));
            });
            return results;
        }

        static void SetAdditional(SendEmailOptions emailOptions)
        {
            if (emailOptions != null)
            {
                //work id
                emailOptions.AddWorkId();
            }
        }

        #endregion
    }
}
