using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Type extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Allow null type
        /// </summary>
        static readonly Type AllowNullType = typeof(Nullable<>);

        #region Generate dictionary by enum

        /// <summary>
        /// Generate dictionary by enum
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <returns>Return the dictionary value</returns>
        public static Dictionary<int, string> GetEnumValueAndNames(this Type enumType)
        {
            if (enumType == null)
            {
                return new Dictionary<int, string>(0);
            }
            var values = Enum.GetValues(enumType);
            Dictionary<int, string> enumValues = new Dictionary<int, string>();
            foreach (int val in values)
            {
                enumValues.Add(val, Enum.GetName(enumType, val));
            }
            return enumValues;
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
    }
}
