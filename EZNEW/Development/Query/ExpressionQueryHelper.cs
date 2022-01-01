using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;
using EZNEW.Exceptions;
using EZNEW.Reflection;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Expression query helper
    /// </summary>
    internal static class ExpressionQueryHelper
    {
        /// <summary>
        /// Get condtion by expression
        /// </summary>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return a condition</returns>
        internal static ICondition GetExpressionCondition(CriterionConnectionOperator connectionOperator, Expression conditionExpression)
        {
            var nodeType = conditionExpression.NodeType;
            ExpressionType queryNodeType = connectionOperator == CriterionConnectionOperator.Or ? ExpressionType.OrElse : ExpressionType.AndAlso;
            if (ExpressionHelper.IsCompareNodeType(nodeType))
            {
                return GetExpressionCriterion(queryNodeType, conditionExpression);
            }
            else if (ExpressionHelper.IsBoolNodeType(nodeType))
            {
                if (conditionExpression is BinaryExpression binaryExpression)
                {
                    IQuery query = QueryManager.Create();
                    var leftQuery = GetExpressionCondition(connectionOperator, binaryExpression.Left);
                    if (leftQuery != null)
                    {
                        query.AddCondition(leftQuery);
                    }
                    CriterionConnectionOperator rightQueryConnectionOperator = nodeType == ExpressionType.OrElse ? CriterionConnectionOperator.Or : CriterionConnectionOperator.And;
                    var rightQuery = GetExpressionCondition(rightQueryConnectionOperator, binaryExpression.Right);
                    if (rightQuery != null)
                    {
                        query.AddCondition(rightQuery);
                    }
                    query.ConnectionOperator = connectionOperator;
                    return query;
                }
            }
            else if (nodeType == ExpressionType.Call)
            {
                return GetMethodCallExpressionCriterion(connectionOperator, conditionExpression);
            }
            else if (nodeType == ExpressionType.Not)
            {
                UnaryExpression unaryExpress = conditionExpression as UnaryExpression;
                if (unaryExpress != null && unaryExpress.Operand is MethodCallExpression)
                {
                    return GetMethodCallExpressionCriterion(connectionOperator, unaryExpress.Operand, true);
                }
            }
            if (conditionExpression is ConstantExpression constantExpression && constantExpression.Value is bool boolValue)
            {
                if (boolValue)
                {
                    return null;
                }
                return Criterion.Create(string.Empty, CriterionOperator.False, null);
            }
            throw new EZNEWException($"Expression type:{conditionExpression?.GetType()} are not supported");
        }

        /// <summary>
        /// Get expression criterion
        /// </summary>
        /// <param name="conditionExpressionType">Expression type</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return a criterion</returns>
        internal static Criterion GetExpressionCriterion(ExpressionType conditionExpressionType, Expression conditionExpression)
        {
            if (conditionExpression is BinaryExpression binaryExpression)
            {
                CriterionConnectionOperator connectionOperator = conditionExpressionType == ExpressionType.OrElse ? CriterionConnectionOperator.Or : CriterionConnectionOperator.And;
                Tuple<Expression, Expression> nameAndValue = GetNameAndValueExpression(binaryExpression.Left, binaryExpression.Right);
                if (nameAndValue == null)
                {
                    return null;
                }
                string name = ExpressionHelper.GetExpressionPropertyName(nameAndValue.Item1);
                object value = nameAndValue.Item2;
                if (string.IsNullOrEmpty(name) || value == null)
                {
                    return null;
                }
                return Criterion.Create(name, GetCriterionOperator(binaryExpression.NodeType), value, connectionOperator);
            }
            throw new EZNEWException($"Expression type:{conditionExpression?.GetType()} are not supported");
        }

        /// <summary>
        /// Get field and value expression
        /// </summary>
        /// <param name="firstExpression">First expression</param>
        /// <param name="secondExpression">Eecond expression</param>
        /// <returns>Return field and value expression</returns>
        internal static Tuple<Expression, Expression> GetNameAndValueExpression(Expression firstExpression, Expression secondExpression)
        {
            Tuple<Expression, Expression> result = null;
            bool firstIsNameExp = IsNameExpression(firstExpression);
            bool secondIsNameExp = IsNameExpression(secondExpression);
            if (!firstIsNameExp && !secondIsNameExp)
            {
                return result;
            }
            if (firstIsNameExp && secondIsNameExp)
            {
                Expression firstChildExp = ExpressionHelper.GetLastChildExpression(firstExpression);
                Expression secondChildExp = ExpressionHelper.GetLastChildExpression(secondExpression);
                result = firstChildExp.NodeType >= secondChildExp.NodeType ? new Tuple<Expression, Expression>(firstExpression, secondExpression) : new Tuple<Expression, Expression>(secondExpression, firstExpression);
                return result;
            }
            result = firstIsNameExp ? new Tuple<Expression, Expression>(firstExpression, secondExpression) : new Tuple<Expression, Expression>(secondExpression, firstExpression);
            return result;
        }

        /// <summary>
        /// Check is field name expression
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns></returns>
        internal static bool IsNameExpression(Expression expression)
        {
            if (expression == null)
            {
                return false;
            }
            bool result = false;
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    result = true;
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryExpression unaryExp = expression as UnaryExpression;
                    if (unaryExp.Operand.NodeType == ExpressionType.MemberAccess)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Get criterion operator by expression type
        /// </summary>
        /// <param name="expressType">Expression type</param>
        /// <returns>Return criterion operator</returns>
        internal static CriterionOperator GetCriterionOperator(ExpressionType expressType)
        {
            CriterionOperator criterionOperator = CriterionOperator.Equal;
            switch (expressType)
            {
                case ExpressionType.Equal:
                default:
                    criterionOperator = CriterionOperator.Equal;
                    break;
                case ExpressionType.NotEqual:
                    criterionOperator = CriterionOperator.NotEqual;
                    break;
                case ExpressionType.LessThanOrEqual:
                    criterionOperator = CriterionOperator.LessThanOrEqual;
                    break;
                case ExpressionType.LessThan:
                    criterionOperator = CriterionOperator.LessThan;
                    break;
                case ExpressionType.GreaterThan:
                    criterionOperator = CriterionOperator.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    criterionOperator = CriterionOperator.GreaterThanOrEqual;
                    break;
            }
            return criterionOperator;
        }

        /// <summary>
        /// Get criterion by method call expression for IEnumerable
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="memberArg">MemberArg</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="negation">Whether is negation</param>
        /// <returns>Return a criterion</returns>
        internal static Criterion GetIEnumerableMethodCallExpressionCriterion(string methodName, Expression memberArg, Expression parameter, bool negation = false)
        {
            Criterion criterion = null;
            CriterionOperator criterionOperator = CriterionOperator.In;
            IEnumerable values = null;
            string parameterName = string.Empty;
            if (memberArg is ParameterExpression)
            {
                parameterName = (memberArg as ParameterExpression)?.Name;
            }
            else if (memberArg is MemberExpression)
            {
                parameterName = ExpressionHelper.GetExpressionPropertyName(memberArg as MemberExpression);
            }
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new EZNEWException($"conditional expression is not well-formed");
            }
            switch (methodName)
            {
                case "Contains":
                    values = Expression.Lambda(parameter)?.Compile().DynamicInvoke() as IEnumerable;
                    if (values == null)
                    {
                        throw new EZNEWException($"The value of the collection type is null or empty");
                    }
                    values = ReflectionManager.Collections.ResolveCollection(values);
                    criterionOperator = negation ? CriterionOperator.NotIn : CriterionOperator.In;
                    criterion = Criterion.Create(parameterName, criterionOperator, values);
                    break;
            }
            return criterion;
        }

        /// <summary>
        /// Get method call expression criterion
        /// </summary>
        /// <param name="connectionOperator">Connection connection operator</param>
        /// <param name="conditionExpression">Connection expression</param>
        /// <param name="negation">Whether is negation</param>
        /// <returns></returns>
        internal static Criterion GetMethodCallExpressionCriterion(CriterionConnectionOperator connectionOperator, Expression conditionExpression, bool negation = false)
        {
            MethodCallExpression callExpression = conditionExpression as MethodCallExpression;
            Expression memberArg = null;
            Expression parameterExpression = null;
            if (callExpression.Object != null)
            {
                memberArg = callExpression.Object;
                parameterExpression = callExpression.Arguments[0];
            }
            else if (callExpression.Arguments.Count == 2)
            {
                memberArg = callExpression.Arguments[0];
                parameterExpression = callExpression.Arguments[1];
            }
            if (memberArg == null || parameterExpression == null)
            {
                return null;
            }
            Criterion criterion = null;
            var dataType = memberArg.Type;
            if (dataType == typeof(string))
            {
                criterion = GetMethodExpressionCriterion(callExpression.Method.Name, memberArg, parameterExpression, negation);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(memberArg.Type))
            {
                criterion = GetIEnumerableMethodCallExpressionCriterion(callExpression.Method.Name, parameterExpression, memberArg, negation);
            }
            if (criterion != null)
            {
                criterion.ConnectionOperator = connectionOperator;
                return criterion;
            }
            return criterion;
        }

        /// <summary>
        /// Get a query item by method call expression with string type
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="memberArg">Expression</param>
        /// <param name="negation">Whether is negation</param>
        /// <returns>criterion</returns>
        internal static Criterion GetMethodExpressionCriterion(string methodName, Expression memberArg, Expression parameter, bool negation = false)
        {
            Criterion criterion = null;
            CriterionOperator criterionOperator = CriterionOperator.Like;
            //parameter name
            string parameterName = string.Empty;
            if (memberArg is ParameterExpression)
            {
                parameterName = (memberArg as ParameterExpression)?.Name;
            }
            else if (memberArg is MemberExpression)
            {
                parameterName = ExpressionHelper.GetExpressionPropertyName(memberArg as MemberExpression);
            }
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return null;
            }
            string value = Expression.Lambda(parameter)?.Compile().DynamicInvoke()?.ToString();
            switch (methodName)
            {
                case "EndsWith":
                    criterionOperator = negation ? CriterionOperator.NotEndLike : CriterionOperator.EndLike;
                    criterion = Criterion.Create(parameterName, criterionOperator, value);
                    break;
                case "StartsWith":
                    criterionOperator = negation ? CriterionOperator.NotBeginLike : CriterionOperator.BeginLike;
                    criterion = Criterion.Create(parameterName, criterionOperator, value);
                    break;
                case "Contains":
                    criterionOperator = negation ? CriterionOperator.NotLike : CriterionOperator.Like;
                    criterion = Criterion.Create(parameterName, criterionOperator, value);
                    break;
            }
            return criterion;
        }
    }
}
