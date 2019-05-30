using EZNEW.Develop.CQuery.CriteriaConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Atomic Query Conditions
    /// </summary>
    public class Criteria : IQueryItem
    {
        #region Constructor

        /// <summary>
        /// Initialize a Condition Instance
        /// </summary>
        /// <param name="name">field name</param>
        /// <param name="operator">conditional operator</param>
        /// <param name="value">value</param>
        private Criteria(string name, CriteriaOperator @operator, dynamic value)
        {
            _name = name;
            _operator = @operator;
            _value = value;
        }

        #endregion

        #region Fields

        /// <summary>
        /// field name
        /// </summary>
        string _name;
        /// <summary>
        /// conditional operator
        /// </summary>
        CriteriaOperator _operator;
        /// <summary>
        /// value
        /// </summary>
        dynamic _value;
        dynamic _realValue = null;
        bool _calculateValue = false;//init real value

        #endregion

        #region Propertys

        /// <summary>
        /// Field Name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Condition Operator
        /// </summary>
        public CriteriaOperator Operator
        {
            get
            {
                return _operator;
            }
        }

        /// <summary>
        /// Value
        /// </summary>
        public dynamic Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Criteria Convert
        /// </summary>
        public ICriteriaConvert Convert
        {
            get;set;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Get the real value
        /// </summary>
        /// <returns></returns>
        public dynamic GetCriteriaRealValue()
        {
            if (_calculateValue)
            {
                return _realValue;
            }
            Expression valueExpression = _value as Expression;
            if (valueExpression != null)
            {
                _realValue = GetExpressionValue(valueExpression);
            }
            else
            {
                _realValue = _value;
            }
            _calculateValue = true;
            return _realValue;
        }

        /// <summary>
        /// calculate Expression value
        /// </summary>
        /// <param name="valueExpression">value expression</param>
        /// <returns></returns>
        dynamic GetExpressionValue(Expression valueExpression)
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

        #region Static Method

        /// <summary>
        /// Create a Criteria Instance
        /// </summary>
        /// <param name="name">fieldName</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <returns>creteria</returns>
        public static Criteria CreateNewCriteria(string name, CriteriaOperator @operator, dynamic value)
        {
            return new Criteria(name, @operator, value);
        }

        #endregion
    }
}
