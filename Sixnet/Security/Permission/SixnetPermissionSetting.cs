using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Security.Permission
{
    /// <summary>
    /// Permission setting
    /// </summary>
    public class SixnetPermissionSetting
    {
        /// <summary>
        /// Application tag
        /// </summary>
        public string AppTag { get; set; }

        /// <summary>
        /// Get permission func
        /// </summary>
        public Func<Dictionary<PermissionObjectType, List<SixnetObjectPermission>>> GetPermissionFunc { get; set; }
    }
}
