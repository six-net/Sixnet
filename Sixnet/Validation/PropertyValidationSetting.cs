using System.Collections.Generic;

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
        public List<ValidatorSetting> Rules { get; set; }
    }
}
