using System;
using Sixnet.Development.Data.Field.Formatting;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Entity field attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EntityFieldAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the field description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the field role
        /// </summary>
        public FieldRole Role { get; set; } = FieldRole.None;

        /// <summary>
        /// Gets or sets the cache role
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
    }
}
