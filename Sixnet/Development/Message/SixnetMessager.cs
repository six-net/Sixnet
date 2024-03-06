﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.DependencyInjection;
using Sixnet.Development.Work;
using Sixnet.Exceptions;
using Sixnet.Model;
using Sixnet.Net.Email;
using Sixnet.Net.Sms;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message manager
    /// </summary>
    public static partial class SixnetMessager
    {
        #region Fields

        static readonly AsyncLocal<MessageBox> _messageBox = new();
        static readonly MessageOptions _defaultOptions = new();
        static readonly DefaultMessageProvider _defaultMessageProvider = new();
        public const string EmailMessageTypeName = "SIXNET_DEV_MESSAGE_EMAIL";
        public const string SmsMessageTypeName = "SIXNET_DEV_MESSAGE_SMS";
        public const string NotificationMessageTypeName = "SIXNET_DEV_MESSAGE_NOTIFICATION";
        static readonly Dictionary<string, ISixnetMessageHandler> _defaultMessageHandlers = new();

        #endregion

        #region Constructor

        static SixnetMessager()
        {
            _defaultMessageHandlers.Add(EmailMessageTypeName, new EmailMessageHandler());
            _defaultMessageHandlers.Add(SmsMessageTypeName, new SmsMessageHandler());
            _defaultMessageHandlers.Add(NotificationMessageTypeName, new NotificationMessageHandler());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current message box
        /// </summary>
        internal static MessageBox MessageBox
        {
            get
            {
                return _messageBox?.Value;
            }
            set
            {
                _messageBox.Value = value;
            }
        }

        #endregion

        #region Init

        /// <summary>
        /// Init message manager
        /// Dispose current message box
        /// Create a new message box
        /// </summary>
        internal static void Init()
        {
            MessageBox = MessageBox.Create();
        }


        #endregion

        #region Send message

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns></returns>
        public static void Send(params MessageInfo[] messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(messages.IsNullOrEmpty(), nameof(messages));

            var parameter = new SendMessageParameter()
            {
                Messages = new List<MessageInfo>(messages)
            };
            var messageProvider = GetMessageProvider();
            messageProvider.Send(parameter);
        }

        #endregion

        #region Store message

        /// <summary>
        /// Store messages to message box
        /// </summary>
        /// <param name="messages">Messages</param>
        public static void Store(params MessageInfo[] messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(MessageBox == null, $"Please call {nameof(Init)} method first to initialize the message box");
            MessageBox.Add(messages);
        }

        #endregion

        #region Commit message

        /// <summary>
        /// Commit stored message
        /// </summary>
        /// <returns>Return send result</returns>
        internal static void Commit()
        {
            if (MessageBox?.Messages?.IsNullOrEmpty() ?? true)
            {
                return;
            }
            var messageProvider = GetMessageProvider();
            messageProvider.Send(new SendMessageParameter()
            {
                Messages = MessageBox.Messages
            });
            MessageBox.Clear();
        }

        #endregion

        #region Clear message

        /// <summary>
        /// Remove all stored messages
        /// </summary>
        internal static void Clear()
        {
            MessageBox?.Clear();
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
        //public static async Task<SendMessageResult> SendEmailMessageAsync(MessageTemplate messageTemplate, MessageInfo message
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

        #region Util

        /// <summary>
        /// Get message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <returns>Return message handler</returns>
        public static ISixnetMessageHandler GetMessageHandler(string messageType)
        {
            var options = GetMessageOptions();
            var handler = options.GetMessageHandler(messageType);
            if(handler == null)
            {
                _defaultMessageHandlers.TryGetValue(messageType, out handler);
            }

            SixnetDirectThrower.ThrowArgNullIf(handler == null, $"Not set message handler for:{messageType}");

            return handler;
        }

        /// <summary>
        /// Resolve template
        /// </summary>
        /// <param name="template">Template</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        public static ResolveMessageTemplateResult ResolveTemplate(string template, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                return ResolveMessageTemplateResult.Create(true);
            }
            var matchEvaluator = new MatchEvaluator(c =>
            {
                if (parameters?.ContainsKey(c.Value) ?? false)
                {
                    return parameters[c.Value];
                }
                return string.Empty;
            });
            var matchRegex = GetKeywordMatchRegex();
            var matchCollection = matchRegex.Matches(template);
            foreach (Match matchVal in matchCollection.Cast<Match>())
            {
                if (!(parameters?.ContainsKey(matchVal.Value) ?? false))
                {
                    return ResolveMessageTemplateResult.Create(false, matchVal.Value);
                }
            }
            var newValue = matchRegex.Replace(template, matchEvaluator);
            return ResolveMessageTemplateResult.Create(true, string.Empty, newValue);
        }

        /// <summary>
        /// Get template parameters
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetTemplateParameters(Dictionary<string, string> originalParameters)
        {
            if (originalParameters.IsNullOrEmpty())
            {
                return new Dictionary<string, string>(0);
            }
            return originalParameters.ToDictionary(c => HandleParameterName(c.Key), c => c.Value);
        }

        /// <summary>
        /// Get message options
        /// </summary>
        /// <returns></returns>
        public static MessageOptions GetMessageOptions()
        {
            return SixnetContainer.GetOptions<MessageOptions>() ?? _defaultOptions;
        }

        /// <summary>
        /// Handle parameter name
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns></returns>
        public static string HandleParameterName(string parameterName)
        {
            var options = GetMessageOptions();
            return options?.HandleParameterName?.Invoke(parameterName) ?? parameterName ?? string.Empty;
        }

        /// <summary>
        /// Get keyword match regex
        /// </summary>
        /// <returns></returns>
        static Regex GetKeywordMatchRegex()
        {
            var options = GetMessageOptions();
            return new Regex(options.KeywordMatchPattern);
        }

        /// <summary>
        /// Get message provider
        /// </summary>
        /// <returns></returns>
        internal static ISixnetMessageProvider GetMessageProvider()
        {
            return SixnetContainer.GetService<ISixnetMessageProvider>() ?? _defaultMessageProvider;
        }

        #endregion
    }
}
