// "Company © 2025. All rights reserved."

using System.Collections.Concurrent;
using System.Text.Json.Serialization;

using Sixnet.Serialization.Json.Converter;

namespace Sixnet.Serialization.Json
{
    /// <summary>
    /// File full path
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SixnetFileFullPathAttribute : JsonConverterAttribute
    {
        static readonly ConcurrentDictionary<string, SixnetFileFullPathJsonConverter> _converters = new();

        public string FileObjectName { get; set; }

        public SixnetFileFullPathAttribute(object fileObjectName = null)
        {
            FileObjectName = fileObjectName?.ToString() ?? string.Empty;
        }

        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            return _converters.GetOrAdd(FileObjectName, k => SixnetFileFullPathJsonConverter.GetInstance(k));
        }
    }
}
