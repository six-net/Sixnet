using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Exceptions;
using Sixnet.Model;
using Sixnet.Net.Email;

namespace Sixnet.Development.Message
{
    internal class EmailMessageHandler : ISixnetMessageHandler
    {
        private readonly string _messageType = SixnetMessager.EmailMessageTypeName;

        public void Send(SendMessageContext context)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(SendMessageContext context)
        {
            SixnetDirectThrower.ThrowArgNullIf(context?.Messages.IsNullOrEmpty() ?? true, $"{nameof(context.Messages)}");

            var templateContext = new GetMessageTemplateContext()
            {
                MessageType = _messageType
            };
            var msgOptions = SixnetMessager.GetMessageOptions();
            var msgTasks = new List<Task>();
            var emails = new List<EmailInfo>();
            foreach (var subjectEntry in context.Messages)
            {
                if (subjectEntry.MessageInfos.IsNullOrEmpty())
                {
                    continue;
                }

                templateContext.MessageSubject = subjectEntry.Subject;
                var msgTemplate = msgOptions.GetMessageTemplate?.Invoke(templateContext);

                SixnetDirectThrower.ThrowInvalidOperationIf(msgTemplate == null, $"Not set message template to subject：{subjectEntry.Subject} for type：{_messageType}");

                foreach (var msgInfo in subjectEntry.MessageInfos)
                {
                    if (!(msgInfo?.Receivers?.TryGetValue(_messageType, out var receivers) ?? false) || receivers.IsNullOrEmpty())
                    {
                        continue;
                    }
                    emails.Add(GetEmailInfo(msgTemplate, msgInfo, subjectEntry.Subject, receivers));
                }

            }
            return SixnetEmailer.SendAsync(emails);
        }

        /// <summary>
        /// Get email info
        /// </summary>
        /// <param name="template"></param>
        /// <param name="message"></param>
        /// <param name="emailCategory"></param>
        /// <param name="emails"></param>
        /// <param name="asynchronously"></param>
        /// <returns></returns>
        EmailInfo GetEmailInfo(MessageTemplate template, MessageInfo message
            , string emailCategory, IEnumerable<string> emails)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(template.Title), "Message template title is null or empty");
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(template.Content), "Message template content is null or empty");
            SixnetDirectThrower.ThrowArgNullIf(message == null, nameof(message));
            SixnetDirectThrower.ThrowArgNullIf(emails.IsNullOrEmpty(), nameof(emails));

            // Parameters
            var parameterDict = message.Parameters?.ToStringDictionary();
            var templateParameters = SixnetMessager.GetTemplateParameters(parameterDict);

            // Title
            var resolveTemplateResult = SixnetMessager.ResolveTemplate(template.Title, templateParameters);
            if (!resolveTemplateResult.Success || string.IsNullOrWhiteSpace(resolveTemplateResult.ErrorParameterName))
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(!string.IsNullOrWhiteSpace(resolveTemplateResult.ErrorParameterName)
                    , $"Not set '{resolveTemplateResult.ErrorParameterName}' value in the email title template");
            }

            // Content
            var contentResolveResult = SixnetMessager.ResolveTemplate(template.Content, templateParameters);
            if (!contentResolveResult.Success || string.IsNullOrWhiteSpace(contentResolveResult.ErrorParameterName))
            {
                SixnetDirectThrower.ThrowInvalidOperationIf(!string.IsNullOrWhiteSpace(contentResolveResult.ErrorParameterName)
                    , $"Not set '{contentResolveResult.ErrorParameterName}' value in the email body template");
            }

            // Email info
            var email = new EmailInfo()
            {
                Subject = emailCategory,
                Content = contentResolveResult.NewContent,
                Title = resolveTemplateResult.NewContent,
                Emails = emails
            };
            email.AddWorkId(message.WorkId);
            email.AddTemplateMessageId(message.Id);
            return email;
        }
    }
}
