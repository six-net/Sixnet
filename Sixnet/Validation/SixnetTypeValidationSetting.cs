// "Company © 2025. All rights reserved."

namespace Sixnet.Validation
{
    /// <summary>
    /// Type validation setting
    /// </summary>
    public class SixnetTypeValidationSetting
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
