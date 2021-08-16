using System;
using System.Collections.Generic;

namespace EZNEW.Development.Command.Modification
{
    /// <summary>
    /// Defines set value modification item
    /// </summary>
    [Serializable]
    public class SetValueModificationItem : IModificationItem
    {
        bool isParsed = false;
        KeyValuePair<string, IModificationValue> parsedValue;

        /// <summary>
        /// Gets the modify field name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the modify value
        /// </summary>
        public IModificationValue Value { get; private set; }

        public SetValueModificationItem(string name, IModificationValue value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Parse modify value
        /// </summary>
        /// <returns>Return the modify values</returns>
        public KeyValuePair<string, IModificationValue> ResolveValue()
        {
            if (!isParsed)
            {
                parsedValue = new KeyValuePair<string, IModificationValue>(Name, Value);
                isParsed = true;
            }
            return parsedValue;
        }
    }
}
