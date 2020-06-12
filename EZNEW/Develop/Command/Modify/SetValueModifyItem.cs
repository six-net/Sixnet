using System;
using System.Collections.Generic;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// Set value modify item
    /// </summary>
    [Serializable]
    public class SetValueModifyItem : IModifyItem
    {
        bool isParsed = false;
        KeyValuePair<string, IModifyValue> parsedValue;

        /// <summary>
        /// Gets the modify field name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the modify value
        /// </summary>
        public IModifyValue Value { get; private set; }

        public SetValueModifyItem(string name, IModifyValue value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Parse modify value
        /// </summary>
        /// <returns>Return the modify values</returns>
        public KeyValuePair<string, IModifyValue> ParseModifyValue()
        {
            if (!isParsed)
            {
                parsedValue = new KeyValuePair<string, IModifyValue>(Name, Value);
                isParsed = true;
            }
            return parsedValue;
        }
    }
}
