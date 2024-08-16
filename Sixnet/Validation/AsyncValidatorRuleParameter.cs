using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Validation
{
    public class AsyncValidatorRuleParameter
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Ignore required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Field type
        /// </summary>
        public Type FieldType { get; set; }

        /// <summary>
        /// Long as string
        /// </summary>
        public bool LongAsString { get; set; } = true;
    }
}
