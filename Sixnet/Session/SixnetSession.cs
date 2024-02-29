using System;

namespace Sixnet.Session
{
    /// <summary>
    /// Defines session data
    /// </summary>
    public class SixnetSession : IDisposable
    {
        /// <summary>
        /// Gets or sets the isolation info
        /// </summary>
        public IsolationInfo Isolation { get; set; }

        /// <summary>
        /// Gets or sets the user info
        /// </summary>
        public UserInfo User { get; set; }

        internal SixnetSession()
        {
            SessionContext.Current = this;
        }

        public void Dispose()
        {
            SessionContext.Current = null;
        }
    }
}
