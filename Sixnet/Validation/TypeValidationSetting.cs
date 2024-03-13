using System.Collections.Generic;

namespace Sixnet.Validation
{
    /// <summary>
    /// Type validation setting
    /// </summary>
    public class TypeValidationSetting
    {
        /// <summary>
        /// Gets or sets the type full name
        /// </summary>
        public string TypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets or set the property rules
        /// </summary>
        public List<PropertyValidationSetting> Properties { get; set; }
    }
}
