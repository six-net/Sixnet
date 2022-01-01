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
        /// Indicates whether break database command
        /// </summary>
        public bool BreakDatabaseCommand { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        public static StartingResult Success(string message = "", Exception exception = null)
        {
            LogManager.LogDebug<DataCacheBehavior>(FrameworkLogEvents.Cache.CacheOperationError, "Continue database access");
            return new StartingResult()
            {
                BreakDatabaseCommand = false,
                Message = message
            };
        }

        public static StartingResult Break(string message = "", Exception ex = null)
        {
            LogManager.LogDebug<StartingResult>(FrameworkLogEvents.Cache.CacheOperationError, $"Interrupt database access,{message}");
            return new StartingResult()
            {
                BreakDatabaseCommand = true,
                Message = message
            };
        }
    }
}
