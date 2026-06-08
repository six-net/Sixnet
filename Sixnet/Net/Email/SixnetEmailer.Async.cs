// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Development.Message;
using Sixnet.Exceptions;

namespace Sixnet.Net.Email
{
    public static partial class SixnetEmailer
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static async Task<List<SixnetSendEmailResult>> SendAsync(IEnumerable<SixnetEmailInfo> emails)
        {
            if (emails.IsNullOrEmpty())
            {
                return new List<SixnetSendEmailResult>(0);
            }

            var emailProvider = GetEmailProvider();
            var emailGroups = new Dictionary<SixnetEmailAccount, List<SixnetEmailInfo>>();
            var emailOptions = GetEmailOptions();
            SixnetEmailAccount emailAccount = null;

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
                    emailGroups.Add(emailAccount, new List<SixnetEmailInfo>() { email });
                }
            }

            #endregion

            #region Execute send

            List<SixnetSendEmailResult> sendResults = null;

            if (emailGroups.Count == 1)
            {
                var firstGroup = emailGroups.First();
                var account = firstGroup.Key;
                sendResults = await emailProvider.SendAsync(account, firstGroup.Value).ConfigureAwait(false);
            }
            else
            {
                var emailTasks = new Task<List<SixnetSendEmailResult>>[emailGroups.Count];
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

            return sendResults ?? new List<SixnetSendEmailResult>(0);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static Task<List<SixnetSendEmailResult>> SendAsync(params SixnetEmailInfo[] emails)
        {
            IEnumerable<SixnetEmailInfo> emailCollection = emails;
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
        public static async Task<SixnetSendEmailResult> SendAsync(string subject, string title, string content, params string[] addresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(subject), nameof(subject));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(addresses.IsNullOrEmpty(), nameof(addresses));

            return (await SendAsync(new SixnetEmailInfo()
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
        public static Task<SixnetSendEmailResult> SendAsync(string title, string content, params string[] addresses)
        {
            return SendAsync(string.Empty, title, content, addresses);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static async Task<List<SixnetSendEmailResult>> SendAsync(SixnetEmailAccount account, IEnumerable<SixnetEmailInfo> emails)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));

            if (emails.IsNullOrEmpty())
            {
                return new List<SixnetSendEmailResult>(0);
            }
            var emailOptions = GetEmailOptions();
            var emailProvider = GetEmailProvider();
            var results = await emailProvider.SendAsync(account, emails).ConfigureAwait(false);
            emailOptions.SendCallback?.Invoke(results);
            return results ?? new List<SixnetSendEmailResult>(0);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static Task<List<SixnetSendEmailResult>> SendAsync(SixnetEmailAccount account, params SixnetEmailInfo[] emails)
        {
            IEnumerable<SixnetEmailInfo> emailCollection = emails;
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
        public static async Task<SixnetSendEmailResult> SendAsync(SixnetEmailAccount account, string subject, string title
            , string content, params string[] addresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(title), nameof(title));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(addresses.IsNullOrEmpty(), nameof(addresses));

            var results = await SendAsync(account, new SixnetEmailInfo()
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
        public static Task<SixnetSendEmailResult> SendAsync(SixnetEmailAccount account, string title, string content, params string[] addresses)
        {
            return SendAsync(account, string.Empty, title, content, addresses);
        }

        /// <summary>
        /// Send template message
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<SixnetSendEmailResult> SendTemplateMessageAsync(SixnetSendMessageContext context)
        {
            return (await SendAsync(GetEmailInfo(context.Template, context.Message, context.Receivers)).ConfigureAwait(false))?.FirstOrDefault();
        }
    }
}
