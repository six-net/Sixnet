using System.Collections;
using System.Reflection;

namespace Sixnet.DataValidation.Validators
{
    /// <summary>
    /// Count property helper
    /// </summary>
    internal static class CountPropertyHelper
    {
        /// <summary>
        /// Try get count
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="count">Count</param>
        /// <returns>Return whether to get data count</returns>
        public static bool TryGetCount(object value, out int count)
        {
            if (value is ICollection collection)
            {
                count = collection.Count;
                return true;
            }

            PropertyInfo property = value.GetType().GetRuntimeProperty("Count");
            if (property != null && property.CanRead && property.PropertyType == typeof(int))
            {
                count = (int)property.GetValue(value);
                return true;
            }
            count = -1;
            return false;
        }
    }
}
