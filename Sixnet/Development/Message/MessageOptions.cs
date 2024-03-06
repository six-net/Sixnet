using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Text;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message options
    /// </summary>
    public class MessageOptions
    {
        #region Fields

        /// <summary>
        /// Message handlers
        /// </summary>
        readonly Dictionary<string, ISixnetMessageHandler> _messageHandlers = new();

        /// <summary>
        /// Default parameter name handler
        /// </summary>
        Func<string, string> _handleParameterName = name => $"${name}$";

        /// <summary>
        /// Subject support message types
        /// </summary>
        readonly Dictionary<string, HashSet<string>> _subjectSupportMessageTypes = new();

        #endregion

        #region Properties

        /// <summary>
        /// Keyword match pattern
        /// </summary>
        public string KeywordMatchPattern { get; set; } = @"\$.*?\$";

        /// <summary>
        /// Gets or sets the handle parameter name
        /// </summary>
        public Func<string, string> HandleParameterName
        {
            get => _handleParameterName;
            set => _handleParameterName = value;
        }

        /// <summary>
        /// Gets or sets the message template func
        /// </summary>
        public Func<GetMessageTemplateContext, MessageTemplate> GetMessageTemplate { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Add message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <param name="handler">Handler</param>
        public void AddHandler(string messageType, ISixnetMessageHandler handler)
        {
            if (!string.IsNullOrWhiteSpace(messageType) && handler != null)
            {
                _messageHandlers[messageType] = handler;
            }
        }

        /// <summary>
        /// Configure message handler
        /// </summary>
        /// <typeparam name="THandler">Message handler type</typeparam>
        /// <param name="messageType">Message type</param>
        public void AddHandler<THandler>(string messageType) where THandler : ISixnetMessageHandler, new()
        {
            AddHandler(messageType, new THandler());
        }

        /// <summary>
        /// Get message handler
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <returns></returns>
        public ISixnetMessageHandler GetMessageHandler(string messageType)
        {
            if (string.IsNullOrWhiteSpace(messageType))
            {
                return null;
            }
            _messageHandlers.TryGetValue(messageType, out var handler);
            return handler;
        }

        /// <summary>
        /// Add subject support message type
        /// </summary>
        /// <param name="messageSubject">Message subject</param>
        /// <param name="messageType">Message type</param>
        public void Support(string messageSubject, string messageType)
        {
            if (!string.IsNullOrWhiteSpace(messageSubject) && !string.IsNullOrWhiteSpace(messageType))
            {
                if (_subjectSupportMessageTypes.TryGetValue(messageSubject, out var msgTypes))
                {
                    msgTypes.Add(messageType);
                }
                _subjectSupportMessageTypes[messageSubject] = new HashSet<string> { messageType };
            }
        }

        /// <summary>
        /// Get message subject support message types
        /// </summary>
        /// <param name="messageSubject">Message subject</param>
        /// <returns></returns>
        public HashSet<string> GetSupportMessageTypes(string messageSubject)
        {
            if (string.IsNullOrWhiteSpace(messageSubject))
            {
                return new HashSet<string>(0);
            }
            _subjectSupportMessageTypes.TryGetValue(messageSubject, out var msgTypes);
            return msgTypes ?? new HashSet<string>(0);
        }

        #endregion
    }
}
