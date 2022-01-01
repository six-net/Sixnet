using System;
using System.Linq.Expressions;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines criterion
    /// </summary>
    [Serializable]
    public class Criterion : ICondition
    {
        #region Constructor

        /// <summary>
        /// Initialize a condition instance
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <param name="value">Value</param>
        private Criterion(string fieldName, CriterionOperator criterionOperator, dynamic value)
        {
            Name = fieldName;
            Operator = criterionOperator;
            Value = CalculateValue(value);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the criterion operator
        /// </summary>
        public CriterionOperator Operator { get; internal set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public dynamic Value { get; private set; }

        /// <summary>
        /// Gets or sets the criterion options
        /// </summary>
        public CriterionOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the connection operator
        /// </summary>
        public CriterionConnectionOperator ConnectionOperator { get; set; } = CriterionConnectionOperator.And;

        /// <summary>
        /// Indicates whether is an equal criterion
        /// </summary>
        public bool IsEqual => Operator == CriterionOperator.Equal || Operator == CriterionOperator.In;

        #endregion

        #region Functions

        /// <summary>
        /// Clone a new criterion
        /// </summary>
        /// <returns></returns>
        public Criterion Clone()
        {
            return new Criterion(Name, Operator, Value)
            {
                Options = Options?.Clone()
            };
        }

        /// <summary>
        /// Gets whether has field conversion
        /// </summary>
        /// <returns></returns>
        public bool HasFieldConversion()
        {
            return !string.IsNullOrWhiteSpace(Options?.FieldConversionOptions?.ConversionName);
        }

        /// <summary>
        /// Set value
        /// </summary>
        /// <param name="value">Value</param>
        internal void SetValue(dynamic value)
        {
            Value = CalculateValue(value);
        }

        /// <summary>
        /// Calculate value
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Return the calculated value</returns>
        static dynamic CalculateValue(dynamic originalValue)
        {
            Expression valueExpression = originalValue as Expression;
            if (valueExpression != null)
            {
                return CalculateExpressionValue(valueExpression);
            }
            return originalValue;
        }

        /// <summary>
        /// Calculate expression value
        /// </summary>
        /// <param name="expression">Value expression</param>
        /// <returns>Return the expression value</returns>
        static dynamic CalculateExpressionValue(Expression expression)
        {
            if (expression == null)
            {
                return null;
            }
            dynamic value = null;
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    value = ((ConstantExpression)expression).Value;
                    break;
                case ExpressionType.Convert:
                    value = CalculateExpressionValue(((UnaryExpression)expression).Operand);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.ArrayIndex:
                case ExpressionType.ArrayLength:
                case ExpressionType.Call:
                case ExpressionType.Coalesce:
                case ExpressionType.Conditional:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Decrement:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Increment:
                case ExpressionType.Invoke:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.New:
                case ExpressionType.Not:
                case ExpressionType.NotEqual:
                case ExpressionType.OnesComplement:
                case ExpressionType.Or:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.MemberAccess:
                    value = Expression.Lambda(expression).Compile().DynamicInvoke();
                    break;
                default:
                    break;
            }
            return value;
        }

        /// <summary>
        /// Indicates whether is a boolean constant
        /// </summary>
        /// <returns>Return is a boolean constant</returns>
        public bool IsBooleanConstant()
        {
            return Operator == CriterionOperator.True || Operator == CriterionOperator.False;
        }

        /// <summary>
        /// Get boolean constant condition
        /// </summary>
        /// <returns></returns>
        public string GetBooleanConstantCondition()
        {
            return Operator == CriterionOperator.True ? "0 = 0" : "0 = 1";
        }

        #endregion

        #region Static method

        /// <summary>
        /// Create a criterion
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return a new criterion</returns>
        public static Criterion Create(string fieldName, CriterionOperator @operator, dynamic value, CriterionConnectionOperator connectionOperator = CriterionConnectionOperator.And, CriterionOptions criterionOptions = null)
        {
            return new Criterion(fieldName, @operator, value)
            {
                ConnectionOperator = connectionOperator,
                Options = criterionOptions
            };
        }

        #endregion
    }
}
