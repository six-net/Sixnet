// "Company © 2025. All rights reserved."

using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;

namespace Sixnet.Reflection
{
    /// <summary>
    /// Sixnet reflecter
    /// </summary>
    public static class SixnetReflecter
    {
        #region Collections

        public static class Collections
        {
            /// <summary>
            /// Collection to list method info
            /// </summary>
            static readonly MethodInfo CollectionToListMethod = null;

            /// <summary>
            /// Collection contains method info
            /// </summary>
            static readonly MethodInfo CollectionContainsMethod = null;

            /// <summary>
            /// Common collection type to list methods
            /// </summary>
            static readonly ConcurrentDictionary<string, MethodInfo> CacheCommonCollectionTypeToListMethods = new();

            /// <summary>
            /// Common collection type contains methods
            /// </summary>
            static readonly ConcurrentDictionary<string, MethodInfo> CacheCommonCollectionTypeContainsMethods = new();

            static Collections()
            {
                CollectionToListMethod = typeof(Enumerable).GetMethods().FirstOrDefault(c => c.Name == "ToList" && c.GetParameters().Length == 1);
                CollectionContainsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(c => c.Name == "Contains" && c.GetParameters().Length == 2);
                List<Type> commonTypes = new()
                {
                    typeof(bool)
                    ,typeof(byte)
                    ,typeof(decimal)
                    ,typeof(double)
                    ,typeof(float)
                    ,typeof(int)
                    ,typeof(long)
                    ,typeof(sbyte)
                    ,typeof(short)
                    ,typeof(uint)
                    ,typeof(ulong)
                    ,typeof(ushort)
                    ,typeof(DateTime)
                    ,typeof(DateTimeOffset)
                    ,typeof(string)
                    ,typeof(Guid)
                    ,typeof(object)
                    ,typeof(TimeSpan)
                    ,typeof(byte?)
                    ,typeof(decimal?)
                    ,typeof(double?)
                    ,typeof(float?)
                    ,typeof(int?)
                    ,typeof(long?)
                    ,typeof(sbyte?)
                    ,typeof(short?)
                    ,typeof(uint?)
                    ,typeof(ulong?)
                    ,typeof(ushort?)
                    ,typeof(DateTime?)
                    ,typeof(DateTimeOffset?)
                    ,typeof(Guid?)
                    ,typeof(TimeSpan?)
                };
                commonTypes.ForEach(type =>
                {
                    CacheCommonCollectionTypeToListMethods[type.GetTypeIdentityKey()] = CollectionToListMethod.MakeGenericMethod(type);
                    CacheCommonCollectionTypeContainsMethods[type.GetTypeIdentityKey()] = CollectionContainsMethod.MakeGenericMethod(type);
                });
            }

            /// <summary>
            /// Resolve and return a collection
            /// </summary>
            /// <param name="originalCollection">The original collection</param>
            /// <returns>Return the resolved collection result</returns>
            public static IEnumerable ResolveCollection(IEnumerable originalCollection)
            {
                if (originalCollection == null)
                {
                    return null;
                }
                var collectionType = originalCollection.GetType();
                if (!collectionType.IsSerializable && collectionType.IsGenericType)
                {
                    var valueType = collectionType.GenericTypeArguments.Last();
                    var valueTypeIdentity = valueType.GetTypeIdentityKey();
                    if (!CacheCommonCollectionTypeToListMethods.TryGetValue(valueTypeIdentity, out var method))
                    {
                        method = CollectionToListMethod.MakeGenericMethod(valueType);
                        CacheCommonCollectionTypeToListMethods.TryAdd(valueTypeIdentity, method);
                    }
                    return method.Invoke(null, new object[1] { originalCollection }) as IEnumerable;
                }
                return originalCollection;
            }

            /// <summary>
            /// Get collection contains method
            /// </summary>
            /// <param name="valueType">Value type</param>
            /// <returns>Return the method</returns>
            public static MethodInfo GetCollectionContainsMethod(Type valueType)
            {
                if (valueType == null)
                {
                    throw new SixnetException("Value type can't be null");
                }
                var typeIdentityKey = valueType.GetTypeIdentityKey();
                if (!CacheCommonCollectionTypeContainsMethods.TryGetValue(typeIdentityKey, out var methodInfo))
                {
                    methodInfo = CollectionContainsMethod.MakeGenericMethod(valueType);
                    CacheCommonCollectionTypeContainsMethods.TryAdd(typeIdentityKey, methodInfo);
                }
                return methodInfo;
            }

            /// <summary>
            /// Check value type whether is a collection 
            /// </summary>
            /// <param name="valueType">Value type</param>
            /// <returns></returns>
            public static bool IsCollectionType(Type valueType)
            {
                return valueType != null && (valueType.IsArray || typeof(IEnumerable).IsAssignableFrom(valueType));
            }
        }

        #endregion

        #region String

        public static class Strings
        {
            /// <summary>
            /// String index method info
            /// </summary>
            public readonly static MethodInfo StringIndexOfMethod = null;

            /// <summary>
            /// String end with method info
            /// </summary>
            public readonly static MethodInfo EndWithMethod = null;

            static Strings()
            {
                StringIndexOfMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "IndexOf" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
                EndWithMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "EndsWith" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
            }
        }

        #endregion

        #region Expression

        public static class Expressions
        {
            /// <summary>
            /// Lambda method info
            /// </summary>
            public static readonly MethodInfo LambdaMethod = null;

            static Expressions()
            {
                var baseExpressMethods = typeof(System.Linq.Expressions.Expression).GetMethods(BindingFlags.Public | BindingFlags.Static);
                LambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
            }

            #region Generate getting value lambda expression

            /// <summary>
            /// Generate getting value lambda expression
            /// </summary>
            /// <param name="objectType"></param>
            /// <param name="member"></param>
            /// <returns></returns>
            public static LambdaExpression GenerateGettingValueLambdaExpression(Type objectType, MemberInfo member)
            {
                SixnetThrower.ThrowArgNullIf(objectType == null || member == null);

                var instanceExpression = Expression.Parameter(objectType);
                var parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
                parameterArray.SetValue(instanceExpression, 0);
                var propertyExpression = Expression.PropertyOrField(instanceExpression, member.Name);
                var funcType = typeof(Func<,>).MakeGenericType(objectType, typeof(object));
                var genericLambdaMethod = LambdaMethod.MakeGenericMethod(funcType);
                var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                {
                    Expression.Convert(propertyExpression,typeof(object)),parameterArray
                });
                return lambdaExpression as LambdaExpression;
            }

            /// <summary>
            /// Generate getting value lambda expression
            /// </summary>
            /// <param name="objectType"></param>
            /// <param name="member"></param>
            /// <returns></returns>
            public static LambdaExpression GenerateGettingListValueLambdaExpression(Type objectType, MemberInfo member)
            {
                SixnetThrower.ThrowArgNullIf(objectType == null || member == null);

                var instanceExpression = Expression.Parameter(objectType);
                var parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
                parameterArray.SetValue(instanceExpression, 0);
                var propertyExpression = Expression.PropertyOrField(instanceExpression, member.Name);
                var funcType = typeof(Func<,>).MakeGenericType(objectType, typeof(List<object>));
                var genericLambdaMethod = LambdaMethod.MakeGenericMethod(funcType);
                var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                {
                    Expression.Convert(propertyExpression,typeof(object)),parameterArray
                });
                return lambdaExpression as LambdaExpression;
            }

            #endregion

            #region Generate setting value expression

            /// <summary>
            /// Generate setting value expression
            /// </summary>
            /// <param name="objectType"></param>
            /// <param name="member"></param>
            /// <returns></returns>
            public static LambdaExpression GenerateSettingValueLambdaExpression(Type objectType, MemberInfo member)
            {
                SixnetThrower.ThrowArgNullIf(objectType == null || member == null);

                var instanceExpression = Expression.Parameter(objectType);
                var valueType = typeof(object);
                var genericLambdaMethod = LambdaMethod.MakeGenericMethod(typeof(Action<,>).MakeGenericType(objectType, valueType));
                var valueParameter = Expression.Parameter(valueType, "value");
                var parameterExpressionArray = new ParameterExpression[2] { instanceExpression, valueParameter };
                LambdaExpression lambdaExpression = null;
                if (member is PropertyInfo propertyInfo)
                {
                    var readValueParameter = SixnetExpressionHelper.EnsureCastExpression(valueParameter, propertyInfo.PropertyType);
                    var setMethod = propertyInfo.GetSetMethod(true) ?? throw new ArgumentException("Property does not have a setter.");
                    var readInstanceParameter = SixnetExpressionHelper.EnsureCastExpression(instanceExpression, member.DeclaringType);
                    var setExpression = Expression.Call(readInstanceParameter, setMethod, readValueParameter);

                    lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                    {
                        setExpression,parameterExpressionArray
                    }) as LambdaExpression;
                }
                else if (member is FieldInfo fieldInfo)
                {
                    var sourceExpression = SixnetExpressionHelper.EnsureCastExpression(instanceExpression, fieldInfo.DeclaringType);
                    var fieldExpression = Expression.Field(sourceExpression, fieldInfo);

                    var valueExpression = SixnetExpressionHelper.EnsureCastExpression(valueParameter, fieldExpression.Type);
                    var assignExpression = Expression.Assign(fieldExpression, valueExpression);

                    lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                    {
                        assignExpression,parameterExpressionArray
                    }) as LambdaExpression;
                }

                return lambdaExpression;
            }

            #endregion
        }

        #endregion

        #region Get property or field

        /// <summary>
        /// Get property or field
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MemberInfo GetPropertyOrField(Type dataType, string name)
        {
            if (dataType == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            MemberInfo member = null;
            member = dataType.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (member == null)
            {
                member = dataType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            return member;
        }

        #endregion

        #region Invoke method

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="getMethodFunc"></param>
        /// <param name="parameters"></param>
        public static void InvokeMethod(object instance, Func<Type, MethodInfo> getMethodFunc, params object[] parameters)
        {
            SixnetThrower.ThrowArgNullIf(instance == null || getMethodFunc == null);
            var objectType = instance.GetType();
            var methodInfo = getMethodFunc(objectType);
            SixnetThrower.ThrowSixnetExceptionIf(methodInfo == null, "Not found the method");
            methodInfo.Invoke(instance, parameters);
        }

        /// <summary>
        /// Invoke method
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        public static void InvokeMethod(object instance, string methodName, params object[] parameters)
        {
            InvokeMethod(instance, t => t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), parameters);
        }

        /// <summary>
        /// Set value
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="propertyOrFieldName"></param>
        /// <param name="value"></param>
        public static void SetValue(object instance, string propertyOrFieldName, object value)
        {
            SixnetThrower.ThrowArgNullIf(instance == null || string.IsNullOrWhiteSpace(propertyOrFieldName));
            var objectType = instance.GetType();

            var property = objectType.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            property?.SetValue(instance, value);

            if (property == null)
            {
                var field = objectType.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                field?.SetValue(instance, value);
            }
        }

        #endregion
    }
}
