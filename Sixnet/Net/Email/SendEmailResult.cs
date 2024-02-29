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
        /// Gets or sets the email info
        /// </summary>
        public EmailInfo EmailInfo { get; set; }

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
        /// <param name="emailInfo">Email info</param>
        /// <returns>Return the email send result</returns>
        public static SendEmailResult FailResult(string message = "", Exception exception = null, EmailInfo emailInfo = null)
        {
            return new SendEmailResult()
            {
                Success = false,
                Exception = exception,
                Message = message,
                EmailInfo = emailInfo
            };
        }

        /// <summary>
        /// Get a success result
        /// </summary>
        /// <param name="emailInfo">Email info</param>
        /// <param name="message">Message</param>
        /// <returns>Return the email send result</returns>
        public static SendEmailResult SuccessResult(EmailInfo emailInfo, string message = "")
        {
            return new SendEmailResult()
            {
                Success = true,
                EmailInfo = emailInfo,
                Message = message,
            };
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
                EmailInfo = EmailInfo?.Clone(),
                EmailAccount = EmailAccount?.Clone()
            };
        }

        #endregion
    }
}
