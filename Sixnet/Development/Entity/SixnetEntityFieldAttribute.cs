// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Validation;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Entity field attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SixnetEntityFieldAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the db field name
        /// </summary>
        public string DbFieldName { get; set; }

        /// <summary>
        /// Gets or sets the field description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the field role
        /// </summary>
        public SixnetFieldRole DbRole { get; set; } = SixnetFieldRole.None;

        /// <summary>
        /// Gets or sets db feature
        /// </summary>
        public SixnetFieldDbFeature DbFeature { get; set; } = SixnetFieldDbFeature.None;

        /// <summary>
        /// Gets or sets the database type
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// Gets or sets the length
        /// </summary>
        public int DbLength { get; set; }

        /// <summary>
        /// Precision
        /// </summary>
        public int DbPrecision { get; set; }

        /// <summary>
        /// Gets or sets default value
        /// </summary>
        public string DbDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the start value
        /// </summary>
        public long DbStartValue { get; set; }

        /// <summary>
        /// Gets or sets the increment value
        /// </summary>
        public int DbIncrementValue { get; set; }

        /// <summary>
        /// Gets or sets the index sequence
        /// </summary>
        public int DbIndexSequence { get; set; }

        /// <summary>
        /// Gets or sets field behavior
        /// </summary>
        public SixnetFieldBehavior Behavior { get; set; } = SixnetFieldBehavior.None;

        /// <summary>
        /// Gets or sets the cache role
        /// </summary>
        public SixnetFieldCacheRole CacheRole { get; set; } = SixnetFieldCacheRole.None;

        /// <summary>
        /// Gets or sets the file object name
        /// </summary>
        public string FileObjectName { get; set; }

        /// <summary>
        /// Gets or sets the field format setting
        /// </summary>
        public SixnetFieldFormatSetting FormatSetting { get; set; }

        /// <summary>
        /// Gets or sets the validators
        /// </summary>
        public SixnetValidatorType[] Validators { get; set; }

        /// <summary>
        /// Gets or sets the validation operator
        /// </summary>
        public SixnetCompareOperator ValidationOperator { get; set; } = SixnetCompareOperator.Equal;

        /// <summary>
        /// Gets or sets the validation value
        /// </summary>
        public dynamic ValidationValue { get; set; }

        /// <summary>
        /// Gets or sets the enum type
        /// </summary>
        public Type ValidationEnumType { get; set; }

        /// <summary>
        /// Gets or sets the max value
        /// </summary>
        public dynamic ValidationMaxValue { get; set; }

        /// <summary>
        /// Gets or sets the min value
        /// </summary>
        public dynamic ValidationMinValue { get; set; }

        /// <summary>
        /// Gets or sets the lower boundary
        /// </summary>
        public SixnetRangeBoundary ValidationLowerBoundary { get; set; } = SixnetRangeBoundary.Include;

        /// <summary>
        /// Gets or sets the upper boundary
        /// </summary>
        public SixnetRangeBoundary ValidationUpperBoundary { get; set; } = SixnetRangeBoundary.Include;

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ValidationErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the tip
        /// </summary>
        public bool ValidationTip { get; set; }

        /// <summary>
        /// Gets or sets the compare type
        /// </summary>
        public SixnetValidationCompareObject ValidationCompareType { get; set; } = SixnetValidationCompareObject.Value;

        /// <summary>
        /// Gets or sets ignore scenarios
        /// </summary>
        public string[] ValidationIgnoreScenarios { get; set; }
    }
}
