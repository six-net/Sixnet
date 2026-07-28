// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;

namespace Sixnet.Development.Data.Field
{
    /// <summary>
    /// Queryable field
    /// </summary>
    public class SixnetQueryableField : ISixnetField
    {
        /// <summary>
        /// Whether has field formatter
        /// </summary>
        public bool HasFormatter => Queryable?.Info.HasFieldFormatter ?? false;

        /// <summary>
        /// Get or set the queryable
        /// </summary>
        public ISixnetQueryable Queryable { get; set; }

        /// <summary>
        /// Gets the field identity
        /// </summary>
        public string FieldIdentity => Queryable?.Info.Id.ToString();

        /// <summary>
        /// Get or set the field output name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the field format options
        /// </summary>
        public SixnetFieldFormatSetting FormatSetting { get; set; }

        /// <summary>
        /// Whether is a constant field and no formatter
        /// </summary>
        public bool IsSimpleConstant => false;

        /// <summary>
        /// Clone a query criterion field
        /// </summary>
        /// <returns></returns>
        public ISixnetField Clone()
        {
            return new SixnetQueryableField()
            {
                Queryable = Queryable?.Clone(),
                PropertyName = PropertyName,
                FormatSetting = FormatSetting?.Clone()
            };
        }

        public override int GetHashCode()
        {
            return $"{PropertyName}{Queryable?.Info.Id}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return obj is SixnetQueryableField queryableField
                    && queryableField.PropertyName == PropertyName
                    && queryableField.Queryable?.Info.Id == Queryable?.Info.Id;
            }
            return true;
        }

        /// <summary>
        /// Create a queryable field
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="outputName">Output name</param>
        /// <returns></returns>
        public static SixnetQueryableField Create(ISixnetQueryable queryable, string outputName = "")
        {
            return new SixnetQueryableField()
            {
                Queryable = queryable,
                PropertyName = outputName
            };
        }

        public bool InRole(SixnetFieldRole fieldRole)
        {
            return false;
        }

        /// <summary>
        /// Get model type
        /// </summary>
        /// <returns></returns>
        public Type GetModelType()
        {
            return null;
        }

        /// <summary>
        /// Get field data type
        /// </summary>
        /// <returns></returns>
        public Type GetDataType()
        {
            return Queryable?.Info.SelectedFields?.FirstOrDefault()?.GetDataType();
        }

        /// <summary>
        /// Get field name
        /// </summary>
        /// <returns></returns>
        public string GetFieldName(SixnetDatabaseType databaseType)
        {
            return Queryable?.Info.SelectedFields?.FirstOrDefault()?.GetFieldName(databaseType) ?? string.Empty;
        }
    }
}
