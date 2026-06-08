// "Company © 2025. All rights reserved."

using Sixnet.DependencyInjection;
using Sixnet.Development.Message;
using Sixnet.Exceptions;
using Sixnet.Model;

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
        static readonly ISixnetEmailProvider _defaultEmailProvider = new SixnetDefaultEmailProvider();

        /// <summary>
        /// Default email options
        /// </summary>
        readonly static SixnetEmailOptions _defaultEmailOptions = new();

        #endregion

        #region Send

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static List<SixnetSendEmailResult> Send(IEnumerable<SixnetEmailInfo> emails)
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
                sendResults = emailProvider.Send(account, firstGroup.Value);
            }
            else
            {
                sendResults ??= new List<SixnetSendEmailResult>();
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

            return sendResults ?? new List<SixnetSendEmailResult>(0);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static List<SixnetSendEmailResult> Send(params SixnetEmailInfo[] emails)
        {
            IEnumerable<SixnetEmailInfo> emailCollection = emails;
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
        public static SixnetSendEmailResult Send(string subject, string title, string content, params string[] addresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(subject), nameof(subject));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(addresses.IsNullOrEmpty(), nameof(addresses));

            return Send(new SixnetEmailInfo()
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
        public static SixnetSendEmailResult Send(string title, string content, params string[] addresses)
        {
            return Send(string.Empty, title, content, addresses);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static List<SixnetSendEmailResult> Send(SixnetEmailAccount account, IEnumerable<SixnetEmailInfo> emails)
        {
            SixnetDirectThrower.ThrowArgNullIf(account == null, nameof(account));

            if (emails.IsNullOrEmpty())
            {
                return new List<SixnetSendEmailResult>(0);
            }
            var emailOptions = GetEmailOptions();
            var emailProvider = GetEmailProvider();
            var results = emailProvider.Send(account, emails);
            emailOptions.SendCallback?.Invoke(results);
            return results ?? new List<SixnetSendEmailResult>(0);
        }

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="account">Email account</param>
        /// <param name="emails">Emails</param>
        /// <returns></returns>
        public static List<SixnetSendEmailResult> Send(SixnetEmailAccount account, params SixnetEmailInfo[] emails)
        {
            IEnumerable<SixnetEmailInfo> emailCollection = emails;
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
        public static SixnetSendEmailResult Send(SixnetEmailAccount account, string subject, string title
            , string content, params string[] addresses)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(title), nameof(title));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(content), nameof(content));
            SixnetDirectThrower.ThrowArgNullIf(addresses.IsNullOrEmpty(), nameof(addresses));

            return Send(account, new SixnetEmailInfo()
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
        public static SixnetSendEmailResult Send(SixnetEmailAccount account, string title, string content, params string[] addresses)
        {
            return Send(account, string.Empty, title, content, addresses);
        }

        /// <summary>
        /// Send template message
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SixnetSendEmailResult SendTemplateMessage(SixnetSendMessageContext context)
        {
            return Send(GetEmailInfo(context.Template, context.Message, context.Receivers))?.FirstOrDefault();
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
        static SixnetEmailAccount GetEmailAccount(SixnetEmailOptions emailOptions, SixnetEmailInfo email)
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
        static SixnetEmailOptions GetEmailOptions()
        {
            return SixnetContainer.GetOptions<SixnetEmailOptions>() ?? _defaultEmailOptions;
        }

        /// <summary>
        /// Get email info
        /// </summary>
        /// <returns></returns>
        static SixnetEmailInfo GetEmailInfo(SixnetMessageTemplate template, SixnetMessageInfo message, IEnumerable<string> emails)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(template.Title), "Message template title is null or empty");
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(template.Content), "Message template content is null or empty");
            SixnetDirectThrower.ThrowArgNullIf(message == null, nameof(message));
            SixnetDirectThrower.ThrowArgNullIf(emails.IsNullOrEmpty(), nameof(emails));

            // Parameters
            var parameterDict = message.Data?.ToStringDictionary();
            var templateParameters = SixnetMessager.GetTemplateParameters(parameterDict);

            // Title
            var titleResolveResult = SixnetMessager.ResolveTemplate(template.Title, templateParameters);
            if (!titleResolveResult.Success || string.IsNullOrWhiteSpace(titleResolveResult.ErrorParameterName))
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(!string.IsNullOrWhiteSpace(titleResolveResult.ErrorParameterName)
                    , $"Not set '{titleResolveResult.ErrorParameterName}' value in the email title template");
            }

            // Content
            var contentResolveResult = SixnetMessager.ResolveTemplate(template.Content, templateParameters);
            if (!contentResolveResult.Success || string.IsNullOrWhiteSpace(contentResolveResult.ErrorParameterName))
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(!string.IsNullOrWhiteSpace(contentResolveResult.ErrorParameterName)
                    , $"Not set '{contentResolveResult.ErrorParameterName}' value in the email body template");
            }

            // Email info
            var email = new SixnetEmailInfo()
            {
                Subject = message.Subject,
                Content = contentResolveResult.NewContent,
                Title = titleResolveResult.NewContent,
                Emails = emails,
                Properties = parameterDict
            };
            email.AddWorkId(message.WorkId);
            email.AddTemplateMessageId(message.Id);
            return email;
        }

        #endregion
    }
}
