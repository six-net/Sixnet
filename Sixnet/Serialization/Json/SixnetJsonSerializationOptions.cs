// "Company © 2025. All rights reserved."

using System.Text.Json;

namespace Sixnet.Serialization.Json
{
    /// <summary>
    /// Json options
    /// </summary>
    public class SixnetJsonSerializationOptions
    {
        /// <summary>
        /// Configure serializer
        /// </summary>
        public Action<JsonSerializerOptions> ConfigureSerializer { get; set; }

        /// <summary>
        /// Indicates whether non-public properties and fields are parsed
        /// </summary>
        public bool ResolveNonPublic { get; set; }

        /// <summary>
        /// Gets or sets the property naming policy
        /// </summary>
        public JsonPropertyNamingPolicy PropertyNamingPolicy { get; set; } = JsonPropertyNamingPolicy.CamelCase;

        /// <summary>
        /// Gets or sets the dictionary key policy
        /// </summary>
        public JsonPropertyNamingPolicy DictionaryKeyNamingPolicy { get; set; } = JsonPropertyNamingPolicy.CamelCase;

        /// <summary>
        /// Gets or sets the deserialized data type
        /// </summary>
        public Type DeserializeType { get; set; }

        /// <summary>
        /// Indicates whether convert big numer to string
        /// Default value is true
        /// </summary>
        public bool ConvertBigNumberToString { get; set; } = true;

        /// <summary>
        /// Indicates whether disable localization converter
        /// </summary>
        public bool DisableLocalConverter { get; set; }
    }
}
