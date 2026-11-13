// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Association
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SixnetAssociatingAttribute(string[] identityPropertyNames, bool includeArchived = true, Type associatedType = null, Type associatedEntityType = null) : Attribute
    {
        internal string[] IdentityPropertyNames { get; set; } = identityPropertyNames;

        internal Type AssociatedType { get; set; } = associatedType;

        internal Type AssociatedEntityType { get; set; } = associatedEntityType;

        internal bool IncludeArchived { get; set; } = includeArchived;
    }
}
