using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sixnet.Extensions;
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
        /// <param name="options">Options</param>
        /// <returns>Return a dictionary</returns>
        public static Dictionary<int, string> GetEnumDictionary<TEnum>(this TEnum enumValue, SixnetEnumOptions options = null) where TEnum : struct, Enum
        {
            return enumValue.GetType().GetEnumValueAndNames(options);
        }

        /// <summary>
        /// Get enum value&code collection
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="options">Enum options</param>
        /// <returns>Return a name code & value collection</returns>
        public static List<NameValue<int>> GetEnumNameValues<TEnum>(this TEnum enumValue, SixnetEnumOptions options = null) where TEnum : struct, Enum
        {
            var enumDict = enumValue.GetEnumDictionary(options);
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
        /// <param name="options">Enum options</param>
        /// <returns></returns>
        public static string GetEnumName<TEnum>(this TEnum enumValue, SixnetEnumOptions options = null) where TEnum : struct, Enum
        {
            var intValue = Convert.ToInt32(enumValue);
            var enumDict = enumValue.GetEnumDictionary(options);
            if (enumDict?.ContainsKey(intValue) ?? false)
            {
                return enumDict[intValue];
            }
            return enumValue.ToString();
        }
    }
}
