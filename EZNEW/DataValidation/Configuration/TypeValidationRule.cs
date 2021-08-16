using System.Collections.Generic;

namespace EZNEW.DataValidation.Configuration
{
    /// <summary>
    /// Type validation rule
    /// </summary>
    public class TypeValidationRule
    {
        /// <summary>
        /// Gets or sets the type full name
        /// </summary>
        public string TypeFullName { get; set; }

        /// <summary>
        /// Gets or set the property rules
        /// </summary>
        public List<PropertyRule> Properties { get; set; }
    }
}
