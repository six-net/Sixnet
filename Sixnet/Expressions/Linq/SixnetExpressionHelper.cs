// "Company © 2025. All rights reserved."

using System.Collections;
using System.Globalization;
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
                func = SixnetCachedExpressionCompiler.Process(lambda);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return "[" + System.Convert.ToString(func(null), CultureInfo.InvariantCulture) + "]";
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

        #region Get expression first property name

        /// <summary>
        /// Get expression first property name
        /// </summary>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        public static string GetExpressionFirstPropertyName(Expression propertyExpression)
        {
            var lastExpression = GetLastChildExpression(propertyExpression, true);
            if (lastExpression is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            return string.Empty;
        }

        #endregion

        #region Get the last child expression

        /// <summary>
        /// Get the last child expression
        /// </summary>
        /// <param name="expression">parent expression</param>
        /// <returns>last child expression expression</returns>
        public static Expression GetLastChildExpression(Expression expression, bool forLastMemberExp = false)
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
                        if (forLastMemberExp && memberExpression.Expression.NodeType != ExpressionType.MemberAccess)
                        {
                            break;
                        }
                        else
                        {
                            childExpression = GetLastChildExpression(memberExpression.Expression);
                        }
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

        #region Get expression compile value

        public static dynamic CompileExpressionValue(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
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
        internal static ISixnetQueryable GetQueryable<T>(Expression conditionExpression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
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
        internal static ISixnetQueryable GetQueryable(Expression conditionExpression, SixnetCriterionConnector connector = SixnetCriterionConnector.And)
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
        static ISixnetQueryable GetQueryableCore(Dictionary<string, int> parameterIndexes, SixnetCriterionConnector connector, Expression conditionExpression)
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
                    ? SixnetCriterionConnector.Or
                    : SixnetCriterionConnector.And;
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
            else if (nodeType == ExpressionType.MemberAccess)
            {
                groupQueryable.Where(GetMemberAccessCriterion(parameterIndexes, connector, conditionExpression));
            }
            if (conditionExpression is ConstantExpression constantExpression && constantExpression.Value is bool boolValue && !boolValue)
            {
                groupQueryable.Where(CreateCriterion(connector, SixnetCriterionOperator.False, null, null));
            }
            return groupQueryable;
        }

        /// <summary>
        /// Get expression criterion
        /// </summary>
        /// <param name="parameterIndexes">Parameter indexes</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return a criterion</returns>
        internal static SixnetCriterion GetExpressionCriterion(Dictionary<string, int> parameterIndexes, SixnetCriterionConnector connector, Expression conditionExpression)
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
                    SixnetDirectThrower.ThrowSixnetExceptionIf(criterionOperator != SixnetCriterionOperator.Equal && criterionOperator != SixnetCriterionOperator.NotEqual, $"Not support for {nodeType}");
                    criterionOperator = criterionOperator == SixnetCriterionOperator.Equal ? SixnetCriterionOperator.IsNull : SixnetCriterionOperator.NotNull;
                    leftField ??= rightField;
                    rightField = null;
                }
                return CreateCriterion(connector, criterionOperator, leftField, rightField);
            }
            throw new SixnetException($"Expression type:{conditionExpression?.GetType()} are not supported");
        }

        /// <summary>
        /// Get member access criterion
        /// </summary>
        /// <param name="parameterIndexes"></param>
        /// <param name="connector"></param>
        /// <param name="conditionExpression"></param>
        /// <returns></returns>
        internal static ISixnetCondition GetMemberAccessCriterion(Dictionary<string, int> parameterIndexes, SixnetCriterionConnector connector, Expression conditionExpression)
        {
            var memberExpression = conditionExpression as MemberExpression;
            var memberName = memberExpression.Member.Name;
            ISixnetCondition condition = null;
            switch (memberName)
            {
                case "HasValue":
                    condition = CreateCriterion(connector, SixnetCriterionOperator.NotNull, GetDataField(memberExpression.Expression, parameterIndexes), null);
                    break;
            }
            return condition;
        }

        /// <summary>
        /// Get method call expression criterion
        /// </summary>
        /// <param name="connector">Connection connection operator</param>
        /// <param name="conditionExpression">Connection expression</param>
        /// <returns></returns>
        internal static ISixnetCondition GetMethodCriterion(Dictionary<string, int> parameterIndexes, SixnetCriterionConnector connector, Expression conditionExpression)
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
                else if (methodType == typeof(SixnetDbFunc) || methodType == typeof(SixnetSixnetFieldExtensions))
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
                    criterion = GetSubqueryMethodCriterion(parameterIndexes, connector, methodCallExpression);
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
        internal static ISixnetCondition GetIEnumerableMethodCriterion(Dictionary<string, int> parameterIndexes, string methodName, SixnetCriterionConnector connector, Expression field, Expression collectionValue)
        {
            ISixnetCondition criterion = null;
            IEnumerable values = null;
            var leftField = GetDataField(field, parameterIndexes);
            switch (methodName)
            {
                case "Contains":
                    values = CompileExpressionValue(collectionValue) as IEnumerable;
                    SixnetException.ThrowIf(values == null, "The value of the collection type is null or empty");
                    values = SixnetReflecter.Collections.ResolveCollection(values);
                    criterion = CreateCriterion(connector, SixnetCriterionOperator.In, leftField, SixnetConstantField.Create(values));
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
        internal static ISixnetCondition GetStringMethodCriterion(Dictionary<string, int> parameterIndexes, string methodName, SixnetCriterionConnector connector, Expression field, Expression stringValue)
        {
            var leftField = GetDataField(field, parameterIndexes);
            if (leftField == null)
            {
                return null;
            }
            ISixnetField rightField = null;
            if (stringValue != null)
            {
                var value = CompileExpressionValue(stringValue)?.ToString();
                rightField = SixnetConstantField.Create(value);
            }
            var criterionOperator = SixnetCriterionOperator.Like;
            switch (methodName)
            {
                case "Contains":
                    criterionOperator = SixnetCriterionOperator.Like;
                    break;
                case "StartsWith":
                    criterionOperator = SixnetCriterionOperator.BeginLike;
                    break;
                case "EndsWith":
                    criterionOperator = SixnetCriterionOperator.EndLike;
                    break;
                case "IsNullOrEmpty":
                    var nullOrEmtpyQueryable = SixnetQuerier.Create();
                    nullOrEmtpyQueryable.Connector = connector;
                    nullOrEmtpyQueryable.Where(CreateCriterion(connector, SixnetCriterionOperator.IsNull, leftField, null));
                    nullOrEmtpyQueryable.Where(CreateCriterion(SixnetCriterionConnector.Or, SixnetCriterionOperator.Equal, leftField, SixnetConstantField.Create("")));
                    return nullOrEmtpyQueryable;
                case "IsNullOrWhiteSpace":
                    var nullOrWhiteSpaceQueryable = SixnetQuerier.Create();
                    nullOrWhiteSpaceQueryable.Connector = connector;
                    nullOrWhiteSpaceQueryable.Where(CreateCriterion(connector, SixnetCriterionOperator.IsNull, leftField, null));
                    var copyField = leftField.Clone();
                    copyField.FormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TRIM);
                    nullOrWhiteSpaceQueryable.Where(CreateCriterion(SixnetCriterionConnector.Or, SixnetCriterionOperator.Equal, copyField, SixnetConstantField.Create("")));
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
        internal static ISixnetCondition GetSubqueryMethodCriterion(Dictionary<string, int> parameterIndexes, SixnetCriterionConnector connector, MethodCallExpression methodCallExpression)
        {
            var valueExpression = methodCallExpression.Object;
            var fieldExpression = !methodCallExpression.Arguments.IsNullOrEmpty() ? methodCallExpression.Arguments[0] : null;
            ISixnetField leftField = null;
            if (fieldExpression != null)
            {
                leftField = GetDataField(fieldExpression, parameterIndexes);
                if (leftField == null)
                {
                    return null;
                }
            }
            ISixnetField rightField = null;
            if (valueExpression != null)
            {
                var value = CompileExpressionValue(valueExpression) as ISixnetQueryable;
                rightField = SixnetQueryableField.Create(value);
            }
            var methodName = methodCallExpression.Method.Name;
            var criterionOperator = SixnetCriterionOperator.Equal;
            switch (methodName)
            {
                case "Contains":
                    criterionOperator = SixnetCriterionOperator.In;
                    break;
                case "NotContains":
                    criterionOperator = SixnetCriterionOperator.NotIn;
                    break;
                case "Equal":
                    criterionOperator = SixnetCriterionOperator.Equal;
                    break;
                case "NotEqual":
                    criterionOperator = SixnetCriterionOperator.NotEqual;
                    break;
                case "LessThanOrEqual":
                    criterionOperator = SixnetCriterionOperator.LessThanOrEqual;
                    break;
                case "LessThan":
                    criterionOperator = SixnetCriterionOperator.LessThan;
                    break;
                case "GreaterThan":
                    criterionOperator = SixnetCriterionOperator.GreaterThan;
                    break;
                case "GreaterThanOrEqual":
                    criterionOperator = SixnetCriterionOperator.GreaterThanOrEqual;
                    break;
                case "Exists":
                    criterionOperator = SixnetCriterionOperator.None;
                    rightField.FormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.EXISTS);
                    break;
                case "NotExists":
                    criterionOperator = SixnetCriterionOperator.None;
                    rightField.FormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.NOT_EXISTS);
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
        internal static ISixnetCondition GetSixnetQueryableExtendMethodCriterion(Dictionary<string, int> parameterIndexes, string methodName, SixnetCriterionConnector connector, Expression field)
        {
            var leftField = GetDataField(field, parameterIndexes);
            if (leftField == null)
            {
                return null;
            }
            var criterionOperator = methodName switch
            {
                "IsNull" => SixnetCriterionOperator.IsNull,
                "DbIsNull" => SixnetCriterionOperator.IsNull,
                "NotNull" => SixnetCriterionOperator.NotNull,
                "DbNotNull" => SixnetCriterionOperator.NotNull,
                _ => SixnetCriterionOperator.None,
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
        internal static SixnetCriterion CreateCriterion(SixnetCriterionConnector connector, SixnetCriterionOperator criterionOperator, ISixnetField left, ISixnetField right)
        {
            return SixnetCriterion.Create(criterionOperator, left, right, connector);
        }

        /// <summary>
        /// Get criterion operator by expression type
        /// </summary>
        /// <param name="expressType">Expression type</param>
        /// <returns>Return criterion operator</returns>
        internal static SixnetCriterionOperator GetCriterionOperator(ExpressionType expressType)
        {
            SixnetCriterionOperator criterionOperator = SixnetCriterionOperator.Equal;
            switch (expressType)
            {
                case ExpressionType.Equal:
                default:
                    criterionOperator = SixnetCriterionOperator.Equal;
                    break;
                case ExpressionType.NotEqual:
                    criterionOperator = SixnetCriterionOperator.NotEqual;
                    break;
                case ExpressionType.LessThanOrEqual:
                    criterionOperator = SixnetCriterionOperator.LessThanOrEqual;
                    break;
                case ExpressionType.LessThan:
                    criterionOperator = SixnetCriterionOperator.LessThan;
                    break;
                case ExpressionType.GreaterThan:
                    criterionOperator = SixnetCriterionOperator.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    criterionOperator = SixnetCriterionOperator.GreaterThanOrEqual;
                    break;
            }
            return criterionOperator;
        }

        /// <summary>
        /// Get negation criterion operator
        /// </summary>
        /// <param name="criterionOperator"></param>
        /// <returns></returns>
        internal static SixnetCriterionOperator GetNegationCriterionOperator(SixnetCriterionOperator criterionOperator)
        {
            switch (criterionOperator)
            {
                case SixnetCriterionOperator.Equal:
                    return SixnetCriterionOperator.NotEqual;
                case SixnetCriterionOperator.NotEqual:
                    return SixnetCriterionOperator.Equal;
                case SixnetCriterionOperator.LessThanOrEqual:
                    return SixnetCriterionOperator.GreaterThan;
                case SixnetCriterionOperator.LessThan:
                    return SixnetCriterionOperator.GreaterThanOrEqual;
                case SixnetCriterionOperator.GreaterThan:
                    return SixnetCriterionOperator.LessThanOrEqual;
                case SixnetCriterionOperator.GreaterThanOrEqual:
                    return SixnetCriterionOperator.LessThan;
                case SixnetCriterionOperator.In:
                    return SixnetCriterionOperator.NotIn;
                case SixnetCriterionOperator.NotIn:
                    return SixnetCriterionOperator.In;
                case SixnetCriterionOperator.Like:
                    return SixnetCriterionOperator.NotLike;
                case SixnetCriterionOperator.NotLike:
                    return SixnetCriterionOperator.Like;
                case SixnetCriterionOperator.BeginLike:
                    return SixnetCriterionOperator.NotBeginLike;
                case SixnetCriterionOperator.NotBeginLike:
                    return SixnetCriterionOperator.BeginLike;
                case SixnetCriterionOperator.EndLike:
                    return SixnetCriterionOperator.NotEndLike;
                case SixnetCriterionOperator.NotEndLike:
                    return SixnetCriterionOperator.EndLike;
                case SixnetCriterionOperator.IsNull:
                    return SixnetCriterionOperator.NotNull;
                case SixnetCriterionOperator.NotNull:
                    return SixnetCriterionOperator.IsNull;
                case SixnetCriterionOperator.True:
                    return SixnetCriterionOperator.False;
                case SixnetCriterionOperator.False:
                    return SixnetCriterionOperator.True;
            }
            throw new NotSupportedException(criterionOperator.ToString());
        }

        /// <summary>
        /// Get a data field
        /// </summary>
        /// <param name="fieldExpression">Field expression</param>
        /// <returns></returns>
        public static ISixnetField GetDataField(Expression fieldExpression, SixnetFieldFormatSetting formatSetting = null)
        {
            return GetDataFields(fieldExpression, formatSetting).FirstOrDefault();
        }

        /// <summary>
        /// Get data field
        /// </summary>
        /// <param name="fieldExpression">Field expression</param>
        /// <returns></returns>
        static ISixnetField GetDataField(Expression fieldExpression, Dictionary<string, int> parameterIndexes, SixnetFieldFormatSetting outFormatSetting = null)
        {
            return GetExpressionDataFields(fieldExpression, parameterIndexes, outFormatSetting).FirstOrDefault();
        }

        /// <summary>
        /// Get data fields
        /// </summary>
        /// <param name="fieldExpression"></param>
        /// <param name="formatSetting"></param>
        /// <returns></returns>
        public static List<ISixnetField> GetDataFields(Expression fieldExpression, SixnetFieldFormatSetting formatSetting = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(fieldExpression == null, $"{nameof(fieldExpression)} is null");
            Dictionary<string, int> parameterIndexes = null;
            if (fieldExpression is LambdaExpression lambdaExp)
            {
                parameterIndexes = GetLambdaParameterIndexes(lambdaExp);
                fieldExpression = lambdaExp.Body;
            }
            return GetExpressionDataFields(fieldExpression, parameterIndexes, formatSetting);
        }

        /// <summary>
        /// Get expression data fields
        /// </summary>
        /// <param name="fieldExpression">Field expression</param>
        /// <returns></returns>
        static List<ISixnetField> GetExpressionDataFields(Expression fieldExpression, Dictionary<string, int> parameterIndexes, SixnetFieldFormatSetting outFormatSetting = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(fieldExpression == null, nameof(fieldExpression));
            var dataExpression = fieldExpression;
            var fields = new List<ISixnetField>();

            if (fieldExpression is LambdaExpression lambdaExp)
            {
                dataExpression = lambdaExp.Body;
            }
            switch (dataExpression.NodeType)
            {
                case ExpressionType.New:
                    var newExpression = dataExpression as NewExpression;
                    if (!(newExpression?.Arguments?.IsNullOrEmpty() ?? true))
                    {
                        for (var i = 0; i < newExpression.Arguments.Count; i++)
                        {
                            var member = newExpression.Members[i];
                            var arg = newExpression.Arguments[i];
                            var argField = GetSingleExpressionDataField(arg, parameterIndexes, outFormatSetting, member.Name);
                            if (argField != null)
                            {
                                fields.Add(argField);
                            }
                        }
                    }
                    break;
                case ExpressionType.NewArrayInit:
                    var newArrayInitExpression = dataExpression as NewArrayExpression;
                    if (!(newArrayInitExpression?.Expressions?.IsNullOrEmpty() ?? true))
                    {
                        foreach (var eleExp in newArrayInitExpression.Expressions)
                        {
                            var argField = GetSingleExpressionDataField(eleExp, parameterIndexes, outFormatSetting);
                            if (argField != null)
                            {
                                fields.Add(argField);
                            }
                        }
                    }
                    break;
                case ExpressionType.ListInit:
                    var listInitExpression = dataExpression as ListInitExpression;
                    if (!(listInitExpression?.Initializers.IsNullOrEmpty() ?? true))
                    {
                        foreach (var ele in listInitExpression.Initializers)
                        {
                            var argField = GetSingleExpressionDataField(ele.Arguments[0], parameterIndexes, outFormatSetting);
                            if (argField != null)
                            {
                                fields.Add(argField);
                            }
                        }
                    }
                    break;
                case ExpressionType.Conditional:
                    fields.Add(GetConditionalDataField(fieldExpression, parameterIndexes));
                    break;
                default:
                    fields.Add(GetSingleExpressionDataField(dataExpression, parameterIndexes, outFormatSetting));
                    break;
            }
            return fields;
        }

        /// <summary>
        /// Get conditional data field
        /// </summary>
        /// <param name="fieldExpression"></param>
        /// <param name="parameterIndexes"></param>
        /// <returns></returns>
        static SixnetConditionalDataField GetConditionalDataField(Expression fieldExpression, Dictionary<string, int> parameterIndexes, SixnetFieldFormatSetting outFormatSetting = null)
        {
            var conditionalExpression = fieldExpression as ConditionalExpression;
            SixnetThrower.ThrowArgErrorIf(conditionalExpression == null, "Expression is not conditional");

            var conditionalField = new SixnetConditionalDataField()
            {
                Conditions = new List<ConditionalDataFieldConditionItem>()
            };
            // test
            var testConditionItem = new ConditionalDataFieldConditionItem()
            {
                Condition = GetQueryableCore(parameterIndexes, SixnetCriterionConnector.And, conditionalExpression.Test),
                Value = GetDataField(conditionalExpression.IfTrue, parameterIndexes)
            };
            conditionalField.Conditions.Add(testConditionItem);

            var falseExpression = conditionalExpression.IfFalse;
            while (falseExpression is ConditionalExpression falseConditionalExpression)
            {
                var falseTestConditionItem = new ConditionalDataFieldConditionItem()
                {
                    Condition = GetQueryableCore(parameterIndexes, SixnetCriterionConnector.And, falseConditionalExpression.Test),
                    Value = GetDataField(falseConditionalExpression.IfTrue, parameterIndexes)
                };
                conditionalField.Conditions.Add(falseTestConditionItem);
                falseExpression = falseConditionalExpression.IfFalse;
            }
            conditionalField.FalseValue = GetDataField(falseExpression, parameterIndexes);

            return conditionalField;
        }

        /// <summary>
        /// Get single data field
        /// </summary>
        /// <param name="fieldExpression">Field expression</param>
        /// <returns></returns>
        static ISixnetField GetSingleExpressionDataField(Expression fieldExpression, Dictionary<string, int> parameterIndexes, SixnetFieldFormatSetting outFormatSetting = null, string cusPropertyName = "")
        {
            var childExpression = fieldExpression;
            SixnetDirectThrower.ThrowArgNullIf(childExpression == null, $"Not support expression:{fieldExpression?.GetType()}");
            ISixnetField dataField = null;
            SixnetFieldFormatSetting fieldFormatSetting = null;
            SixnetFieldFormatSetting innermostFieldFormatSetting = null;
            Expression constantMemberAccessExpression = null;
            var isEnd = false;

            do
            {
                switch (childExpression.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var memberExpression = childExpression as MemberExpression;
                        if (memberExpression.Expression?.NodeType == ExpressionType.Parameter)
                        {
                            var memberName = memberExpression.Member.Name;
                            var parameterExp = memberExpression.Expression as ParameterExpression;
                            var propertyName = string.IsNullOrWhiteSpace(cusPropertyName) ? memberName : cusPropertyName;
                            var modelType = parameterExp.Type;
                            var modelTypeIndex = 0;
                            parameterIndexes?.TryGetValue(parameterExp.Name, out modelTypeIndex);
                            var entityField = SixnetEntityManager.GetField(modelType, memberName);
                            if (entityField?.FormatSetting != null)
                            {
                                var fildOriginalFormattingSetting = entityField.FormatSetting.Clone();
                                fildOriginalFormattingSetting.SetChild(fieldFormatSetting);
                                fieldFormatSetting = fildOriginalFormattingSetting;
                                innermostFieldFormatSetting ??= fildOriginalFormattingSetting;
                            }
                            var propertyField = SixnetDataField.Create(propertyName, modelType, modelTypeIndex, null, entityField?.FieldName);
                            propertyField.DataType = memberExpression.Type;
                            dataField = propertyField;
                            isEnd = true;
                        }
                        else
                        {
                            if (memberExpression.Type.GetRealValueType() == typeof(bool))
                            {
                                dataField = GetConditionalDataField(Expression.Condition(childExpression, Expression.Constant(true), Expression.Constant(false)), parameterIndexes);
                                dataField.PropertyName = cusPropertyName;
                                isEnd = true;
                            }
                            else if (constantMemberAccessExpression == null)
                            {
                                constantMemberAccessExpression = memberExpression;
                            }
                        }
                        break;
                    case ExpressionType.OrElse:
                    case ExpressionType.AndAlso:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                        var conditionalExp = Expression.Condition(childExpression, Expression.Constant(true), Expression.Constant(false));
                        dataField = GetConditionalDataField(conditionalExp, parameterIndexes);
                        dataField.PropertyName = cusPropertyName;
                        isEnd = true;
                        break;
                    case ExpressionType.Conditional:
                        dataField = GetConditionalDataField(childExpression, parameterIndexes);
                        dataField.PropertyName = cusPropertyName;
                        isEnd = true;
                        break;
                    case ExpressionType.Call:
                        var methodCallExp = childExpression as MethodCallExpression;
                        if (methodCallExp.Method.ReturnType.GetRealValueType() == typeof(bool))
                        {
                            dataField = GetConditionalDataField(Expression.Condition(childExpression, Expression.Constant(true), Expression.Constant(false)), parameterIndexes);
                            dataField.PropertyName = cusPropertyName;
                            isEnd = true;
                        }
                        break;
                    case ExpressionType.Constant:
                        if (fieldFormatSetting != null && fieldFormatSetting.HasDataField)
                        {
                            var value = CompileExpressionValue(constantMemberAccessExpression ?? childExpression);
                            if (value != null)
                            {
                                dataField = SixnetConstantField.Create(value);
                            }
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
                        constantMemberAccessExpression = null;
                        newFieldFormatSetting.SetChild(fieldFormatSetting);
                        if (fieldFormatSetting == null)
                        {
                            innermostFieldFormatSetting = newFieldFormatSetting;
                        }
                        fieldFormatSetting = newFieldFormatSetting;
                    }
                    childExpression = GetChildExpression(childExpression);
                }
            } while (childExpression != null);

            if (dataField == null)
            {
                try
                {
                    var value = CompileExpressionValue(fieldExpression);
                    if (value != null)
                    {
                        if (value is ISixnetQueryable)
                        {
                            dataField = SixnetQueryableField.Create(value);
                        }
                        else
                        {
                            dataField = SixnetConstantField.Create(value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SixnetDirectThrower.ThrowNotSupportIf(true, $"Not support for node type:{fieldExpression.NodeType},{ex.Message}");
                }
            }
            else
            {
                if (outFormatSetting != null)
                {
                    if (innermostFieldFormatSetting != null)
                    {
                        innermostFieldFormatSetting.SetChild(outFormatSetting);
                    }
                    else
                    {
                        fieldFormatSetting = outFormatSetting;
                    }
                }
                dataField.FormatSetting = fieldFormatSetting;
            }
            return dataField;
        }

        /// <summary>
        /// Get field format setting
        /// </summary>
        /// <param name="formatExpression">Format expression</param>
        /// <returns></returns>
        static SixnetFieldFormatSetting GetFieldFormatSetting(Expression formatExpression, Dictionary<string, int> parameterIndexes)
        {
            SixnetFieldFormatSetting fieldFormatSetting = null;
            if (formatExpression != null)
            {
                switch (formatExpression.NodeType)
                {
                    #region MemberAccess

                    case ExpressionType.MemberAccess:
                        var memberExpression = formatExpression as MemberExpression;
                        var memberType = memberExpression.Expression?.Type;
                        var memberName = memberExpression.Member.Name;
                        if (memberExpression.Expression?.NodeType == ExpressionType.MemberAccess)
                        {
                            #region String

                            if (memberType == typeof(string)) // String
                            {
                                if (memberName == nameof(string.Length))
                                {
                                    fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CHARLENGTH);
                                }
                            }

                            #endregion

                            #region DateTime

                            else if (memberType == typeof(DateTimeOffset) || memberType == typeof(DateTime))
                            {
                                switch (memberName)
                                {
                                    case "Date":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_DATE);
                                        break;
                                    case "Year":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_YEAR);
                                        break;
                                    case "Month":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_MONTH);
                                        break;
                                    case "Day":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_DAY);
                                        break;
                                    case "DayOfYear":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_DAY_OF_YEAR);
                                        break;
                                    case "DayOfWeek":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_DAY_OF_WEEK);
                                        break;
                                    case "Hour":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_HOUR);
                                        break;
                                    case "Minute":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_MINUTE);
                                        break;
                                    case "Second":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_SECOND);
                                        break;
                                    case "Millisecond":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_MILLISECOND);
                                        break;
                                    case "TimeOfDay":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_TIME_OF_DAY);
                                        break;
                                    case "UtcDateTime":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_UTC);
                                        break;
                                }
                            }

                            #endregion
                        }
                        else if (memberExpression.Expression?.NodeType == ExpressionType.Subtract)
                        {
                            #region TimeSpan

                            if (memberType == typeof(TimeSpan))
                            {
                                switch (memberName)
                                {
                                    case "Days":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_DAYS);
                                        break;
                                    case "Hours":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_HOURS);
                                        break;
                                    case "Minutes":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_MINUTES);
                                        break;
                                    case "Seconds":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_SECONDS);
                                        break;
                                    case "Milliseconds":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_MILLISECONDS);
                                        break;
                                    case "TotalDays":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_TOTAL_DAYS);
                                        break;
                                    case "TotalHours":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_TOTAL_HOURS);
                                        break;
                                    case "TotalMinutes":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_TOTAL_MINUTES);
                                        break;
                                    case "TotalSeconds":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_TOTAL_SECONDS);
                                        break;
                                    case "TotalMilliseconds":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TIME_SPAN_TOTAL_MILLISECONDS);
                                        break;
                                }
                            }

                            #endregion
                        }
                        break;

                    #endregion

                    #region Method

                    case ExpressionType.Call:
                        var methodCallExpression = formatExpression as MethodCallExpression;
                        var methodName = methodCallExpression.Method.Name;
                        var methodDeclaringType = methodCallExpression.Method.DeclaringType;
                        if (methodCallExpression.Object != null)
                        {
                            #region All ToString()

                            if (methodName == nameof(ToString) && (methodCallExpression.Arguments?.Count ?? 0) <= 0)
                            {
                                fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TO_STRING);
                            }

                            #endregion

                            #region String

                            else if (methodDeclaringType == typeof(string))
                            {
                                switch (methodName)
                                {
                                    case "Trim":
                                        if (methodCallExpression.Arguments.Count < 1)
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TRIM);
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TRIM, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        }
                                        break;
                                    case "TrimStart":
                                        if (methodCallExpression.Arguments.Count < 1)
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TRIM_START);
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TRIM_START, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        }
                                        break;
                                    case "TrimEnd":
                                        if (methodCallExpression.Arguments.Count < 1)
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TRIM_END);
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TRIM_END, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        }
                                        break;
                                    case "ToLower":
                                    case "ToLowerInvariant":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TO_LOWER);
                                        break;
                                    case "ToUpper":
                                    case "ToUpperInvariant":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TO_UPPER);
                                        break;
                                    case "Substring":
                                        if (methodCallExpression.Arguments.Count == 1)
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.SUB_STRING, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.SUB_STRING, new Tuple<dynamic, dynamic>(
                                                CompileExpressionValue(methodCallExpression.Arguments[0])
                                                , CompileExpressionValue(methodCallExpression.Arguments[1])));
                                        }
                                        break;
                                    case "Replace":
                                        if (methodCallExpression.Arguments.Count >= 3)
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_REPLACE, new Tuple<dynamic, dynamic, dynamic>(
                                                           CompileExpressionValue(methodCallExpression.Arguments[0])
                                                           , CompileExpressionValue(methodCallExpression.Arguments[1])
                                                           , CompileExpressionValue(methodCallExpression.Arguments[2])));
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_REPLACE, new Tuple<dynamic, dynamic>(
                                                           CompileExpressionValue(methodCallExpression.Arguments[0])
                                                           , CompileExpressionValue(methodCallExpression.Arguments[1])));
                                        }
                                        break;
                                    case "PadLeft":
                                        dynamic leftPadChar = ' ';
                                        if (methodCallExpression.Arguments.Count > 1)
                                        {
                                            leftPadChar = CompileExpressionValue(methodCallExpression.Arguments[1]);
                                        }
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_PAD_LEFT, new Tuple<dynamic, dynamic>(CompileExpressionValue(methodCallExpression.Arguments[0]), leftPadChar));
                                        break;
                                    case "PadRight":
                                        dynamic rightPadChar = ' ';
                                        if (methodCallExpression.Arguments.Count > 1)
                                        {
                                            rightPadChar = CompileExpressionValue(methodCallExpression.Arguments[1]);
                                        }
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_PAD_RIGHT, new Tuple<dynamic, dynamic>(CompileExpressionValue(methodCallExpression.Arguments[0]), rightPadChar));
                                        break;
                                    case "IndexOf":
                                        if (methodCallExpression.Arguments.Count == 3 && methodCallExpression.Arguments[2].Type == typeof(int))
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_INDEX_OF, new Tuple<dynamic, dynamic, dynamic>
                                                (CompileExpressionValue(methodCallExpression.Arguments[0])
                                                , CompileExpressionValue(methodCallExpression.Arguments[1])
                                                , CompileExpressionValue(methodCallExpression.Arguments[2])));
                                        }
                                        else if (methodCallExpression.Arguments.Count == 2 && methodCallExpression.Arguments[1].Type == typeof(int))
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_INDEX_OF, new Tuple<dynamic, dynamic>
                                                (CompileExpressionValue(methodCallExpression.Arguments[0])
                                                , CompileExpressionValue(methodCallExpression.Arguments[1])));
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_INDEX_OF, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        }
                                        break;
                                    case "IndexOfAny":
                                        if (methodCallExpression.Arguments.Count == 3 && methodCallExpression.Arguments[2].Type == typeof(int))
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_INDEX_OF_ANY, new Tuple<dynamic, dynamic, dynamic>
                                                (CompileExpressionValue(methodCallExpression.Arguments[0])
                                                , CompileExpressionValue(methodCallExpression.Arguments[1])
                                                , CompileExpressionValue(methodCallExpression.Arguments[2])));
                                        }
                                        else if (methodCallExpression.Arguments.Count == 2 && methodCallExpression.Arguments[1].Type == typeof(int))
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_INDEX_OF_ANY, new Tuple<dynamic, dynamic>
                                                (CompileExpressionValue(methodCallExpression.Arguments[0])
                                                , CompileExpressionValue(methodCallExpression.Arguments[1])));
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_INDEX_OF_ANY, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        }
                                        break;
                                    case "LastIndexOf":
                                        if (methodCallExpression.Arguments.Count == 3 && methodCallExpression.Arguments[2].Type == typeof(int))
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_LAST_INDEX_OF, new Tuple<dynamic, dynamic, dynamic>
                                                (CompileExpressionValue(methodCallExpression.Arguments[0])
                                                , CompileExpressionValue(methodCallExpression.Arguments[1])
                                                , CompileExpressionValue(methodCallExpression.Arguments[2])));
                                        }
                                        else if (methodCallExpression.Arguments.Count == 2 && methodCallExpression.Arguments[1].Type == typeof(int))
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_LAST_INDEX_OF, new Tuple<dynamic, dynamic>
                                                (CompileExpressionValue(methodCallExpression.Arguments[0])
                                                , CompileExpressionValue(methodCallExpression.Arguments[1])));
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_LAST_INDEX_OF, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        }
                                        break;
                                    case "LastIndexOfAny":
                                        if (methodCallExpression.Arguments.Count == 3 && methodCallExpression.Arguments[2].Type == typeof(int))
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_LAST_INDEX_OF_ANY, new Tuple<dynamic, dynamic, dynamic>
                                                (CompileExpressionValue(methodCallExpression.Arguments[0])
                                                , CompileExpressionValue(methodCallExpression.Arguments[1])
                                                , CompileExpressionValue(methodCallExpression.Arguments[2])));
                                        }
                                        else if (methodCallExpression.Arguments.Count == 2 && methodCallExpression.Arguments[1].Type == typeof(int))
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_LAST_INDEX_OF_ANY, new Tuple<dynamic, dynamic>
                                                (CompileExpressionValue(methodCallExpression.Arguments[0])
                                                , CompileExpressionValue(methodCallExpression.Arguments[1])));
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.STRING_LAST_INDEX_OF_ANY, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        }
                                        break;
                                }
                            }

                            #endregion

                            #region DateTime

                            else if (methodDeclaringType == typeof(DateTime) || methodDeclaringType == typeof(DateTimeOffset))
                            {
                                switch (methodName)
                                {
                                    case "AddDays":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_ADD_DAY, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        break;
                                    case "AddMonths":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_ADD_MONTH, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        break;
                                    case "AddYears":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_ADD_YEAR, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        break;
                                    case "AddHours":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_ADD_HOUR, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        break;
                                    case "AddMinutes":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_ADD_MINUTE, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        break;
                                    case "AddSeconds":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_ADD_SECOND, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        break;
                                    case "AddMilliseconds":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_ADD_MILLISECOND, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        break;
                                    case "ToUniversalTime":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_UTC);
                                        break;
                                    case "ToString":
                                        if (methodCallExpression.Arguments != null && methodCallExpression.Arguments[0].Type == typeof(string))
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_FORMAT_STRING, CompileExpressionValue(methodCallExpression.Arguments[0]));
                                        }
                                        else
                                        {
                                            fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TO_STRING);
                                        }
                                        break;
                                }
                            }

                            #endregion
                        }
                        else if (methodCallExpression.Object == null && methodCallExpression.Method.IsStatic)
                        {
                            #region SixnetFunc

                            if (methodDeclaringType == typeof(SixnetDbFunc))
                            {
                                switch (methodName)
                                {
                                    case "Max":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MAX);
                                        break;
                                    case "Min":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MIN);
                                        break;
                                    case "Avg":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.AVG);
                                        break;
                                    case "Count":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.COUNT);
                                        break;
                                    case "Sum":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.SUM);
                                        break;
                                    case "JsonValue":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.JSON_VALUE, GetDataField(methodCallExpression.Arguments[1], parameterIndexes));
                                        break;
                                    case "JsonObject":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.JSON_OBJECT, GetDataField(methodCallExpression.Arguments[1], parameterIndexes));
                                        break;
                                    case "Distinct":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DISTINCT);
                                        break;
                                    case "IsNull":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.IS_NULL);
                                        break;
                                    case "NotNull":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.NOT_NULL);
                                        break;
                                    case "ToString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TO_STRING, GetDataField(methodCallExpression.Arguments[1], parameterIndexes));
                                        break;
                                    case "ToDateTimeString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_STRING);
                                        break;
                                    case "ToDateTimeWithMillisecondString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_WITH_MILLISECOND_STRING);
                                        break;
                                    case "ToDateString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_STRING);
                                        break;
                                    case "ToUSDateString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.US_DATE_STRING);
                                        break;
                                    case "ToJapanDateString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.JAPAN_DATE_STRING);
                                        break;
                                }
                            }

                            #endregion

                            #region SixnetSixnetFieldExtensions

                            else if (methodDeclaringType == typeof(SixnetSixnetFieldExtensions))
                            {
                                switch (methodName)
                                {
                                    case "DbMax":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MAX);
                                        break;
                                    case "DbMin":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MIN);
                                        break;
                                    case "DbAvg":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.AVG);
                                        break;
                                    case "DbCount":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.COUNT);
                                        break;
                                    case "DbSum":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.SUM);
                                        break;
                                    case "DbJsonValue":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.JSON_VALUE, GetDataField(methodCallExpression.Arguments[1], parameterIndexes));
                                        break;
                                    case "DbJsonObject":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.JSON_OBJECT, GetDataField(methodCallExpression.Arguments[1], parameterIndexes));
                                        break;
                                    case "DbDistinct":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DISTINCT);
                                        break;
                                    case "DbIsNull":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.IS_NULL);
                                        break;
                                    case "DbNotNull":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.NOT_NULL);
                                        break;
                                    case "DbToString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TO_STRING, GetDataField(methodCallExpression.Arguments[1], parameterIndexes));
                                        break;
                                    case "DbToDateTimeString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_STRING);
                                        break;
                                    case "DbToDateTimeWithMillisecondString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_TIME_WITH_MILLISECOND_STRING);
                                        break;
                                    case "DbToDateString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DATE_STRING);
                                        break;
                                    case "DbToUSDateString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.US_DATE_STRING);
                                        break;
                                    case "DbToJapanDateString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.JAPAN_DATE_STRING);
                                        break;
                                }
                            }
                            #endregion

                            #region Convert

                            else if (methodDeclaringType == typeof(Convert))
                            {
                                switch (methodName)
                                {
                                    case "ToInt32":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_INT);
                                        break;
                                    case "ToBoolean":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_BOOLEAN);
                                        break;
                                    case "ToByte":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_BYTE);
                                        break;
                                    case "ToChar":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_CHAR);
                                        break;
                                    case "ToDateTime":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_DATE_TIME);
                                        break;
                                    case "ToDecimal":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_DECIMAL);
                                        break;
                                    case "ToDouble":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_DOUBLE);
                                        break;
                                    case "ToInt16":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_INT_16);
                                        break;
                                    case "ToInt64":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_INT_64);
                                        break;
                                    case "ToSByte":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_SBYTE);
                                        break;
                                    case "ToSingle":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_SINGLE);
                                        break;
                                    case "ToString":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.TO_STRING);
                                        break;
                                    case "ToUInt16":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_UINT_16);
                                        break;
                                    case "ToUInt32":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_UINT_32);
                                        break;
                                    case "ToUInt64":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.CONVERT_TO_UINT_64);
                                        break;
                                }
                            }

                            #endregion

                            #region Math

                            else if (methodDeclaringType == typeof(Math))
                            {
                                switch (methodName)
                                {
                                    case "Round":
                                        dynamic digits = 2;
                                        if (methodCallExpression.Arguments.Count >= 2)
                                        {
                                            digits = CompileExpressionValue(methodCallExpression.Arguments[1]);
                                        }
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_ROUND, digits);
                                        break;
                                    case "Abs":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_ABS);
                                        break;
                                    case "Ceiling":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_CEILING);
                                        break;
                                    case "Floor":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_FLOOR);
                                        break;
                                    case "Truncate":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_TRUNCATE);
                                        break;
                                    case "Sign":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_SIGN);
                                        break;
                                    case "Pow":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_POW, CompileExpressionValue(methodCallExpression.Arguments[1]));
                                        break;
                                    case "Sqrt":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_SQRT);
                                        break;
                                    case "Exp":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_EXP);
                                        break;
                                    case "Log":
                                        dynamic baseVal = Math.E;
                                        if (methodCallExpression.Arguments.Count > 1)
                                        {
                                            baseVal = CompileExpressionValue(methodCallExpression.Arguments[1]);
                                        }
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_LOG, baseVal);
                                        break;
                                    case "Log2":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_LOG, 2);
                                        break;
                                    case "Log10":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_LOG, 10);
                                        break;
                                    case "ILogB":
                                        var logformatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_LOG, 2);
                                        var floorFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_FLOOR);
                                        floorFormatSetting.SetChild(logformatSetting);
                                        fieldFormatSetting = floorFormatSetting;
                                        break;
                                    case "Cos":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_COS);
                                        break;
                                    case "Sin":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_SIN);
                                        break;
                                    case "Tan":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_TAN);
                                        break;
                                    case "Acos":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_ACOS);
                                        break;
                                    case "Asin":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_ASIN);
                                        break;
                                    case "Atan":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_ATAN);
                                        break;
                                    case "Atan2":
                                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MATH_ATAN2, CompileExpressionValue(methodCallExpression.Arguments[1]));
                                        break;
                                }
                            }

                            #endregion
                        }
                        break;
                    #endregion

                    #region Others

                    case ExpressionType.And:
                        var andExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.AND, GetDataField(andExpression.Right));
                        break;
                    case ExpressionType.Or:
                        var orExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.OR, GetDataField(orExpression.Right));
                        break;
                    case ExpressionType.ExclusiveOr:
                        var exclusiveOrExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.XOR, GetDataField(exclusiveOrExpression.Right));
                        break;
                    case ExpressionType.Not:
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.NOT);
                        break;
                    case ExpressionType.Add:
                        var addExpression = formatExpression as BinaryExpression;
                        var parameterField = GetDataField(addExpression.Right);
                        var fieldDataType = parameterField?.GetDataType();
                        fieldFormatSetting = SixnetFieldFormatSetting.Create((fieldDataType == _stringType || fieldDataType == _charType) ? SixnetFieldFormatterNames.STRING_CONCAT : SixnetFieldFormatterNames.ADD, parameterField);
                        break;
                    case ExpressionType.Subtract:
                        var subtractExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.SUBTRACT, GetDataField(subtractExpression.Right));
                        break;
                    case ExpressionType.Multiply:
                        var multiplyExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MULTIPLY, GetDataField(multiplyExpression.Right));
                        break;
                    case ExpressionType.Divide:
                        var divideExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.DIVIDE, GetDataField(divideExpression.Right));
                        break;
                    case ExpressionType.Modulo:
                        var moduloExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MODULO, GetDataField(moduloExpression.Right));
                        break;
                    case ExpressionType.RightShift:
                        var rightShiftExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.RIGHT_SHIFT, GetDataField(rightShiftExpression.Right));
                        break;
                    case ExpressionType.LeftShift:
                        var leftShiftExpression = formatExpression as BinaryExpression;
                        fieldFormatSetting = SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.LEFT_SHIFT, GetDataField(leftShiftExpression.Right));
                        break;

                        #endregion
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
        public static SixnetFieldsAssignment GetFieldsAssignment(Expression updateExpression)
        {
            SixnetDirectThrower.ThrowArgNullIf(updateExpression == null, nameof(updateExpression));
            SixnetDirectThrower.ThrowNotSupportIf(updateExpression.NodeType != ExpressionType.Lambda, updateExpression.NodeType.ToString());

            var fieldsAssignment = SixnetFieldsAssignment.Create();
            var lambdaExp = updateExpression as LambdaExpression;
            var parameterIndexes = GetLambdaParameterIndexes(lambdaExp);

            GenerateFieldsAssignmentCore(fieldsAssignment, parameterIndexes, lambdaExp.Body);
            return fieldsAssignment;
        }

        internal static SixnetFieldsAssignment AppendToFieldsAssignment(SixnetFieldsAssignment fieldsAssignment, Expression updateExpression)
        {
            SixnetDirectThrower.ThrowArgNullIf(updateExpression == null, nameof(updateExpression));
            SixnetDirectThrower.ThrowNotSupportIf(updateExpression.NodeType != ExpressionType.Lambda, updateExpression.NodeType.ToString());

            var lambdaExp = updateExpression as LambdaExpression;
            var parameterIndexes = GetLambdaParameterIndexes(lambdaExp);

            GenerateFieldsAssignmentCore(fieldsAssignment, parameterIndexes, lambdaExp.Body);
            return fieldsAssignment;
        }

        static void GenerateFieldsAssignmentCore(SixnetFieldsAssignment fieldsAssignment, Dictionary<string, int> parameterIndexes, Expression updateExpression)
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
