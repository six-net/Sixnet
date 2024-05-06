using System.Collections.Generic;
using System.Linq;
using Sixnet.Model;

namespace System
{
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Gets enum dictionary
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="startWithEnumName">Whether start with enum name</param>
        /// <param name="displayFriendly">Display friendly</param>
        /// <returns>Return a dictionary</returns>
        public static Dictionary<int, string> GetEnumDictionary<TEnum>(this TEnum enumValue, bool startWithEnumName = false, bool displayFriendly = true) where TEnum : struct, Enum
        {
            return enumValue.GetType().GetEnumValueAndNames(startWithEnumName, displayFriendly);
        }

        /// <summary>
        /// Gets enum key&value collection
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="startWithEnumName">Whether start with enum name</param>
        /// <param name="displayFriendly">Display friendly</param>
        /// <returns>Return a key&value collection</returns>
        public static KeyValueCollection<int, string> GetEnumKeyValueCollection<TEnum>(this TEnum enumValue, bool startWithEnumName = false, bool displayFriendly = true) where TEnum : struct, Enum
        {
            return new KeyValueCollection<int, string>(enumValue.GetEnumDictionary(startWithEnumName, displayFriendly).Select(c => c));
        }

        /// <summary>
        /// Get enum value&code collection
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="startWithEnumName">Whether start with enum name</param>
        /// <param name="displayFriendly">Display friendly</param>
        /// <returns>Return a name code & value collection</returns>
        public static List<NameValue<int>> GetEnumNameValues<TEnum>(this TEnum enumValue, bool startWithEnumName = true, bool displayFriendly = false) where TEnum : struct, Enum
        {
            var enumDict = enumValue.GetEnumDictionary(startWithEnumName, displayFriendly);
            return enumDict.Select(c => new NameValue<int>()
            {
                Value = c.Key,
                Name = c.Value
            }).ToList();
        }

        /// <summary>
        /// Get enum 
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="startWithEnumName">Whether start with enum name</param>
        /// <param name="displayFriendly">Display friendly</param>
        /// <returns></returns>
        public static string GetEnumName<TEnum>(this TEnum enumValue, bool startWithEnumName = true, bool displayFriendly = false) where TEnum : struct, Enum
        {
            var intValue = Convert.ToInt32(enumValue);
            var enumDict = enumValue.GetEnumDictionary(startWithEnumName, displayFriendly);
            if (enumDict?.ContainsKey(intValue) ?? false)
            {
                return enumDict[intValue];
            }
            return enumValue.ToString();
        }
    }
}
