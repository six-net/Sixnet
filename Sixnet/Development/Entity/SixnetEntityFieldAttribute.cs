// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field.Formatting;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Entity field attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SixnetEntityFieldAttribute : Attribute
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
        public SixnetFieldRole Role { get; set; } = SixnetFieldRole.None;

        /// <summary>
        /// Gets or sets the cache role
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
        /// Gets or sets the field format setting
        /// </summary>
        public SixnetFieldFormatSetting FormatSetting { get; set; }

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
        /// Gets or sets the start value
        /// </summary>
        public long StartValue { get; set; }

        /// <summary>
        /// Gets or sets the increment value
        /// </summary>
        public int IncrementValue { get; set; }
    }
}
