// "Company © 2025. All rights reserved."

using System.Text.Json;
using System.Text.Json.Serialization;

using Sixnet.IO;

namespace Sixnet.Serialization.Json.Converter
{
    public class SixnetFileFullPathJsonConverter : JsonConverter<string>
    {
        readonly string fileObjectName;

        private SixnetFileFullPathJsonConverter(string fileObjectName)
        {
            this.fileObjectName = fileObjectName;
        }

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(SixnetFileManager.GetFileAccessPath(fileObjectName, value));
        }

        public static SixnetFileFullPathJsonConverter GetInstance(string fileObjectName)
        {
            return new SixnetFileFullPathJsonConverter(fileObjectName);
        }
    }
}
