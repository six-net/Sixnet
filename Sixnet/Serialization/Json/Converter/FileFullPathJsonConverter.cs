// "Company © 2025. All rights reserved."

using System.Text.Json;
using System.Text.Json.Serialization;

using Sixnet.IO;

namespace Sixnet.Serialization.Json.Converter
{
    public class FileFullPathJsonConverter : JsonConverter<string>
    {
        readonly string fileObjectName;

        private FileFullPathJsonConverter(string fileObjectName)
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

        public static FileFullPathJsonConverter GetInstance(string fileObjectName)
        {
            return new FileFullPathJsonConverter(fileObjectName);
        }
    }
}
