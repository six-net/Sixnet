// "Company © 2025. All rights reserved."

using Sixnet.Token.Jwt;

namespace Sixnet.Security.Authentication
{
    /// <summary>
    /// Authentication options
    /// </summary>
    public class SixnetAuthenticationOptions
    {
        /// <summary>
        /// Expire seconds
        /// </summary>
        public int ExpireSeconds { get; set; } = 7200;

        /// <summary>
        /// Jwt token issuer
        /// </summary>
        public string JwtValidIssuer { get; set; } = "http://localhost:5000";

        /// <summary>
        /// Jwt token audience
        /// </summary>
        public string JwtValidAudience { get; set; } = "api";

        /// <summary>
        /// Jwt token issuer signing key
        /// </summary>
        public string JwtIssuerSigningKey { get; set; } = "token.sixnet.net_token.sixnet.net";

        /// <summary>
        /// Jwt clock skew seconds
        /// </summary>
        public int JwtClockSkewSeconds { get; set; } = -1;

        /// <summary>
        /// Jwt token security algorithms
        /// </summary>
        public string JwtSecurityAlgorithms { get; set; }

        /// <summary>
        /// Authentication score
        /// </summary>
        public AuthenticationScore Score { get; set; } = AuthenticationScore.Single;

        /// <summary>
        /// Ignore server validation
        /// </summary>
        public bool IgnoreServerValidation {  get; set; }

        internal JwtSetting GetJwtSetting()
        {
            return new JwtSetting()
            {
                TokenExpirationSeconds = ExpireSeconds,
                ClockSkewSeconds = JwtClockSkewSeconds,
                IssuerSigningKey = JwtIssuerSigningKey,
                Score = Score,
                SecurityAlgorithms = JwtSecurityAlgorithms,
                ValidAudience = JwtValidAudience,
                ValidIssuer = JwtValidIssuer,
            };
        }
    }
}
