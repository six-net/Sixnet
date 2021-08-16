using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EZNEW.Serialization.Json.Converter
{
    /// <summary>
    /// Long number json data converter
    /// </summary>
    public class LongJsonConverter : System.Text.Json.Serialization.JsonConverter<long>
    {
        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="typeToConvert">Type convert</param>
        /// <param name="options">Options</param>
        /// <returns>Return big number value</returns>
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!long.TryParse(reader.GetString(), out long result))
            {
                result = 0;
            }
            return result;
        }

        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Value</param>
        /// <param name="options">Options</param>
        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public static LongJsonConverter Instance = new LongJsonConverter();
    }

    /// <summary>
    /// Allow null long number json converter
    /// </summary>
    public class LongAllowNullJsonConverter : System.Text.Json.Serialization.JsonConverter<long?>
    {
        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="typeToConvert">Type convert</param>
        /// <param name="options">Options</param>
        /// <returns>Return big number value</returns>
        public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!long.TryParse(reader.GetString(), out long result))
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Value</param>
        /// <param name="options">Options</param>
        public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString() ?? string.Empty);
        }

        public static LongAllowNullJsonConverter Instance = new LongAllowNullJsonConverter();
    }

    /// <summary>
    /// Unsign long number json converter
    /// </summary>
    public class ULongJsonConverter : System.Text.Json.Serialization.JsonConverter<ulong>
    {
        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="typeToConvert">Type convert</param>
        /// <param name="options">Options</param>
        /// <returns>Return big number value</returns>
        public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!ulong.TryParse(reader.GetString(), out ulong result))
            {
                result = 0;
            }
            return result;
        }

        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Value</param>
        /// <param name="options">Options</param>
        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public static ULongJsonConverter Instance = new ULongJsonConverter();
    }

    /// <summary>
    /// Unsign allow null long number json converter
    /// </summary>
    public class ULongAllowNullJsonConverter : System.Text.Json.Serialization.JsonConverter<ulong?>
    {
        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="typeToConvert">Type convert</param>
        /// <param name="options">Options</param>
        /// <returns>Return big number value</returns>
        public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!ulong.TryParse(reader.GetString(), out ulong result))
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Value</param>
        /// <param name="options">Options</param>
        public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString() ?? string.Empty);
        }

        public static ULongAllowNullJsonConverter Instance = new ULongAllowNullJsonConverter();
    }

    /// <summary>
    /// Decimal data json converter
    /// </summary>
    public class DecimalJsonConverter : System.Text.Json.Serialization.JsonConverter<decimal>
    {
        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="typeToConvert">Type convert</param>
        /// <param name="options">Options</param>
        /// <returns>Return decimal value</returns>
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!decimal.TryParse(reader.GetString(), out decimal result))
            {
                result = 0.00M;
            }
            return result;
        }

        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Value</param>
        /// <param name="options">Options</param>
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public static DecimalJsonConverter Instance = new DecimalJsonConverter();
    }

    /// <summary>
    /// Allow null decimal data json converter
    /// </summary>
    public class DecimalAllowNullJsonConverter : System.Text.Json.Serialization.JsonConverter<decimal?>
    {
        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="typeToConvert">Type convert</param>
        /// <param name="options">Options</param>
        /// <returns>Return decimal value</returns>
        public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!decimal.TryParse(reader.GetString(), out decimal result))
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Value</param>
        /// <param name="options">Options</param>
        public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString() ?? string.Empty);
        }

        public static DecimalAllowNullJsonConverter Instance = new DecimalAllowNullJsonConverter();
    }
}
