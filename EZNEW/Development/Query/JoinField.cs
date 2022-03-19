using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines join field
    /// </summary>
    public class JoinField : IInnerClone<JoinField>
    {
        /// <summary>
        /// Gets or sets the field / value
        /// </summary>
        public object Name { get; set; }

        /// <summary>
        /// Gets or sets the field type
        /// </summary>
        public JoinFieldType Type { get; set; } = JoinFieldType.Field;

        public JoinField Clone()
        {
            object targetName = Name;
            if (Name is FieldInfo field)
            {
                targetName = field.Clone();
            }
            return new JoinField()
            {
                Name = targetName,
                Type = Type
            };
        }
    }
}
