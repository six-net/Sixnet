using EZNEW.Framework.Extension;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    ///entity field
    /// </summary>
    public class EntityField : IComparable
    {
        #region propertys

        /// <summary>
        /// name
        /// </summary>
        public string FieldName
        {
            get; set;
        }

        /// <summary>
        /// property name
        /// </summary>
        public string PropertyName
        {
            get; set;
        }

        /// <summary>
        /// query format
        /// </summary>
        public string QueryFormat
        {
            get; set;
        }

        /// <summary>
        /// is primary key
        /// </summary>
        public bool IsPrimaryKey
        {
            get; set;
        }

        /// <summary>
        /// cache option
        /// </summary>
        public EntityFieldCacheOption CacheOption
        {
            get; set;
        }

        /// <summary>
        /// is disable query
        /// </summary>
        public bool IsDisableQuery
        {
            get; set;
        }

        /// <summary>
        /// is disable edit
        /// </summary>
        public bool IsDisableEdit
        {
            get; set;
        }

        /// <summary>
        /// is version field
        /// </summary>
        public bool IsVersion
        {
            get; set;
        }

        /// <summary>
        /// is refresh date
        /// </summary>
        public bool IsRefreshDate
        {
            get; set;
        }

        /// <summary>
        /// data type
        /// </summary>
        public Type DataType 
        {
            get;set;
        }

        /// <summary>
        /// query format fields
        /// </summary>
        ConcurrentDictionary<string, string> queryFormatFields = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// edit format fields
        /// </summary>
        ConcurrentDictionary<string, string> editFormatFields = new ConcurrentDictionary<string, string>();

        #endregion

        #region methods

        /// <summary>
        /// implicit convert to string
        /// </summary>
        /// <param name="field"></param>
        public static implicit operator string(EntityField field)
        {
            return field?.PropertyName ?? string.Empty;
        }

        /// <summary>
        /// implicit convert to entityfield
        /// </summary>
        /// <param name="value">value</param>
        public static implicit operator EntityField(string value)
        {
            return new EntityField() { FieldName = value, PropertyName = value };
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            var targetObj = obj as EntityField;
            if (targetObj == null)
            {
                return false;
            }
            return targetObj.PropertyName == PropertyName;
        }

        public override string ToString()
        {
            return PropertyName;
        }

        public int CompareTo(object obj)
        {
            var targetField = obj as EntityField;
            if (targetField == null)
            {
                return 1;
            }
            return PropertyName.CompareTo(targetField.PropertyName);
        }

        /// <summary>
        /// get query format
        /// </summary>
        /// <param name="typeKey">type key</param>
        /// <returns></returns>
        public string GetQueryFormat(string typeKey)
        {
            queryFormatFields.TryGetValue(typeKey, out string value);
            return value ?? string.Empty;
        }

        /// <summary>
        /// set query format value
        /// </summary>
        /// <param name="typeKey">type key</param>
        /// <param name="formatValue">format value</param>
        /// <returns></returns>
        public void SetQueryFormat(string typeKey, string formatValue)
        {
            if (typeKey.IsNullOrEmpty() || formatValue.IsNullOrEmpty())
            {
                return;
            }
            queryFormatFields.TryAdd(typeKey, formatValue);
        }

        /// <summary>
        /// get edit format value
        /// </summary>
        /// <param name="typeKey"></param>
        /// <returns></returns>
        public string GetEditFormat(string typeKey)
        {
            editFormatFields.TryGetValue(typeKey, out string value);
            return value ?? string.Empty;
        }

        /// <summary>
        /// set eidt format value
        /// </summary>
        /// <param name="typeKey"></param>
        /// <param name="formatValue"></param>
        public void SetEditFormat(string typeKey, string formatValue)
        {
            if (typeKey.IsNullOrEmpty() || formatValue.IsNullOrEmpty())
            {
                return;
            }
            editFormatFields.TryAdd(typeKey, formatValue);
        }

        #endregion
    }
}
