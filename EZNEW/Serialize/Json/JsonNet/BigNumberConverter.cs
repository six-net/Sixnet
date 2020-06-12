using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EZNEW.Serialize.Json.JsonNet
{
    /// <summary>
    /// Provides a conversion implementation for Int64 serialization
    /// </summary>
    public class BigNumberConverter : JsonConverter
    {
        static readonly HashSet<Guid> AllowTypeValues = new HashSet<Guid> { typeof(long).GUID, typeof(long?).GUID, typeof(ulong).GUID, typeof(ulong?).GUID, typeof(decimal).GUID, typeof(decimal?).GUID };

        /// <summary>
        /// Determines whether the specified data type is allowed to be converted
        /// </summary>
        /// <param name="objectType">Data type</param>
        /// <returns>Return whether allowed to be converted</returns>
        public override bool CanConvert(Type objectType)
        {
            return AllowTypeValues.Contains(objectType.GUID);
        }

        /// <summary>
        /// Read the data from the JSON data
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="objectType">Object type</param>
        /// <param name="existingValue">Existing value</param>
        /// <param name="serializer">Json serializer</param>
        /// <returns>Return the object data</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return existingValue;
        }

        /// <summary>
        /// Write the object data to JSON
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">Object value</param>
        /// <param name="serializer">Json serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string valueString = value.ToString();
            writer.WriteValue(valueString);
        }
    }
}
