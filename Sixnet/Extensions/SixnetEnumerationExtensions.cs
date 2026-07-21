// "Company © 2025. All rights reserved."

using Sixnet.Extensions;
using Sixnet.Model;

namespace System
{
    public static class SixnetEnumerationExtensions
    {
        /// <summary>
        /// Gets enum dictionary
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="configure">Configure options</param>
        /// <returns>Return a dictionary</returns>
        public static Dictionary<int, string> GetEnumDictionary<TEnum>(this TEnum enumValue, Action<SixnetEnumOptions> configure = null) where TEnum : Enum
        {
            return enumValue.GetType().GetEnumValueAndNames(configure);
        }

        /// <summary>
        /// Get enum value&code collection
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="configure">Configure options</param>
        /// <returns>Return a name code & value collection</returns>
        public static List<SixnetNameValue<int>> GetEnumNameValues<TEnum>(this TEnum enumValue, Action<SixnetEnumOptions> configure = null) where TEnum : Enum
        {
            var enumDict = enumValue.GetEnumDictionary(configure);
            return enumDict.Select(c => new SixnetNameValue<int>()
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
        /// <param name="configure">Configure options</param>
        /// <returns></returns>
        public static string GetEnumName<TEnum>(this TEnum enumValue, Action<SixnetEnumOptions> configure = null) where TEnum : Enum
        {
            var intValue = Convert.ToInt32(enumValue);
            var enumDict = enumValue.GetEnumDictionary(configure);
            if (enumDict?.ContainsKey(intValue) ?? false)
            {
                return enumDict[intValue];
            }
            return enumValue.ToString();
        }
    }
}
