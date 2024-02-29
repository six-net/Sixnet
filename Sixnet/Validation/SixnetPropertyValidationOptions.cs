using System.Collections.Generic;

namespace Sixnet.Validation
{
    /// <summary>
    /// Property validation options
    /// </summary>
    public class SixnetPropertyValidationOptions
    {
        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        public List<SixnetValidatorOptions> Rules { get; set; }
    }
}
