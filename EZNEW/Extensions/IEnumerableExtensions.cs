using System.Linq;
using EZNEW.ValueType;

namespace System.Collections.Generic
{
    /// <summary>
    /// Collection extensions
    /// </summary>
    public static class IEnumerableExtensions
    {
        #region Determines whether the collection is null or empty

        /// <summary>
        /// Determines whether the collection is null or empty
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns>Return whether value is null or empty</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> datas)
        {
            return datas == null || !datas.Any();
        }

        #endregion

        #region Dictionary extension methods

        #region Dynamic dictionary

        /// <summary>
        /// Set value to the dictionary,update current value if the key already exists or add if not
        /// </summary>
        /// <param name="dict">Dictionary value</param>
        /// <param name="name">Key name</param>
        /// <param name="value">Value</param>
        public static void SetValue(this IDictionary<dynamic, dynamic> dict, dynamic name, dynamic value)
        {
            if (dict == null)
            {
                return;
            }
            dict[name] = value;
        }

        /// <summary>
        /// Get value from the dictionary,return default value if the key doesn't exists
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="dict">Dictionary</param>
        /// <param name="name">Key name</param>
        /// <returns>Return the value</returns>
        public static T GetValue<T>(this IDictionary<dynamic, dynamic> dict, dynamic name)
        {
            dynamic value = default(T);
            if (dict != null && dict.ContainsKey(name))
            {
                value = dict[name];
            }
            if (!(value is T))
            {
                return DataConverter.Convert<T>(value);
            }
            return value;
        }

        #endregion

        #region String dictionary

        /// <summary>
        /// Set value to the dictionary,update current value if the key already exists or add if not
        /// </summary>
        /// <param name="dict">dictionary</param>
        /// <param name="name">key name</param>
        /// <param name="value">value</param>
        public static void SetValue(this IDictionary<string, dynamic> dict, string name, dynamic value)
        {
            if (dict == null)
            {
                return;
            }
            dict[name] = value;
        }

        /// <summary>
        /// Get value from the dictionary,return default value if the key doesn't exists
        /// </summary>
        /// <param name="dict">dictionary</param>
        /// <param name="name">key name</param>
        /// <param name="value">value</param>
        public static void SetValue<T>(this IDictionary<string, dynamic> dict, string name, T value)
        {
            if (dict == null)
            {
                return;
            }
            dict[name] = value;
        }

        /// <summary>
        /// Get value from the dictionary,return default value if the key doesn't exists
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="dict">dictionary</param>
        /// <param name="name">key name</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDictionary<string, dynamic> dict, string name)
        {
            dynamic value = default(T);
            if (dict != null && dict.ContainsKey(name))
            {
                value = dict[name];
            }
            if (!(value is T))
            {
                return DataConverter.Convert<T>(value);
            }
            return value;
        }

        #endregion

        #endregion

        #region Get count

        /// <summary>
        /// Get count
        /// </summary>
        /// <param name="values">Values</param>
        /// <returns>Return count value</returns>
        public static int GetCount<T>(this IEnumerable<T> values)
        {
            if (values is ICollection collection)
            {
                return collection.Count;
            }
            return values?.Count() ?? 0;
        }

        #endregion
    }
}
