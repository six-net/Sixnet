using System;
using System.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;

namespace Sixnet.Development.Entity
{
    /// <summary>
    ///Entity field
    /// </summary>
    public class EntityField : PropertyField, IComparable
    {
        #region Properties

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the field role
        /// </summary>
        public FieldRole Role { get; set; } = FieldRole.None;

        /// <summary>
        /// Gets or sets cache option
        /// </summary>
        public FieldCacheRole CacheRole { get; set; } = FieldCacheRole.None;

        /// <summary>
        /// Gets or sets db feature
        /// </summary>
        public FieldDbFeature DbFeature { get; set; } = FieldDbFeature.None;

        /// <summary>
        /// Gets or sets field behavior
        /// </summary>
        public FieldBehavior Behavior { get; set; } = FieldBehavior.None;

        /// <summary>
        /// Gets or sets the field format options
        /// </summary>
        public FieldFormatOption FormatOptions { get; set; }

        /// <summary>
        /// Gets or sets the database type
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Precision
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// Gets or sets default value
        /// </summary>
        public string DefaultValue { get; set; }

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
        /// Whether in role
        /// </summary>
        /// <param name="role">Field role</param>
        /// <returns></returns>
        public override bool InRole(FieldRole role)
        {
            return (Role & role) == role;
        }

        /// <summary>
        /// Whether has feature
        /// </summary>
        /// <param name="feature">Field db feature</param>
        /// <returns></returns>
        public bool HasDbFeature(FieldDbFeature feature)
        {
            return (DbFeature & feature) == feature;
        }

        /// <summary>
        /// Whether allow behavior
        /// </summary>
        /// <param name="behavior">Behvaior</param>
        /// <returns></returns>
        public bool AllowBehavior(FieldBehavior behavior)
        {
            return (Behavior & behavior) == behavior;
        }

        /// <summary>
        /// Indicates field has the specified cache role
        /// </summary>
        /// <param name="cacheRole">Cache role</param>
        /// <returns></returns>
        public bool InCacheRole(FieldCacheRole cacheRole)
        {
            return (CacheRole & cacheRole) == cacheRole;
        }

        /// <summary>
        /// Indicates field is necessary query field
        /// </summary>
        /// <returns></returns>
        public bool IsNecessaryQueryField()
        {
            return InRole(FieldRole.PrimaryKey) || InRole(FieldRole.Revision);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return PropertyName.GetHashCode();
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
