using System.Collections.Generic;

namespace Sixnet.DataValidation.Configuration
{
    /// <summary>
    /// Type validation rule
    /// </summary>
    public class TypeValidationRule
    {
        /// <summary>
        /// Gets or sets the type full name
        /// </summary>
        public string TypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets or set the property rules
        /// </summary>
        public List<PropertyRule> Properties { get; set; }
    }
}
