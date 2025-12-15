// "Company © 2025. All rights reserved."

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;

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
        /// <param name="jwtSetting"></param>
        /// <returns></returns>
        static JwtToken CreateTokenCore(IEnumerable<Claim> claims, JwtSetting jwtSetting)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.IssuerSigningKey));
            var signingCredentials = new SigningCredentials(signingKey, string.IsNullOrWhiteSpace(jwtSetting.SecurityAlgorithms) ? SecurityAlgorithms.HmacSha256Signature : jwtSetting.SecurityAlgorithms);

            //access token
            var authTime = DateTime.UtcNow;
            int accessTokenExpirSeconds = jwtSetting.TokenExpirationSeconds;
            accessTokenExpirSeconds = accessTokenExpirSeconds < 1 ? JwtSetting.DEFAULT_TOKEN_EXP_SECONDS : accessTokenExpirSeconds;
            var accessTokenExpiresAt = authTime.AddSeconds(accessTokenExpirSeconds);
            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = accessTokenExpiresAt,
                NotBefore = authTime,
                IssuedAt = authTime,
                Issuer = jwtSetting.ValidIssuer,
                Audience = jwtSetting.ValidAudience,
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims)
            };
            var userInfo = UserInfo.GetUserFromClaims(claims);
            SixnetAuthenticationManager.SetAuthenticationToken(setting =>
            {
                setting.AppTag = userInfo.AppTag;
                setting.Token = userInfo.Token;
                setting.UserId = userInfo.Id;
                setting.Score = jwtSetting.Score;
                setting.ExpireSeconds = jwtSetting.TokenExpirationSeconds;
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
        /// Create token core
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="jwtSetting"></param>
        /// <returns></returns>
        static async Task<JwtToken> CreateTokenCoreAsync(IEnumerable<Claim> claims, JwtSetting jwtSetting)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.IssuerSigningKey));
            var signingCredentials = new SigningCredentials(signingKey, string.IsNullOrWhiteSpace(jwtSetting.SecurityAlgorithms) ? SecurityAlgorithms.HmacSha256Signature : jwtSetting.SecurityAlgorithms);

            //access token
            var authTime = DateTime.UtcNow;
            int accessTokenExpirSeconds = jwtSetting.TokenExpirationSeconds;
            accessTokenExpirSeconds = accessTokenExpirSeconds < 1 ? JwtSetting.DEFAULT_TOKEN_EXP_SECONDS : accessTokenExpirSeconds;
            var accessTokenExpiresAt = authTime.AddSeconds(accessTokenExpirSeconds);
            var accessTokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = accessTokenExpiresAt,
                NotBefore = authTime,
                IssuedAt = authTime,
                Issuer = jwtSetting.ValidIssuer,
                Audience = jwtSetting.ValidAudience,
                SigningCredentials = signingCredentials,
                Subject = new ClaimsIdentity(claims)
            };
            var userInfo = UserInfo.GetUserFromClaims(claims);
            await SixnetAuthenticationManager.SetAuthenticationTokenAsync(setting =>
            {
                setting.AppTag = userInfo.AppTag;
                setting.Token = userInfo.Token;
                setting.UserId = userInfo.Id;
                setting.Score = jwtSetting.Score;
                setting.ExpireSeconds = jwtSetting.TokenExpirationSeconds;
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
        /// <param name="configure">Configure jwt setting</param>
        /// <returns></returns>
        public static JwtToken Create(IEnumerable<Claim> claims, Action<JwtSetting> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(claims == null, nameof(claims));

            var jwtSetting = SixnetContainer.GetOptions<SixnetAuthenticationOptions>()?.GetJwtSetting() ?? new JwtSetting();
            configure?.Invoke(jwtSetting);
            SixnetDirectThrower.ThrowArgNullIf(jwtSetting == null, nameof(jwtSetting));

            return CreateTokenCore(claims, jwtSetting);
        }

        /// <summary>
        /// Create jwt token
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="configure">Configure jwt setting</param>
        /// <returns></returns>
        public static async Task<JwtToken> CreateAsync(IEnumerable<Claim> claims, Action<JwtSetting> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(claims == null, nameof(claims));

            var jwtSetting = SixnetContainer.GetOptions<SixnetAuthenticationOptions>()?.GetJwtSetting() ?? new JwtSetting();
            configure?.Invoke(jwtSetting);
            SixnetDirectThrower.ThrowArgNullIf(jwtSetting == null, nameof(jwtSetting));

            return await CreateTokenCoreAsync(claims, jwtSetting);
        }

        /// <summary>
        /// Create token
        /// </summary>
        /// <param name="user">User info</param>
        /// <param name="configure">Configure jwt options</param>
        /// <returns></returns>
        public static JwtToken Create(UserInfo user, Action<JwtSetting> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(user == null, nameof(user));
            return Create(user.GetClaims(), configure);
        }

        /// <summary>
        /// Create token
        /// </summary>
        /// <param name="user">User info</param>
        /// <param name="configure">Configure jwt options</param>
        /// <returns></returns>
        public static async Task<JwtToken> CreateAsync(UserInfo user, Action<JwtSetting> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(user == null, nameof(user));
            return await CreateAsync(user.GetClaims(), configure);
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <returns></returns>
        public JwtToken Refresh(Action<JwtSetting> configure = null)
        {
            var jwtSetting = SixnetContainer.GetOptions<SixnetAuthenticationOptions>()?.GetJwtSetting() ?? new JwtSetting();
            configure?.Invoke(jwtSetting);
            SixnetDirectThrower.ThrowArgNullIf(jwtSetting == null, nameof(jwtSetting));

            SixnetDirectThrower.ThrowArgErrorIf(string.IsNullOrWhiteSpace(AccessToken), "Access token is null or empty");

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SixnetDirectThrower.ThrowNotSupportIf(!jwtSecurityTokenHandler.CanReadToken(AccessToken), "Access token is fault");

            // Validate parameter
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.IssuerSigningKey));
            var validateParameter = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSetting.ValidIssuer,
                ValidAudience = jwtSetting.ValidAudience,
                IssuerSigningKey = signingKey
            };

            // Validate access token
            jwtSecurityTokenHandler.ValidateToken(AccessToken, validateParameter, out var validatedToken);

            // Create new access token
            var jwtToken = validatedToken as JwtSecurityToken;
            var tokenUser = UserInfo.GetUserFromClaims(jwtToken.Claims);
            return CreateTokenCore(tokenUser.GetClaims(), jwtSetting);
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <returns></returns>
        public async Task<JwtToken> RefreshAsync(Action<JwtSetting> configure = null)
        {
            var jwtSetting = SixnetContainer.GetOptions<SixnetAuthenticationOptions>()?.GetJwtSetting() ?? new JwtSetting();
            configure?.Invoke(jwtSetting);
            SixnetDirectThrower.ThrowArgNullIf(jwtSetting == null, nameof(jwtSetting));

            SixnetDirectThrower.ThrowArgErrorIf(string.IsNullOrWhiteSpace(AccessToken), "Access token is null or empty");

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SixnetDirectThrower.ThrowNotSupportIf(!jwtSecurityTokenHandler.CanReadToken(AccessToken), "Access token is fault");

            // Validate parameter
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.IssuerSigningKey));
            var validateParameter = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSetting.ValidIssuer,
                ValidAudience = jwtSetting.ValidAudience,
                IssuerSigningKey = signingKey
            };

            // Validate access token
            jwtSecurityTokenHandler.ValidateToken(AccessToken, validateParameter, out var validatedToken);

            // Create new access token
            var jwtToken = validatedToken as JwtSecurityToken;
            var tokenUser = UserInfo.GetUserFromClaims(jwtToken.Claims);
            return await CreateTokenCoreAsync(tokenUser.GetClaims(), jwtSetting);
        }

        /// <summary>
        /// Destory token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="configure"></param>
        public static void Destroy(UserInfo user, Action<JwtSetting> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(user == null, nameof(user));

            var jwtSetting = SixnetContainer.GetOptions<SixnetAuthenticationOptions>()?.GetJwtSetting() ?? new JwtSetting();
            configure?.Invoke(jwtSetting);
            SixnetDirectThrower.ThrowArgNullIf(jwtSetting == null, nameof(jwtSetting));

            SixnetAuthenticationManager.RemoveAuthenticationToken(setting =>
            {
                setting.AppTag = user.AppTag;
                setting.Token = user.Token;
                setting.UserId = user.Id;
                setting.Score = jwtSetting.Score;
                setting.ExpireSeconds = jwtSetting.TokenExpirationSeconds;
            });
        }

        /// <summary>
        /// Destory token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="configure"></param>
        public static async Task DestroyAsync(UserInfo user, Action<JwtSetting> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(user == null, nameof(user));

            var jwtSetting = SixnetContainer.GetOptions<SixnetAuthenticationOptions>()?.GetJwtSetting() ?? new JwtSetting();
            configure?.Invoke(jwtSetting);
            SixnetDirectThrower.ThrowArgNullIf(jwtSetting == null, nameof(jwtSetting));

            await SixnetAuthenticationManager.RemoveAuthenticationTokenAsync(setting =>
            {
                setting.AppTag = user.AppTag;
                setting.Token = user.Token;
                setting.UserId = user.Id;
                setting.Score = jwtSetting.Score;
                setting.ExpireSeconds = jwtSetting.TokenExpirationSeconds;
            });
        }

        /// <summary>
        /// Destory current user token
        /// </summary>
        /// <param name="configure"></param>
        public static void DestroyCurrentUser(Action<JwtSetting> configure = null)
        {
            var currentUser = SessionContext.Current?.User;
            if (currentUser != null)
            {
                Destroy(SessionContext.Current?.User, configure);
            }
        }

        /// <summary>
        /// Destory current user token
        /// </summary>
        /// <param name="configure"></param>
        public static async Task DestroyCurrentUserAsync(Action<JwtSetting> configure = null)
        {
            var currentUser = SessionContext.Current?.User;
            if (currentUser != null)
            {
                await DestroyAsync(SessionContext.Current?.User, configure);
            }
        }

        #endregion
    }
}
