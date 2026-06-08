// "Company © 2025. All rights reserved."

namespace Sixnet.Security.Authorization
{
    /// <summary>
    /// Authorization result
    /// </summary>
    [Serializable]
    public class SixnetAuthorizationResult
    {
        /// <summary>
        /// Gets or sets the authorization status
        /// </summary>
        public SixnetAuthorizationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the controller
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the action
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the area
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Gets or sets the url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the route values
        /// </summary>
        public object RouteValues { get; set; }

        /// <summary>
        /// Gets or sets how redirect when authorization failed
        /// </summary>
        public SixnetAuthorizeRedirectType RedirectType { get; set; } = SixnetAuthorizeRedirectType.Default;

        /// <summary>
        /// Gets or sets whether allow to access
        /// </summary>
        public bool AllowAccess => Status == SixnetAuthorizationStatus.Success;

        /// <summary>
        /// Gets verify authorization result with challenge status
        /// </summary>
        /// <returns></returns>
        public static SixnetAuthorizationResult ChallengeResult()
        {
            return GetAuthorizationResult(SixnetAuthorizationStatus.Challenge);
        }

        /// <summary>
        /// Gets verify authorization result with forbid status
        /// </summary>
        /// <returns></returns>
        public static SixnetAuthorizationResult ForbidResult()
        {
            return GetAuthorizationResult(SixnetAuthorizationStatus.Forbid);
        }

        /// <summary>
        /// Gets verify authorization result with success status
        /// </summary>
        /// <returns></returns>
        public static SixnetAuthorizationResult SuccessResult()
        {
            return GetAuthorizationResult(SixnetAuthorizationStatus.Success);
        }

        /// <summary>
        /// Get autthorize result
        /// </summary>
        /// <param name="status">Verification status</param>
        /// <returns></returns>
        public static SixnetAuthorizationResult GetAuthorizationResult(SixnetAuthorizationStatus status = SixnetAuthorizationStatus.Forbid)
        {
            return new SixnetAuthorizationResult()
            {
                Status = status
            };
        }
    }
}
