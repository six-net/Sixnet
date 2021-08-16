using System;

namespace EZNEW.Development.Command.Modification
{
    /// <summary>
    /// Defines fixed mofification value
    /// </summary>
    [Serializable]
    public class FixedModificationValue : IModificationValue
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public dynamic Value { get; set; }

        /// <summary>
        /// Initializes a new instance of the EZNEW.Development.Command.Modification.FixedModificationValue
        /// </summary>
        /// <param name="value">Modification value</param>
        public FixedModificationValue(dynamic value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the modified value
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Return the modified value</returns>
        public dynamic GetModifiedValue(dynamic originalValue)
        {
            return Value;
        }
    }
}
