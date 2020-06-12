using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace EZNEW.ValueType
{
    /// <summary>
    /// Data converter
    /// </summary>
    public static class DataConverter
    {
        /// <summary>
        /// Convert collections to xml
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="itemName">Root element name</param>
        /// <param name="dataList">Data list</param>
        /// <returns>Reeturn xml object</returns>
        public static object ConvertIEnumerableToXml<T>(string itemName, IEnumerable<T> dataList)
        {
            if (dataList == null)
            {
                return DBNull.Value;
            }
            using (var stringWriter = new StringWriter())
            {
                var xmlWriter = new XmlTextWriter(stringWriter);
                xmlWriter.WriteStartElement(itemName + "s");
                foreach (T data in dataList)
                {
                    xmlWriter.WriteStartElement(itemName);
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            xmlWriter.WriteAttributeString(property.Name, property.GetValue(data, null) == null ? "" : property.GetValue(data, null).ToString());
                        }
                        catch
                        {
                            xmlWriter.WriteAttributeString(property.Name, DBNull.Value.ToString(CultureInfo.CurrentCulture));
                        }
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.Close();
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// Convert collections to datatable
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="dataList">Data list</param>
        /// <returns>Return datatable</returns>
        public static DataTable ConvertIEnumerableToTable<T>(IEnumerable<T> dataList)
        {
            if (dataList == null || !dataList.Any())
            {
                return null;
            }
            DataTable table = new DataTable();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                table.Columns.Add(property.Name);
            }
            foreach (var data in dataList)
            {
                DataRow dataRow = table.NewRow();
                foreach (var property in properties)
                {
                    dataRow[property.Name] = property.GetValue(data);
                }
                table.Rows.Add(dataRow);
            }
            return table;
        }

        /// <summary>
        /// Convert data type
        /// </summary>
        /// <typeparam name="T">Target data type</typeparam>
        /// <param name="data">Data</param>
        /// <returns>Return the target data</returns>
        public static T Convert<T>(object data)
        {
            if (data == null)
            {
                return default;
            }
            return Convert(data, typeof(T));
        }

        /// <summary>
        /// Convert data type
        /// </summary>
        /// <param name="targetType">Target data type</param>
        /// <param name="data">Data</param>
        /// <returns>Return the targete data</returns>
        public static dynamic Convert(object data, Type targetType)
        {
            if (data == null)
            {
                return null;
            }
            return System.Convert.ChangeType(data, targetType);
        }

        /// <summary>
        /// Determine whether is a simple data type
        /// </summary>
        /// <param name="type">Data type</param>
        /// <returns>Return whether is simple data type</returns>
        public static bool IsSimpleType(Type type)
        {
            if (type == null)
            {
                return false;
            }
            var typeCode = Type.GetTypeCode(type);
            bool simpleType;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    simpleType = true;
                    break;
                default:
                    simpleType = false;
                    break;
            }
            return simpleType;
        }
    }
}
