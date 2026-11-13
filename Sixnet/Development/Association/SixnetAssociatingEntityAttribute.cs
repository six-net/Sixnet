// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Association
{
    /// <summary>
    /// Associated entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SixnetAssociatingEntityAttribute : Attribute
    {
        public SixnetAssociatingEntityAttribute(Type entityType)
        {
            EntityType = entityType;
        }

        internal Type EntityType { get; set; }
    }
}
