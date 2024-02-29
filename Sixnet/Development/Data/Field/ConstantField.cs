using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data.Field
{
    /// <summary>
    /// Constant field
    /// </summary>
    public class ConstantField : ISixnetDataField
    {
        /// <summary>
        /// Whether has field formatter
        /// </summary>
        public bool HasFormatter => false;

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public dynamic Value { get; set; }

        /// <summary>
        /// Gets the field identity
        /// </summary>
        public string FieldIdentity => Value;

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
        public bool IsSimpleConstant => !HasFormatter;

        /// <summary>
        /// Create a criterion constant value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static ConstantField Create(dynamic value, string outputName = "")
        {
            return new ConstantField()
            {
                Value = CalculateValue(value),
                PropertyName = outputName,
            };
        }

        /// <summary>
        /// Calculate value
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Return the calculated value</returns>
        static dynamic CalculateValue(dynamic originalValue)
        {
            if (originalValue is Expression valueExpression)
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

        public ISixnetDataField Clone()
        {
            return new ConstantField()
            {
                Value = Value,
                PropertyName = PropertyName,
                FormatOption = FormatOption?.Clone()
            };
        }

        public override int GetHashCode()
        {
            return $"{PropertyName}{Value}".GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return obj is ConstantField constantField && constantField.PropertyName == PropertyName && constantField.Value == Value;
            }
            return true;
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
            if (Value != null)
            {
                return Value.GetType();
            }
            return null;
        }

        /// <summary>
        /// Get field name
        /// </summary>
        /// <returns></returns>
        public string GetFieldName()
        {
            return Value?.ToString() ?? string.Empty;
        }
    }
}
