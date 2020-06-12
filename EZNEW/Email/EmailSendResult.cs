using System;

namespace EZNEW.Email
{
    /// <summary>
    /// Email send result
    /// </summary>
    [Serializable]
    public class EmailSendResult
    {
        /// <summary>
        /// Empty email send result
        /// </summary>
        public static readonly EmailSendResult Empty = new EmailSendResult() { Message = "Empty email send result" };

        /// <summary>
        /// Gets or sets whether the email send successful
        /// </summary>
        public bool Success
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the result message
        /// </summary>
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the send info
        /// </summary>
        public EmailSendInfo SendInfo
        {
            get; set;
        }

        /// <summary>
        /// Get a fail result
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="exception">Exception</param>
        /// <param name="sendInfo">Email send info</param>
        /// <returns>Return the email send result</returns>
        public static EmailSendResult FailResult(string message = "", Exception exception = null, EmailSendInfo sendInfo = null)
        {
            return new EmailSendResult()
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
        public static EmailSendResult SuccessResult(EmailSendInfo sendInfo)
        {
            return new EmailSendResult()
            {
                Success = true,
                SendInfo = sendInfo
            };
        }
    }
}
