using EZNEW.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// execute result
    /// </summary>
    public class DomainEventExecuteResult
    {
        #region Propertys

        /// <summary>
        /// success
        /// </summary>
        public bool Success
        {
            get; set;
        }

        /// <summary>
        /// code
        /// </summary>
        public string Code
        {
            get; set;
        }

        /// <summary>
        /// message
        /// </summary>
        public string Message
        {
            get; set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// empty result
        /// </summary>
        /// <returns></returns>
        public static DomainEventExecuteResult EmptyResult(string message = "")
        {
            return CodeResult(DomainEventExecuteResultCode.Empty, message);
        }

        /// <summary>
        /// get execute result by cody
        /// </summary>
        /// <param name="code">result code</param>
        /// <param name="message">message</param>
        /// <returns></returns>
        public static DomainEventExecuteResult CodeResult(DomainEventExecuteResultCode code, string message = "")
        {
            if (message.IsNullOrEmpty())
            {
                message = code.ToString();
            }
            var success = GetSuccessByCode(code);
            return new DomainEventExecuteResult()
            {
                Code = ((int)code).ToString(),
                Message = message,
                Success = success
            };
        }

        /// <summary>
        /// get success by code
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        static bool GetSuccessByCode(DomainEventExecuteResultCode code)
        {
            return code == DomainEventExecuteResultCode.Success;
        }

        #endregion
    }

    /// <summary>
    /// execute result code
    /// </summary>
    public enum DomainEventExecuteResultCode
    {
        Empty = 0,
        Success = 20000
    }
}
