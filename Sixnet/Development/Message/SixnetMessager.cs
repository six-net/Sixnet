using System;
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
