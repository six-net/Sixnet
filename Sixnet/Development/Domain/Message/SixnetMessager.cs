using Sixnet.DependencyInjection;
using Sixnet.Development.Work;
using Sixnet.Model;
using Sixnet.MQ;
using Sixnet.Net.Email;
using Sixnet.Net.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Development.Domain.Message
{
    /// <summary>
    /// Message manager
    /// </summary>
    public static class SixnetMessager
    {
        #region Fields

        static readonly AsyncLocal<MessageBox> _messageBox = new AsyncLocal<MessageBox>();
        const string _defaultKeywordMatchPattern = @"\$.*?\$";
        static string keywordMatchPattern = _defaultKeywordMatchPattern;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the template keyword match regex
        /// </summary>
        public static Regex KeywordMatchRegex { get; private set; }

        /// <summary>
        /// Gets or sets the template keyword match pattern
        /// </summary>
        public static string KeywordMatchPattern { get; set; }

        /// <summary>
        /// Gets or sets the message provider
        /// </summary>
        public static ISixnetMessageProvider MessageProvider => SixnetContainer.GetService<ISixnetMessageProvider>();

        /// <summary>
        /// Message handlers
        /// </summary>
        private static Dictionary<string, ISixnetMessageHandler> MessageHandlers
        {
            get;
        } = new Dictionary<string, ISixnetMessageHandler>();

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

        /// <summary>
        /// Process parameter name func
        /// </summary>
        private static Func<string, string> ParameterNameHandlingFunc = name => $"${name}$";

        #endregion

        #region Constructor

        static SixnetMessager()
        {
            InitKeywordMatchRegex();
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

        #region Process parameter name func

        /// <summary>
        /// Configure parameter name handling
        /// </summary>
        /// <param name="func"></param>
        public static void ConfigureParameterNameHandling(Func<string, string> func)
        {
            ParameterNameHandlingFunc = func;
        }

        /// <summary>
        /// Handle parameter name
        /// </summary>
        /// <param name="originalName">Original name</param>
        /// <returns>Return new parameter name</returns>
        public static string HandleParameterName(string originalName)
        {
            return ParameterNameHandlingFunc?.Invoke(originalName) ?? originalName;
        }

        #endregion

        #region Keyword pattern

        /// <summary>
        /// Configure template keyword match pattern
        /// Will use the default match pattern value automatic when the new value is null or empty
        /// </summary>
        /// <param name="matchPattern">Keyword match pattern</param>
        public static void ConfigureKeywordMatchPattern(string matchPattern)
        {
            if (string.IsNullOrWhiteSpace(matchPattern))
            {
                matchPattern = _defaultKeywordMatchPattern;
            }
            if (keywordMatchPattern.Equals(matchPattern))
            {
                return;
            }
            keywordMatchPattern = matchPattern;
        }

        static void InitKeywordMatchRegex()
        {
            KeywordMatchRegex = new Regex(string.IsNullOrWhiteSpace(KeywordMatchPattern) ? _defaultKeywordMatchPattern : KeywordMatchPattern);
        }

        #endregion

        #region Message handler

        /// <summary>
        /// Configure message handler
        /// </summary>
        /// <typeparam name="THandler">Message handler type</typeparam>
        /// <param name="messageType">Message type</param>
        /// <param name="handler">Message handler</param>
        public static void ConfigureMessageHandler<THandler>(string messageType, THandler handler) where THandler : ISixnetMessageHandler
        {
            if (string.IsNullOrWhiteSpace(messageType))
            {
                return;
            }
            MessageHandlers[messageType] = handler;
        }

        /// <summary>
        /// Configure message handler
        /// </summary>
        /// <typeparam name="THandler">Message handler type</typeparam>
        /// <param name="messageType">Message type</param>
        public static void ConfigureMessageHandler<THandler>(string messageType) where THandler : ISixnetMessageHandler, new()
        {
            ConfigureMessageHandler(messageType, new THandler());
        }

        /// <summary>
        /// Configure message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="func">Func</param>
        public static void ConfigureMessageHandler(string messageType, Func<SendMessageContext, SendMessageResult> func)
        {
            if (func == null)
            {
                return;
            }
            Task<SendMessageResult> asyncFunc(SendMessageContext context) => Task.Run(() =>
             {
                 return func(context);
             });
            DefaultMessageHandler defaultMessageHandler = new DefaultMessageHandler(asyncFunc);
            ConfigureMessageHandler(messageType, defaultMessageHandler);
        }

        /// <summary>
        /// Configure message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="func">Func</param>
        public static void ConfigureMessageHandler(string messageType, Func<SendMessageContext, Task<SendMessageResult>> func)
        {
            if (func == null)
            {
                return;
            }
            DefaultMessageHandler defaultMessageHandler = new DefaultMessageHandler(func);
            ConfigureMessageHandler(messageType, defaultMessageHandler);
        }

        /// <summary>
        /// Remove message handler
        /// </summary>
        /// <param name="messageTypes">Message types</param>
        public static void RemoveMessageHandler(params string[] messageTypes)
        {
            if (messageTypes.IsNullOrEmpty())
            {
                return;
            }
            foreach (var msgType in messageTypes)
            {
                MessageHandlers.Remove(msgType);
            }
        }

        /// <summary>
        /// Clear message handler
        /// </summary>
        public static void ClearMessageHandler()
        {
            MessageHandlers.Clear();
        }

        #endregion

        #region Direct send message

        /// <summary>
        /// Direct send message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> DirectSendAsync(SixnetMessageInfo message)
        {
            var sendMessageOptions = new SendMessageOptions()
            {
                Messages = new List<SixnetMessageInfo>(1) { message }
            };
            return await (MessageProvider?.SendAsync(sendMessageOptions) ?? Task.FromResult(SendMessageResult.NoProvider())).ConfigureAwait(false);
        }

        /// <summary>
        /// Direct send message
        /// </summary>
        /// <param name="message">message</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult DirectSend(SixnetMessageInfo message)
        {
            return DirectSendAsync(message).Result;
        }

        #endregion

        #region Register message

        /// <summary>
        /// Register message
        /// </summary>
        /// <param name="messages">Messages</param>
        public static void Register(params SixnetMessageInfo[] messages)
        {
            if (MessageBox == null)
            {
                throw new Exception($"Please call {nameof(SixnetMessager.Init)} method first to initialize MessageManager");
            }
            MessageBox.Add(messages);
        }

        /// <summary>
        /// Register message
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="receiverType">Reveiver</param>
        /// <param name="receiverValue"></param>
        public static void Register(string messageType, object parameter, string receiverType, string receiverValue)
        {
            if (string.IsNullOrWhiteSpace(messageType))
            {
                throw new ArgumentNullException(nameof(messageType));
            }
            Register(new SixnetMessageInfo()
            {
                Id = SixnetMessageInfo.CreateMessageId(),
                Type = messageType,
                Parameters = parameter?.ToStringDictionary(),
                Receivers = new Dictionary<string, List<string>>()
                {
                    {
                        receiverType,
                        new List<string>(1)
                        {
                            receiverValue
                        }
                    }
                },
                WorkId = UnitOfWork.Current?.WorkId
            });
        }

        #endregion

        #region Commit message

        /// <summary>
        /// Commit message
        /// Send the messages which are registered
        /// </summary>
        /// <param name="asynchronously">Commit message asynchronously</param>
        /// <returns>Return send result</returns>
        internal static async Task<SendMessageResult> CommitAsync(bool asynchronously = true)
        {
            if (MessageBox?.Messages?.IsNullOrEmpty() ?? true)
            {
                return SendMessageResult.MessageIsNullOrEmpty();
            }
            SendMessageResult sendMessageResult = null;
            var currentMessages = MessageBox.Messages.Select(c => c).ToList();
            MessageBox.Clear();
            if (asynchronously)
            {
                var messageInternalCommand = new InProcessQueueDomainMessage(MessageProvider, currentMessages, UnitOfWork.Current);
                InProcessQueueManager.GetQueue(SixnetMQ.InProcessQueueNames.Message).Enqueue(messageInternalCommand);
                sendMessageResult = SendMessageResult.SendSuccess();
            }
            else
            {
                sendMessageResult = await (MessageProvider?.SendAsync(new SendMessageOptions()
                {
                    Messages = currentMessages
                }) ?? Task.FromResult(SendMessageResult.NoProvider())).ConfigureAwait(false);
            }
            return sendMessageResult;
        }

        /// <summary>
        /// Commit message
        /// Send the messages which are registered
        /// </summary>
        /// <param name="asynchronously">Commit message asynchronously</param>
        /// <returns>Return send result</returns>
        internal static SendMessageResult Commit(bool asynchronously = true)
        {
            return CommitAsync(asynchronously).Result;
        }

        #endregion

        #region Clear message

        /// <summary>
        /// Remove all current message
        /// </summary>
        internal static void Clear()
        {
            MessageBox?.Clear();
        }

        #endregion

        #region Send email message

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="message">Message</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="emails">Emails</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendEmailMessageAsync(SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string emailCategory, IEnumerable<string> emails, bool asynchronously = true)
        {
            var msgResult = GetEmailOptions(messageTemplate, message, emailCategory, emails, asynchronously, out var emailOptions);
            if (!msgResult.Success || emailOptions == null)
            {
                return msgResult;
            }
            SendEmailResult emailResult = await SixnetEmailer.SendAsync(emailOptions).ConfigureAwait(false);
            return (emailResult?.Success ?? false) ? SendMessageResult.SendSuccess() : SendMessageResult.SendFailed(emailResult?.Message);
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="message">Message</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="emails">Emails</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendEmailMessage(SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string emailCategory, IEnumerable<string> emails, bool asynchronously = true)
        {
            return SendEmailMessageAsync(messageTemplate, message, emailCategory, emails, asynchronously).Result;
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="emails">Emails</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendEmailMessageAsync(SixnetMessageTemplate messageTemplate, string emailCategory, object parameters, IEnumerable<string> emails, bool asynchronously = true)
        {
            return await SendEmailMessageAsync(messageTemplate, new SixnetMessageInfo()
            {
                Parameters = parameters?.ToStringDictionary()
            }, emailCategory, emails, asynchronously).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="emails">Emails</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendEmailMessage(SixnetMessageTemplate messageTemplate, string emailCategory, object parameters, IEnumerable<string> emails, bool asynchronously = true)
        {
            return SendEmailMessageAsync(messageTemplate, emailCategory, parameters, emails, asynchronously).Result;
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="email">Email</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendEmailMessageAsync(SixnetMessageTemplate messageTemplate, string emailCategory, object parameters, string email, bool asynchronously = true)
        {
            return await SendEmailMessageAsync(messageTemplate, emailCategory, parameters, new string[1] { email }, asynchronously).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="email">Email</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendEmailMessage(SixnetMessageTemplate messageTemplate, string emailCategory, object parameters, string email, bool asynchronously = true)
        {
            return SendEmailMessageAsync(messageTemplate, emailCategory, parameters, email, asynchronously).Result;
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="message">Message</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="emails">Emails</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendEmailMessageAsync(EmailAccount emailAccount, SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string emailCategory, IEnumerable<string> emails, bool asynchronously = true)
        {
            var msgResult = GetEmailOptions(messageTemplate, message, emailCategory, emails, asynchronously, out var emailOptions);
            if (!msgResult.Success || emailOptions == null)
            {
                return msgResult;
            }
            SendEmailResult emailSendResult = (await SixnetEmailer.SendAsync(emailAccount, emailOptions).ConfigureAwait(false))?.FirstOrDefault();
            return (emailSendResult?.Success ?? false) ? SendMessageResult.SendSuccess() : SendMessageResult.SendFailed(emailSendResult?.Message);
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="message">Message</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="emails">Emails</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendEmailMessage(EmailAccount emailAccount, SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string emailCategory, IEnumerable<string> emails, bool asynchronously = true)
        {
            return SendEmailMessageAsync(emailAccount, messageTemplate, message, emailCategory, emails, asynchronously).Result;
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="emails">Emails</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendEmailMessageAsync(EmailAccount emailAccount, SixnetMessageTemplate messageTemplate, string emailCategory, object parameters, IEnumerable<string> emails, bool asynchronously = true)
        {
            return await SendEmailMessageAsync(emailAccount, messageTemplate, new SixnetMessageInfo()
            {
                Parameters = parameters
            }, emailCategory, emails, asynchronously).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="emails">Emails</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendEmailMessage(EmailAccount emailAccount, SixnetMessageTemplate messageTemplate, string emailCategory, object parameters, IEnumerable<string> emails, bool asynchronously = true)
        {
            return SendEmailMessageAsync(emailAccount, messageTemplate, emailCategory, parameters, emails, asynchronously).Result;
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="email">Email</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendEmailMessageAsync(EmailAccount emailAccount, SixnetMessageTemplate messageTemplate, string emailCategory, object parameters, string email, bool asynchronously = true)
        {
            return await SendEmailMessageAsync(emailAccount, messageTemplate, emailCategory, parameters, new string[1] { email }, asynchronously).ConfigureAwait(false);
        }

        /// <summary>
        /// Send email message
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailCategory">Email category</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="email">Email</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendEmailMessage(EmailAccount emailAccount, SixnetMessageTemplate messageTemplate, string emailCategory, object parameters, string email, bool asynchronously = true)
        {
            return SendEmailMessageAsync(emailAccount, messageTemplate, emailCategory, parameters, email, asynchronously).Result;
        }

        static SendMessageResult GetEmailOptions(SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string emailCategory, IEnumerable<string> emails, bool asynchronously, out EmailInfo sendEmailOptions)
        {
            sendEmailOptions = null;
            if (messageTemplate == null)
            {
                throw new ArgumentNullException(nameof(SixnetMessageTemplate));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(SixnetMessageInfo));
            }
            if (string.IsNullOrWhiteSpace(messageTemplate.Title))
            {
                throw new Exception("The message template title is null or empty");
            }
            if (string.IsNullOrWhiteSpace(messageTemplate.Content))
            {
                throw new Exception("The message template content is null or empty");
            }
            if (emails.IsNullOrEmpty())
            {
                throw new Exception("The emails is null or empty");
            }
            var parameterDict = message.Parameters?.ToStringDictionary();
            Dictionary<string, string> templateParameters = GetTemplateParameters(parameterDict);

            //title
            var titleResolveResult = ResolveTemplate(messageTemplate.Title, templateParameters);
            if (!titleResolveResult.Item1 || string.IsNullOrWhiteSpace(titleResolveResult.Item3))
            {
                if (!string.IsNullOrWhiteSpace(titleResolveResult.Item2))
                {
                    return SendMessageResult.NoParameter($"Not set '{titleResolveResult.Item2}' value in the email title template");
                }
                return SendMessageResult.MessageIsNullOrEmpty($"The email title is null or empty");
            }

            //content
            var contentResolveResult = ResolveTemplate(messageTemplate.Content, templateParameters);
            if (!contentResolveResult.Item1 || string.IsNullOrWhiteSpace(contentResolveResult.Item3))
            {
                if (!string.IsNullOrWhiteSpace(contentResolveResult.Item2))
                {
                    return SendMessageResult.NoParameter($"Not set '{contentResolveResult.Item2}' value in the email body template");
                }
                return SendMessageResult.MessageIsNullOrEmpty($"The email body is null or empty");
            }

            //Send email
            sendEmailOptions = new EmailInfo()
            {
                Category = emailCategory,
                Content = contentResolveResult.Item3,
                Subject = titleResolveResult.Item3,
                Asynchronously = asynchronously,
                Emails = emails
            };
            sendEmailOptions.AddWorkId(message.WorkId);
            sendEmailOptions.AddTemplateMessageId(message.Id);
            return SendMessageResult.SendSuccess();
        }

        #endregion

        #region Send sms message

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="message">Message</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendSmsMessageAsync(SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously = true)
        {
            var msgResult = GetSmsOptions(messageTemplate, message, smsTag, mobiles, asynchronously, out var smsOptions);
            if (!msgResult.Success || smsOptions == null)
            {
                return msgResult;
            }
            SendSmsResult smsResult = await SixnetSms.SendAsync(smsOptions).ConfigureAwait(false);
            return smsResult.Success ? SendMessageResult.SendSuccess() : SendMessageResult.SendFailed(smsResult.Description);
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="message">Message</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendSmsMessage(SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously = true)
        {
            return SendSmsMessageAsync(messageTemplate, message, smsTag, mobiles, asynchronously).Result;
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendSmsMessageAsync(SixnetMessageTemplate messageTemplate, string smsTag, object parameters, IEnumerable<string> mobiles, bool asynchronously = true)
        {
            return await SendSmsMessageAsync(messageTemplate, new SixnetMessageInfo()
            {
                Parameters = parameters?.ToStringDictionary()
            }, smsTag, mobiles, asynchronously).ConfigureAwait(false);
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendSmsMessage(SixnetMessageTemplate messageTemplate, string smsTag, object parameters, IEnumerable<string> mobiles, bool asynchronously = true)
        {
            return SendSmsMessageAsync(messageTemplate, smsTag, parameters, mobiles, asynchronously).Result;
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobile">Mobile</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendSmsMessageAsync(SixnetMessageTemplate messageTemplate, string smsTag, object parameters, string mobile, bool asynchronously = true)
        {
            return await SendSmsMessageAsync(messageTemplate, smsTag, parameters, new string[1] { mobile }, asynchronously).ConfigureAwait(false);
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobile">Mobile</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendSmsMessage(SixnetMessageTemplate messageTemplate, string smsTag, object parameters, string mobile, bool asynchronously = true)
        {
            return SendSmsMessageAsync(messageTemplate, smsTag, parameters, mobile, asynchronously).Result;
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="message">Message</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendSmsMessageAsync(SmsAccount smsAccount, SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously = true)
        {
            var msgResult = GetSmsOptions(messageTemplate, message, smsTag, mobiles, asynchronously, out var smsOptions);
            if (!msgResult.Success || smsOptions == null)
            {
                return msgResult;
            }
            SendSmsResult smsSendResult = await SixnetSms.SendAsync(smsAccount, smsOptions).ConfigureAwait(false);
            return smsSendResult.Success ? SendMessageResult.SendSuccess() : SendMessageResult.SendFailed(smsSendResult.Description);
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="message">Message</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendSmsMessage(SmsAccount smsAccount, SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously = true)
        {
            return SendSmsMessageAsync(smsAccount, messageTemplate, message, smsTag, mobiles, asynchronously).Result;
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobile">Mobiles</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendSmsMessageAsync(SmsAccount smsAccount, SixnetMessageTemplate messageTemplate, string smsTag, object parameters, IEnumerable<string> mobiles, bool asynchronously = true)
        {
            return await SendSmsMessageAsync(smsAccount, messageTemplate, new SixnetMessageInfo()
            {
                Parameters = parameters
            }, smsTag, mobiles, asynchronously).ConfigureAwait(false);
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendSmsMessage(SmsAccount smsAccount, SixnetMessageTemplate messageTemplate, string smsTag, object parameters, IEnumerable<string> mobiles, bool asynchronously = true)
        {
            return SendSmsMessageAsync(smsAccount, messageTemplate, smsTag, parameters, mobiles, asynchronously).Result;
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobile">Mobile</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static async Task<SendMessageResult> SendSmsMessageAsync(SmsAccount smsAccount, SixnetMessageTemplate messageTemplate, string smsTag, object parameters, string mobile, bool asynchronously = true)
        {
            return await SendSmsMessageAsync(smsAccount, messageTemplate, smsTag, parameters, new string[1] { mobile }, asynchronously).ConfigureAwait(false);
        }

        /// <summary>
        /// Send sms message
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="smsTag">Sms tag</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="mobile">Mobile</param>
        /// <param name="asynchronously">Whether send by asynchronously</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendSmsMessage(SmsAccount smsAccount, SixnetMessageTemplate messageTemplate, string smsTag, object parameters, string mobile, bool asynchronously = true)
        {
            return SendSmsMessageAsync(smsAccount, messageTemplate, smsTag, parameters, mobile, asynchronously).Result;
        }

        static SendMessageResult GetSmsOptions(SixnetMessageTemplate messageTemplate, SixnetMessageInfo message, string smsTag, IEnumerable<string> mobiles, bool asynchronously, out SendSmsOptions sendSmsOptions)
        {
            sendSmsOptions = null;
            if (messageTemplate == null)
            {
                throw new ArgumentNullException(nameof(SixnetMessageTemplate));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(SixnetMessageInfo));
            }
            if (string.IsNullOrWhiteSpace(messageTemplate.Content))
            {
                throw new Exception("The message template content is null or empty");
            }
            if (mobiles.IsNullOrEmpty())
            {
                throw new Exception("The mobiles is null or empty");
            }
            var parameterDict = message.Parameters?.ToStringDictionary();
            Dictionary<string, string> templateParameters = GetTemplateParameters(parameterDict);
            //content
            var contentResolveResult = ResolveTemplate(messageTemplate.Content, templateParameters);
            if (!contentResolveResult.Item1 || string.IsNullOrWhiteSpace(contentResolveResult.Item3))
            {
                if (!string.IsNullOrWhiteSpace(contentResolveResult.Item2))
                {
                    return SendMessageResult.NoParameter($"Not set '{contentResolveResult.Item2}' value in the sms body template");
                }
                return SendMessageResult.MessageIsNullOrEmpty($"The sms body is null or empty");
            }

            //Send sms
            sendSmsOptions = new SendSmsOptions()
            {
                Tag = smsTag,
                Content = contentResolveResult.Item3,
                Parameters = parameterDict,
                Asynchronously = asynchronously,
                Mobiles = mobiles,
            };
            sendSmsOptions.AddWorkId(message.WorkId);
            sendSmsOptions.AddTemplateMessageId(message.Id);
            return SendMessageResult.SendSuccess();
        }

        #endregion

        #region Util

        /// <summary>
        /// Get message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <returns>Return message handler</returns>
        public static ISixnetMessageHandler GetMessageHandler(string messageType)
        {
            if (MessageHandlers.IsNullOrEmpty())
            {
                return null;
            }
            if (MessageHandlers.ContainsKey(messageType))
            {
                return MessageHandlers[messageType];
            }
            return null;
        }

        /// <summary>
        /// Resolve
        /// </summary>
        /// <param name="template">Template</param>
        /// <param name="templateParameters">Parameters</param>
        /// <returns>Item1:Resolve success,Item2:Parameter name,Item3:New value</returns>
        static Tuple<bool, string, string> ResolveTemplate(string template, Dictionary<string, string> templateParameters)
        {
            if (template.IsNullOrEmpty())
            {
                return new Tuple<bool, string, string>(false, string.Empty, string.Empty);
            }
            MatchEvaluator matchEvaluator = new MatchEvaluator(c =>
            {
                if (templateParameters?.ContainsKey(c.Value) ?? false)
                {
                    return templateParameters[c.Value];
                }
                return string.Empty;
            });
            MatchCollection matchCollection = KeywordMatchRegex.Matches(template);
            foreach (Match matchVal in matchCollection)
            {
                if (!(templateParameters?.ContainsKey(matchVal.Value) ?? false))
                {
                    return new Tuple<bool, string, string>(false, matchVal.Value, string.Empty);
                }
            }
            string newValue = KeywordMatchRegex.Replace(template, matchEvaluator);
            return new Tuple<bool, string, string>(true, string.Empty, newValue);
        }

        static Dictionary<string, string> GetTemplateParameters(Dictionary<string, string> originalParameters)
        {
            if (originalParameters.IsNullOrEmpty())
            {
                return null;
            }
            var templateParameters = originalParameters;
            if (ParameterNameHandlingFunc != null)
            {
                templateParameters = originalParameters.ToDictionary(c => HandleParameterName(c.Key), c => c.Value);
            }
            return templateParameters;
        }

        #endregion
    }
}
