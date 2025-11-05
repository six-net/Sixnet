// "Company © 2025. All rights reserved."

namespace Sixnet.Validation
{
    /// <summary>
    /// Property validation setting
    /// </summary>
    public class PropertyValidationSetting
    {
        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        public List<SixnetValidatorSetting> Rules { get; set; }
    }
}
