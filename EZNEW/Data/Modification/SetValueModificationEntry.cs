using System;
using System.Collections.Generic;

namespace EZNEW.Data.Modification
{
    /// <summary>
    /// Defines set value modification entry
    /// </summary>
    [Serializable]
    public class SetValueModificationEntry : IModificationEntry
    {
        /// <summary>
        /// Gets the field name
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Gets the modification value
        /// </summary>
        public IModificationValue Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of SetValueModificationEntry
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public SetValueModificationEntry(string name, IModificationValue value)
        {
            FieldName = name;
            Value = value;
        }
    }
}
