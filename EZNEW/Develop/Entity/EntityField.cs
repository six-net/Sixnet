using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    ///Entity field
    /// </summary>
    public class EntityField : IComparable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string FieldName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        public string PropertyName
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the query format
        /// </summary>
        public string QueryFormat
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether the property is primary key
        /// </summary>
        public bool IsPrimaryKey
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets cache option
        /// </summary>
        public EntityFieldCacheOption CacheOption
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether disable the field in query
        /// </summary>
        public bool IsDisableQuery
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether disable the field in edit
        /// </summary>
        public bool IsDisableEdit
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether the property is version field
        /// </summary>
        public bool IsVersion
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether the property is refresh data field
        /// </summary>
        public bool IsRefreshDate
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        public Type DataType
        {
            get; set;
        }

        /// <summary>
        /// query format fields
        /// </summary>
        readonly ConcurrentDictionary<string, string> queryFormatFields = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// edit format fields
        /// </summary>
        readonly ConcurrentDictionary<string, string> editFormatFields = new ConcurrentDictionary<string, string>();

        #endregion

        #region Methods

        /// <summary>
        /// Implicit convert to string
        /// </summary>
        /// <param name="field">field</param>
        public static implicit operator string(EntityField field)
        {
            return field?.PropertyName ?? string.Empty;
        }

        /// <summary>
        /// Implicit convert to entity field
        /// </summary>
        /// <param name="value">Value</param>
        public static implicit operator EntityField(string value)
        {
            return new EntityField() { FieldName = value, PropertyName = value };
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
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
        /// Get query format
        /// </summary>
        /// <param name="typeKey">Type key</param>
        /// <returns>Return query format</returns>
        public string GetQueryFormat(string typeKey)
        {
            queryFormatFields.TryGetValue(typeKey, out string value);
            return value ?? string.Empty;
        }

        /// <summary>
        /// Set query format value
        /// </summary>
        /// <param name="typeKey">Type key</param>
        /// <param name="formatValue">Format value</param>
        /// <returns></returns>
        public void SetQueryFormat(string typeKey, string formatValue)
        {
            if (string.IsNullOrWhiteSpace(typeKey) || string.IsNullOrWhiteSpace(formatValue))
            {
                return;
            }
            queryFormatFields.TryAdd(typeKey, formatValue);
        }

        /// <summary>
        /// Get edit format value
        /// </summary>
        /// <param name="typeKey">Type key</param>
        /// <returns>Return edit format value</returns>
        public string GetEditFormat(string typeKey)
        {
            editFormatFields.TryGetValue(typeKey, out string value);
            return value ?? string.Empty;
        }

        /// <summary>
        /// Set eidt format value
        /// </summary>
        /// <param name="typeKey">Type key</param>
        /// <param name="formatValue">Format value</param>
        public void SetEditFormat(string typeKey, string formatValue)
        {
            if (string.IsNullOrWhiteSpace(typeKey) || string.IsNullOrWhiteSpace(formatValue))
            {
                return;
            }
            editFormatFields.TryAdd(typeKey, formatValue);
        }

        #endregion
    }
}
