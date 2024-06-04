using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Security.Authorization
{
    /// <summary>
    /// Sixnet permission authorization
    /// </summary>
    public class SixnetPermissionAuthorization
    {
        /// <summary>
        /// Object id
        /// </summary>
        public string AuthObjectId { get; set; }

        /// <summary>
        /// Permissions
        /// </summary>
        public List<string> Permissions { get; set; }
    }
}
