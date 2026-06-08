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
    public class SixnetUpdateDatabaseResult
    {
        /// <summary>
        /// Success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        public static SixnetUpdateDatabaseResult SuccessResult(string message)
        {
            return new SixnetUpdateDatabaseResult() 
            {
                Success = true,
                Message = message
            };
        }

        public static SixnetUpdateDatabaseResult FailedResult(string message)
        {
            return new SixnetUpdateDatabaseResult()
            {
                Success = false,
                Message = message
            };
        }
    }
}
