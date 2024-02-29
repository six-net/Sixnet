using System.Collections.Generic;

namespace Sixnet.Validation
{
    /// <summary>
    /// Type validation options
    /// </summary>
    public class SixnetTypeValidationOptions
    {
        /// <summary>
        /// Gets or sets the type full name
        /// </summary>
        public string TypeAssemblyQualifiedName { get; set; }

        /// <summary>
        /// Gets or set the property rules
        /// </summary>
        public List<SixnetPropertyValidationOptions> Properties { get; set; }
    }
}
