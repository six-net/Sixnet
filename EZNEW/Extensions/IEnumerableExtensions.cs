using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
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

        #region To xml

        /// <summary>
        /// Fasten the data set to XML
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Data set</param>
        /// <param name="nodeName">Node name,Use the data type name when it is null or empty</param>
        /// <param name="rootNodeName">Root node name,Use the default format like NodeName+"s" when is is null or empty</param>
        /// <param name="formatValue">Whether format value</param>
        /// <returns>Return xml</returns>
        public static string ToXml<T>(this IEnumerable<T> datas, string nodeName = "", string rootNodeName = "", bool formatValue = true)
        {
            if (string.IsNullOrWhiteSpace(nodeName))
            {
                nodeName = typeof(T).Name;
            }
            if (string.IsNullOrWhiteSpace(rootNodeName))
            {
                rootNodeName = $"{nodeName}s";
            }
            using (var sw = new StringWriter())
            {
                var writer = new XmlTextWriter(sw);
                writer.WriteStartElement(rootNodeName);
                if (!datas.IsNullOrEmpty())
                {
                    var dataType = typeof(T);
                    var boolType = typeof(bool);
                    var properties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    var fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (T data in datas)
                    {
                        writer.WriteStartElement(nodeName);
                        foreach (PropertyInfo property in properties)
                        {
                            try
                            {
                                object value = property.GetValue(data, null);
                                if (value != null && formatValue && (value.GetType().IsEnum || value.GetType() == boolType))
                                {
                                    value = Convert.ToInt32(value);
                                }
                                writer.WriteAttributeString(property.Name, value == null ? string.Empty : value.ToString());
                            }
                            catch
                            {
                                writer.WriteAttributeString(property.Name, DBNull.Value.ToString(CultureInfo.InvariantCulture));
                            }
                        }
                        foreach (FieldInfo field in fields)
                        {
                            try
                            {
                                object value = field.GetValue(data);
                                if (value != null && formatValue && formatValue && (value.GetType().IsEnum || value.GetType() == boolType))
                                {
                                    value = Convert.ToInt32(value); ;
                                }
                                writer.WriteAttributeString(field.Name, value == null ? string.Empty : value.ToString());
                            }
                            catch
                            {
                                writer.WriteAttributeString(field.Name, DBNull.Value.ToString(CultureInfo.InvariantCulture));
                            }
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                writer.Close();
                return sw.ToString();
            }
        }

        #endregion

        #region To datatable

        /// <summary>
        /// Fasten the data set to datatable
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Data set</param>
        /// <returns>Return datatable</returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> datas)
        {
            DataTable table = new DataTable();
            var dataType = typeof(T);
            var properties = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = dataType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                table.Columns.Add(property.Name, property.PropertyType);
            }
            foreach (var field in fields)
            {
                table.Columns.Add(field.Name, field.FieldType);
            }
            if (!datas.IsNullOrEmpty())
            {
                Type boolType = typeof(bool);
                foreach (var data in datas)
                {
                    DataRow row = table.NewRow();
                    foreach (var property in properties)
                    {
                        row[property.Name] = property.GetValue(data);
                    }
                    foreach (var field in fields)
                    {
                        row[field.Name] = field.GetValue(data);
                    }
                    table.Rows.Add(row);
                }
            }
            return table;
        }

        #endregion

        #region Write to csv file

        /// <summary>
        /// Write datas to a csv file
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="savePath">Save path,Will save file to the application root directory when the parameter value is null or empty</param>
        /// <param name="fileName">Without extension file name,Will use a random file name when the parameter value is null or empty</param>
        /// <param name="ignoreTitle">Whether ignore column name,Default is false</param>
        /// <param name="allowEmptyData">Whether generate file when datas is null or empty</param>
        /// <returns>Return the file full path</returns>
        public static string WriteToCSVFile<T>(this IEnumerable<T> datas, string savePath = "", string fileName = "", bool ignoreTitle = false, bool allowEmptyData = true)
        {
            if (datas.IsNullOrEmpty())
            {
                if (!allowEmptyData)
                {
                    return string.Empty;
                }
                datas = new T[0];
            }
            var dataTable = datas.ToDataTable();
            return dataTable.WriteToCSVFile(savePath, fileName, ignoreTitle);
        }

        #endregion
    }
}
