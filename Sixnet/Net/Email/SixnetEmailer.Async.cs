using Sixnet.Development.Repository;
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
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static async Task<List<SendEmailResult>> SendAsync(IEnumerable<EmailInfo> emails)
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
                sendResults = await emailProvider.SendAsync(account, firstGroup.Value).ConfigureAwait(false);
            }
            else
            {
                var emailTasks = new Task<List<SendEmailResult>>[emailGroups.Count];
                var groupIndex = 0;
                foreach (var emailGroup in emailGroups)
                {
                    var account = emailGroup.Key;
                    emailTasks[groupIndex] = emailProvider.SendAsync(account, emailGroup.Value);
                    groupIndex++;
                }
                sendResults = (await Task.WhenAll(emailTasks).ConfigureAwait(false)).SelectMany(c => c).ToList();
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
        public static Task<List<SendEmailResult>> SendAsync(params EmailInfo[] emails)
        {
            IEnumerable<EmailInfo> emailCollection = emails;
            return SendAsync(emailCollection);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="subject">Subject</param>
        /// <param name="subject">Title</param>
        /// <param name="content">Content</param>
        /// <param name="addresses">Email addresses</param>
        /// <returns></returns>
        public static async Task<SendEmailResult> SendAsync(string subject, string title, string content, params string[] addresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(subject), nameof(subject));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(addresses.IsNullOrEmpty(), nameof(addresses));

            return (await SendAsync(new EmailInfo()
            {
                Subject = subject,
                Title = title,
                Content = content,
                Emails = addresses,
            }).ConfigureAwait(false))?.FirstOrDefault();
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="title">Subject</param>
        /// <param name="content">Content</param>
        /// <param name="addresses">Email addresses</param>
        /// <returns></returns>
        public static Task<SendEmailResult> SendAsync(string title, string content, params string[] addresses)
        {
            return SendAsync(string.Empty, title, content, addresses);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static async Task<List<SendEmailResult>> SendAsync(EmailAccount account, IEnumerable<EmailInfo> emails)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));

            if (emails.IsNullOrEmpty())
            {
                return new List<SendEmailResult>(0);
            }
            var emailOptions = GetEmailOptions();
            var emailProvider = GetEmailProvider();
            var results = await emailProvider.SendAsync(account, emails).ConfigureAwait(false);
            emailOptions.SendCallback?.Invoke(results);
            return results ?? new List<SendEmailResult>(0);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static Task<List<SendEmailResult>> SendAsync(EmailAccount account, params EmailInfo[] emails)
        {
            IEnumerable<EmailInfo> emailCollection = emails;
            return SendAsync(account, emailCollection);
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
        public static async Task<SendEmailResult> SendAsync(EmailAccount account, string subject, string title
            , string content, params string[] addresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(title), nameof(title));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(addresses.IsNullOrEmpty(), nameof(addresses));

            var results = await SendAsync(account, new EmailInfo()
            {
                Subject = subject,
                Title = title,
                Content = content,
                Emails = addresses,
            }).ConfigureAwait(false);
            return results?.FirstOrDefault();
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Account</param>
        /// <param name="title">Title</param>
        /// <param name="content">Content</param>>
        /// <param name="addresses">Email addresses</param>
        /// <returns></returns>
        public static Task<SendEmailResult> SendAsync(EmailAccount account, string title, string content, params string[] addresses)
        {
            return SendAsync(account, string.Empty, title, content, addresses);
        }
    }
}
