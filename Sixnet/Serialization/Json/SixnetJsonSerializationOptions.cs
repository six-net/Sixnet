using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        /// Gets or sets the default naming policy
        /// </summary>
        public JsonPropertyNamingPolicy DefaultNamingPolicy { get; set; } = JsonPropertyNamingPolicy.Default;

        /// <summary>
        /// Gets or sets the deserialized data type
        /// </summary>
        public Type DeserializeType { get; set; }

        /// <summary>
        /// Indicates whether convert big numer to string
        /// Default value is true
        /// </summary>
        public bool ConvertBigNumberToString { get; set; } = true;
    }
}
