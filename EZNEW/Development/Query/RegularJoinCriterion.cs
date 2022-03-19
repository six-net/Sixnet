using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines regular join criterion
    /// </summary>
    public class RegularJoinCriterion : IJoinCriterion
    {
        /// <summary>
        /// Determins whether is right criterion
        /// </summary>
        public bool IsRightCriterion { get; set; } = false;

        /// <summary>
        /// Gets or sets the field
        /// </summary>
        public FieldInfo Field { get; set; }

        /// <summary>
        /// Gets or sets the connector
        /// </summary>
        public CriterionConnector Connector { get; set; } = CriterionConnector.And;

        /// <summary>
        /// Gets or sets the join operator
        /// </summary>
        public CriterionOperator Operator { get; set; } = CriterionOperator.Equal;

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Create criterion
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="operator">Operator</param>
        /// <param name="value">Value</param>
        /// <param name="isRightCriterion">Indicates whether is right criterion</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        public static RegularJoinCriterion Create(FieldInfo field, CriterionOperator @operator, object value, bool isRightCriterion = false, CriterionConnector connector = CriterionConnector.And)
        {
            var fieldValue = value;
            if (value is IQuery valueQuery)
            {
                fieldValue = QueryManager.HandleParameterQueryBeforeUse(valueQuery);
            }
            return new RegularJoinCriterion()
            {
                Field = field,
                IsRightCriterion = isRightCriterion,
                Operator = @operator,
                Value = fieldValue,
                Connector = connector
            };
        }

        public IJoinCriterion Clone()
        {
            object targetValue = Value;
            if (Value is FieldInfo fieldInfo)
            {
                targetValue = fieldInfo.Clone();
            }
            if (Value is IQuery valueQuery)
            {
                targetValue = valueQuery.Clone();
            }
            return new RegularJoinCriterion()
            {
                IsRightCriterion = IsRightCriterion,
                Field = Field?.Clone(),
                Operator = Operator,
                Connector = Connector,
                Value = targetValue
            };
        }
    }
}
