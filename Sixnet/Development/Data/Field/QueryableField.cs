using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;

namespace Sixnet.Development.Data.Field
{
    /// <summary>
    /// Queryable field
    /// </summary>
    public class QueryableField : ISixnetDataField
    {
        /// <summary>
        /// Whether has field formatter
        /// </summary>
        public bool HasFormatter => Queryable?.HasFieldFormatter ?? false;

        /// <summary>
        /// Get or set the queryable
        /// </summary>
        public ISixnetQueryable Queryable { get; set; }

        /// <summary>
        /// Gets the field identity
        /// </summary>
        public string FieldIdentity => Queryable?.Id.ToString();

        /// <summary>
        /// Get or set the field output name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the field format options
        /// </summary>
        public FieldFormatSetting FormatOption { get; set; }

        /// <summary>
        /// Whether is a constant field and no formatter
        /// </summary>
        public bool IsSimpleConstant => false;

        /// <summary>
        /// Clone a query criterion field
        /// </summary>
        /// <returns></returns>
        public ISixnetDataField Clone()
        {
            return new QueryableField()
            {
                Queryable = Queryable?.Clone(),
                PropertyName = PropertyName,
                FormatOption = FormatOption?.Clone()
            };
        }

        public override int GetHashCode()
        {
            return $"{PropertyName}{Queryable?.Id}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return obj is QueryableField queryableField && queryableField.PropertyName == PropertyName && queryableField.Queryable?.Id == Queryable?.Id;
            }
            return true;
        }

        /// <summary>
        /// Create a queryable field
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="outputName">Output name</param>
        /// <returns></returns>
        public static QueryableField Create(ISixnetQueryable queryable, string outputName = "")
        {
            return new QueryableField()
            {
                Queryable = queryable,
                PropertyName = outputName
            };
        }

        public bool InRole(FieldRole fieldRole)
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
        public Type GetFieldDataType()
        {
            return Queryable?.SelectedFields?.FirstOrDefault()?.GetFieldDataType();
        }

        /// <summary>
        /// Get field name
        /// </summary>
        /// <returns></returns>
        public string GetFieldName()
        {
            return Queryable?.SelectedFields?.FirstOrDefault()?.GetFieldName() ?? string.Empty;
        }
    }
}
