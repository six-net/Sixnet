using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Reflection;

namespace Sixnet.Expressions.Linq
{
    public static class SixnetExpressionHelper
    {
        #region Fields

        /// <summary>
        /// Expression base type
        /// </summary>
        static readonly Type _baseExpressType = typeof(Expression);

        /// <summary>
        /// Generate lambda method
        /// </summary>
        static readonly MethodInfo _lambdaMethod = null;

        /// <summary>
        /// String type
        /// </summary>
        static readonly Type _stringType = typeof(string);

        /// <summary>
        /// Char type
        /// </summary>
        static readonly Type _charType = typeof(char);

        #endregion

        #region Constructor

        static SixnetExpressionHelper()
        {
            var baseExpressMethods = _baseExpressType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            _lambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
        }

        #endregion

        #region Util

        #region Get expression text

        /// <summary>
        /// Get expression text
        /// </summary>
        /// <param name="expression">expression</param>
        /// <returns>expression text</returns>
        public static string GetExpressionText(LambdaExpression expression)
        {
            // Split apart the expression string for property/field accessors to create its name
            Stack<string> nameParts = new();
            Expression part = expression.Body;
            while (part != null)
            {
                if (part.NodeType == ExpressionType.Call)
                {
                    MethodCallExpression methodExpression = (MethodCallExpression)part;

                    if (!IsSingleArgumentIndexer(methodExpression))
                    {
                        break;
                    }
                    nameParts.Push(
                        GetIndexerInvocation(
                            methodExpression.Arguments.Single(),
                            expression.Parameters.ToArray()));

                    part = methodExpression.Object;
                }
                else if (part.NodeType == ExpressionType.ArrayIndex)
                {
                    BinaryExpression binaryExpression = (BinaryExpression)part;

                    nameParts.Push(
                        GetIndexerInvocation(
                            binaryExpression.Right,
                            expression.Parameters.ToArray()));

                    part = binaryExpression.Left;
                }
                else if (part.NodeType == ExpressionType.MemberAccess)
                {
                    MemberExpression memberExpressionPart = (MemberExpression)part;
                    nameParts.Push("." + memberExpressionPart.Member.Name);
                    part = memberExpressionPart.Expression;
                }
                else if (part.NodeType == ExpressionType.Parameter)
                {
                    // Dev10 Bug #907611
                    // When the expression is parameter based (m => m.Something...), we'll push an empty
                    // string onto the stack and stop evaluating. The extra empty string makes sure that
                    // we don't accidentally cut off too much of m => m.Model.
                    nameParts.Push(string.Empty);
                    part = null;
                }
                else if (part.NodeType == ExpressionType.Convert)
                {
                    part = ((UnaryExpression)part).Operand;
                }
                else
                {
                    break;
                }
            }

            // If it starts with "model", then strip that away
            if (nameParts.Count > 0 && string.Equals(nameParts.Peek(), ".model", StringComparison.OrdinalIgnoreCase))
            {
                nameParts.Pop();
            }

            if (nameParts.Count > 0)
            {
                return nameParts.Aggregate((left, right) => left + right).TrimStart('.');
            }

            return string.Empty;
        }

        #endregion

        #region Get indexer invocation

        /// <summary>
        /// Get indexer invocation
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="parameters">parameters</param>
        /// <returns>indexer invocation</returns>
        private static string GetIndexerInvocation(Expression expression, ParameterExpression[] parameters)
        {
            Expression converted = Expression.Convert(expression, typeof(object));
            ParameterExpression fakeParameter = Expression.Parameter(typeof(object), null);
            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(converted, fakeParameter);
            Func<object, object> func;

            try
            {
                func = CachedExpressionCompiler.Process(lambda);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return "[" + Convert.ToString(func(null), CultureInfo.InvariantCulture) + "]";
        }

        #endregion

        #region Whether is a single argument indexer

        /// <summary>
        /// Whether is a tsingle argument indexer
        /// </summary>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        internal static bool IsSingleArgumentIndexer(Expression expression)
        {
            if (expression is not MethodCallExpression methodExpression || methodExpression.Arguments.Count != 1)
            {
                return false;
            }
            return methodExpression.Method
                .DeclaringType
                .GetDefaultMembers()
                .OfType<PropertyInfo>()
                .Any(p => p.GetGetMethod() == methodExpression.Method);
        }

        #endregion

        #region Get expression value

        /// <summary>
        /// Get expression value
        /// </summary>
        /// <param name="valueExpression">expression</param>
        /// <returns>value</returns>
        public static object GetExpressionValue(Expression valueExpression)
        {
            object value = null;
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

        #region Whether is compare node type

        /// <summary>
        /// Whether is compare node type
        /// </summary>
        /// <param name="nodeType">node type</param>
        /// <returns>is compare node type</returns>
        public static bool IsCompareNodeType(ExpressionType nodeType)
        {
            bool compareNode = false;
            switch (nodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    compareNode = true;
                    break;
            }
            return compareNode;
        }

        #endregion

        #region Whether is boolean node type

        /// <summary>
        /// Whether is boolean node type
        /// </summary>
        /// <param name="nodeType">node type</param>
        /// <returns>is boolean nodetype</returns>
        public static bool IsConnectionNodeType(ExpressionType nodeType)
        {
            bool boolNode = false;
            switch (nodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    boolNode = true;
                    break;
            }
            return boolNode;
        }

        #endregion

        #region Ensure cast expression

        public static Expression EnsureCastExpression(Expression expression, Type targetType, bool allowWidening = false)
        {
            Type expressionType = expression.Type;

            // check if a cast or conversion is required
            if (expressionType == targetType || !expressionType.IsValueType && targetType.IsAssignableFrom(expressionType))
            {
                return expression;
            }

            if (targetType.IsValueType)
            {
                Expression convert = Expression.Unbox(expression, targetType);

                if (allowWidening && targetType.IsPrimitive)
                {
                    MethodInfo toTargetTypeMethod = typeof(Convert)
                        .GetMethod("To" + targetType.Name, new[] { typeof(object) });

                    if (toTargetTypeMethod != null)
                    {
                        convert = Expression.Condition(
                            Expression.TypeIs(expression, targetType),
                            convert,
                            Expression.Call(toTargetTypeMethod, expression));
                    }
                }

                return Expression.Condition(
                    Expression.Equal(expression, Expression.Constant(null, typeof(object))),
                    Expression.Default(targetType),
                    convert);
            }

            return Expression.Convert(expression, targetType);
        }

        #endregion 

        #region Get property name by expression

        /// <summary>
        /// Get property name by expression parameter
        /// </summary>
        public static string GetExpressionLastPropertyName(Expression propertyExpression)
        {
            string name = string.Empty;
            switch (propertyExpression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    name = (propertyExpression as MemberExpression).Member.Name;
                    break;
                case ExpressionType.Constant:
                    var constantExpression = propertyExpression as ConstantExpression;
                    if (constantExpression.Type == typeof(MethodInfo))
                    {
                        name = (constantExpression.Value as MethodInfo).Name;
                    }
                    else
                    {
                        name = constantExpression.Value?.ToString();
                    }
                    break;
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    UnaryExpression unaryExp = propertyExpression as UnaryExpression;
                    if (unaryExp.Operand.NodeType == ExpressionType.MemberAccess)
                    {
                        name = (unaryExp.Operand as MemberExpression).Member.Name;
                    }
                    break;
            }
            return name;
        }

        /// <summary>
        /// Get property or field name by expression
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="propertyOrField">property or field expression</param>
        /// <returns>property or field name</returns>
        public static string GetExpressionLastPropertyName<T>(Expression<Func<T, dynamic>> propertyOrField)
        {
            return GetExpressionLastPropertyName(propertyOrField.Body);
        }

        #endregion

        #region Get the last child expression

        /// <summary>
        /// Get the last child expression
        /// </summary>
        /// <param name="expression">parent expression</param>
        /// <returns>last child expression expression</returns>
        public static Expression GetLastChildExpression(Expression expression)
        {
            if (expression == null)
            {
                return expression;
            }
            Expression childExpression = expression;
            if (expression.CanReduce)
            {
                return GetLastChildExpression(childExpression.Reduce());
            }
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    childExpression = GetLastChildExpression((expression as LambdaExpression)?.Body);
                    break;
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                case ExpressionType.Conditional:
                case ExpressionType.DebugInfo:
                case ExpressionType.Default:
                case ExpressionType.Dynamic:
                case ExpressionType.Goto:
                case ExpressionType.Index:
                case ExpressionType.Label:
                case ExpressionType.MemberInit:
                case ExpressionType.New:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.NewArrayInit:
                default:
                    break;
                case ExpressionType.Invoke:
                    childExpression = GetLastChildExpression((expression as InvocationExpression)?.Expression);
                    break;
                case ExpressionType.MemberAccess:
                    var memberExpression = expression as MemberExpression;
                    if (memberExpression?.Expression != null)
                    {
                        childExpression = GetLastChildExpression(memberExpression.Expression);
                    }
                    break;
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    childExpression = GetLastChildExpression((expression as UnaryExpression)?.Operand);
                    break;
                case ExpressionType.Call:
                    childExpression = GetLastChildExpression((expression as MethodCallExpression)?.Object);
                    break;
            }
            return childExpression ?? expression;
        }

        #endregion

        #region Get the next expression

        /// <summary>
        /// Get the next expression
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns></returns>
        public static Expression GetChildExpression(Expression expression)
        {
            Expression childExpression = null;
            if (expression != null)
            {
                if (expression.CanReduce)
                {
                    childExpression = expression.Reduce();
                }
                switch (expression.NodeType)
                {
                    case ExpressionType.Lambda:
                        childExpression = (expression as LambdaExpression)?.Body;
                        break;
                    case ExpressionType.And:
                    case ExpressionType.Or:
                    case ExpressionType.ExclusiveOr:
                    case ExpressionType.Add:
                    case ExpressionType.Subtract:
                    case ExpressionType.Multiply:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                    case ExpressionType.LeftShift:
                    case ExpressionType.RightShift:
                        childExpression = (expression as BinaryExpression).Left;
                        break;
                    case ExpressionType.Constant:
                    case ExpressionType.Parameter:
                    case ExpressionType.Conditional:
                    case ExpressionType.DebugInfo:
                    case ExpressionType.Default:
                    case ExpressionType.Dynamic:
                    case ExpressionType.Goto:
                    case ExpressionType.Index:
                    case ExpressionType.Label:
                    case ExpressionType.MemberInit:
                    case ExpressionType.New:
                    case ExpressionType.NewArrayBounds:
                    case ExpressionType.NewArrayInit:
                    default:
                        break;
                    case ExpressionType.Invoke:
                        childExpression = (expression as InvocationExpression)?.Expression;
                        break;
                    case ExpressionType.MemberAccess:
                        var memberExpression = expression as MemberExpression;
                        if (memberExpression?.Expression != null)
                        {
                            childExpression = memberExpression.Expression;
                        }
                        break;
                    case ExpressionType.ArrayLength:
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                    case ExpressionType.Negate:
                    case ExpressionType.NegateChecked:
                    case ExpressionType.Not:
                    case ExpressionType.Quote:
                    case ExpressionType.TypeAs:
                    case ExpressionType.UnaryPlus:
                        childExpression = (expression as UnaryExpression)?.Operand;
                        break;
                    case ExpressionType.Call:
                        var methodCallExpression = expression as MethodCallExpression;
                        childExpression = methodCallExpression.Object;
                        if (childExpression == null && methodCallExpression.Arguments.Count > 0)
                        {
                            childExpression = methodCallExpression.Arguments[0];
                        }
                        break;
                }
            }
            return childExpression;
        }

        #endregion

        #region Get lambda parameter indexes

        /// <summary>
        /// Get lambda parameter indexes
        /// </summary>
        /// <param name="lambdaExpression">Lambda expression</param>
        /// <returns></returns>
        static Dictionary<string, int> GetLambdaParameterIndexes(LambdaExpression lambdaExpression)
        {
            var parameterIndexes = new Dictionary<string, int>();

            if (lambdaExpression != null)
            {
                var parameterExps = lambdaExpression.Parameters;
                if (!parameterExps.IsNullOrEmpty())
                {
                    for (int i = 0; i < parameterExps.Count; i++)
                    {
                        parameterIndexes[parameterExps[i].Name] = i;
                    }
                }
            }

            return parameterIndexes;
        }

        #endregion

        #region Get parameter property expression

        /// <summary>
        /// Get parameter property expression
        /// </summary>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        public static MemberExpression GetParameterMemberAccessExpression(Expression expression)
        {
            MemberExpression memberExpression = null;
            if (expression != null)
            {
                var currentExpression = expression;
                do
                {
                    if (currentExpression is MemberExpression memExp && memExp.Expression != null && memExp.Expression.NodeType == ExpressionType.Parameter)
                    {
                        memberExpression = currentExpression as MemberExpression;
                        break;
                    }
                    currentExpression = GetChildExpression(currentExpression);
                } while (currentExpression != null);
            }
            return memberExpression;
        }

        #endregion

        #endregion

        #region Queryable

        /// <summary>
        /// Get queryable
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <returns>Return a condition</returns>
        internal static ISixnetQueryable GetQueryable<T>(Expression conditionExpression, CriterionConnector connector = CriterionConnector.And)
        {
            var queryable = GetQueryable(conditionExpression, connector) ?? SixnetQuerier.Create<T>();
            if (queryable != null)
            {
                queryable.SetModelType(typeof(T));
            }
            return queryable;
        }

        /// <summary>
        /// Get queryable
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <returns>Return a condition</returns>
        internal static ISixnetQueryable GetQueryable(Expression conditionExpression, CriterionConnector connector = CriterionConnector.And)
        {
            if (conditionExpression == null)
            {
                return null;
            }
            SixnetException.ThrowIf(conditionExpression is not LambdaExpression, "Not a condition expression");

            var lambdaExp = conditionExpression as LambdaExpression;
            var parameterIndexes = GetLambdaParameterIndexes(lambdaExp);
            var conditionExpBody = lambdaExp.Body;
            return GetQueryableCore(parameterIndexes, connector, conditionExpBody);
        }

        /// <summary>
        /// Expression queryable core
        /// </summary>
        /// <param name="parameterIndexes"></param>
        /// <param name="connector"></param>
        /// <param name="conditionExpression"></param>
        /// <returns></returns>
        static ISixnetQueryable GetQueryableCore(Dictionary<string, int> parameterIndexes, CriterionConnector connector, Expression conditionExpression)
        {
            var nodeType = conditionExpression.NodeType;
            var groupQueryable = SixnetQuerier.Create();
            groupQueryable.Connector = connector;

            if (IsCompareNodeType(nodeType))
            {
                groupQueryable.Where(GetExpressionCriterion(parameterIndexes, connector, conditionExpression));
            }
            else if (IsConnectionNodeType(nodeType) && conditionExpression is BinaryExpression binaryExpression)
            {
                var leftQueryable = GetQueryableCore(parameterIndexes, connector, binaryExpression.Left);
                groupQueryable.WhereIf(leftQueryable != null, leftQueryable);

                var rightQueryableConnector = nodeType == ExpressionType.OrElse
                    ? CriterionConnector.Or
                    : CriterionConnector.And;
                var rightQueryable = GetQueryableCore(parameterIndexes, rightQueryableConnector, binaryExpression.Right);
                groupQueryable.WhereIf(rightQueryable != null, rightQueryable);
            }
            else if (nodeType == ExpressionType.Call)
            {
                groupQueryable.Where(GetMethodCriterion(parameterIndexes, connector, conditionExpression));
            }
            else if (nodeType == ExpressionType.Not)
            {
                var unaryExp = conditionExpression as UnaryExpression;
                groupQueryable.Where(GetQueryableCore(parameterIndexes, connector, unaryExp.Operand).Negate());
            }
            if (conditionExpression is ConstantExpression constantExpression && constantExpression.Value is bool boolValue && !boolValue)
            {
                groupQueryable.Where(CreateCriterion(connector, CriterionOperator.False, null, null));
            }
            return groupQueryable;
        }

        /// <summary>
        /// Get expression criterion
        /// </summary>
        /// <param name="parameterIndexes">Parameter indexes</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return a criterion</returns>
        internal static Criterion GetExpressionCriterion(Dictionary<string, int> parameterIndexes, CriterionConnector connector, Expression conditionExpression)
        {
            if (conditionExpression is BinaryExpression binaryExpression)
            {
                var nodeType = binaryExpression.NodeType;
                var leftField = GetDataField(binaryExpression.Left, parameterIndexes);
                var rightField = GetDataField(binaryExpression.Right, parameterIndexes);

                SixnetDirectThrower.ThrowSixnetExceptionIf(leftField == null && rightField == null, "Left and right expression is null");

                var criterionOperator = GetCriterionOperator(nodeType);
                if (leftField == null || rightField == null)
                {
                    SixnetDirectThrower.ThrowSixnetExceptionIf(criterionOperator != CriterionOperator.Equal && criterionOperator != CriterionOperator.NotEqual, $"Not support for {nodeType}");
                    criterionOperator = criterionOperator == CriterionOperator.Equal ? CriterionOperator.IsNull : CriterionOperator.NotNull;
                    leftField ??= rightField;
                    rightField = null;
                }
                return CreateCriterion(connector, criterionOperator, leftField, rightField);
            }
            throw new SixnetException($"Expression type:{conditionExpression?.GetType()} are not supported");
        }

        /// <summary>
        /// Get method call expression criterion
        /// </summary>
        /// <param name="connector">Connection connection operator</param>
        /// <param name="conditionExpression">Connection expression</param>
        /// <returns></returns>
        internal static ISixnetCondition GetMethodCriterion(Dictionary<string, int> parameterIndexes, CriterionConnector connector, Expression conditionExpression)
        {
            var methodCallExpression = conditionExpression as MethodCallExpression;
            Expression fieldExpression = null;
            Expression valueExpression = null;
            ISixnetCondition criterion = null;

            if (methodCallExpression.Method.IsStatic) // Static method
            {
                var methodType = methodCallExpression.Method.DeclaringType;
                // string
                if (methodType == typeof(string))
                {
                    fieldExpression = methodCallExpression.Arguments[0];
                    criterion = GetStringMethodCriterion(parameterIndexes, methodCallExpression.Method.Name, connector, fieldExpression, valueExpression);
                }
                // collection extension method
                else if (methodType == typeof(Enumerable))
                {
                    fieldExpression = methodCallExpression.Arguments[1];
                    valueExpression = methodCallExpression.Arguments[0];
                    criterion = GetIEnumerableMethodCriterion(parameterIndexes, methodCallExpression.Method.Name, connector, fieldExpression, valueExpression);
                }
                else if (methodType == typeof(SixnetQuerier))
                {
                    fieldExpression = methodCallExpression.Arguments[0];
                    criterion = GetSixnetQueryableExtendMethodCriterion(parameterIndexes, methodCallExpression.Method.Name, connector, fieldExpression);
                }

            }
            else // Instance method
            {
                var instanceType = methodCallExpression.Object.Type;
                if (instanceType == typeof(string)) // string
                {
                    fieldExpression = methodCallExpression.Object;
                    valueExpression = methodCallExpression.Arguments[0];
                    criterion = GetStringMethodCriterion(parameterIndexes, methodCallExpression.Method.Name, connector, fieldExpression, valueExpression);
                }
                else if (typeof(IEnumerable).IsAssignableFrom(instanceType)) // collection
                {
                    valueExpression = methodCallExpression.Object;
                    fieldExpression = methodCallExpression.Arguments[0];
                    criterion = GetIEnumerableMethodCriterion(parameterIndexes, methodCallExpression.Method.Name, connector, fieldExpression, valueExpression);
                }
                else if (typeof(ISixnetQueryable).IsAssignableFrom(instanceType)) // subquery
                {
                    valueExpression = methodCallExpression.Object;
                    fieldExpression = methodCallExpression.Arguments[0];
                    criterion = GetSubqueryMethodCriterion(parameterIndexes, methodCallExpression.Method.Name, connector, fieldExpression, valueExpression);
                }
            }

            SixnetDirectThrower.ThrowSixnetExceptionIf(criterion == null, $"Not support {methodCallExpression.Method.Name}");

            return criterion;
        }

        /// <summary>
        /// Get criterion by method call expression for IEnumerable
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="field">MemberArg</param>
        /// <param name="collectionValue">Parameter</param>
        /// <returns>Return a criterion</returns>
        internal static ISixnetCondition GetIEnumerableMethodCriterion(Dictionary<string, int> parameterIndexes, string methodName, CriterionConnector connector, Expression field, Expression collectionValue)
        {
            ISixnetCondition criterion = null;
            IEnumerable values = null;
            var leftField = GetDataField(field, parameterIndexes);
            switch (methodName)
            {
                case "Contains":
                    values = Expression.Lambda(collectionValue)?.Compile().DynamicInvoke() as IEnumerable;
                    SixnetException.ThrowIf(values == null, "The value of the collection type is null or empty");
                    values = SixnetReflecter.Collections.ResolveCollection(values);
                    criterion = CreateCriterion(connector, CriterionOperator.In, leftField, ConstantField.Create(values));
                    break;
                default:
                    throw new NotSupportedException(nameof(methodName));
            }
            return criterion;
        }

        /// <summary>
        /// Get a query item by method call expression with string type
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="field">Expression</param>
        /// <returns>criterion</returns>
        internal static ISixnetCondition GetStringMethodCriterion(Dictionary<string, int> parameterIndexes, string methodName, CriterionConnector connector, Expression field, Expression stringValue)
        {
            var leftField = GetDataField(field, parameterIndexes);
            if (leftField == null)
            {
                return null;
            }
            ISixnetField rightField = null;
            if (stringValue != null)
            {
                var value = Expression.Lambda(stringValue)?.Compile().DynamicInvoke()?.ToString();
                rightField = ConstantField.Create(value);
            }
            var criterionOperator = CriterionOperator.Like;
            switch (methodName)
            {
                case "Contains":
                    criterionOperator = CriterionOperator.Like;
                    break;
                case "StartsWith":
                    criterionOperator = CriterionOperator.BeginLike;
                    break;
                case "EndsWith":
                    criterionOperator = CriterionOperator.EndLike;
                    break;
                case "IsNullOrEmpty":
                    var nullOrEmtpyQueryable = SixnetQuerier.Create();
                    nullOrEmtpyQueryable.Connector = connector;
                    nullOrEmtpyQueryable.Where(CreateCriterion(connector, CriterionOperator.IsNull, leftField, null));
                    nullOrEmtpyQueryable.Where(CreateCriterion(CriterionConnector.Or, CriterionOperator.Equal, leftField, ConstantField.Create("")));
                    return nullOrEmtpyQueryable;
                case "IsNullOrWhiteSpace":
                    var nullOrWhiteSpaceQueryable = SixnetQuerier.Create();
                    nullOrWhiteSpaceQueryable.Connector = connector;
                    nullOrWhiteSpaceQueryable.Where(CreateCriterion(connector, CriterionOperator.IsNull, leftField, null));
                    var copyField = leftField.Clone();
                    copyField.FormatSetting = FieldFormatSetting.Create(FieldFormatterNames.TRIM);
                    nullOrWhiteSpaceQueryable.Where(CreateCriterion(CriterionConnector.Or, CriterionOperator.Equal, copyField, ConstantField.Create("")));
                    return nullOrWhiteSpaceQueryable;
                default:
                    throw new NotSupportedException(methodName);
            }
            return CreateCriterion(connector, criterionOperator, leftField, rightField);
        }

        /// <summary>
        /// Get a subquery condition
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="field">Expression</param>
        /// <returns>criterion</returns>
        internal static ISixnetCondition GetSubqueryMethodCriterion(Dictionary<string, int> parameterIndexes, string methodName, CriterionConnector connector, Expression field, Expression subquery)
        {
            var leftField = GetDataField(field, parameterIndexes);
            if (leftField == null)
            {
                return null;
            }
            ISixnetField rightField = null;
            if (subquery != null)
            {
                var value = Expression.Lambda(subquery)?.Compile().DynamicInvoke() as ISixnetQueryable;
                rightField = QueryableField.Create(value);
            }
            var criterionOperator = CriterionOperator.Equal;
            switch (methodName)
            {
                case "Contains":
                    criterionOperator = CriterionOperator.In;
                    break;
                case "NotContains":
                    criterionOperator = CriterionOperator.NotIn;
                    break;
                case "Equal":
                    criterionOperator = CriterionOperator.Equal;
                    break;
                case "NotEqual":
                    criterionOperator = CriterionOperator.NotEqual;
                    break;
                case "LessThanOrEqual":
                    criterionOperator = CriterionOperator.LessThanOrEqual;
                    break;
                case "LessThan":
                    criterionOperator = CriterionOperator.LessThan;
                    break;
                case "GreaterThan":
                    criterionOperator = CriterionOperator.GreaterThan;
                    break;
                case "GreaterThanOrEqual":
                    criterionOperator = CriterionOperator.GreaterThanOrEqual;
                    break;
                default:
                    throw new NotSupportedException(methodName);
            }
            return CreateCriterion(connector, criterionOperator, leftField, rightField);
        }

        /// <summary>
        /// Get criterion for queryable extension method
        /// </summary>
        /// <param name="parameterIndexes"></param>
        /// <param name="methodName"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static ISixnetCondition GetSixnetQueryableExtendMethodCriterion(Dictionary<string, int> parameterIndexes, string methodName, CriterionConnector connector, Expression field)
        {
            var leftField = GetDataField(field, parameterIndexes);
            if (leftField == null)
            {
                return null;
            }
            var criterionOperator = methodName switch
            {
                "IsNull" => CriterionOperator.IsNull,
                "NotNull" => CriterionOperator.NotNull,
                _ => throw new NotSupportedException(methodName),
            };
            return CreateCriterion(connector, criterionOperator, leftField, null);
        }

        /// <summary>
        /// Create criterion
        /// </summary>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <param name="left">Left field</param>
        /// <param name="right">Right field</param>
        /// <returns></returns>
        internal static Criterion CreateCriterion(CriterionConnector connector, CriterionOperator criterionOperator, ISixnetField left, ISixnetField right)
        {
            return Criterion.Create(criterionOperator, left, right, connector);
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
        /// Get negation criterion operator
        /// </summary>
        /// <param name="criterionOperator"></param>
        /// <returns></returns>
        internal static CriterionOperator GetNegationCriterionOperator(CriterionOperator criterionOperator)
        {
            switch (criterionOperator)
            {
                case CriterionOperator.Equal:
                    return CriterionOperator.NotEqual;
                case CriterionOperator.NotEqual:
                    return CriterionOperator.Equal;
                case CriterionOperator.LessThanOrEqual:
                    return CriterionOperator.GreaterThan;
                case CriterionOperator.LessThan:
                    return CriterionOperator.GreaterThanOrEqual;
                case CriterionOperator.GreaterThan:
                    return CriterionOperator.LessThanOrEqual;
                case CriterionOperator.GreaterThanOrEqual:
                    return CriterionOperator.LessThan;
                case CriterionOperator.In:
                    return CriterionOperator.NotIn;
                case CriterionOperator.NotIn:
                    return CriterionOperator.In;
                case CriterionOperator.Like:
                    return CriterionOperator.NotLike;
                case CriterionOperator.NotLike:
                    return CriterionOperator.Like;
                case CriterionOperator.BeginLike:
                    return CriterionOperator.NotBeginLike;
                case CriterionOperator.NotBeginLike:
                    return CriterionOperator.BeginLike;
                case CriterionOperator.EndLike:
                    return CriterionOperator.NotEndLike;
                case CriterionOperator.NotEndLike:
                    return CriterionOperator.EndLike;
                case CriterionOperator.IsNull:
                    return CriterionOperator.NotNull;
                case CriterionOperator.NotNull:
                    return CriterionOperator.IsNull;
                case CriterionOperator.True:
                    return CriterionOperator.False;
                case CriterionOperator.False:
                    return CriterionOperator.True;
            }
            throw new NotSupportedException(criterionOperator.ToString());
        }

        /// <summary>
        /// Get a data field
        /// </summary>
        /// <param name="fieldExpression">Field expression</param>
        /// <returns></returns>
        public static ISixnetField GetDataField(Expression fieldExpression)
        {
            SixnetDirectThrower.ThrowArgNullIf(fieldExpression == null, $"{nameof(fieldExpression)} is null");
            Dictionary<string, int> parameterIndexes = null;
            if (fieldExpression is LambdaExpression lambdaExp)
            {
                parameterIndexes = GetLambdaParameterIndexes(lambdaExp);
                fieldExpression = lambdaExp.Body;
            }
            return GetDataField(fieldExpression, parameterIndexes);
        }

        /// <summary>
        /// Get criterion field
        /// </summary>
        /// <param name="fieldExpression">Field expression</param>
        /// <returns></returns>
        public static ISixnetField GetDataField(Expression fieldExpression, Dictionary<string, int> parameterIndexes)
        {
            var childExpression = fieldExpression;
            SixnetDirectThrower.ThrowArgNullIf(childExpression == null, $"Not support expression:{fieldExpression?.GetType()}");
            ISixnetField criterionField = null;
            FieldFormatSetting fieldFormatSetting = null;
            FieldFormatSetting innermostFieldFormatSetting = null;
            var isEnd = false;

            do
            {
                switch (childExpression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var memberExpression = childExpression as MemberExpression;
                        if (memberExpression.Expression.NodeType == ExpressionType.Parameter)
                        {
                            var parameterExp = memberExpression.Expression as ParameterExpression;
                            var propertyName = memberExpression.Member.Name;
                            var modelType = parameterExp.Type;
                            var modelTypeIndex = 0;
                            parameterIndexes?.TryGetValue(parameterExp.Name, out modelTypeIndex);
                            var entityField = SixnetEntityManager.GetField(modelType, propertyName);
                            if (entityField?.FormatSetting != null)
                            {
                                innermostFieldFormatSetting.Child = entityField.FormatSetting;
                            }
                            var propertyField = DataField.Create(propertyName, modelType, modelTypeIndex, null, entityField?.FieldName);
                            propertyField.DataType = memberExpression.Type;
                            criterionField = propertyField;
                            isEnd = true;
                        }
                        break;
                    case ExpressionType.Constant:
                        var value = Expression.Lambda(childExpression).Compile().DynamicInvoke();
                        if (value != null)
                        {
                            criterionField = ConstantField.Create(value);
                        }
                        isEnd = true;
                        break;
                    default:
                        break;
                }
                if (isEnd)
                {
                    childExpression = null;
                }
                else
                {
                    // field format setting
                    var newFieldFormatSetting = GetFieldFormatSetting(childExpression, parameterIndexes);
                    if (newFieldFormatSetting != null)
                    {
                        newFieldFormatSetting.Child = fieldFormatSetting;
                        if (fieldFormatSetting == null)
                        {
                            innermostFieldFormatSetting = newFieldFormatSetting;
                        }
                        fieldFormatSetting = newFieldFormatSetting;
                    }
                    childExpression = GetChildExpression(childExpression);
                }
            } while (childExpression != null);

            if (criterionField != null)
            {
                criterionField.FormatSetting = fieldFormatSetting;
            }
            return criterionField;
        }

        /// <summary>
        /// Get output data field
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="fieldExpression">Field expression</param>
        /// <returns></returns>
        public static ISixnetField GetOutputDataField(ISixnetQueryable queryable, Expression fieldExpression)
        {
            #region Type index

            var typeIndexes = new Dictionary<string, int>();
            var queryableType = queryable?.GetType();
            if (queryableType != null && queryableType.IsGenericType)
            {
                var genericParameters = queryableType.GenericTypeArguments;
                if (!genericParameters.IsNullOrEmpty())
                {
                    for (int i = 0; i < genericParameters.Length; i++)
                    {
                        typeIndexes[genericParameters[i].FullName] = i;
                    }
                }
            }

            #endregion

            return GetDataField(fieldExpression, typeIndexes);
        }

        /// <summary>
        /// Get field format setting
        /// </summary>
        /// <param name="formatExpression">Format expression</param>
        /// <returns></returns>
        static FieldFormatSetting GetFieldFormatSetting(Expression formatExpression, Dictionary<string, int> parameterIndexes)
        {
            FieldFormatSetting fieldFormatSetting = null;
            if (formatExpression != null)
            {
                switch (formatExpression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var memberExpression = formatExpression as MemberExpression;
                        // char length
                        if (memberExpression.Member.Name == nameof(string.Length)
                            && memberExpression.Expression.NodeType == ExpressionType.MemberAccess
                            && memberExpression.Expression.Type == typeof(string))
                        {
                            fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.CHARLENGTH);
                        }
                        break;
                    case ExpressionType.Call:
                        var methodCallExpression = formatExpression as MethodCallExpression;
                        if (methodCallExpression.Object != null
                            && methodCallExpression.Method.DeclaringType == typeof(string))
                        {
                            if (methodCallExpression.Method.Name == nameof(string.Trim)) // trim
                            {
                                fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.TRIM);
                            }
                        }
                        else if (methodCallExpression.Object == null
                            && methodCallExpression.Method.IsStatic
                            && methodCallExpression.Method.DeclaringType == typeof(SixnetQuerier))
                        {
                            if (methodCallExpression.Method.Name == nameof(SixnetQuerier.Max)) // max
                            {
                                fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.MAX);
                            }
                            else if (methodCallExpression.Method.Name == nameof(SixnetQuerier.Min)) // min
                            {
                                fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.MIN);
                            }
                            else if (methodCallExpression.Method.Name == nameof(SixnetQuerier.Avg)) // avg
                            {
                                fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.AVG);
                            }
                            else if (methodCallExpression.Method.Name == nameof(SixnetQuerier.Count)) // count
                            {
                                fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.COUNT);
                            }
                            else if (methodCallExpression.Method.Name == nameof(SixnetQuerier.Sum)) // sum
                            {
                                fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.SUM);
                            }
                            else if (methodCallExpression.Method.Name == nameof(SixnetQuerier.JsonValue)) // json value
                            {
                                fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.JSON_VALUE, GetDataField(methodCallExpression.Arguments[1], parameterIndexes));
                            }
                            else if (methodCallExpression.Method.Name == nameof(SixnetQuerier.JsonObject)) // json object
                            {
                                fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.JSON_OBJECT, GetDataField(methodCallExpression.Arguments[1], parameterIndexes));
                            }
                        }
                        break;
                    case ExpressionType.And:
                        var andExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.AND, GetDataField(andExpression.Right));
                        break;
                    case ExpressionType.Or:
                        var orExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.OR, GetDataField(orExpression.Right));
                        break;
                    case ExpressionType.ExclusiveOr:
                        var exclusiveOrExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.XOR, GetDataField(exclusiveOrExpression.Right));
                        break;
                    case ExpressionType.Not:
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.NOT);
                        break;
                    case ExpressionType.Add:
                        var addExpression = formatExpression as BinaryExpression;
                        var parameterField = GetDataField(addExpression.Right);
                        var fieldDataType = parameterField.GetDataType();
                        fieldFormatSetting = FieldFormatSetting.Create((fieldDataType == _stringType || fieldDataType == _charType) ? FieldFormatterNames.STRING_CONCAT : FieldFormatterNames.ADD, parameterField);
                        break;
                    case ExpressionType.Subtract:
                        var subtractExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.SUBTRACT, GetDataField(subtractExpression.Right));
                        break;
                    case ExpressionType.Multiply:
                        var multiplyExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.MULTIPLY, GetDataField(multiplyExpression.Right));
                        break;
                    case ExpressionType.Divide:
                        var divideExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.DIVIDE, GetDataField(divideExpression.Right));
                        break;
                    case ExpressionType.Modulo:
                        var moduloExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.MODULO, GetDataField(moduloExpression.Right));
                        break;
                    case ExpressionType.RightShift:
                        var rightShiftExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.RIGHT_SHIFT, GetDataField(rightShiftExpression.Right));
                        break;
                    case ExpressionType.LeftShift:
                        var leftShiftExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = FieldFormatSetting.Create(FieldFormatterNames.LEFT_SHIFT, GetDataField(leftShiftExpression.Right));
                        break;
                }
            }

            return fieldFormatSetting;
        }

        #endregion

        #region Fields assignment 

        /// <summary>
        /// Get modification
        /// </summary>
        /// <param name="updateExpression">Update expression</param>
        /// <returns></returns>
        public static FieldsAssignment GetFieldsAssignment(Expression updateExpression)
        {
            SixnetDirectThrower.ThrowArgNullIf(updateExpression == null, nameof(updateExpression));
            SixnetDirectThrower.ThrowNotSupportIf(updateExpression.NodeType != ExpressionType.Lambda, updateExpression.NodeType.ToString());

            var fieldsAssignment = FieldsAssignment.Create();
            var lambdaExp = updateExpression as LambdaExpression;
            var parameterIndexes = GetLambdaParameterIndexes(lambdaExp);

            GenerateFieldsAssignmentCore(fieldsAssignment, parameterIndexes, lambdaExp.Body);
            return fieldsAssignment;
        }

        static void GenerateFieldsAssignmentCore(FieldsAssignment fieldsAssignment, Dictionary<string, int> parameterIndexes, Expression updateExpression)
        {
            var binaryExp = updateExpression as BinaryExpression;
            SixnetDirectThrower.ThrowNotSupportIf(binaryExp == null, binaryExp.NodeType.ToString());

            switch (updateExpression.NodeType)
            {
                case ExpressionType.AndAlso:
                    GenerateFieldsAssignmentCore(fieldsAssignment, parameterIndexes, binaryExp.Left);
                    GenerateFieldsAssignmentCore(fieldsAssignment, parameterIndexes, binaryExp.Right);
                    break;
                case ExpressionType.Equal:
                    var equalExp = updateExpression as BinaryExpression;

                    // left field
                    var leftMemberExp = GetParameterMemberAccessExpression(equalExp.Left);
                    SixnetException.ThrowIf(leftMemberExp == null, $"Left expression isn't a member");
                    var propertyName = leftMemberExp.Member.Name;

                    // right
                    var rightField = GetDataField(equalExp.Right, parameterIndexes);

                    fieldsAssignment.SetNewValue(propertyName, rightField);
                    break;
                default:
                    throw new NotSupportedException(updateExpression.NodeType.ToString());
            }
        }

        #endregion
    }
}
