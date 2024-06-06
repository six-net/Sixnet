using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Security.Permission
{
    /// <summary>
    /// Sixnet object permission
    /// </summary>
    public class SixnetObjectPermission
    {
        /// <summary>
        /// Object value
        /// </summary>
        public string ObjectValue { get; set; }

        /// <summary>
        /// Permissions
        /// </summary>
        public List<string> Permissions { get; set; }
    }
}
