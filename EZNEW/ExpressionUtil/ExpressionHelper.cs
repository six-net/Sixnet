using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EZNEW.ExpressionUtil
{
    public static class ExpressionHelper
    {
        /// <summary>
        /// //Properties or fields access functions
        /// </summary>
        static readonly ConcurrentDictionary<string, dynamic> PropertyOrFieldAccessFunctions = new ConcurrentDictionary<string, dynamic>();

        /// <summary>
        /// Expression base type
        /// </summary>
        static readonly Type BaseExpressType = typeof(System.Linq.Expressions.Expression);

        /// <summary>
        /// Generate lambda method
        /// </summary>
        static readonly MethodInfo LambdaMethod = null;

        static ExpressionHelper()
        {
            var baseExpressMethods = BaseExpressType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            LambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
        }

        #region Get expression text

        /// <summary>
        /// Get expression text
        /// </summary>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        public static string GetExpressionText(string expression)
        {
            return string.Equals(expression, "model", StringComparison.OrdinalIgnoreCase)
                    ? string.Empty // If it's exactly "model", then give them an empty string, to replicate the lambda behavior
                    : expression;
        }

        #endregion

        #region Get expression text

        /// <summary>
        /// Get expression text
        /// </summary>
        /// <param name="expression">expression</param>
        /// <returns>expression text</returns>
        public static string GetExpressionText(LambdaExpression expression)
        {
            // Split apart the expression string for property/field accessors to create its name
            Stack<string> nameParts = new Stack<string>();
            System.Linq.Expressions.Expression part = expression.Body;
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
                    nameParts.Push(String.Empty);
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
            if (nameParts.Count > 0 && String.Equals(nameParts.Peek(), ".model", StringComparison.OrdinalIgnoreCase))
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
        private static string GetIndexerInvocation(System.Linq.Expressions.Expression expression, ParameterExpression[] parameters)
        {
            System.Linq.Expressions.Expression converted = System.Linq.Expressions.Expression.Convert(expression, typeof(object));
            ParameterExpression fakeParameter = System.Linq.Expressions.Expression.Parameter(typeof(object), null);
            Expression<Func<object, object>> lambda = System.Linq.Expressions.Expression.Lambda<Func<object, object>>(converted, fakeParameter);
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

        #region Whether is a tsingle argument indexer

        /// <summary>
        /// Whether is a tsingle argument indexer
        /// </summary>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        internal static bool IsSingleArgumentIndexer(System.Linq.Expressions.Expression expression)
        {
            MethodCallExpression methodExpression = expression as MethodCallExpression;
            if (methodExpression == null || methodExpression.Arguments.Count != 1)
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

        #region get expression value

        /// <summary>
        /// Get expression value
        /// </summary>
        /// <param name="valueExpression">expression</param>
        /// <returns>value</returns>
        public static object GetExpressionValue(System.Linq.Expressions.Expression valueExpression)
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
                    value = System.Linq.Expressions.Expression.Lambda(valueExpression).Compile().DynamicInvoke();
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
        public static bool IsBoolNodeType(ExpressionType nodeType)
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

        #region Get property name by expression

        /// <summary>
        /// Get property name by expression
        /// </summary>
        public static string GetExpressionPropertyName(System.Linq.Expressions.Expression propertyExpression)
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
        public static string GetExpressionPropertyName<T>(Expression<Func<T, dynamic>> propertyOrField)
        {
            return GetExpressionPropertyName(propertyOrField.Body);
        }

        /// <summary>
        /// Get property or field name list by expression
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="propertyOrFields">property or field expression</param>
        /// <returns>name list</returns>
        public static List<string> GetExpressionPropertyNames<T>(params Expression<Func<T, dynamic>>[] propertyOrFields)
        {
            if (propertyOrFields == null || propertyOrFields.Length <= 0)
            {
                return new List<string>(0);
            }
            return propertyOrFields.Select(c => GetExpressionPropertyName(c)).ToList();
        }

        #endregion

        #region Get the last child expression

        /// <summary>
        /// Get the last child expression
        /// </summary>
        /// <param name="expression">parent expression</param>
        /// <returns>last child expression expression</returns>
        public static System.Linq.Expressions.Expression GetLastChildExpression(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                return expression;
            }
            System.Linq.Expressions.Expression childExpression = expression;
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

        #region Get the proerty or field access method with the specified type

        /// <summary>
        /// Get the proerty or field access method with the specified type
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="name">field name</param>
        /// <returns>access method</returns>
        public static Func<T, dynamic> GetPropertyOrFieldFunction<T>(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            Type type = typeof(T);
            string funcKey = string.Format("{0}_{1}", type.GUID, name);
            if (PropertyOrFieldAccessFunctions.TryGetValue(funcKey, out dynamic funcValue))
            {
                return funcValue;
            }
            ParameterExpression parExp = System.Linq.Expressions.Expression.Parameter(type);//parameter expression
            Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
            parameterArray.SetValue(parExp, 0);
            string[] propertyNameArray = name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            System.Linq.Expressions.Expression propertyExpress = null;
            foreach (string pname in propertyNameArray)
            {
                if (propertyExpress == null)
                {
                    propertyExpress = System.Linq.Expressions.Expression.PropertyOrField(parExp, pname);
                }
                else
                {
                    propertyExpress = System.Linq.Expressions.Expression.PropertyOrField(propertyExpress, pname);
                }
            }
            Type funcType = typeof(Func<,>).MakeGenericType(type, typeof(object));// make method
            var genericLambdaMethod = LambdaMethod.MakeGenericMethod(funcType);
            var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
            {
                System.Linq.Expressions.Expression.Convert(propertyExpress,typeof(object)), parameterArray
            }) as Expression<Func<T, object>>;
            if (lambdaExpression == null)
            {
                return null;
            }
            var function = lambdaExpression.Compile();
            PropertyOrFieldAccessFunctions[funcKey] = function;
            return function;
        }

        #endregion

        #region Ensure cast expression

        public static Expression EnsureCastExpression(Expression expression, Type targetType, bool allowWidening = false)
        {
            Type expressionType = expression.Type;

            // check if a cast or conversion is required
            if (expressionType == targetType || (!expressionType.IsValueType && targetType.IsAssignableFrom(expressionType)))
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
    }
}
