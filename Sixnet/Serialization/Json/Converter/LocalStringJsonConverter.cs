using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sixnet.Localization;

namespace Sixnet.Serialization.Json.Converter
{
    /// <summary>
    /// Local string json converter
    /// </summary>
    public sealed class LocalStringJsonConverter : JsonConverter<string>
    {
        private LocalStringJsonConverter() { }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(SixnetLocalizer.GetString(value));
        }

        public static LocalStringJsonConverter Instance = new();

    }
}
