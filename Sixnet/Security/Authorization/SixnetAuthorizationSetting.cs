using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Security.Authorization
{
    public class SixnetAuthorizationSetting
    {
        /// <summary>
        /// Application tag
        /// </summary>
        public string AppTag { get; set; }

        /// <summary>
        /// Get operation authorization func
        /// </summary>
        public Func<Dictionary<AuthorizationObject, List<SixnetPermissionAuthorization>>> GetAuthorizationFunc { get; set; }
    }
}
