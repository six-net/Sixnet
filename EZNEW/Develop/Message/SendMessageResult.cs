using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Develop.Message
{
    /// <summary>
    /// Send message result
    /// </summary>
    public class SendMessageResult
    {
        /// <summary>
        /// success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// result type
        /// </summary>
        public MessageResultType ResultType { get; set; }

        /// <summary>
        /// message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the inner results
        /// </summary>
        public List<SendMessageResult> InnerResults { get; } = new List<SendMessageResult>();

        /// <summary>
        /// Return none message provider result
        /// </summary>
        /// <returns>Return send result</returns>
        public static SendMessageResult NoProvider()
        {
            return new SendMessageResult()
            {
                Success = false,
                Message = "No message provider",
                Code = ((int)MessageResultType.NoMessageProvider).ToString(),
                ResultType = MessageResultType.NoMessageProvider
            };
        }

        /// <summary>
        /// Return unknown message result
        /// </summary>
        /// <param name="message">Result message</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult UnknownMessageType(string message = "")
        {
            return new SendMessageResult()
            {
                Success = false,
                Message = message,
                Code = ((int)MessageResultType.UnknownMessageType).ToString(),
                ResultType = MessageResultType.UnknownMessageType
            };
        }

        /// <summary>
        /// Return no message template result
        /// </summary>
        /// <param name="message">Result message</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult NoTemplate(string message = "")
        {
            return new SendMessageResult()
            {
                Success = false,
                Message = message,
                Code = ((int)MessageResultType.NoMessageTemplate).ToString(),
                ResultType = MessageResultType.NoMessageTemplate
            };
        }

        /// <summary>
        /// Return no message options result
        /// </summary>
        /// <param name="message">Result message</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult NoOptions(string message = "")
        {
            return new SendMessageResult()
            {
                Success = false,
                Message = message,
                Code = ((int)MessageResultType.SendOptionsIsNull).ToString(),
                ResultType = MessageResultType.SendOptionsIsNull
            };
        }

        /// <summary>
        /// Return no template parameter value result
        /// </summary>
        /// <param name="message">Result message</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult NoParameter(string message = "")
        {
            return new SendMessageResult()
            {
                Success = false,
                Message = message,
                Code = ((int)MessageResultType.NotSetTemplateParameterValue).ToString(),
                ResultType = MessageResultType.NotSetTemplateParameterValue
            };
        }

        /// <summary>
        /// Return no message receiver result
        /// </summary>
        /// <param name="message">Result message</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult NoReceiver(string message = "")
        {
            return new SendMessageResult()
            {
                Success = false,
                Message = message,
                Code = ((int)MessageResultType.NoReceiver).ToString(),
                ResultType = MessageResultType.NoReceiver
            };
        }

        /// <summary>
        /// Return no message handler result
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult NoHandler(string message = "")
        {
            return new SendMessageResult()
            {
                Success = false,
                Message = message,
                Code = ((int)MessageResultType.NoHandler).ToString(),
                ResultType = MessageResultType.NoHandler
            };
        }

        /// <summary>
        /// Return null message result
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns></returns>
        public static SendMessageResult MessageIsNullOrEmpty(string message = "")
        {
            return new SendMessageResult()
            {
                Success = false,
                Message = message,
                Code = ((int)MessageResultType.MessageIsNullOrEmpty).ToString(),
                ResultType = MessageResultType.MessageIsNullOrEmpty
            };
        }

        /// <summary>
        /// Return send failed result
        /// </summary>
        /// <param name="message">Result message</param>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendFailed(string message = "")
        {
            return new SendMessageResult()
            {
                Success = false,
                Message = message,
                Code = ((int)MessageResultType.Failed).ToString(),
                ResultType = MessageResultType.Failed
            };
        }

        /// <summary>
        /// Return send success result
        /// </summary>
        /// <returns>Return send result</returns>
        public static SendMessageResult SendSuccess()
        {
            return new SendMessageResult()
            {
                Success = true,
                Code = ((int)MessageResultType.Success).ToString(),
                ResultType = MessageResultType.Success
            };
        }

        /// <summary>
        /// Add inner results
        /// </summary>
        /// <param name="results">results</param>
        /// <returns></returns>
        public SendMessageResult Add(params SendMessageResult[] results)
        {
            if (results.IsNullOrEmpty())
            {
                return this;
            }
            Success = !results.Any(r => !r.Success);
            InnerResults.AddRange(results);
            return this;
        }
    }
}
