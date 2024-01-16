using System;
using System.Text.Json;

namespace Sixnet.Serialization.Json.Converter
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
            long value = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    long.TryParse(reader.GetString(), out value);
                    break;
                case JsonTokenType.Number:
                    value = reader.GetInt64();
                    break;
            }
            return value;
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

        public static readonly LongJsonConverter Instance = new LongJsonConverter();
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
            long? value = null;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    if (long.TryParse(reader.GetString(), out var numValue))
                    {
                        value = numValue;
                    }
                    break;
                case JsonTokenType.Number:
                    value = reader.GetInt64();
                    break;
            }
            return value;
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

        public static readonly LongAllowNullJsonConverter Instance = new LongAllowNullJsonConverter();
    }

    /// <summary>
    /// Unsign long number json converter
    /// </summary>
#pragma warning disable CS3009 // 基类型不符合 CLS
    public class ULongJsonConverter : System.Text.Json.Serialization.JsonConverter<ulong>
#pragma warning restore CS3009 // 基类型不符合 CLS
    {
        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="typeToConvert">Type convert</param>
        /// <param name="options">Options</param>
        /// <returns>Return big number value</returns>
#pragma warning disable CS3002 // 返回类型不符合 CLS
        public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
#pragma warning restore CS3002 // 返回类型不符合 CLS
        {
            ulong value = 0;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    ulong.TryParse(reader.GetString(), out value);
                    break;
                case JsonTokenType.Number:
                    value = reader.GetUInt64();
                    break;
            }
            return value;
        }

        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Value</param>
        /// <param name="options">Options</param>
#pragma warning disable CS3001 // 参数类型不符合 CLS
        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
#pragma warning restore CS3001 // 参数类型不符合 CLS
        {
            writer.WriteStringValue(value.ToString());
        }

        public static readonly ULongJsonConverter Instance = new ULongJsonConverter();
    }

    /// <summary>
    /// Unsign allow null long number json converter
    /// </summary>
#pragma warning disable CS3009 // 基类型不符合 CLS
    public class ULongAllowNullJsonConverter : System.Text.Json.Serialization.JsonConverter<ulong?>
#pragma warning restore CS3009 // 基类型不符合 CLS
    {
        /// <summary>
        /// Read value
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="typeToConvert">Type convert</param>
        /// <param name="options">Options</param>
        /// <returns>Return big number value</returns>
#pragma warning disable CS3002 // 返回类型不符合 CLS
        public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
#pragma warning restore CS3002 // 返回类型不符合 CLS
        {
            ulong? value = null;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    if (ulong.TryParse(reader.GetString(), out var numValue))
                    {
                        value = numValue;
                    }
                    break;
                case JsonTokenType.Number:
                    value = reader.GetUInt64();
                    break;
            }
            return value;
        }

        /// <summary>
        /// Write value
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Value</param>
        /// <param name="options">Options</param>
#pragma warning disable CS3001 // 参数类型不符合 CLS
        public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
#pragma warning restore CS3001 // 参数类型不符合 CLS
        {
            writer.WriteStringValue(value?.ToString() ?? string.Empty);
        }

        public static readonly ULongAllowNullJsonConverter Instance = new ULongAllowNullJsonConverter();
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
            decimal value = 0m;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    decimal.TryParse(reader.GetString(), out value);
                    break;
                case JsonTokenType.Number:
                    value = reader.GetDecimal();
                    break;
            }
            return value;
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

        public static readonly DecimalJsonConverter Instance = new DecimalJsonConverter();
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
            decimal? value = null;
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    if (decimal.TryParse(reader.GetString(), out var numValue))
                    {
                        value = numValue;
                    }
                    break;
                case JsonTokenType.Number:
                    value = reader.GetDecimal();
                    break;
            }
            return value;
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

        public static readonly DecimalAllowNullJsonConverter Instance = new DecimalAllowNullJsonConverter();
    }
}
