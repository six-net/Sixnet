using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Sixnet.Token.Jwt
{
    /// <summary>
    /// Jwt configuration
    /// </summary>
    public class JwtConfiguration
    {
        /// <summary>
        /// Token expiration seconds
        /// </summary>
        public int TokenExpirationSeconds { get; set; }

        /// <summary>
        /// Token issuer
        /// </summary>
        public string ValidIssuer { get; set; }

        /// <summary>
        /// Token audience
        /// </summary>
        public string ValidAudience { get; set; }

        /// <summary>
        /// Token issuer signing key
        /// </summary>
        public string IssuerSigningKey { get; set; }

        /// <summary>
        /// Clock skew seconds
        /// </summary>
        public int ClockSkewSeconds { get; set; } = -1;

        /// <summary>
        /// Gets or sets the security algorithms
        /// </summary>
        public string SecurityAlgorithms { get; set; }

        /// <summary>
        /// Default token exp seconds
        /// </summary>
        public const int DEFAULT_TOKEN_EXP_SECONDS = 7200;
    }
}
