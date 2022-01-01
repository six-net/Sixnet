using System.Collections.Generic;

namespace EZNEW.Data.Modification
{
    /// <summary>
    /// Defines modification entry contract
    /// </summary>
    public interface IModificationEntry
    {
        /// <summary>
        /// Gets the field name
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// Gets the modification value
        /// </summary>
        public IModificationValue Value { get; }
    }
}
