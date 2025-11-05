// "Company © 2025. All rights reserved."

using System.Collections;

using Sixnet.Serialization.Json;

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

        public string Trigger { get; set; }
    }
}
