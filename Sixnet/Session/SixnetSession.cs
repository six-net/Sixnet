// "Company © 2025. All rights reserved."

namespace Sixnet.Session
{
    /// <summary>
    /// Defines session data
    /// </summary>
    public class SixnetSession : IDisposable
    {
        /// <summary>
        /// Gets or sets the create date
        /// </summary>
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets or sets the isolation info
        /// </summary>
        public SixnetIsolationInfo Isolation { get; set; }

        /// <summary>
        /// Gets or sets the user info
        /// </summary>
        public SixnetUserInfo User { get; set; }

        /// <summary>
        /// Gets or sets the other info
        /// </summary>
        public object Info { get; set; }

        internal SixnetSession()
        {
            SixnetSessionContext.Current = this;
        }

        public void Dispose()
        {
            SixnetSessionContext.Current = null;
        }
    }
}
