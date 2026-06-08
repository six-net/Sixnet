// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Data.Field
{
    /// <summary>
    /// Data field
    /// </summary>
    public class SixnetDataField : ISixnetField, IComparable
    {
        #region Fields

        protected string identityValue = string.Empty;

        bool fixedFieldName = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the model type
        /// </summary>
        internal Type ModelType { get; set; }

        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        internal Type DataType { get; set; }

        /// <summary>
        /// Field name
        /// </summary>
        internal string FieldName { get; set; }

        /// <summary>
        /// Gets the field identity
        /// </summary>
        public string FieldIdentity => identityValue;

        /// <summary>
        /// Get or set the field property name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Get or set the model type index
        /// </summary>
        public int ModelTypeIndex { get; set; }

        /// <summary>
        /// Gets or sets the field format setting
        /// </summary>
        public SixnetFieldFormatSetting FormatSetting { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the field role
        /// </summary>
        public SixnetFieldRole Role { get; set; } = SixnetFieldRole.None;

        /// <summary>
        /// Gets or sets cache option
        /// </summary>
        public SixnetFieldCacheRole CacheRole { get; set; } = SixnetFieldCacheRole.None;

        /// <summary>
        /// Gets or sets db feature
        /// </summary>
        public SixnetFieldDbFeature DbFeature { get; set; } = SixnetFieldDbFeature.None;

        /// <summary>
        /// Gets or sets field behavior
        /// </summary>
        public SixnetFieldBehavior Behavior { get; set; } = SixnetFieldBehavior.None;

        /// <summary>
        /// Gets or sets the file object name
        /// </summary>
        public string FileObjectName { get; set; }

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
        internal ISixnetEntityPropertyValueProvider ValueProvider { get; set; }

        /// <summary>
        /// Whether has field formatter
        /// </summary>
        public bool HasFormatter => FormatSetting != null;

        /// <summary>
        /// Whether is a constant field and no formatter
        /// </summary>
        public bool IsSimpleConstant => false;

        /// <summary>
        /// Gets or sets the start value
        /// </summary>
        public long StartValue { get; set; }

        /// <summary>
        /// Gets or sets the increment value
        /// </summary>
        public int IncrementValue { get; set; }

        #endregion

        #region Methods

        #region Create

        /// <summary>
        /// Create a data field
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="modelType">Model type</param>
        /// <param name="modelTypeIndex">Model type index</param>
        /// <param name="fieldFormatSetting">Field format setting</param>
        /// <param name="fieldName">Field name</param>
        /// <returns></returns>
        public static SixnetDataField Create(string propertyName, Type modelType = null, int modelTypeIndex = 0, SixnetFieldFormatSetting fieldFormatSetting = null, string fieldName = "")
        {
            SixnetDirectThrower.ThrowArgErrorIf(string.IsNullOrWhiteSpace(propertyName), "Property name is null or empty");
            SixnetDataField field = null;
            if (modelType != null)
            {
                field = SixnetEntityManager.GetField(modelType, propertyName)?.Clone() as SixnetDataField;
            }
            field ??= new SixnetDataField()
            {
                PropertyName = propertyName,
                ModelType = modelType,
            };
            field.ModelTypeIndex = modelTypeIndex;
            field.FormatSetting = fieldFormatSetting;
            field.FieldName = string.IsNullOrWhiteSpace(fieldName) ? propertyName : fieldName;
            field.identityValue = field.GetIdentity();
            return field;
        }

        string GetIdentity()
        {
            return $"{ModelType?.GUID}_{FieldName}_{PropertyName}";
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone a field
        /// </summary>
        /// <returns></returns>
        public virtual ISixnetField Clone()
        {
            return new SixnetDataField()
            {
                Description = Description,
                Role = Role,
                CacheRole = CacheRole,
                DbFeature = DbFeature,
                Behavior = Behavior,
                FormatSetting = FormatSetting?.Clone(),
                DbType = DbType,
                Length = Length,
                Precision = Precision,
                DefaultValue = DefaultValue,
                ValueProvider = ValueProvider,
                identityValue = identityValue,
                ModelType = ModelType,
                DataType = DataType,
                FieldName = FieldName,
                PropertyName = PropertyName,
                ModelTypeIndex = ModelTypeIndex,
                fixedFieldName = fixedFieldName,
                FileObjectName = FileObjectName,
                IncrementValue = IncrementValue,
                StartValue = StartValue,
            };
        }

        #endregion

        #region Equals

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return obj is SixnetDataField targetField
                    && ModelType == targetField.ModelType
                    && PropertyName == targetField.PropertyName;
            }
            return true;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return $"{ModelType?.GUID}{FieldName}{PropertyName}".GetHashCode();
        }

        public override string ToString()
        {
            return PropertyName ?? string.Empty;
        }

        public int CompareTo(object obj)
        {
            if (obj is not SixnetDataField targetField)
            {
                return 1;
            }
            return PropertyName.CompareTo(targetField.PropertyName);
        }

        #endregion

        #region Role

        /// <summary>
        /// Indicates field has the specified role
        /// </summary>
        /// <param name="role">Field role</param>
        /// <returns></returns>
        public virtual bool InRole(SixnetFieldRole role)
        {
            return (Role & role) == role;
        }

        /// <summary>
        /// Whether is unmodifiable field
        /// </summary>
        /// <returns></returns>
        internal bool IsUnmodifiableField()
        {
            return InRole(SixnetFieldRole.CreateDate)
                || InRole(SixnetFieldRole.CreateUserId)
                || InRole(SixnetFieldRole.CreateUserName)
                || InRole(SixnetFieldRole.CreateUserDisplayName)
                || InRole(SixnetFieldRole.PrimaryKey)
                || InRole(SixnetFieldRole.Increment)
                || InRole(SixnetFieldRole.Revision)
                || InRole(SixnetFieldRole.SplitValue)
                || InRole(SixnetFieldRole.Archive)
                || InRole(SixnetFieldRole.Isolation);
        }

        #endregion

        #region Cache role

        /// <summary>
        /// Indicates field has the specified cache role
        /// </summary>
        /// <param name="cacheRole">Cache role</param>
        /// <returns></returns>
        public bool InCacheRole(SixnetFieldCacheRole cacheRole)
        {
            return (CacheRole & cacheRole) == cacheRole;
        }

        #endregion

        #region Db feature

        /// <summary>
        /// Whether has feature
        /// </summary>
        /// <param name="feature">Field db feature</param>
        /// <returns></returns>
        public bool HasDbFeature(SixnetFieldDbFeature feature)
        {
            return (DbFeature & feature) == feature;
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Whether allow behavior
        /// </summary>
        /// <param name="behavior">Behvaior</param>
        /// <returns></returns>
        public bool AllowBehavior(SixnetFieldBehavior behavior)
        {
            return (Behavior & behavior) == behavior;
        }

        #endregion

        #region Get model type

        /// <summary>
        /// Get model type
        /// </summary>
        /// <returns></returns>
        public Type GetModelType()
        {
            return ModelType;
        }

        #endregion

        #region Get data type

        /// <summary>
        /// Get data type
        /// </summary>
        /// <returns></returns>
        public Type GetDataType()
        {
            return DataType;
        }

        #endregion

        #region Get field name

        /// <summary>
        /// Get field name
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        public string GetFieldName(SixnetDatabaseType databaseType)
        {
            var fieldName = FieldName;
            var entityType = GetModelType();
            if (entityType != null && !fixedFieldName)
            {
                var entitySetting = SixnetDataManager.GetEntitySetting(databaseType, entityType);
                if ((entitySetting?.Fields?.TryGetValue(PropertyName, out var fieldSetting) ?? false)
                    && !string.IsNullOrWhiteSpace(fieldSetting?.FieldName))
                {
                    fieldName = fieldSetting.FieldName;
                }
            }
            return fieldName;
        }

        #endregion

        #region Necessary field

        /// <summary>
        /// Whether is necessary field
        /// </summary>
        /// <returns></returns>
        public bool IsNecessaryField()
        {
            return InRole(SixnetFieldRole.PrimaryKey) || InRole(SixnetFieldRole.Revision);
        }

        #endregion

        #endregion
    }
}
