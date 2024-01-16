using System.Collections.Generic;

namespace Sixnet.DataValidation.Configuration
{
    /// <summary>
    /// Property validation rule
    /// </summary>
    public class PropertyRule
    {
        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        public List<ValidatorRule> Rules { get; set; }
    }
}
