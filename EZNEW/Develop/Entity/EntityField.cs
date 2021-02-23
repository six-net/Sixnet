using System;

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
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the query format
        /// </summary>
        public string QueryFormat { get; set; }

        /// <summary>
        /// Gets or sets whether the property is primary key
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets cache option
        /// </summary>
        public FieldCacheRole CacheRole { get; set; }

        /// <summary>
        /// Gets or sets whether disable the field in query
        /// </summary>
        public bool IsDisableQuery { get; set; }

        /// <summary>
        /// Gets or sets whether disable the field in edit
        /// </summary>
        public bool IsDisableEdit { get; set; }

        /// <summary>
        /// Gets or sets whether the property is version field
        /// </summary>
        public bool IsVersion { get; set; }

        /// <summary>
        /// Gets or sets whether the property is refresh data field
        /// </summary>
        public bool IsRefreshDate { get; set; }

        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the database type name
        /// </summary>
        public string DbTypeName { get; set; }

        /// <summary>
        /// Gets or sets the max length
        /// </summary>
        public int MaxLength { get; set; } = -1;

        /// <summary>
        /// Whether is fixed length
        /// </summary>
        public bool IsFixedLength { get; set; }

        /// <summary>
        /// Whether is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Whether is parent field
        /// </summary>
        public bool IsParent { get; set; }

        /// <summary>
        /// Whether is sort field
        /// </summary>
        public bool IsSort { get; set; }

        /// <summary>
        /// Whether is level field
        /// </summary>
        public bool IsLevel { get; set; }

        /// <summary>
        /// Whether is display name field
        /// </summary>
        public bool IsDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the value provider
        /// </summary>
        internal IEntityPropertyValueProvider ValueProvider { get; set; }

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

        #endregion
    }
}
