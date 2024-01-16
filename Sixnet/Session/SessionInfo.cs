using System;

namespace Sixnet.Session
{
    /// <summary>
    /// Defines default session
    /// </summary>
    public class SessionInfo : IDisposable
    {
        /// <summary>
        /// Gets or sets the isolation data
        /// </summary>
        public IsolationData IsolationData { get; set; }

        /// <summary>
        /// Gets or sets the user info
        /// </summary>
        public UserInfo User { get; set; }

        internal SessionInfo()
        {
            SessionContext.Current = this;
        }

        public void Dispose()
        {
            SessionContext.Current = null;
        }
    }
}
