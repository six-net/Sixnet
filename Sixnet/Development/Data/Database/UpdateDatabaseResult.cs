// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Update database result
    /// </summary>
    public class UpdateDatabaseResult
    {
        /// <summary>
        /// Success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        public static UpdateDatabaseResult SuccessResult(string message)
        {
            return new UpdateDatabaseResult() 
            {
                Success = true,
                Message = message
            };
        }

        public static UpdateDatabaseResult FailedResult(string message)
        {
            return new UpdateDatabaseResult()
            {
                Success = false,
                Message = message
            };
        }
    }
}
