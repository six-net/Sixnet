// "Company © 2025. All rights reserved."

using Sixnet.Development.Data;

namespace Sixnet.Development.Work
{
    /// <summary>
    /// Unit of work setting
    /// </summary>
    public class SixnetUnitOfWorkSetting
    {
        /// <summary>
        /// Gets or sets the isolation level
        /// </summary>
        public SixnetDataIsolationLevel? IsolationLevel { get; set; }

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
        internal static SixnetUnitOfWorkSetting GetDefaultSetting()
        {
            return new SixnetUnitOfWorkSetting();
        }
    }
}
