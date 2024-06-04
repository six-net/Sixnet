using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Sixnet.Model;
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
        const string ADMIN_TAG_KEY = "SIXNET_SUPER_ADMIN";

        /// <summary>
        /// Virtual user tag key
        /// </summary>
        const string VIRTUAL_TAG_KEY = "SIXNET_VIRTUAL_USER";

        /// <summary>
        /// Relation user tag key
        /// </summary>
        const string RELATIONUSER_TAG_KEY = "SIXNET_RELATION_USER";

        /// <summary>
        /// Application tag key
        /// </summary>
        const string APPLICATION_TAG_KEY = "SIXNET_APP";

        /// <summary>
        /// Token tag key
        /// </summary>
        const string TOKEN_TAG_KEY = "SIXNET_TOKEN_";

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
            var idClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            idClaim ??= claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);

            var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            nameClaim ??= claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name);

            var givenNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            givenNameClaim ??= claims.FirstOrDefault(c => c.Type == JwtClaimTypes.GivenName);

            var nickNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            nickNameClaim ??= claims.FirstOrDefault(c => c.Type == JwtClaimTypes.NickName);

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
                IsAdmin = adminClaim?.Value == $"{nameClaim?.Value}{ADMIN_TAG_KEY}".MD5(),
                IsVirual = virtualClaim?.Value == $"{nameClaim?.Value}{VIRTUAL_TAG_KEY}".MD5(),
                RelationUserId = relationUserClaim?.Value ?? string.Empty,
                AppTag = appTagClaim?.Value ?? string.Empty,
                Token = tokenTagClaim?.Value ?? string.Empty
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
                new Claim(ADMIN_TAG_KEY,IsAdmin ? $"{Name}{ADMIN_TAG_KEY}".MD5() : ""),
                new Claim(VIRTUAL_TAG_KEY,IsVirual ? $"{Name}{VIRTUAL_TAG_KEY}".MD5() : ""),
                new Claim(RELATIONUSER_TAG_KEY,RelationUserId??string.Empty),
                new Claim(APPLICATION_TAG_KEY,AppTag??string.Empty),
                new Claim(TOKEN_TAG_KEY,Token??string.Empty)
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
