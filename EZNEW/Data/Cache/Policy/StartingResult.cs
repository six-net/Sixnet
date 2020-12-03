using EZNEW.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Add data starting policy result
    /// </summary>
    public class StartingResult
    {
        /// <summary>
        /// Whether break database command
        /// </summary>
        public bool BreakDatabaseCommand { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        public static StartingResult Success(string message = "", Exception exception = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                LogManager.LogInformation<StartingResult>(message);
            }
            if (exception != null)
            {
                LogManager.LogError<StartingResult>(exception, exception.Message);
            }
            return new StartingResult()
            {
                BreakDatabaseCommand = false,
                Message = message
            };
        }

        public static StartingResult Break(string message = "", Exception ex = null)
        {
            LogManager.LogError<StartingResult>(ex, message);
            return new StartingResult()
            {
                BreakDatabaseCommand = true,
                Message = message
            };
        }
    }
}
