using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace System
{
    /// <summary>
    /// Type extensions
    /// </summary>
    public static class TypeExtensions
    {
        #region Fields

        /// <summary>
        /// Allow null type
        /// </summary>
        static readonly Type AllowNullType = typeof(Nullable<>);

        /// <summary>
        /// Enum value and names
        /// </summary>
        static readonly ConcurrentDictionary<string, Dictionary<int, string>> CacheEnumValueAndNames = new ConcurrentDictionary<string, Dictionary<int, string>>(); 

        #endregion

        #region Generate dictionary by enum

        /// <summary>
        /// Generate dictionary by enum
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="displayFriendly">Display friendly</param>
        /// <returns>Return the dictionary value</returns>
        public static Dictionary<int, string> GetEnumValueAndNames(this Type enumType, bool displayFriendly = true)
        {
            if (enumType == null)
            {
                return new Dictionary<int, string>(0);
            }

            string formatedKey = $"{enumType.GUID}_{displayFriendly}";
            if (CacheEnumValueAndNames.TryGetValue(formatedKey, out var valueAndNames))
            {
                return valueAndNames ?? new Dictionary<int, string>(0);
            }
            var values = Enum.GetValues(enumType);
            Dictionary<int, string> enumValues = new Dictionary<int, string>();
            var displayAttrType = typeof(DisplayAttribute);
            foreach (int val in values)
            {
                var enumName = Enum.GetName(enumType, val);
                if (displayFriendly)
                {
                    var enumField = enumType.GetField(enumName);

                    if (enumField != null && enumField.IsDefined(displayAttrType, false))
                    {
                        var displayName = (enumField.GetCustomAttributes(displayAttrType, false).First() as DisplayAttribute)?.Name;
                        enumName = string.IsNullOrWhiteSpace(displayName) ? enumName : displayName;
                    }
                }
                enumValues.Add(val, enumName);
            }
            CacheEnumValueAndNames[formatedKey] = enumValues;
            return enumValues;
        }

        #endregion

        #region Get enum display name

        /// <summary>
        /// Get enum display name
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns></returns>
        public static string GetEnumDisplayName(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var valueAndNames = GetEnumValueAndNames(enumType, true);
            valueAndNames.TryGetValue(Convert.ToInt32(enumValue), out string displayName);
            return displayName ?? enumValue.ToString();
        }

        #endregion

        #region Allow set null value

        /// <summary>
        /// Allow set null value
        /// </summary>
        /// <param name="type">Type object</param>
        /// <returns>Return whether the type instance allow to set null</returns>
        public static bool AllowNull(this Type type)
        {
            return !type.IsValueType || (type.IsGenericType && AllowNullType.Equals(type.GetGenericTypeDefinition()));
        }

        #endregion

        #region Get real value type

        /// <summary>
        /// Get real value type
        /// </summary>
        /// <param name="originalType">Original type</param>
        /// <returns>Return the real value type</returns>
        public static Type GetRealValueType(this Type originalType)
        {
            if (originalType.IsGenericType && typeof(Nullable<>).Equals(originalType.GetGenericTypeDefinition()))
            {
                return originalType.GenericTypeArguments[0];
            }
            return originalType;
        }

        #endregion
    }
}
