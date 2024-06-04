using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Security.Authentication
{
    public class AuthenticationTokenSetting
    {
        /// <summary>
        /// App tag
        /// </summary>
        public string AppTag { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Expire seconds
        /// </summary>
        public int ExpireSeconds { get; set; }

        /// <summary>
        /// Score
        /// </summary>
        public AuthenticationScore Score { get; set; } = AuthenticationScore.Single;

        internal bool IsUnlimited => Score == AuthenticationScore.Unlimited;
    }
}
