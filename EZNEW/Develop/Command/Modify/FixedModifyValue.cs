using System;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// Fixed modify value
    /// </summary>
    [Serializable]
    public class FixedModifyValue : IModifyValue
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public dynamic Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the EZNEW.Develop.Command.Modify.FixedModifyValue
        /// </summary>
        /// <param name="value">Modify value</param>
        public FixedModifyValue(dynamic value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets modify value
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Return the modify value</returns>
        public dynamic GetModifyValue(dynamic originalValue)
        {
            return Value;
        }
    }
}
