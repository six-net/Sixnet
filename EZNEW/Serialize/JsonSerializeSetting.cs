using System;

namespace EZNEW.Serialize
{
    /// <summary>
    /// Json serialize setting
    /// </summary>
    public class JsonSerializeSetting
    {
        /// <summary>
        /// Gets or sets whether non-public properties and fields are parsed
        /// </summary>
        public bool ResolveNonPublic { get; set; }

        /// <summary>
        /// Gets or sets the deserialized data type
        /// </summary>
        public Type DeserializeType { get; set; }
    }
}
