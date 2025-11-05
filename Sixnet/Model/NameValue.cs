// "Company © 2025. All rights reserved."

using Sixnet.Serialization.Json;

namespace Sixnet.Model
{
    /// <summary>
    /// Name / value
    /// </summary>
    public class NameValue<T>
    {
        [LocalString]
        public string Name { get; set; }

        public T Value { get; set; }
    }
}
