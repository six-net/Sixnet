// "Company © 2025. All rights reserved."

namespace Sixnet.Security.Authentication
{
    public class SixnetAuthenticationTokenSetting
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

        /// <summary>
        /// Ignore server validation
        /// </summary>
        public bool IgnoreServerValidation { get; set; }

        internal bool IsUnlimited => Score == AuthenticationScore.Unlimited;
    }
}
