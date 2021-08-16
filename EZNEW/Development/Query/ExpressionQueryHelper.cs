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
        /// Get a query item by expression
        /// </summary>
        /// <param name="queryOperator">Connect operator</param>
        /// <param name="expression">Condition expression</param>
        /// <returns>IQueryItem object</returns>
        internal static Tuple<QueryOperator, IQueryItem> GetExpressionQuery(QueryOperator queryOperator, Expression expression)
        {
            var nodeType = expression.NodeType;
            ExpressionType queryNodeType = queryOperator == QueryOperator.OR ? ExpressionType.OrElse : ExpressionType.AndAlso;
            if (ExpressionHelper.IsCompareNodeType(nodeType))
            {
                return GetSingleExpressionQueryItem(queryNodeType, expression);
            }
            else if (ExpressionHelper.IsBoolNodeType(nodeType))
            {
                BinaryExpression binExpression = expression as BinaryExpression;
                if (binExpression == null)
                {
                    throw new EZNEWException("Expression is error");
                }
                IQuery query = QueryManager.Create();
                var leftQuery = GetExpressionQuery(queryOperator, binExpression.Left);
                if (leftQuery != null)
                {
                    query.AddQueryItem(leftQuery.Item1, leftQuery.Item2);
                }
                QueryOperator rightQueryOperator = nodeType == ExpressionType.OrElse ? QueryOperator.OR : QueryOperator.AND;
                var rightQuery = GetExpressionQuery(rightQueryOperator, binExpression.Right);
                if (rightQuery != null)
                {
                    query.AddQueryItem(rightQuery.Item1, rightQuery.Item2);
                }
                return new Tuple<QueryOperator, IQueryItem>(queryOperator, query);
            }
            else if (nodeType == ExpressionType.Call)
            {
                return GetCallExpressionQueryItem(queryOperator, expression);
            }
            else if (nodeType == ExpressionType.Not)
            {
                UnaryExpression unaryExpress = expression as UnaryExpression;
                if (unaryExpress != null && unaryExpress.Operand is MethodCallExpression)
                {
                    return GetCallExpressionQueryItem(queryOperator, unaryExpress.Operand, true);
                }
            }
            return null;
        }

        /// <summary>
        /// Get a query item by method call expression with string type
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="memberArg">Expression</param>
        /// <param name="negation">Whether is negation</param>
        /// <returns>criteria</returns>
        internal static Criteria GetStringCallExpressionQueryItem(string methodName, Expression memberArg, Expression parameter, bool negation = false)
        {
            Criteria criteria = null;
            CriteriaOperator criteriaOperator = CriteriaOperator.Like;
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
                    criteriaOperator = negation ? CriteriaOperator.NotEndLike : CriteriaOperator.EndLike;
                    criteria = Criteria.CreateNewCriteria(parameterName, criteriaOperator, value);
                    break;
                case "StartsWith":
                    criteriaOperator = negation ? CriteriaOperator.NotBeginLike : CriteriaOperator.BeginLike;
                    criteria = Criteria.CreateNewCriteria(parameterName, criteriaOperator, value);
                    break;
                case "Contains":
                    criteriaOperator = negation ? CriteriaOperator.NotLike : CriteriaOperator.Like;
                    criteria = Criteria.CreateNewCriteria(parameterName, criteriaOperator, value);
                    break;
            }
            return criteria;
        }

        /// <summary>
        /// Get a query item
        /// </summary>
        /// <param name="expressionType">Expression node type</param>
        /// <param name="expression">Expression</param>
        /// <returns>IQueryItem object</returns>
        internal static Tuple<QueryOperator, IQueryItem> GetSingleExpressionQueryItem(ExpressionType expressionType, Expression expression)
        {
            if (expression == null)
            {
                throw new EZNEWException("expression is null");
            }
            BinaryExpression binaryExpression = expression as BinaryExpression;
            if (binaryExpression == null)
            {
                throw new EZNEWException("expression is error");
            }
            QueryOperator qOperator = expressionType == ExpressionType.OrElse ? QueryOperator.OR : QueryOperator.AND;
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
            CriteriaOperator cOperator = GetCriteriaOperator(binaryExpression.NodeType);
            return new Tuple<QueryOperator, IQueryItem>(qOperator, Criteria.CreateNewCriteria(name, cOperator, value));
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
        /// Is field name expression
        /// </summary>
        /// <param name="expression">expression</param>
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
        /// Get condition operator by expression type
        /// </summary>
        /// <param name="expressType">Expression type</param>
        /// <returns>Return criteria operator</returns>
        internal static CriteriaOperator GetCriteriaOperator(ExpressionType expressType)
        {
            CriteriaOperator cOperator = CriteriaOperator.Equal;
            switch (expressType)
            {
                case ExpressionType.Equal:
                default:
                    cOperator = CriteriaOperator.Equal;
                    break;
                case ExpressionType.NotEqual:
                    cOperator = CriteriaOperator.NotEqual;
                    break;
                case ExpressionType.LessThanOrEqual:
                    cOperator = CriteriaOperator.LessThanOrEqual;
                    break;
                case ExpressionType.LessThan:
                    cOperator = CriteriaOperator.LessThan;
                    break;
                case ExpressionType.GreaterThan:
                    cOperator = CriteriaOperator.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    cOperator = CriteriaOperator.GreaterThanOrEqual;
                    break;
            }
            return cOperator;
        }

        /// <summary>
        /// Get a query item by method call expression with IEnumerable
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="memberArg">MemberArg</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="negation">Whether is negation</param>
        /// <returns>Return a criteria</returns>
        internal static Criteria GetIEnumerableCallExpressionQueryItem(string methodName, Expression memberArg, Expression parameter, bool negation = false)
        {
            Criteria criteria = null;
            CriteriaOperator criteriaOperator = CriteriaOperator.In;
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
                    criteriaOperator = negation ? CriteriaOperator.NotIn : CriteriaOperator.In;
                    criteria = Criteria.CreateNewCriteria(parameterName, criteriaOperator, values);
                    break;
            }
            return criteria;
        }

        /// <summary>
        /// Get a query item by method call expression
        /// </summary>
        /// <param name="expressionType">Expression node type</param>
        /// <param name="expression">Expression</param>
        /// <param name="negation">Whether is negation</param>
        /// <returns></returns>
        internal static Tuple<QueryOperator, IQueryItem> GetCallExpressionQueryItem(QueryOperator queryOperator, Expression expression, bool negation = false)
        {
            MethodCallExpression callExpression = expression as MethodCallExpression;
            MemberExpression memberArg = null;
            Expression parameterExpression = null;
            if (callExpression.Object != null)
            {
                memberArg = callExpression.Object as MemberExpression;
                parameterExpression = callExpression.Arguments[0];
            }
            else if (callExpression.Arguments.Count == 2)
            {
                memberArg = callExpression.Arguments[0] as MemberExpression;
                parameterExpression = callExpression.Arguments[1];
            }
            if (memberArg == null || parameterExpression == null)
            {
                return null;
            }
            Criteria criteria = null;
            var dataType = memberArg.Type;
            if (dataType == typeof(string))
            {
                criteria = GetStringCallExpressionQueryItem(callExpression.Method.Name, memberArg, parameterExpression, negation);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(memberArg.Type))
            {
                criteria = GetIEnumerableCallExpressionQueryItem(callExpression.Method.Name, parameterExpression, memberArg, negation);
            }
            if (criteria != null)
            {
                return new Tuple<QueryOperator, IQueryItem>(queryOperator, criteria);
            }
            return null;
        }
    }
}
