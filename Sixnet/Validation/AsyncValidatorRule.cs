using Sixnet.Serialization.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Validation
{
    /// <summary>
    /// Async validator rule
    /// </summary>
    public class AsyncValidatorRule
    {
        public bool Required { get; set; }

        public string Type { get; set; }

        public string Pattern { get; set; }

        public dynamic Min { get; set; }

        public dynamic Max { get; set; }

        public dynamic Len { get; set; }

        public IEnumerable Enum { get; set; }

        [LocalString]
        public string Message { get; set; }
    }
}
