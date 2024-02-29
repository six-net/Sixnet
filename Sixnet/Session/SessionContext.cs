using System;
using System.Threading;

namespace Sixnet.Session
{
    /// <summary>
    /// Defines session context
    /// </summary>
    public static class SessionContext
    {
        #region Fields

        /// <summary>
        /// Current session info
        /// </summary>
        static readonly AsyncLocal<SixnetSession> current = new();

        /// <summary>
        /// Isolation id key
        /// </summary>
        public static string IsolationIdKey = "sixnet_tnt_id";

        /// <summary>
        /// Isolation code key
        /// </summary>
        public static string IsolationCodeKey = "sixnet_tnt_code";

        /// <summary>
        /// Isolation name key
        /// </summary>
        public static string IsolationNameKey = "sixnet_tnt_name";

        /// <summary>
        /// User id key
        /// </summary>
        public static string UserIdKey = "sixnet_usr_id";

        /// <summary>
        /// User name key
        /// </summary>
        public static string UserNameKey = "sixnet_usr_name";

        /// <summary>
        /// Personal name key
        /// </summary>
        public static string PersonalNameKey = "sixnet_personal_name";

        /// <summary>
        /// User display name key
        /// </summary>
        public static string UserDisplayNameKey = "sixnet_usr_disname";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current session info
        /// </summary>
        public static SixnetSession Current
        {
            get
            {
                return current?.Value;
            }
            internal set
            {
                current.Value = value;
            }
        }

        #endregion

        #region Methods

        #region Create

        /// <summary>
        /// Create session
        /// </summary>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public static SixnetSession Create(Action<SixnetSession> configure = null)
        {
            if (Current != null)
            {
                return Current;
            }
            var newSession = new SixnetSession();
            configure?.Invoke(newSession);
            return newSession;
        }

        #endregion

        #region User

        /// <summary>
        /// Gets the user info
        /// </summary>
        /// <returns></returns>
        public static UserInfo GetUser()
        {
            return current?.Value?.User;
        }

        /// <summary>
        /// Gets the user id
        /// </summary>
        /// <typeparam name="TId">Id type</typeparam>
        /// <returns></returns>
        public static TId GetUserId<TId>()
        {
            var user = GetUser();
            if (user == null)
            {
                return default;
            }
            return user.GetId<TId>();
        }

        #endregion

        #endregion
    }
}
