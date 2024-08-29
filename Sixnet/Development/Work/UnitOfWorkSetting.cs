using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data;

namespace Sixnet.Development.Work
{
    /// <summary>
    /// Unit of work setting
    /// </summary>
    public class UnitOfWorkSetting
    {
        /// <summary>
        /// Gets or sets the isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Whether auto commit work
        /// </summary>
        public bool AutoCommit { get; set; }

        /// <summary>
        /// Whether not retry
        /// </summary>
        public bool NotRetry { get; set; }

        /// <summary>
        /// Gets or sets the retry times
        /// </summary>
        public int? RetryTimes { get; set; }

        /// <summary>
        /// Get default setting
        /// </summary>
        /// <returns></returns>
        internal static UnitOfWorkSetting GetDefaultSetting()
        {
            return new UnitOfWorkSetting();
        }
    }
}
