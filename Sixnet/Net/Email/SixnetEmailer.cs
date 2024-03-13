using Sixnet.DependencyInjection;
using Sixnet.Exceptions;
using Sixnet.Model;
using Sixnet.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        /// Default email options
        /// </summary>
        readonly static EmailOptions _defaultEmailOptions = new();

        #endregion

        #region Send

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static List<SendEmailResult> Send(IEnumerable<EmailInfo> emails)
        {
            if (emails.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }

            var emailProvider = GetEmailProvider();
            var emailGroups = new Dictionary<EmailAccount, List<EmailInfo>>();
            var emailOptions = GetEmailOptions();
            EmailAccount emailAccount = null;

            #region Gets email account

            foreach (var email in emails)
            {
                if (email == null)
                {
                    continue;
                }
                if (!emailOptions.UseSameAccount || emailAccount == null)
                {
                    emailAccount = GetEmailAccount(emailOptions, email);
                    if (emailAccount == null)
                    {
                        continue;
                    }
                }
                if (emailOptions.UseSameAccount)
                {
                    emailGroups[emailAccount] = emails.ToList();
                    break;
                }
                if (emailGroups.ContainsKey(emailAccount))
                {
                    emailGroups[emailAccount].Add(email);
                }
                else
                {
                    emailGroups.Add(emailAccount, new List<EmailInfo>() { email });
                }
            }

            #endregion

            #region Execute send

            List<SendEmailResult> sendResults = null;
            if (emailGroups.Count == 1)
            {
                var firstGroup = emailGroups.First();
                var account = firstGroup.Key;
                sendResults = emailProvider.Send(account, firstGroup.Value);
            }
            else
            {
                sendResults ??= new List<SendEmailResult>();
                foreach (var emailGroup in emailGroups)
                {
                    var account = emailGroup.Key;
                    var groupResults = emailProvider.Send(account, emailGroup.Value);
                    if (!groupResults.IsNullOrEmpty())
                    {
                        sendResults.AddRange(groupResults);
                    }
                }
            }

            #endregion

            #region Callback

            emailOptions.SendCallback?.Invoke(sendResults);

            #endregion

            return sendResults ?? new List<SendEmailResult>(0);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static List<SendEmailResult> Send(params EmailInfo[] emails)
        {
            IEnumerable<EmailInfo> emailCollection = emails;
            return Send(emailCollection);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="subject">Title</param>
        /// <param name="content">Content</param>
        /// <param name="addresses">Email addresses</param>
        /// <returns></returns>
        public static SendEmailResult Send(string subject, string title, string content, params string[] addresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(subject), nameof(subject));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(addresses.IsNullOrEmpty(), nameof(addresses));

            return Send(new EmailInfo()
            {
                Subject = subject,
                Title = title,
                Content = content,
                Emails = addresses,
            })?.FirstOrDefault();
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="title">Subject</param>
        /// <param name="content">Content</param>
        /// <param name="addresses">Email addresses</param>
        /// <returns></returns>
        public static SendEmailResult Send(string title, string content, params string[] addresses)
        {
            return Send(string.Empty, title, content, addresses);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static List<SendEmailResult> Send(EmailAccount account, IEnumerable<EmailInfo> emails)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));

            if (emails.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            var emailOptions = GetEmailOptions();
            var emailProvider = GetEmailProvider();
            var results = emailProvider.Send(account, emails);
            emailOptions.SendCallback?.Invoke(results);
            return results ?? new List<SendEmailResult>(0);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static List<SendEmailResult> Send(EmailAccount account, params EmailInfo[] emails)
        {
            IEnumerable<EmailInfo> emailCollection = emails;
            return Send(account, emailCollection);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="subject">Subject</param>
        /// <param name="title">Title</param>
        /// <param name="content">Content</param>
        /// <param name="addresses">Email addresses</param>
        /// <returns></returns>
        public static SendEmailResult Send(EmailAccount account, string subject, string title
            , string content, params string[] addresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(title), nameof(title));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(addresses.IsNullOrEmpty(), nameof(addresses));

            return Send(account, new EmailInfo()
            {
                Subject = subject,
                Title = title,
                Content = content,
                Emails = addresses,
            })?.FirstOrDefault();
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="title">Title</param>
        /// <param name="content">Content</param>>
        /// <param name="addresses">Email addresses</param>
        /// <returns></returns>
        public static SendEmailResult Send(EmailAccount account, string title, string content, params string[] addresses)
        {
            return Send(account, string.Empty, title, content, addresses);
        }

        #endregion

        #region Util

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

        /// <summary>
        /// Get email account
        /// </summary>
        /// <param name="emailOptions">Email options</param>
        /// <param name="email">Email info</param>
        /// <returns></returns>
        static EmailAccount GetEmailAccount(EmailOptions emailOptions, EmailInfo email)
        {
            SixnetDirectThrower.ThrowArgNullIf(email == null, nameof(email));

            var emailAccount = emailOptions.GetEmailAccount?.Invoke(email) ?? emailOptions.Account;

            SixnetDirectThrower.ThrowSixnetExceptionIf(emailAccount == null, "No set email account");

            return emailAccount;
        }

        /// <summary>
        /// Get email options
        /// </summary>
        /// <returns></returns>
        static EmailOptions GetEmailOptions()
        {
            return SixnetContainer.GetOptions<EmailOptions>() ?? _defaultEmailOptions;
        }

        #endregion
    }
}
