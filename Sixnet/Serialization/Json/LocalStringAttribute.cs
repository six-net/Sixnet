using System;
using System.Text.Json.Serialization;
using Sixnet.Serialization.Json.Converter;

namespace Sixnet.Serialization.Json
{
    /// <summary>
    /// Local string
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class LocalStringAttribute : JsonConverterAttribute
    {
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            return LocalStringJsonConverter.Instance;
        }
    }
}
