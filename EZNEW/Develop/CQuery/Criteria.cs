using System;
using System.Linq.Expressions;
using EZNEW.Develop.CQuery.CriteriaConverter;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Atomic query conditions
    /// </summary>
    [Serializable]
    public class Criteria : IQueryItem
    {
        #region Constructor

        /// <summary>
        /// Initialize a condition instance
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Conditional operator</param>
        /// <param name="value">Value</param>
        private Criteria(string fieldName, CriteriaOperator @operator, dynamic value)
        {
            Name = fieldName;
            Operator = @operator;
            Value = value;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Real value
        /// </summary>
        dynamic _realValue = null;

        /// <summary>
        /// Whether has inited real value
        /// </summary>
        bool _calculateValue = false;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the condition operator
        /// </summary>
        public CriteriaOperator Operator { get; internal set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public dynamic Value { get; private set; }

        /// <summary>
        /// Gets or sets the criteria convert
        /// </summary>
        public ICriteriaConverter Converter
        {
            get; set;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Copy a new IQueryItem
        /// </summary>
        /// <returns></returns>
        public Criteria Clone()
        {
            return new Criteria(Name, Operator, Value)
            {
                Converter = Converter?.Clone(),
                _calculateValue = _calculateValue,
                _realValue = _realValue
            };
        }

        /// <summary>
        /// Gets the real value
        /// </summary>
        /// <returns>Return the criteria real value</returns>
        public dynamic GetCriteriaRealValue()
        {
            if (_calculateValue)
            {
                return _realValue;
            }
            _realValue = ComputeCriteriaValue(Value);
            _calculateValue = true;
            return _realValue;
        }

        /// <summary>
        /// Set value
        /// </summary>
        /// <param name="value">Value</param>
        internal void SetValue(dynamic value)
        {
            Value = _realValue = ComputeCriteriaValue(value);
            _calculateValue = true;
        }

        /// <summary>
        /// Compute criteria value
        /// </summary>
        /// <param name="originalValue">original value</param>
        /// <returns>Return the computed value</returns>
        public static dynamic ComputeCriteriaValue(dynamic originalValue)
        {
            Expression valueExpression = originalValue as Expression;
            if (valueExpression != null)
            {
                return GetExpressionValue(valueExpression);
            }
            return originalValue;
        }

        /// <summary>
        /// Calculate Expression value
        /// </summary>
        /// <param name="valueExpression">Value expression</param>
        /// <returns>Return the expression value</returns>
        public static dynamic GetExpressionValue(Expression valueExpression)
        {
            if (valueExpression == null)
            {
                return null;
            }
            dynamic value = null;
            switch (valueExpression.NodeType)
            {
                case ExpressionType.Constant:
                    value = ((ConstantExpression)valueExpression).Value;
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.ArrayIndex:
                case ExpressionType.ArrayLength:
                case ExpressionType.Call:
                case ExpressionType.Coalesce:
                case ExpressionType.Conditional:
                case ExpressionType.Convert:
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
                    value = Expression.Lambda(valueExpression).Compile().DynamicInvoke();
                    break;
                default:
                    break;
            }
            return value;
        }

        #endregion

        #region Static method

        /// <summary>
        /// Create a criteria instance
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return a new creteria</returns>
        public static Criteria CreateNewCriteria(string fieldName, CriteriaOperator @operator, dynamic value)
        {
            return new Criteria(fieldName, @operator, ComputeCriteriaValue(value));
        }

        #endregion
    }
}
