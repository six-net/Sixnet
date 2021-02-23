using EZNEW.Fault;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EZNEW.Reflection
{
    /// <summary>
    /// Reflection manager
    /// </summary>
    public static class ReflectionManager
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
            static readonly Dictionary<Guid, MethodInfo> CacheCommonCollectionTypeToListMethods = new Dictionary<Guid, MethodInfo>();

            /// <summary>
            /// Common collection type contains methods
            /// </summary>
            static readonly Dictionary<Guid, MethodInfo> CacheCommonCollectionTypeContainsMethods = new Dictionary<Guid, MethodInfo>();

            static Collections()
            {
                CollectionToListMethod = typeof(Enumerable).GetMethods().FirstOrDefault(c => c.Name == "ToList" && c.GetParameters().Length == 1);
                CollectionContainsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(c => c.Name == "Contains" && c.GetParameters().Length == 2);
                List<Type> commonTypes = new List<Type>()
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
                };
                commonTypes.ForEach(type =>
                {
                    CacheCommonCollectionTypeToListMethods[type.GUID] = CollectionToListMethod.MakeGenericMethod(type);
                    CacheCommonCollectionTypeContainsMethods[type.GUID] = CollectionContainsMethod.MakeGenericMethod(type);
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
                    Type valueType = null;
                    foreach (var val in originalCollection)
                    {
                        valueType = val.GetType();
                        break;
                    }
                    if (CacheCommonCollectionTypeToListMethods.TryGetValue(valueType.GUID, out var method))
                    {
                        return method.Invoke(null, new object[1] { originalCollection }) as IEnumerable;
                    }
                    else
                    {
                        var toListMethod = CollectionToListMethod.MakeGenericMethod(valueType);
                        return toListMethod.Invoke(null, new object[1] { originalCollection }) as IEnumerable;
                    }
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
                    throw new EZNEWException("Value type can't be null");
                }
                if (CacheCommonCollectionTypeContainsMethods.TryGetValue(valueType.GUID, out var method))
                {
                    return method;
                }
                return CollectionContainsMethod.MakeGenericMethod(valueType);
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
