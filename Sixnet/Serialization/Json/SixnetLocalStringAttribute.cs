// "Company © 2025. All rights reserved."

using System.Text.Json.Serialization;

using Sixnet.Serialization.Json.Converter;

namespace Sixnet.Serialization.Json
{
    /// <summary>
    /// Local string
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SixnetLocalStringAttribute : JsonConverterAttribute
    {
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            return SixnetLocalStringJsonConverter.Instance;
        }
    }
}
