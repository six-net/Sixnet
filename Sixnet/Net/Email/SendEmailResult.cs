using System;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Send email result
    /// </summary>
    [Serializable]
    public class SendEmailResult
    {
        #region Properties

        /// <summary>
        /// Indicates whether the email send successful
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

        #endregion

        #region Methods

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
        /// <param name="message">Message</param>
        /// <returns>Return the email send result</returns>
        public static SendEmailResult SuccessResult(SendEmailOptions sendInfo, string message = "")
        {
            return new SendEmailResult()
            {
                Success = true,
                SendInfo = sendInfo,
                Message = message,
            };
        }

        /// <summary>
        /// Get an empty result
        /// </summary>
        /// <returns></returns>
        public static SendEmailResult Empty()
        {
            return SuccessResult(null, "Empty email send result");
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
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

        #endregion
    }
}
