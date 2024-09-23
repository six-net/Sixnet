using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sixnet.App;
using Sixnet.DependencyInjection;
using Sixnet.Exceptions;
using Sixnet.Security.Authentication;
using Sixnet.Session;

namespace Sixnet.Token.Jwt
{
    /// <summary>
    /// Jwt token
    /// </summary>
    public class JwtToken
    {
        #region Properties

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the refresh token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the expiry(Seconds)
        /// </summary>
        public int ExpSeconds { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create token core
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="jwtOptions"></param>
        /// <returns></returns>
        static JwtToken CreateTokenCore(IEnumerable<Claim> claims, JwtOptions jwtOptions)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey));
            var signingCredentials = new SigningCredentials(signingKey, string.IsNullOrWhiteSpace(jwtOptions.SecurityAlgorithms) ? SecurityAlgorithms.HmacSha256Signature : jwtOptions.SecurityAlgorithms);

            //access token
            var authTime = DateTime.UtcNow;
            int accessTokenExpirSeconds = jwtOptions.TokenExpirationSeconds;
            accessTokenExpirSeconds = accessTokenExpirSeconds < 1 ? JwtOptions.DEFAULT_TOKEN_EXP_SECONDS : accessTokenExpirSeconds;
            var accessTokenExpiresAt = authTime.AddSeconds(accessTokenExpirSeconds);
            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = accessTokenExpiresAt,
                NotBefore = authTime,
                IssuedAt = authTime,
                Issuer = jwtOptions.ValidIssuer,
                Audience = jwtOptions.ValidAudience,
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims)
            };
            var userInfo = UserInfo.GetUserFromClaims(claims);
            SixnetAuthenticationManager.SetAuthenticationToken(setting =>
            {
                setting.AppTag = userInfo.AppTag;
                setting.Token = userInfo.Token;
                setting.UserId = userInfo.Id;
            });
            var accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(accessTokenDescriptor));

            // refresh token
            accessTokenDescriptor.Expires = accessTokenExpiresAt.AddSeconds(accessTokenExpirSeconds);
            accessTokenDescriptor.Claims = new Dictionary<string, object>()
            {
                { JwtClaimTypes.Role, "refresh" }
            };
            var refreshToken = tokenHandler.WriteToken(tokenHandler.CreateToken(accessTokenDescriptor));

            return new JwtToken()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpSeconds = accessTokenExpirSeconds
            };
        }

        /// <summary>
        /// Create jwt token
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="configure">Configure jwt options</param>
        /// <returns></returns>
        public static JwtToken CreateToken(IEnumerable<Claim> claims, Action<JwtOptions> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(claims == null, nameof(claims));

            var jwtOptions = SixnetContainer.GetOptions<JwtOptions>();
            configure?.Invoke(jwtOptions);
            SixnetDirectThrower.ThrowArgNullIf(jwtOptions == null, nameof(jwtOptions));

            return CreateTokenCore(claims, jwtOptions);
        }

        /// <summary>
        /// Create token
        /// </summary>
        /// <param name="user">User info</param>
        /// <param name="configure">Configure jwt options</param>
        /// <returns></returns>
        public static JwtToken CreateToken(UserInfo user, Action<JwtOptions> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(user == null, nameof(user));
            return CreateToken(user.GetClaims(), configure);
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <returns></returns>
        public JwtToken Refresh(Action<JwtOptions> configure = null)
        {
            var jwtOptions = SixnetContainer.GetOptions<JwtOptions>();
            configure?.Invoke(jwtOptions);
            SixnetDirectThrower.ThrowArgNullIf(jwtOptions == null, nameof(jwtOptions));

            SixnetDirectThrower.ThrowArgErrorIf(string.IsNullOrWhiteSpace(AccessToken), "Access token is null or empty");

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SixnetDirectThrower.ThrowNotSupportIf(!jwtSecurityTokenHandler.CanReadToken(AccessToken), "Access token is fault");

            // Validate parameter
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey));
            var validateParameter = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.ValidIssuer,
                ValidAudience = jwtOptions.ValidAudience,
                IssuerSigningKey = signingKey
            };

            // Validate access token
            jwtSecurityTokenHandler.ValidateToken(AccessToken, validateParameter, out var validatedToken);

            // Create new access token
            var jwtToken = validatedToken as JwtSecurityToken;
            var tokenUser = UserInfo.GetUserFromClaims(jwtToken.Claims);
            return CreateTokenCore(tokenUser.GetClaims(), jwtOptions);
        }

        #endregion
    }
}
