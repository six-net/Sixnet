// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;

namespace Sixnet.Development.Data.Field
{
    /// <summary>
    /// Conditional data field
    /// </summary>
    public class ConditionalDataField : ISixnetField
    {
        /// <summary>
        /// Conditions
        /// </summary>
        public List<ConditionalDataFieldConditionItem> Conditions { get; set; }

        /// <summary>
        /// False value
        /// </summary>
        public ISixnetField FalseValue { get; set; }

        public bool IsSimpleConstant => false;

        public bool HasFormatter => false;

        public string FieldIdentity => string.Empty;

        public string PropertyName { get; set; }

        public FieldFormatSetting FormatSetting { get; set; }

        public ISixnetField Clone()
        {
            return new ConditionalDataField()
            {
                Conditions = Conditions?.Select(c => c.Clone()).ToList(),
                FalseValue = FalseValue?.Clone(),
                PropertyName = PropertyName,
                FormatSetting = FormatSetting?.Clone()
            };
        }

        public Type GetDataType()
        {
            return null;
        }

        public string GetFieldName(DatabaseType databaseType)
        {
            return string.Empty;
        }

        public Type GetModelType()
        {
            return null;
        }

        public bool InRole(FieldRole fieldRole)
        {
            return false;
        }
    }

    /// <summary>
    /// Condition item
    /// </summary>
    public struct ConditionalDataFieldConditionItem
    {
        /// <summary>
        /// Condition
        /// </summary>
        public ISixnetCondition Condition { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public ISixnetField Value { get; set; }

        public ConditionalDataFieldConditionItem Clone()
        {
            return new ConditionalDataFieldConditionItem()
            {
                Condition = Condition?.Clone(),
                Value = Value?.Clone()
            };
        }
    }
}
