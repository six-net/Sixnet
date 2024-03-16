using Sixnet.Net.Sms;
using System;
using System.Collections.Generic;

namespace Sixnet.Development.Message
{
    /// <summary>
    /// Message options
    /// </summary>
    public class MessageOptions
    {
        #region Fields

        /// <summary>
        /// Default parameter name handler
        /// </summary>
        Func<string, string> _handleParameterName = name => $"${name}$";

        /// <summary>
        /// Generate message id
        /// </summary>
        Func<string> _getMessageId = () => Guid.NewGuid().ToString();

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
        /// Gets or sets the get message id
        /// </summary>
        public Func<string> GetMessageId
        {
            set
            {
                if (value != null)
                {
                    _getMessageId = value;
                }
            }
            get => _getMessageId;
        }

        #endregion
    }
}
