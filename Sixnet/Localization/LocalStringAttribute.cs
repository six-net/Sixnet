using System;
using System.Text.Json.Serialization;

namespace Sixnet.Localization
{
    /// <summary>
    /// Local string
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class LocalStringAttribute : JsonConverterAttribute
    {
        public LocalStringAttribute()
        {

        }
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            return null;//LanguageTranslateJsonConverter.Instance;
        }
    }
}
