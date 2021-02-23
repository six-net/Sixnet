using System;

namespace EZNEW.Email
{
    /// <summary>
    /// Send email result
    /// </summary>
    [Serializable]
    public class SendEmailResult
    {
        /// <summary>
        /// Empty email result
        /// </summary>
        public static readonly SendEmailResult Empty = new SendEmailResult() { Message = "Empty email send result" };

        /// <summary>
        /// Gets or sets whether the email send successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the result message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the send info
        /// </summary>
        public SendEmailOptions SendInfo { get; set; }

        /// <summary>
        /// Gets or sets the email account
        /// </summary>
        public EmailAccount EmailAccount { get; set; }

        /// <summary>
        /// Get a fail result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="sendInfo">Email send info</param>
        /// <returns>Return the email send result</returns>
        public static SendEmailResult FailResult(string message = "", Exception exception = null, SendEmailOptions sendInfo = null)
        {
            return new SendEmailResult()
            {
                Success = false,
                Exception = exception,
                Message = message,
                SendInfo = sendInfo
            };
        }

        /// <summary>
        /// Get a success result
        /// </summary>
        /// <param name="sendInfo">Email send info</param>
        /// <returns>Return the email send result</returns>
        public static SendEmailResult SuccessResult(SendEmailOptions sendInfo)
        {
            return new SendEmailResult()
            {
                Success = true,
                SendInfo = sendInfo
            };
        }

        internal SendEmailResult Clone()
        {
            return new SendEmailResult()
            {
                Success = Success,
                Exception = Exception,
                Message = Message,
                SendInfo = SendInfo?.Clone(),
                EmailAccount = EmailAccount?.Clone()
            };
        }
    }
}
