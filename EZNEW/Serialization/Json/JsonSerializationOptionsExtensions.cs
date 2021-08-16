using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EZNEW.Serialization.Json.Converter;
using EZNEW.Serialization.Json.NamingPolicy;

namespace EZNEW.Serialization.Json
{
    public static class JsonSerializationOptionsExtensions
    {
        #region Fields

        static readonly Dictionary<JsonPropertyNamingPolicy, JsonNamingPolicy> NamingPolicies = new Dictionary<JsonPropertyNamingPolicy, JsonNamingPolicy>
        {
            {JsonPropertyNamingPolicy.Original, OriginalJsonNamingPolicy.Instance},
            {JsonPropertyNamingPolicy.CamelCase, JsonNamingPolicy.CamelCase},
            {JsonPropertyNamingPolicy.Uppercase, UppercaseJsonNamingPolicy.Instance},
            {JsonPropertyNamingPolicy.Lowercase, LowercaseJsonNamingPolicy.Instance},
        };

        #endregion

        /// <summary>
        /// Convert to JsonSerializerOptions
        /// </summary>
        /// <param name="currentOptions">Current json serialization options</param>
        /// <returns></returns>
        public static JsonSerializerOptions ConvertToJsonSerializerOptions(this JsonSerializationOptions currentOptions)
        {
            JsonSerializerOptions serializerOptions = new();
            return currentOptions.MergeToJsonSerializerOptions(serializerOptions);
        }

        /// <summary>
        /// Merge to JsonSerializerOptions
        /// </summary>
        /// <param name="jsonSerializationOptions">Current json serialization options</param>
        /// <param name="sourceJsonSerializerOptions">Source json serializer options</param>
        public static JsonSerializerOptions MergeToJsonSerializerOptions(this JsonSerializationOptions jsonSerializationOptions, JsonSerializerOptions sourceJsonSerializerOptions)
        {
            if (sourceJsonSerializerOptions == null)
            {
                throw new ArgumentNullException(nameof(sourceJsonSerializerOptions));
            }
            if (jsonSerializationOptions == null)
            {
                return sourceJsonSerializerOptions;
            }
            sourceJsonSerializerOptions.DefaultIgnoreCondition = jsonSerializationOptions.DefaultIgnoreCondition;
            sourceJsonSerializerOptions.ReadCommentHandling = jsonSerializationOptions.ReadCommentHandling;
            sourceJsonSerializerOptions.PropertyNameCaseInsensitive = jsonSerializationOptions.PropertyNameCaseInsensitive;
            sourceJsonSerializerOptions.PropertyNamingPolicy = jsonSerializationOptions.PropertyNamingPolicy;
            sourceJsonSerializerOptions.MaxDepth = jsonSerializationOptions.MaxDepth;
            sourceJsonSerializerOptions.IncludeFields = jsonSerializationOptions.IncludeFields;
            sourceJsonSerializerOptions.IgnoreReadOnlyFields = jsonSerializationOptions.IgnoreReadOnlyFields;
            sourceJsonSerializerOptions.IgnoreReadOnlyProperties = jsonSerializationOptions.IgnoreReadOnlyProperties;
            sourceJsonSerializerOptions.ReferenceHandler = jsonSerializationOptions.ReferenceHandler;
            sourceJsonSerializerOptions.WriteIndented = jsonSerializationOptions.WriteIndented;
            sourceJsonSerializerOptions.IgnoreNullValues = jsonSerializationOptions.IgnoreNullValues;
            sourceJsonSerializerOptions.DictionaryKeyPolicy = jsonSerializationOptions.DictionaryKeyPolicy;
            sourceJsonSerializerOptions.Encoder = jsonSerializationOptions.Encoder;
            sourceJsonSerializerOptions.DefaultBufferSize = jsonSerializationOptions.DefaultBufferSize;
            sourceJsonSerializerOptions.AllowTrailingCommas = jsonSerializationOptions.AllowTrailingCommas;
            sourceJsonSerializerOptions.NumberHandling = jsonSerializationOptions.NumberHandling;

            if (sourceJsonSerializerOptions.PropertyNamingPolicy == null)
            {
                sourceJsonSerializerOptions.PropertyNamingPolicy = GetJsonNamingPolicy(jsonSerializationOptions.DefaultNamingPolicy);
            }
            if (sourceJsonSerializerOptions.DictionaryKeyPolicy == null)
            {
                sourceJsonSerializerOptions.DictionaryKeyPolicy = sourceJsonSerializerOptions.PropertyNamingPolicy;
            }
            if (!jsonSerializationOptions.Converters.IsNullOrEmpty())
            {
                foreach (var converter in jsonSerializationOptions.Converters)
                {
                    sourceJsonSerializerOptions.Converters.Add(converter);
                }
            }
            if (jsonSerializationOptions.UseCustomizedBigNumberConverter)
            {
                sourceJsonSerializerOptions.Converters.Add(LongJsonConverter.Instance);
                sourceJsonSerializerOptions.Converters.Add(LongAllowNullJsonConverter.Instance);
                sourceJsonSerializerOptions.Converters.Add(ULongJsonConverter.Instance);
                sourceJsonSerializerOptions.Converters.Add(ULongAllowNullJsonConverter.Instance);
                sourceJsonSerializerOptions.Converters.Add(DecimalJsonConverter.Instance);
                sourceJsonSerializerOptions.Converters.Add(DecimalAllowNullJsonConverter.Instance);
            }
            return sourceJsonSerializerOptions;
        }

        /// <summary>
        /// Get json naming policy
        /// </summary>
        /// <param name="propertyNamingPolicy"></param>
        /// <returns></returns>
        static JsonNamingPolicy GetJsonNamingPolicy(JsonPropertyNamingPolicy propertyNamingPolicy)
        {
            NamingPolicies.TryGetValue(propertyNamingPolicy, out var jsonNamingPolicy);
            return jsonNamingPolicy;
        }
    }
}
