﻿using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Sixnet.Exceptions;
using Sixnet.Mapper;
using Sixnet.Model;
using Sixnet.Serialization.Json;
using Sixnet.Validation;
using static Sixnet.Validation.ValidationConstants;

namespace System
{
    /// <summary>
    /// Object extensions
    /// </summary>
    public static class ObjectExtensions
    {
        #region Generate an object dictionary

        /// <summary>
        /// Generate an object dictionary
        /// </summary>
        /// <param name="value">Object</param>
        /// <returns>Return a dictionary</returns>
        public static Dictionary<string, object> ToObjectDictionary(this object value)
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
            var nowPropertyCollection = TypeDescriptor.GetProperties(value);
            var valueDictionary = new Dictionary<string, object>(nowPropertyCollection.Count);
            foreach (PropertyDescriptor ps in nowPropertyCollection)
            {
                valueDictionary.Add(ps.Name, ps.GetValue(value));
            }
            return valueDictionary;
        }

        /// <summary>
        /// Generate a dynamic dictionary
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static Dictionary<string, dynamic> ToDynamicDictionary(this object value)
        {
            if (value == null)
            {
                return new Dictionary<string, dynamic>(0);
            }
            if (value is Dictionary<string, dynamic> valueDict)
            {
                return valueDict;
            }
            if (value is IEnumerable<KeyValuePair<string, dynamic>> keyValuePairs)
            {
                return keyValuePairs.ToDictionary(c => c.Key, c => c.Value);
            }
            var nowPropertyCollection = TypeDescriptor.GetProperties(value);
            var valueDictionary = new Dictionary<string, dynamic>(nowPropertyCollection.Count);
            foreach (PropertyDescriptor ps in nowPropertyCollection)
            {
                valueDictionary.Add(ps.Name, ps.GetValue(value));
            }
            return valueDictionary;
        }

        /// <summary>
        /// Generate a string dictionary
        /// </summary>
        /// <param name="value">Object</param>
        /// <returns>Return the dictionary value</returns>
        public static Dictionary<string, string> ToStringDictionary(this object value)
        {
            if (value == null)
            {
                return new Dictionary<string, string>(0);
            }
            var nowPropertyCollection = TypeDescriptor.GetProperties(value);
            var valueDictionary = new Dictionary<string, string>(nowPropertyCollection.Count);
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
        /// Return string.Empty if data is null
        /// </summary>
        /// <param name="data">Object</param>
        /// <returns>Return the string value</returns>
        public static string ToNullableString(this object data)
        {
            return data?.ToString() ?? string.Empty;
        }

        #endregion

        #region Map object

        /// <summary>
        /// Map an object to an other object
        /// </summary>
        /// <typeparam name="TTarget">Target data type</typeparam>
        /// <param name="source">Source object</param>
        /// <returns>Return the target object</returns>
        public static TTarget MapTo<TTarget>(this object source)
        {
            return SixnetMapper.MapTo<TTarget>(source);
        }

        #endregion

        #region Convert object to Int32

        /// <summary>
        /// Convert object to Int32
        /// </summary>
        /// <param name="value">Object value</param>
        /// <returns>Return the int32 value</returns>
        public static int ToInt32(this object value)
        {
            int newIntValue = 0;
            if (value != null)
            {
                if (value is Guid guidValue)
                {
                    newIntValue = guidValue.GuidToInt32();
                }
                else
                {
                    int.TryParse(value.ToString(), out newIntValue);
                }
            }
            return newIntValue;
        }

        #endregion

        #region Convert object to int64

        /// <summary>
        /// convert object to int64
        /// </summary>
        /// <param name="value">Object value</param>
        /// <returns>Return the int64 value</returns>
        public static long ToInt64(this object value)
        {
            long newIntValue = 0;
            if (value != null)
            {
                if (value is Guid guidValue)
                {
                    newIntValue = guidValue.GuidToInt64();
                }
                else
                {
                    long.TryParse(value.ToString(), out newIntValue);
                }
            }
            return newIntValue;
        }

        #endregion

        #region Convert

        /// <summary>
        /// Convert data type
        /// </summary>
        /// <typeparam name="T">Target data type</typeparam>
        /// <param name="data">Data</param>
        /// <returns>Return the target data</returns>
        public static T ConvertTo<T>(this object data)
        {
            return ConvertTo(data, typeof(T));
        }

        /// <summary>
        /// Convert data type
        /// </summary>
        /// <param name="targetType">Target data type</param>
        /// <param name="data">Data</param>
        /// <returns>Return the targete data</returns>
        public static dynamic ConvertTo(this object data, Type targetType)
        {
            if (data != null)
            {
                return Convert.ChangeType(data, targetType);
            }
            return null;
        }

        #endregion

        #region Validate

        /// <summary>
        /// Validate object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="useCase"></param>
        public static void Validate(this object obj, string nullMsg = "", bool allowNull = false, string useCase = UseCaseNames.Domain)
        {
            SixnetDirectThrower.ThrowArgNullIf(obj == null && !allowNull, string.IsNullOrEmpty(nullMsg) ? nameof(obj) : nullMsg);
            if (obj != null)
            {
                var verifyResults = SixnetValidations.Validate(obj, useCase);
                var errorMessages = verifyResults.GetErrorMessages(true);
                SixnetDirectThrower.ThrowSixnetExceptionIf(!errorMessages.IsNullOrEmpty(), SixnetJsonSerializer.Serialize(errorMessages));
            }
        }

        #endregion
    }
}
