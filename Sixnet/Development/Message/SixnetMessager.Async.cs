using Sixnet.Development.Work;
using Sixnet.Exceptions;
using Sixnet.MQ;
using Sixnet.Net.Email;
using Sixnet.Net.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Message
{
    public static partial class SixnetMessager
    {
        #region Send message

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns></returns>
        public static Task SendAsync(params MessageInfo[] messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(messages.IsNullOrEmpty(), nameof(messages));

            var parameter = new SendMessageParameter()
            {
                Messages = new List<MessageInfo>(messages)
            };
            var messageProvider = GetMessageProvider();
            return messageProvider.SendAsync(parameter);
        }

        #endregion

        #region Commit message

        /// <summary>
        /// Commit stored messages
        /// </summary>
        /// <returns>Return send result</returns>
        internal static async Task CommitAsync()
        {
            if (MessageBox?.Messages?.IsNullOrEmpty() ?? true)
            {
                return;
            }
            var messageProvider = GetMessageProvider();
            await messageProvider.SendAsync(new SendMessageParameter()
            {
                Messages = MessageBox.Messages
            }).ConfigureAwait(false);
            MessageBox.Clear();
        }

        #endregion

        //#region Send email message

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="message">Message</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="emailAddresses">Email addresses</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns></returns>
        //public static async Task SendEmailMessageAsync(MessageTemplate messageTemplate, MessageInfo message
        //    , string emailCategory, IEnumerable<string> emailAddresses, bool asynchronously = true)
        //{
        //    var emailInfo = GetEmailInfo(messageTemplate, message, emailCategory, emailAddresses, asynchronously);
        //    var emailResults = await SixnetEmailer.SendAsync(emailInfo).ConfigureAwait(false);

        //    SixnetDirectThrower.ThrowSixnetExceptionIf(emailResults.IsNullOrEmpty(), "Send email results is null or empty");

        //    var sendResult = emailResults.FirstOrDefault();
        //    return sendResult?.Success ?? false
        //        ? SendMessageResult.SendSuccess()
        //        : SendMessageResult.SendFailed(sendResult?.Message);
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="message">Message</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="emails">Emails</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendEmailMessage(MessageTemplate messageTemplate, MessageInfo message, string emailCategory, IEnumerable<string> emails, bool asynchronously = true)
        //{
        //    return SendEmailMessageAsync(messageTemplate, message, emailCategory, emails, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="emails">Emails</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendEmailMessageAsync(MessageTemplate messageTemplate, string emailCategory, object parameters, IEnumerable<string> emails, bool asynchronously = true)
        //{
        //    return await SendEmailMessageAsync(messageTemplate, new MessageInfo()
        //    {
        //        Parameters = parameters?.ToStringDictionary()
        //    }, emailCategory, emails, asynchronously).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="emails">Emails</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendEmailMessage(MessageTemplate messageTemplate, string emailCategory, object parameters, IEnumerable<string> emails, bool asynchronously = true)
        //{
        //    return SendEmailMessageAsync(messageTemplate, emailCategory, parameters, emails, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="email">Email</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendEmailMessageAsync(MessageTemplate messageTemplate, string emailCategory, object parameters, string email, bool asynchronously = true)
        //{
        //    return await SendEmailMessageAsync(messageTemplate, emailCategory, parameters, new string[1] { email }, asynchronously).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="email">Email</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendEmailMessage(MessageTemplate messageTemplate, string emailCategory, object parameters, string email, bool asynchronously = true)
        //{
        //    return SendEmailMessageAsync(messageTemplate, emailCategory, parameters, email, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="emailAccount">Email account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="message">Message</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="emails">Emails</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendEmailMessageAsync(EmailAccount emailAccount, MessageTemplate messageTemplate, MessageInfo message, string emailCategory, IEnumerable<string> emails, bool asynchronously = true)
        //{
        //    var emailInfo = GetEmailInfo(messageTemplate, message, emailCategory, emails, asynchronously);
        //    var emailSendResult = (await SixnetEmailer.SendAsync(emailAccount, emailInfo).ConfigureAwait(false))?.FirstOrDefault();
        //    return emailSendResult?.Success ?? false
        //        ? SendMessageResult.SendSuccess()
        //        : SendMessageResult.SendFailed(emailSendResult?.Message);
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="emailAccount">Email account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="message">Message</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="emails">Emails</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendEmailMessage(EmailAccount emailAccount, MessageTemplate messageTemplate, MessageInfo message, string emailCategory, IEnumerable<string> emails, bool asynchronously = true)
        //{
        //    return SendEmailMessageAsync(emailAccount, messageTemplate, message, emailCategory, emails, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="emailAccount">Email account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="emails">Emails</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendEmailMessageAsync(EmailAccount emailAccount, MessageTemplate messageTemplate, string emailCategory, object parameters, IEnumerable<string> emails, bool asynchronously = true)
        //{
        //    return await SendEmailMessageAsync(emailAccount, messageTemplate, new MessageInfo()
        //    {
        //        Parameters = parameters
        //    }, emailCategory, emails, asynchronously).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="emailAccount">Email account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="emails">Emails</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendEmailMessage(EmailAccount emailAccount, MessageTemplate messageTemplate, string emailCategory, object parameters, IEnumerable<string> emails, bool asynchronously = true)
        //{
        //    return SendEmailMessageAsync(emailAccount, messageTemplate, emailCategory, parameters, emails, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="emailAccount">Email account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="email">Email</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendEmailMessageAsync(EmailAccount emailAccount, MessageTemplate messageTemplate, string emailCategory, object parameters, string email, bool asynchronously = true)
        //{
        //    return await SendEmailMessageAsync(emailAccount, messageTemplate, emailCategory, parameters, new string[1] { email }, asynchronously).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// Send email message
        ///// </summary>
        ///// <param name="emailAccount">Email account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="emailCategory">Email category</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="email">Email</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendEmailMessage(EmailAccount emailAccount, MessageTemplate messageTemplate, string emailCategory
        //    , object parameters, string email, bool asynchronously = true)
        //{
        //    return SendEmailMessageAsync(emailAccount, messageTemplate, emailCategory, parameters, email, asynchronously).Result;
        //}

        ///// <summary>
        ///// Get email info
        ///// </summary>
        ///// <param name="template"></param>
        ///// <param name="message"></param>
        ///// <param name="emailCategory"></param>
        ///// <param name="emails"></param>
        ///// <param name="asynchronously"></param>
        ///// <returns></returns>
        //static EmailInfo GetEmailInfo(MessageTemplate template, MessageInfo message
        //    , string emailCategory, IEnumerable<string> emails, bool asynchronously)
        //{
        //    SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(template.Title), "Message template title is null or empty");
        //    SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(template.Content), "Message template content is null or empty");
        //    SixnetDirectThrower.ThrowArgNullIf(message == null, nameof(message));
        //    SixnetDirectThrower.ThrowArgNullIf(emails.IsNullOrEmpty(), nameof(emails));

        //    // Parameters
        //    var parameterDict = message.Parameters?.ToStringDictionary();
        //    var templateParameters = GetTemplateParameters(parameterDict);

        //    // Title
        //    var resolveTemplateResult = ResolveTemplate(template.Title, templateParameters);
        //    if (!resolveTemplateResult.Success || string.IsNullOrWhiteSpace(resolveTemplateResult.ErrorParameterName))
        //    {
        //        SixnetDirectThrower.ThrowInvalidOperationIf(!string.IsNullOrWhiteSpace(resolveTemplateResult.ErrorParameterName)
        //            , $"Not set '{resolveTemplateResult.ErrorParameterName}' value in the email title template");
        //    }

        //    // Content
        //    var contentResolveResult = ResolveTemplate(template.Content, templateParameters);
        //    if (!contentResolveResult.Success || string.IsNullOrWhiteSpace(contentResolveResult.ErrorParameterName))
        //    {
        //        SixnetDirectThrower.ThrowInvalidOperationIf(!string.IsNullOrWhiteSpace(contentResolveResult.ErrorParameterName)
        //            , $"Not set '{contentResolveResult.ErrorParameterName}' value in the email body template");
        //    }

        //    // Email info
        //    var email = new EmailInfo()
        //    {
        //        Subject = emailCategory,
        //        Content = contentResolveResult.NewContent,
        //        Title = resolveTemplateResult.NewContent,
        //        Asynchronously = asynchronously,
        //        Emails = emails
        //    };
        //    email.AddWorkId(message.WorkId);
        //    email.AddTemplateMessageId(message.Id);
        //    return email;
        //}

        //#endregion

        //#region Send sms message

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="message">Message</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="mobiles">Mobiles</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendSmsMessageAsync(MessageTemplate messageTemplate, MessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously = true)
        //{
        //    var msgResult = GetSmsOptions(messageTemplate, message, smsTag, mobiles, asynchronously, out var smsOptions);
        //    if (!msgResult.Success || smsOptions == null)
        //    {
        //        return msgResult;
        //    }
        //    SendSmsResult smsResult = await SixnetSms.SendAsync(smsOptions).ConfigureAwait(false);
        //    return smsResult.Success ? SendMessageResult.SendSuccess() : SendMessageResult.SendFailed(smsResult.Description);
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="message">Message</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="mobiles">Mobiles</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendSmsMessage(MessageTemplate messageTemplate, MessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously = true)
        //{
        //    return SendSmsMessageAsync(messageTemplate, message, smsTag, mobiles, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="mobiles">Mobiles</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendSmsMessageAsync(MessageTemplate messageTemplate, string smsTag, object parameters, IEnumerable<string> mobiles, bool asynchronously = true)
        //{
        //    return await SendSmsMessageAsync(messageTemplate, new MessageInfo()
        //    {
        //        Parameters = parameters?.ToStringDictionary()
        //    }, smsTag, mobiles, asynchronously).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="mobiles">Mobiles</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendSmsMessage(MessageTemplate messageTemplate, string smsTag, object parameters, IEnumerable<string> mobiles, bool asynchronously = true)
        //{
        //    return SendSmsMessageAsync(messageTemplate, smsTag, parameters, mobiles, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="mobile">Mobile</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendSmsMessageAsync(MessageTemplate messageTemplate, string smsTag, object parameters, string mobile, bool asynchronously = true)
        //{
        //    return await SendSmsMessageAsync(messageTemplate, smsTag, parameters, new string[1] { mobile }, asynchronously).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="mobile">Mobile</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendSmsMessage(MessageTemplate messageTemplate, string smsTag, object parameters, string mobile, bool asynchronously = true)
        //{
        //    return SendSmsMessageAsync(messageTemplate, smsTag, parameters, mobile, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="smsAccount">Sms account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="message">Message</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="mobiles">Mobiles</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendSmsMessageAsync(SmsAccount smsAccount, MessageTemplate messageTemplate, MessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously = true)
        //{
        //    var msgResult = GetSmsOptions(messageTemplate, message, smsTag, mobiles, asynchronously, out var smsOptions);
        //    if (!msgResult.Success || smsOptions == null)
        //    {
        //        return msgResult;
        //    }
        //    SendSmsResult smsSendResult = await SixnetSms.SendAsync(smsAccount, smsOptions).ConfigureAwait(false);
        //    return smsSendResult.Success ? SendMessageResult.SendSuccess() : SendMessageResult.SendFailed(smsSendResult.Description);
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="smsAccount">Sms account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="message">Message</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="mobiles">Mobiles</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendSmsMessage(SmsAccount smsAccount, MessageTemplate messageTemplate, MessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously = true)
        //{
        //    return SendSmsMessageAsync(smsAccount, messageTemplate, message, smsTag, mobiles, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="smsAccount">Sms account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="mobile">Mobiles</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendSmsMessageAsync(SmsAccount smsAccount, MessageTemplate messageTemplate, string smsTag, object parameters, IEnumerable<string> mobiles, bool asynchronously = true)
        //{
        //    return await SendSmsMessageAsync(smsAccount, messageTemplate, new MessageInfo()
        //    {
        //        Parameters = parameters
        //    }, smsTag, mobiles, asynchronously).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="smsAccount">Sms account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="mobiles">Mobiles</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendSmsMessage(SmsAccount smsAccount, MessageTemplate messageTemplate, string smsTag, object parameters, IEnumerable<string> mobiles, bool asynchronously = true)
        //{
        //    return SendSmsMessageAsync(smsAccount, messageTemplate, smsTag, parameters, mobiles, asynchronously).Result;
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="smsAccount">Sms account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="mobile">Mobile</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static async Task<SendMessageResult> SendSmsMessageAsync(SmsAccount smsAccount, MessageTemplate messageTemplate, string smsTag, object parameters, string mobile, bool asynchronously = true)
        //{
        //    return await SendSmsMessageAsync(smsAccount, messageTemplate, smsTag, parameters, new string[1] { mobile }, asynchronously).ConfigureAwait(false);
        //}

        ///// <summary>
        ///// Send sms message
        ///// </summary>
        ///// <param name="smsAccount">Sms account</param>
        ///// <param name="messageTemplate">Message template</param>
        ///// <param name="smsTag">Sms tag</param>
        ///// <param name="parameters">Parameters</param>
        ///// <param name="mobile">Mobile</param>
        ///// <param name="asynchronously">Whether send by asynchronously</param>
        ///// <returns>Return send result</returns>
        //public static SendMessageResult SendSmsMessage(SmsAccount smsAccount, MessageTemplate messageTemplate, string smsTag, object parameters, string mobile, bool asynchronously = true)
        //{
        //    return SendSmsMessageAsync(smsAccount, messageTemplate, smsTag, parameters, mobile, asynchronously).Result;
        //}

        //static SendMessageResult GetSmsOptions(MessageTemplate messageTemplate, MessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously, out SendSmsParameter sendSmsOptions)
        //{
        //    sendSmsOptions = null;
        //    if (messageTemplate == null)
        //    {
        //        throw new ArgumentNullException(nameof(MessageTemplate));
        //    }
        //    if (message == null)
        //    {
        //        throw new ArgumentNullException(nameof(MessageInfo));
        //    }
        //    if (string.IsNullOrWhiteSpace(messageTemplate.Content))
        //    {
        //        throw new Exception("The message template content is null or empty");
        //    }
        //    if (mobiles.IsNullOrEmpty())
        //    {
        //        throw new Exception("The mobiles is null or empty");
        //    }
        //    var parameterDict = message.Parameters?.ToStringDictionary();
        //    Dictionary<string, string> templateParameters = GetTemplateParameters(parameterDict);
        //    //content
        //    var contentResolveResult = ResolveTemplate(messageTemplate.Content, templateParameters);
        //    if (!contentResolveResult.Item1 || string.IsNullOrWhiteSpace(contentResolveResult.Item3))
        //    {
        //        if (!string.IsNullOrWhiteSpace(contentResolveResult.Item2))
        //        {
        //            return SendMessageResult.NoParameter($"Not set '{contentResolveResult.Item2}' value in the sms body template");
        //        }
        //        return SendMessageResult.MessageIsNullOrEmpty($"The sms body is null or empty");
        //    }

        //    //Send sms
        //    sendSmsOptions = new SendSmsParameter()
        //    {
        //        Tag = smsTag,
        //        Content = contentResolveResult.Item3,
        //        Properties = parameterDict,
        //        Asynchronously = asynchronously,
        //        Mobiles = mobiles,
        //    };
        //    sendSmsOptions.AddWorkId(message.WorkId);
        //    sendSmsOptions.AddTemplateMessageId(message.Id);
        //    return SendMessageResult.SendSuccess();
        //}

        //#endregion
    }
}
