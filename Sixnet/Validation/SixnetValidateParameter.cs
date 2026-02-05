// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Validation
{
    /// <summary>
    /// Validate parameter
    /// </summary>
    public class SixnetValidateParameter
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public object Value {  get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage {  get; set; }

        /// <summary>
        /// Gets or sets the message args
        /// </summary>
        public List<string> MessageArgs { get; set; }
    }
}
