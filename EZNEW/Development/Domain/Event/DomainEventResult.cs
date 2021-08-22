using System;
using System.Collections.Generic;

namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Domain event result
    /// </summary>
    [Serializable]
    public class DomainEventResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether execute successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the result code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a empty result
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Return domain event result</returns>
        public static DomainEventResult EmptyResult(string message = "")
        {
            return CodeResult(DomainEventExecuteResultCode.Empty, message);
        }

        /// <summary>
        /// Gets a execute result by code
        /// </summary>
        /// <param name="code">Result code</param>
        /// <param name="message">Message</param>
        /// <returns>Return domain event result</returns>
        public static DomainEventResult CodeResult(DomainEventExecuteResultCode code, string message = "")
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = code.ToString();
            }
            var success = GetSuccessByCode(code);
            return new DomainEventResult()
            {
                Code = ((int)code).ToString(),
                Message = message,
                Success = success
            };
        }

        /// <summary>
        /// Get a success result by code
        /// </summary>
        /// <param name="code">Code</param>
        /// <returns></returns>
        static bool GetSuccessByCode(DomainEventExecuteResultCode code)
        {
            return code == DomainEventExecuteResultCode.Success;
        }

        #endregion
    }
}
