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

        #region Send

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns></returns>
        public static void Send(IEnumerable<MessageInfo> messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(messages.IsNullOrEmpty(), nameof(messages));

            var imdiateMessages = new List<MessageInfo>();
            var workMessages = new List<MessageInfo>();
            foreach (var msg in messages)
            {
                if (msg.SendTime == MessageSendTime.Immediately)
                {
                    imdiateMessages.Add(msg);
                }
                else
                {
                    workMessages.Add(msg);
                }
            }

            if (!imdiateMessages.IsNullOrEmpty())
            {
                var parameter = new SendMessageParameter()
                {
                    Messages = imdiateMessages
                };
                var messageProvider = GetMessageProvider();
                messageProvider.Send(parameter);
            }

            if (!workMessages.IsNullOrEmpty())
            {
                SixnetDirectThrower.ThrowArgNullIf(MessageBox == null, $"Please call {nameof(Init)} method first to initialize the message box");
                MessageBox.Store(workMessages);
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="subject">Message subject</param>
        /// <param name="data">Message data</param>
        /// <param name="sendTime">Send time</param>
        public static void Send(string subject, object data, MessageSendTime sendTime = MessageSendTime.WorkCompleted)
        {
            Send(new List<MessageInfo>(1) {
                new MessageInfo()
                {
                    Subject = subject,
                    Data = data,
                    SendTime = sendTime
                }
            });
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns></returns>
        public static Task SendAsync(IEnumerable<MessageInfo> messages)
        {
            SixnetDirectThrower.ThrowArgNullIf(messages.IsNullOrEmpty(), nameof(messages));

            var imdiateMessages = new List<MessageInfo>();
            var workMessages = new List<MessageInfo>();
            foreach (var msg in messages)
            {
                if (msg.SendTime == MessageSendTime.Immediately)
                {
                    imdiateMessages.Add(msg);
                }
                else
                {
                    workMessages.Add(msg);
                }
            }

            Task sendTask = null;

            if (!workMessages.IsNullOrEmpty())
            {
                SixnetDirectThrower.ThrowArgNullIf(MessageBox == null, $"Please call {nameof(Init)} method first to initialize the message box");
                MessageBox.Store(workMessages);
            }
            if (!imdiateMessages.IsNullOrEmpty())
            {
                var parameter = new SendMessageParameter()
                {
                    Messages = imdiateMessages
                };
                var messageProvider = GetMessageProvider();
                sendTask = messageProvider.SendAsync(parameter);
            }
            return sendTask ?? Task.CompletedTask;
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="subject">Message subject</param>
        /// <param name="data">Message data</param>
        /// <param name="sendTime">Send time</param>
        public static Task SendAsync(string subject, object data, MessageSendTime sendTime = MessageSendTime.WorkCompleted)
        {
            return SendAsync(new List<MessageInfo>(1) {
                new MessageInfo()
                {
                    Subject = subject,
                    Data = data,
                    SendTime = sendTime
                }
            });
        }

        #endregion

        #region Commit

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
            _ = messageProvider.SendAsync(new SendMessageParameter()
            {
                Messages = new List<MessageInfo>(MessageBox.Messages)
            });
            MessageBox.Clear();
        }

        #endregion

        #region Clear

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
        /// Get keyword match regex
        /// </summary>
        /// <returns></returns>
        static Regex GetKeywordMatchRegex()
        {
            var options = GetMessageOptions();
            return new Regex(options.KeywordMatchPattern);
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
