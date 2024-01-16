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
        /// <param name="displayFriendly">Display friendly</param>
        /// <returns>Return a dictionary</returns>
        public static Dictionary<int, string> GetEnumDictionary<TEnum>(this TEnum enumValue, bool displayFriendly = true) where TEnum : struct, Enum
        {
            return enumValue.GetType().GetEnumValueAndNames(displayFriendly);
        }

        /// <summary>
        /// Gets enum key&value collection
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="displayFriendly">Display friendly</param>
        /// <returns>Return a key&value collection</returns>
        public static KeyValueCollection<int, string> GetEnumKeyValueCollection<TEnum>(this TEnum enumValue, bool displayFriendly = true) where TEnum : struct, Enum
        {
            return new KeyValueCollection<int, string>(enumValue.GetEnumDictionary(displayFriendly).Select(c => c));
        }
    }
}
