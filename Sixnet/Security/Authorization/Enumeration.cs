// "Company © 2025. All rights reserved."

namespace Sixnet.Security.Authorization
{
    /// <summary>
    /// Defines authorization status
    /// </summary>
    [Serializable]
    public enum SixnetAuthorizationStatus
    {
        /// <summary>
        /// not log in
        /// </summary>
        Challenge = 110,
        /// <summary>
        /// not authorized
        /// </summary>
        Forbid = 120,
        /// <summary>
        /// successful
        /// </summary>
        Success = 130
    }

    /// <summary>
    /// Defines authorize redirect type
    /// </summary>
    public enum SixnetAuthorizeRedirectType
    {
        Default = 0,
        RedirectToAction = 10,
        RedirectToRoute = 20,
        RedirectToUrl = 30
    }
}
