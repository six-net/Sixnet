using System;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Sixnet.Serialization.Json.Converter;

namespace Sixnet.Serialization.Json
{
    /// <summary>
    /// File full path
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class FileFullPathAttribute : JsonConverterAttribute
    {
        static readonly ConcurrentDictionary<string, FileFullPathJsonConverter> _converters = new();

        public string FileObjectName { get; set; }

        public FileFullPathAttribute(string fileObjectName = "")
        {
            FileObjectName = fileObjectName;
        }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            return _converters.GetOrAdd(FileObjectName, k => FileFullPathJsonConverter.GetInstance(k));
        }
    }
}
