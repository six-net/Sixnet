﻿using System;
using System.Collections.Generic;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Defines data access event execution result
    /// </summary>
    [Serializable]
    public class DataAccessEventExecutionResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether event execute successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the status code
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
        /// <returns>Return data access event result</returns>
        public static DataAccessEventExecutionResult EmptyResult(string message = "")
        {
            return CodeResult(DataAccessEventResultCode.Empty, message);
        }

        /// <summary>
        /// Gets result by cody
        /// </summary>
        /// <param name="code">Result code</param>
        /// <param name="message">Message</param>
        /// <returns>Return data access event result</returns>
        public static DataAccessEventExecutionResult CodeResult(DataAccessEventResultCode code, string message = "")
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = code.ToString();
            }
            var success = GetSuccessByCode(code);
            return new DataAccessEventExecutionResult()
            {
                Code = ((int)code).ToString(),
                Message = message,
                Success = success
            };
        }

        /// <summary>
        /// Gets success by code
        /// </summary>
        /// <param name="code">Code</param>
        /// <returns>Return whether success by code</returns>
        static bool GetSuccessByCode(DataAccessEventResultCode code)
        {
            return code == DataAccessEventResultCode.Success;
        }

        #endregion
    }

    /// <summary>
    /// Execute result code
    /// </summary>
    [Serializable]
    public enum DataAccessEventResultCode
    {
        Empty = 0,
        Success = 200
    }
}
