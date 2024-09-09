using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using Sixnet.Development.Data.Field;
using Sixnet.Exceptions;
using Sixnet.Model;

namespace System.Collections.Generic
{
    /// <summary>
    /// Collection extensions
    /// </summary>
    public static class IEnumerableExtensions
    {
        #region Whether the collection is null or empty

        /// <summary>
        /// Whether the collection is null or empty
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
            if (dict != null)
            {
                dict[name] = value;
            }
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
            var value = default(T);
            if (dict != null && dict.ContainsKey(name))
            {
                value = dict[name];
                if (value is not T)
                {
                    return value.ConvertTo<T>();
                }
            }
            return value;
        }

        /// <summary>
        /// Get value from the dictionary,return default value if the key doesn't exists
        /// </summary>
        /// <param name="dict">Dictionary</param>
        /// <param name="name">Key name</param>
        /// <returns>Return the value</returns>
        public static FieldsAssignment GetFieldsAssignment(this IDictionary<string, dynamic> dict)
        {
            var fieldsAssignment = new FieldsAssignment();

            if (!dict.IsNullOrEmpty())
            {
                foreach (var valueItem in dict)
                {
                    fieldsAssignment.SetNewValue(valueItem.Key, valueItem.Value);
                }
            }

            return fieldsAssignment;
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
            if (dict != null)
            {
                dict[name] = value;
            }
        }

        /// <summary>
        /// Get value from the dictionary,return default value if the key doesn't exists
        /// </summary>
        /// <param name="dict">dictionary</param>
        /// <param name="name">key name</param>
        /// <param name="value">value</param>
        public static void SetValue<T>(this IDictionary<string, dynamic> dict, string name, T value)
        {
            if (dict != null)
            {
                dict[name] = value;
            }
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
            var value = default(T);
            if (dict != null && dict.ContainsKey(name))
            {
                value = dict[name];
                if (value is not T)
                {
                    return value.ConvertTo<T>();
                }
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
                table.Columns.Add(property.Name, property.PropertyType.GetRealValueType());
            }
            foreach (var field in fields)
            {
                table.Columns.Add(field.Name, field.FieldType.GetRealValueType());
            }
            if (!datas.IsNullOrEmpty())
            {
                foreach (var data in datas)
                {
                    DataRow row = table.NewRow();
                    foreach (var property in properties)
                    {
                        var propertyVal = property.GetValue(data);
                        propertyVal ??= DBNull.Value;
                        row[property.Name] = propertyVal;
                    }
                    foreach (var field in fields)
                    {
                        var fieldVal = field.GetValue(data);
                        fieldVal ??= DBNull.Value;
                        row[field.Name] = fieldVal;
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

        #region Get cascading value

        public static List<SixnetCascadingValue<TValue>> GetCascadingValues<TModel, TValue>(this IEnumerable<TModel> datas
            , Func<TModel, bool> topDataFilter
            , Func<TModel, string> labelSelector
            , Func<TModel, TValue> valueSelector
            , Func<TModel, TValue> parentValueSelector
            , Func<TModel, double> sequenceSelector = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<SixnetCascadingValue<TValue>>(0);
            }
            SixnetDirectThrower.ThrowArgNullIf(topDataFilter == null, nameof(topDataFilter));
            SixnetDirectThrower.ThrowArgNullIf(labelSelector == null, nameof(labelSelector));
            SixnetDirectThrower.ThrowArgNullIf(valueSelector == null, nameof(valueSelector));
            SixnetDirectThrower.ThrowArgNullIf(parentValueSelector == null, nameof(parentValueSelector));

            var topDatas = datas.Where(topDataFilter).OrderBy(c => sequenceSelector?.Invoke(c) ?? 0).ToList();
            if (topDatas.IsNullOrEmpty())
            {
                return new List<SixnetCascadingValue<TValue>>(0);
            }
            var values = new List<SixnetCascadingValue<TValue>>();
            foreach (var data in topDatas)
            {
                var newCascadingValue = new SixnetCascadingValue<TValue>()
                {
                    Label = labelSelector(data),
                    Value = valueSelector(data),
                    Sequence = sequenceSelector?.Invoke(data) ?? 0,
                    Level = 1
                };
                ResolveCascadingChildValues(2, newCascadingValue, datas, labelSelector, valueSelector, parentValueSelector, sequenceSelector);
                values.Add(newCascadingValue);
            }
            return values;
        }

        static void ResolveCascadingChildValues<TModel, TValue>(int level, SixnetCascadingValue<TValue> parentValue, IEnumerable<TModel> datas
            , Func<TModel, string> labelSelector
            , Func<TModel, TValue> valueSelector
            , Func<TModel, TValue> parentValueSelector
            , Func<TModel, double> sequenceSelector = null)
        {
            if (parentValue == null || datas.IsNullOrEmpty())
            {
                return;
            }
            var childDatas = datas.Where(c => parentValueSelector(c).Equals(parentValue.Value)).OrderBy(c => sequenceSelector?.Invoke(c) ?? 0).ToList();
            if (childDatas.IsNullOrEmpty())
            {
                return;
            }
            var childValues = new List<SixnetCascadingValue<TValue>>();
            foreach (var data in childDatas)
            {
                var newCascadingValue = new SixnetCascadingValue<TValue>()
                {
                    Label = labelSelector(data),
                    Value = valueSelector(data),
                    Sequence = sequenceSelector?.Invoke(data) ?? 0,
                    Level = level
                };
                ResolveCascadingChildValues(level++, newCascadingValue, datas, labelSelector, valueSelector, parentValueSelector, sequenceSelector);
                childValues.Add(newCascadingValue);
            }
            parentValue.Children = childValues;
        }

        #endregion

        #region Get name values

        /// <summary>
        /// Get name values
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="datas"></param>
        /// <param name="nameSelector"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static List<NameValue<TValue>> GetNameValues<TModel, TValue>(this IEnumerable<TModel> datas, Func<TModel, string> nameSelector, Func<TModel, TValue> valueSelector)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<NameValue<TValue>>(0);
            }
            SixnetDirectThrower.ThrowArgNullIf(nameSelector == null, nameof(nameSelector));
            SixnetDirectThrower.ThrowArgNullIf(valueSelector == null, nameof(valueSelector));

            return datas.Select(c => new NameValue<TValue>()
            {
                Name = nameSelector(c),
                Value = valueSelector(c)
            }).ToList();
        }

        #endregion

        #region Get tree data

        /// <summary>
        /// Get tree data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static List<T> ToTreeDatas<T>(this IEnumerable<T> datas
            , Func<IEnumerable<T>, IEnumerable<T>> levelOneDatasSelector
            , Func<IEnumerable<T>, T, List<T>> childrenSelector) where T : ISixnetTreeModel<T>
        {
            if (datas.IsNullOrEmpty() || levelOneDatasSelector == null || childrenSelector == null)
            {
                return new List<T>(0);
            }
            var levelOneDatas = levelOneDatasSelector(datas);
            if (levelOneDatas.IsNullOrEmpty())
            {
                return new List<T>(0);
            }
            var treeDatas = new List<T>();
            foreach (var topData in levelOneDatas)
            {
                ResolveChildren(datas, topData, childrenSelector);
                treeDatas.Add(topData);
            }
            return treeDatas;
        }

        static void ResolveChildren<T>(IEnumerable<T> datas, T parent
            , Func<IEnumerable<T>, T, List<T>> childrenSelector) where T : ISixnetTreeModel<T>
        {
            if (datas.IsNullOrEmpty() || parent == null)
            {
                return;
            }
            var children = childrenSelector(datas, parent);
            if (children.IsNullOrEmpty())
            {
                return;
            }
            parent.Children = children;
            foreach (var child in children)
            {
                ResolveChildren(datas, child, childrenSelector);
            }
        }

        #endregion
    }
}
