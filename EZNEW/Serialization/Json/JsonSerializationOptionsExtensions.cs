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

        #region Convert to JsonSerializerOptions

        /// <summary>
        /// Convert to JsonSerializerOptions
        /// </summary>
        /// <param name="currentOptions">Current json serialization options</param>
        /// <returns></returns>
        public static JsonSerializerOptions ConvertToJsonSerializerOptions(this JsonSerializationOptions currentOptions)
        {
            JsonSerializerOptions serializerOptions = new();
            return currentOptions.ApplyToJsonSerializerOptions(serializerOptions);
        }

        #endregion

        #region Apply to JsonSerializerOptions

        /// <summary>
        /// Apply to JsonSerializerOptions
        /// </summary>
        /// <param name="jsonSerializationOptions">Current json serialization options</param>
        /// <param name="sourceJsonSerializerOptions">Source json serializer options</param>
        public static JsonSerializerOptions ApplyToJsonSerializerOptions(this JsonSerializationOptions jsonSerializationOptions, JsonSerializerOptions sourceJsonSerializerOptions)
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

            //naming policy
            if (jsonSerializationOptions.PropertyNamingPolicy == null)
            {
                jsonSerializationOptions.PropertyNamingPolicy = GetJsonNamingPolicy(jsonSerializationOptions.DefaultNamingPolicy);
            }
            if (jsonSerializationOptions.PropertyNamingPolicy != null)
            {
                sourceJsonSerializerOptions.PropertyNamingPolicy = jsonSerializationOptions.PropertyNamingPolicy;
            }
            if (jsonSerializationOptions.DictionaryKeyPolicy == null)
            {
                jsonSerializationOptions.DictionaryKeyPolicy = jsonSerializationOptions.PropertyNamingPolicy;
            }
            if (jsonSerializationOptions.DictionaryKeyPolicy != null)
            {
                sourceJsonSerializerOptions.DictionaryKeyPolicy = jsonSerializationOptions.DictionaryKeyPolicy;
            }

            //converters
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

        #endregion

        #region Merge from JsonSerializerOptions

        /// <summary>
        /// Merge from JsonSerializerOptions
        /// </summary>
        /// <param name="jsonSerializationOptions">Json serialization options</param>
        /// <param name="sourceJsonSerializerOptions">Source json serializer options</param>
        /// <returns></returns>
        public static JsonSerializationOptions MergeFromJsonSerializerOptions(this JsonSerializationOptions jsonSerializationOptions, JsonSerializerOptions sourceJsonSerializerOptions)
        {
            if (jsonSerializationOptions == null || sourceJsonSerializerOptions == null)
            {
                return jsonSerializationOptions;
            }
            jsonSerializationOptions.DefaultIgnoreCondition = sourceJsonSerializerOptions.DefaultIgnoreCondition;
            jsonSerializationOptions.ReadCommentHandling = sourceJsonSerializerOptions.ReadCommentHandling;
            jsonSerializationOptions.PropertyNameCaseInsensitive = sourceJsonSerializerOptions.PropertyNameCaseInsensitive;
            jsonSerializationOptions.MaxDepth = sourceJsonSerializerOptions.MaxDepth;
            jsonSerializationOptions.IncludeFields = sourceJsonSerializerOptions.IncludeFields;
            jsonSerializationOptions.IgnoreReadOnlyFields = sourceJsonSerializerOptions.IgnoreReadOnlyFields;
            jsonSerializationOptions.IgnoreReadOnlyProperties = sourceJsonSerializerOptions.IgnoreReadOnlyProperties;
            jsonSerializationOptions.ReferenceHandler = sourceJsonSerializerOptions.ReferenceHandler;
            jsonSerializationOptions.WriteIndented = sourceJsonSerializerOptions.WriteIndented;
            jsonSerializationOptions.IgnoreNullValues = sourceJsonSerializerOptions.IgnoreNullValues;
            jsonSerializationOptions.DictionaryKeyPolicy = sourceJsonSerializerOptions.DictionaryKeyPolicy;
            jsonSerializationOptions.Encoder = sourceJsonSerializerOptions.Encoder;
            jsonSerializationOptions.DefaultBufferSize = sourceJsonSerializerOptions.DefaultBufferSize;
            jsonSerializationOptions.AllowTrailingCommas = sourceJsonSerializerOptions.AllowTrailingCommas;
            jsonSerializationOptions.NumberHandling = sourceJsonSerializerOptions.NumberHandling;

            return jsonSerializationOptions;
        } 

        #endregion

        #region Get json naming policy

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

        #endregion
    }
}
