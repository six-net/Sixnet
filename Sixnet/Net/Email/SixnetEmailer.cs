using Sixnet.DependencyInjection;
using Sixnet.Exceptions;
using Sixnet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Email manager
    /// </summary>
    public static partial class SixnetEmailer
    {
        #region Fields

        /// <summary>
        /// Email provider
        /// </summary>
        static readonly ISixnetEmailProvider _defaultEmailProvider = new DefaultEmailProvider();

        /// <summary>
        /// Gets or sets the method to gets email account
        /// </summary>
        static Func<EmailInfo, EmailAccount> _getEmailAccountFunc;

        /// <summary>
        /// Indicates whether use the same email account
        /// </summary>
        static bool _useSameEmailAccount;

        /// <summary>
        /// Email sent callback
        /// </summary>
        static Action<IEnumerable<SendEmailResult>> _sendEmailCallback;

        /// <summary>
        /// Email options
        /// </summary>
        readonly static EmailOptions _emailOptions = new EmailOptions();

        #endregion

        #region Send

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        public static List<SendEmailResult> Send(IEnumerable<EmailInfo> emailInfos)
        {
            return ExecuteSend(emailInfos);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emailInfo">Email info</param>
        /// <returns>Return send result</returns>
        public static SendEmailResult Send(EmailInfo emailInfo)
        {
            return Send(new EmailInfo[1] { emailInfo })?.FirstOrDefault();
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
            return Send(categoryName, subject, content, asynchronously, receiveAddresses);
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
            return Send(subject, content, asynchronously, receiveAddresses);
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
            return Send(subject, content, receiveAddresses);
        }

        #endregion

        #region Configure

        /// <summary>
        /// Configure email
        /// </summary>
        /// <param name="configure">Get email account function</param>
        public static void Configure(Action<EmailOptions> configure)
        {
            configure?.Invoke(_emailOptions);
        }

        #endregion

        #region Util

        /// <summary>
        /// Execute send emails
        /// </summary>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        internal static List<SendEmailResult> ExecuteSend(IEnumerable<EmailInfo> emailInfos)
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
                sendResults = emailProvider.Send(firstGroup.Key, firstGroup.Value);
            }
            else
            {
                //Multiple email account
                var emailGroupResults = new List<SendEmailResult[]>(emailInfoGroups.Count);
                foreach (var emailGroup in emailInfoGroups)
                {
                    emailGroupResults.Add(emailProvider.Send(emailGroup.Key, emailGroup.Value));
                }
                sendResults = emailGroupResults.SelectMany(c => c);
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
        /// <param name="account">Email account</param>
        /// <param name="emailInfos">Email infos</param>
        /// <returns>Return the email send results</returns>
        internal static List<SendEmailResult> ExecuteSend(EmailAccount account, IEnumerable<EmailInfo> emailInfos)
        {
            var emailProvider = GetEmailProvider();
            var results = emailProvider.Send(account, emailInfos);

            #region Callback

            ThreadPool.QueueUserWorkItem(s =>
            {
                _sendEmailCallback?.Invoke(results?.Select(c => c.Clone()).ToList() ?? new List<SendEmailResult>(0));
            });

            #endregion

            return results?.ToList() ?? new List<SendEmailResult>(0);
        }

        static void SetAdditional(EmailInfo emailInfo)
        {
            //work id
            emailInfo?.AddWorkId();
        }

        /// <summary>
        /// Get email provider
        /// </summary>
        /// <returns></returns>
        static ISixnetEmailProvider GetEmailProvider()
        {
            var emailProvider = SixnetContainer.GetService<ISixnetEmailProvider>();
            emailProvider ??= _defaultEmailProvider;

            SixnetDirectThrower.ThrowSixnetExceptionIf(emailProvider == null, "Not configure email provider");

            return emailProvider;
        }

        #region Get email account

        /// <summary>
        /// Get email account
        /// </summary>
        /// <param name="sendInfo">Email send info</param>
        /// <returns>Return the email accounts</returns>
        static EmailAccount GetAccount(EmailInfo sendInfo)
        {
            SixnetDirectThrower.ThrowArgNullIf(sendInfo == null, nameof(sendInfo));

            var emailAccount = _getEmailAccountFunc?.Invoke(sendInfo);
            SixnetDirectThrower.ThrowArgNullIf(emailAccount == null, "No mail sending account was specified");

            return emailAccount;
        }

        #endregion

        #endregion
    }
}
