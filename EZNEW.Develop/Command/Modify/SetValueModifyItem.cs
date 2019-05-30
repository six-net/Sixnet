using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// set value modify item
    /// </summary>
    public class SetValueModifyItem : IModifyItem
    {
        bool isParsed = false;
        KeyValuePair<string, IModifyValue> parsedValue;

        /// <summary>
        /// name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// value
        /// </summary>
        public IModifyValue Value { get; private set; }

        public SetValueModifyItem(string name, IModifyValue value)
        {
            Name = name;
            Value = value;
        }

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
