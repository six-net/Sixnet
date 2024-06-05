using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Sixnet.Token.Jwt;

namespace Sixnet.Session
{
    /// <summary>
    /// Defines user info
    /// </summary>
    public class UserInfo : IIdentity
    {
        #region Fields

        /// <summary>
        /// Admin tag key
        /// </summary>
        const string ADMIN_TAG_KEY = "ssupadm";

        /// <summary>
        /// Virtual user tag key
        /// </summary>
        const string VIRTUAL_TAG_KEY = "svirusr";

        /// <summary>
        /// Relation user tag key
        /// </summary>
        const string RELATIONUSER_TAG_KEY = "sralusr";

        /// <summary>
        /// Application tag key
        /// </summary>
        const string APPLICATION_TAG_KEY = "sapptg";

        /// <summary>
        /// Token tag key
        /// </summary>
        const string TOKEN_TAG_KEY = "sauentkn";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the authentication type
        /// </summary>
        public string AuthenticationType => string.Empty;

        /// <summary>
        /// Gets or sets whether is authenticated
        /// </summary>
        public bool IsAuthenticated => true;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the person name
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets whether is admin
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Indecates whether is virtual user
        /// </summary>
        public bool IsVirual { get; set; }

        /// <summary>
        /// Gets or sets the relation user id
        /// </summary>
        public string RelationUserId { get; set; }

        /// <summary>
        /// Gets or sets the app tag
        /// </summary>
        public string AppTag { get; set; }

        /// <summary>
        /// Gets or sets the token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the roles
        /// </summary>
        public HashSet<string> Roles { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Get user info from principal
        /// </summary>
        /// <param name="principal">Principal</param>
        /// <returns>Return authentication user</returns>
        public static UserInfo GetUserFromPrincipal(ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                return null;
            }
            return GetUserFromClaims(principal.Claims);
        }

        /// <summary>
        /// Get user info from claims
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <returns>Return authentication user</returns>
        public static UserInfo GetUserFromClaims(IEnumerable<Claim> claims)
        {
            if (claims.IsNullOrEmpty())
            {
                return null;
            }
            var idClaim = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);
            idClaim ??= claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            var nameClaim = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name);
            nameClaim ??= claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            var givenNameClaim = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName);
            givenNameClaim ??= claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);

            var nickNameClaim = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NickName);
            nickNameClaim ??= claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);

            var roleClaim = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Role);
            roleClaim ??= claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            var adminClaim = claims.FirstOrDefault(c => c.Type == ADMIN_TAG_KEY);
            var virtualClaim = claims.FirstOrDefault(c => c.Type == VIRTUAL_TAG_KEY);
            var relationUserClaim = claims.FirstOrDefault(c => c.Type == RELATIONUSER_TAG_KEY);
            var appTagClaim = claims.FirstOrDefault(c => c.Type == APPLICATION_TAG_KEY);
            var tokenTagClaim = claims.FirstOrDefault(c => c.Type == TOKEN_TAG_KEY);

            if (idClaim == null)
            {
                return null;
            }
            return new UserInfo()
            {
                Id = idClaim.Value,
                Name = nameClaim?.Value,
                PersonName = givenNameClaim?.Value,
                DisplayName = nickNameClaim?.Value,
                IsAdmin = adminClaim?.Value == $"{idClaim?.Value}{nameClaim?.Value}{nameof(ADMIN_TAG_KEY)}{tokenTagClaim?.Value}".MD5(),
                IsVirual = virtualClaim?.Value == $"{idClaim?.Value}{nameClaim?.Value}{nameof(VIRTUAL_TAG_KEY)}{tokenTagClaim?.Value}".MD5(),
                RelationUserId = relationUserClaim?.Value ?? string.Empty,
                AppTag = appTagClaim?.Value ?? string.Empty,
                Token = tokenTagClaim?.Value ?? string.Empty,
                Roles = new HashSet<string>(roleClaim?.Value?.LSplit(",") ?? Array.Empty<string>()),
            };
        }

        /// <summary>
        /// Get claims
        /// </summary>
        /// <returns>Return claims</returns>
        public virtual List<Claim> GetClaims()
        {
            return new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject,Id.ToString()),
                new Claim(JwtClaimTypes.Name,Name??string.Empty),
                new Claim(JwtClaimTypes.NickName,DisplayName??string.Empty),
                new Claim(JwtClaimTypes.GivenName,PersonName??string.Empty),
                new Claim(ADMIN_TAG_KEY,IsAdmin ? $"{Id}{Name}{nameof(ADMIN_TAG_KEY)}{Token}".MD5() : ""),
                new Claim(VIRTUAL_TAG_KEY,IsVirual ? $"{Id}{Name}{nameof(VIRTUAL_TAG_KEY)}{Token}".MD5() : ""),
                new Claim(RELATIONUSER_TAG_KEY,RelationUserId??string.Empty),
                new Claim(APPLICATION_TAG_KEY,AppTag??string.Empty),
                new Claim(TOKEN_TAG_KEY,Token??string.Empty),
                new Claim(JwtClaimTypes.Role,Roles.IsNullOrEmpty()? "": string.Join(',',Roles))
            };
        }

        /// <summary>
        /// Get user id
        /// </summary>
        /// <typeparam name="TId">Id data type</typeparam>
        /// <returns></returns>
        public TId GetId<TId>()
        {
            return GetIdValue<TId>(Id);
        }

        /// <summary>
        /// Get relation user id
        /// </summary>
        /// <typeparam name="TId">Id data type</typeparam>
        /// <returns></returns>
        public TId GetRelationUserId<TId>()
        {
            return GetIdValue<TId>(RelationUserId);
        }

        static TId GetIdValue<TId>(string originalId)
        {
            if (string.IsNullOrWhiteSpace(originalId))
            {
                return default;
            }
            object idValue;
            if (typeof(TId) == typeof(Guid))
            {
                idValue = Guid.Parse(originalId);
            }
            else
            {
                idValue = originalId.ConvertTo<TId>();
            }
            return (TId)idValue;
        }

        #endregion
    }
}
