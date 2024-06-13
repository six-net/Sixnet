using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Validation
{
    /// <summary>
    /// Validation options
    /// </summary>
    public class SixnetValidationOptions
    {
        /// <summary>
        /// Whether use the base type's validation rules
        /// </summary>
        public bool UseInheritance { get; set; } = true;
    }
}
