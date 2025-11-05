// "Company © 2025. All rights reserved."

using Sixnet.App;
using Sixnet.Session;

namespace Sixnet.Security.Authorization
{
    /// <summary>
    /// Authorization context
    /// </summary>
    [Serializable]
    public class SixnetAuthorizationContext
    {
        /// <summary>
        /// Gets or sets the operation
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Gets or sets the application info
        /// </summary>
        public ApplicationInfo Application { get; set; }

        /// <summary>
        /// Gets or sets the user info
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// Gets or sets the claims
        /// </summary>
        public Dictionary<string, string> Claims { get; set; }
    }
}
