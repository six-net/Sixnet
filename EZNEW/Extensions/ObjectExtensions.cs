using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using EZNEW.DependencyInjection;
using EZNEW.Mapper;
using EZNEW.Serialization;
using EZNEW.Serialization.Json;

namespace System
{
    /// <summary>
    /// Object extensions
    /// </summary>
    public static class ObjectExtensions
    {
        #region Generate a dictionary by an object

        /// <summary>
        /// Generate a dictionary by an object
        /// </summary>
        /// <param name="value">Object</param>
        /// <returns>Return the dictionary value</returns>
        public static Dictionary<string, object> ObjectToDcitionary(this object value)
        {
            if (value == null)
            {
                return new Dictionary<string, object>(0);
            }
            if (value is Dictionary<string, object> valueDict)
            {
                return valueDict;
            }
            if (value is IEnumerable<KeyValuePair<string, object>> keyValuePairs)
            {
                return keyValuePairs.ToDictionary(c => c.Key, c => c.Value);
            }
            PropertyDescriptorCollection nowPropertyCollection = TypeDescriptor.GetProperties(value);
            Dictionary<string, object> valueDictionary = new Dictionary<string, object>(nowPropertyCollection.Count);
            foreach (PropertyDescriptor ps in nowPropertyCollection)
            {
                valueDictionary.Add(ps.Name, ps.GetValue(value));
            }
            return valueDictionary;
        }

        /// <summary>
        /// Generate a dictionary by an object
        /// </summary>
        /// <param name="value">Object</param>
        /// <returns>Return the dictionary value</returns>
        public static Dictionary<string, string> ObjectToStringDcitionary(this object value)
        {
            if (value == null)
            {
                return new Dictionary<string, string>(0);
            }
            PropertyDescriptorCollection nowPropertyCollection = TypeDescriptor.GetProperties(value);
            Dictionary<string, string> valueDictionary = new Dictionary<string, string>(nowPropertyCollection.Count);
            foreach (PropertyDescriptor ps in nowPropertyCollection)
            {
                valueDictionary.Add(ps.Name, ps.GetValue(value)?.ToString() ?? string.Empty);
            }
            return valueDictionary;
        }

        #endregion

        #region Get a string by an object

        /// <summary>
        /// Get a string by an object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Return the string value</returns>
        public static string ToNullableString(this object obj)
        {
            return obj?.ToString() ?? string.Empty;
        }

        #endregion

        #region Object map

        /// <summary>
        /// Map an object to an other object
        /// </summary>
        /// <typeparam name="TTarget">Target data type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>Return the target object</returns>
        public static TTarget MapTo<TTarget>(this object source)
        {
            return ObjectMapper.MapTo<TTarget>(source);
        }

        #endregion

        #region Object deep clone

        /// <summary>
        /// Object deep clone
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Return the new object</returns>
        public static T FieldDeepClone<T>(this T sourceObject, ObjectCloneMethod cloneMethod = ObjectCloneMethod.Json)
        {
            if (cloneMethod == ObjectCloneMethod.Binary)
            {
                return DeepCloneByBinary(sourceObject);
            }
            else
            {
                return FieldDeepCloneByJson(sourceObject);
            }
        }

        /// <summary>
        /// Object deep clone
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Return the new object</returns>
        public static T DeepClone<T>(this T sourceObject, ObjectCloneMethod cloneMethod = ObjectCloneMethod.Json)
        {
            if (cloneMethod == ObjectCloneMethod.Binary)
            {
                return DeepCloneByBinary(sourceObject);
            }
            else
            {
                return DeepCloneByJson(sourceObject);
            }
        }

        static T FieldDeepCloneByJson<T>(T sourceObject)
        {
            if (sourceObject == null)
            {
                return default(T);
            }
            var objectString = JsonSerializer.Serialize(sourceObject, true);
            return JsonSerializer.Deserialize<T>(objectString, new JsonSerializationOptions()
            {
                ResolveNonPublic = true,
                DeserializeType = sourceObject.GetType()
            });
        }

        static T DeepCloneByJson<T>(T sourceObject)
        {
            if (sourceObject == null)
            {
                return default(T);
            }
            var objectString = JsonSerializer.Serialize(sourceObject, false);
            return JsonSerializer.Deserialize<T>(objectString, new JsonSerializationOptions()
            {
                ResolveNonPublic = false,
                DeserializeType = sourceObject.GetType()
            });
        }

        /// <summary>
        /// Clone by binary
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Return the new object</returns>
        static T DeepCloneByBinary<T>(T sourceObject)
        {
            if (sourceObject == null)
            {
                return default;
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, sourceObject);
                memoryStream.Position = 0;
                var data = (T)formatter.Deserialize(memoryStream);
                return data;
            }
        }

        #endregion

        #region Convert object to Int32

        /// <summary>
        /// Convert object to Int32
        /// </summary>
        /// <param name="value">Object value</param>
        /// <returns>Return the int32 value</returns>
        public static int ObjectToInt32(this object value)
        {
            if (value == null)
            {
                return 0;
            }
            int newIntValue = 0;
            int.TryParse(value.ToString(), out newIntValue);
            return newIntValue;
        }

        #endregion

        #region Convert object to int64

        /// <summary>
        /// convert object to int64
        /// </summary>
        /// <param name="value">Object value</param>
        /// <returns>Return the int64 value</returns>
        public static long ObjectToInt64(this object value)
        {
            if (value == null)
            {
                return 0;
            }
            long newIntValue = 0;
            long.TryParse(value.ToString(), out newIntValue);
            return newIntValue;
        }

        #endregion
    }

    /// <summary>
    /// Defines object clone method
    /// </summary>
    [Serializable]
    public enum ObjectCloneMethod
    {
        Binary = 2,
        Json = 4
    }
}
