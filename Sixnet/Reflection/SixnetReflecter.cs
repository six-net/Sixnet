// "Company © 2025. All rights reserved."

using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;

using Sixnet.Exceptions;

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

        public static class String
        {
            /// <summary>
            /// String index method info
            /// </summary>
            public readonly static MethodInfo StringIndexOfMethod = null;

            /// <summary>
            /// String end with method info
            /// </summary>
            public readonly static MethodInfo EndWithMethod = null;

            static String()
            {
                StringIndexOfMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "IndexOf" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
                EndWithMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "EndsWith" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
            }
        }

        #endregion

        #region Expression

        public static class Expression
        {
            /// <summary>
            /// Lambda method info
            /// </summary>
            public static readonly MethodInfo LambdaMethod = null;

            static Expression()
            {
                var baseExpressMethods = typeof(System.Linq.Expressions.Expression).GetMethods(BindingFlags.Public | BindingFlags.Static);
                LambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
            }
        }

        #endregion
    }
}
