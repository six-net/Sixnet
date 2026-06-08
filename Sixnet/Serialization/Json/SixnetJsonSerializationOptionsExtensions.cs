// "Company © 2025. All rights reserved."

using System.Text.Json;

using Sixnet.Exceptions;
using Sixnet.Serialization.Json.Converter;
using Sixnet.Serialization.Json.NamingPolicy;

namespace Sixnet.Serialization.Json
{
    public static class SixnetJsonSerializationOptionsExtensions
    {
        #region Fields

        static readonly Dictionary<SixnetJsonPropertyNamingPolicy, JsonNamingPolicy> NamingPolicies = new Dictionary<SixnetJsonPropertyNamingPolicy, JsonNamingPolicy>
        {
            {SixnetJsonPropertyNamingPolicy.Default, JsonNamingPolicy.CamelCase},
            {SixnetJsonPropertyNamingPolicy.Original, SixnetOriginalJsonNamingPolicy.Instance},
            {SixnetJsonPropertyNamingPolicy.CamelCase, JsonNamingPolicy.CamelCase},
            {SixnetJsonPropertyNamingPolicy.Uppercase, SixnetUppercaseJsonNamingPolicy.Instance},
            {SixnetJsonPropertyNamingPolicy.Lowercase, SixnetLowercaseJsonNamingPolicy.Instance},
        };

        #endregion

        #region Convert to JsonSerializerOptions

        /// <summary>
        /// Convert to JsonSerializerOptions
        /// </summary>
        /// <param name="sixnetJsonOptions">Current json serialization options</param>
        /// <returns></returns>
        public static JsonSerializerOptions ConvertToJsonSerializerOptions(this SixnetJsonSerializationOptions sixnetJsonOptions)
        {
            var serializerOptions = new JsonSerializerOptions();
            return sixnetJsonOptions.MergeToJsonSerializerOptions(serializerOptions);
        }

        #endregion

        #region Merge to JsonSerializerOptions

        /// <summary>
        /// Merge to JsonSerializerOptions
        /// </summary>
        /// <param name="sixnetJsonOptions">Sixnet serialization options</param>
        /// <param name="jsonSerializerOptions">Json serializer options</param>
        public static JsonSerializerOptions MergeToJsonSerializerOptions(this SixnetJsonSerializationOptions sixnetJsonOptions, JsonSerializerOptions jsonSerializerOptions)
        {
            if (sixnetJsonOptions == null)
            {
                return jsonSerializerOptions;
            }

            SixnetDirectThrower.ThrowArgNullIf(jsonSerializerOptions == null, nameof(jsonSerializerOptions));

            //naming policy
            if (sixnetJsonOptions.PropertyNamingPolicy != SixnetJsonPropertyNamingPolicy.Default)
            {
                jsonSerializerOptions.PropertyNamingPolicy = GetJsonNamingPolicy(sixnetJsonOptions.PropertyNamingPolicy);
            }
            if (sixnetJsonOptions.DictionaryKeyNamingPolicy != SixnetJsonPropertyNamingPolicy.Default)
            {
                jsonSerializerOptions.DictionaryKeyPolicy = GetJsonNamingPolicy(sixnetJsonOptions.DictionaryKeyNamingPolicy);
            }

            //converter
            if (sixnetJsonOptions.ConvertBigNumberToString)
            {
                jsonSerializerOptions.Converters.Add(LongJsonConverter.Instance);
                jsonSerializerOptions.Converters.Add(LongAllowNullJsonConverter.Instance);
                jsonSerializerOptions.Converters.Add(ULongJsonConverter.Instance);
                jsonSerializerOptions.Converters.Add(ULongAllowNullJsonConverter.Instance);
                jsonSerializerOptions.Converters.Add(DecimalJsonConverter.Instance);
                jsonSerializerOptions.Converters.Add(DecimalAllowNullJsonConverter.Instance);
            }

            sixnetJsonOptions.ConfigureSerializer?.Invoke(jsonSerializerOptions);

            return jsonSerializerOptions;
        }

        #endregion

        #region Get json naming policy

        /// <summary>
        /// Get json naming policy
        /// </summary>
        /// <param name="propertyNamingPolicy"></param>
        /// <returns></returns>
        static JsonNamingPolicy GetJsonNamingPolicy(SixnetJsonPropertyNamingPolicy propertyNamingPolicy)
        {
            NamingPolicies.TryGetValue(propertyNamingPolicy, out var jsonNamingPolicy);
            return jsonNamingPolicy;
        }

        #endregion
    }
}
