using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EZNEW.Develop.Entity;
using EZNEW.ExpressionUtil;
using EZNEW.Reflection;

namespace EZNEW.Configuration
{
    /// <summary>
    /// Configuration manager
    /// </summary>
    public partial class ConfigurationManager
    {
        internal static class Entity
        {
            static Entity()
            {
                Init();
            }

            /// <summary>
            /// Value:Entity configuration
            /// Key:Entity type guid
            /// </summary>
            internal static readonly Dictionary<Guid, EntityConfiguration> EntityConfigurations = new Dictionary<Guid, EntityConfiguration>();

            #region Entity

            #region Configure entity

            /// <summary>
            /// Configure entity
            /// </summary>
            /// <param name="entityType">Entity type</param>
            internal static void ConfigureEntity(Type entityType)
            {
                if (entityType == null)
                {
                    return;
                }
                var typeGuid = entityType.GUID;
                if (EntityConfigurations.ContainsKey(typeGuid) || !((entityType.GetCustomAttributes(typeof(EntityAttribute), false)?.FirstOrDefault()) is EntityAttribute entityAttribute))
                {
                    return;
                }
                IEnumerable<MemberInfo> memberInfos = new List<MemberInfo>(0);
                memberInfos = memberInfos.Union(entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                memberInfos = memberInfos.Union(entityType.GetFields(BindingFlags.Public | BindingFlags.Instance));
                string objectName = string.IsNullOrWhiteSpace(entityAttribute.ObjectName) ? entityType.Name : entityAttribute.ObjectName;
                if (!EntityConfigurations.TryGetValue(typeGuid, out EntityConfiguration entityConfig))
                {
                    entityConfig = new EntityConfiguration();
                }
                entityConfig.Structure = entityAttribute.Structure;
                //table name
                if (string.IsNullOrWhiteSpace(entityConfig.TableName))
                {
                    entityConfig.TableName = objectName;
                }
                //fields
                List<EntityField> allFields = new List<EntityField>();
                List<string> primaryKeys = new List<string>();

                //cache keys
                List<string> cacheKeys = new List<string>();
                List<string> cachePrefixKeys = new List<string>();
                List<string> cacheIgnoreKeys = new List<string>();

                List<string> mustQueryFields = new List<string>();
                List<EntityField> editFields = new List<EntityField>();
                List<EntityField> queryEntityFields = new List<EntityField>();
                string versionField = null;
                string refreshDateField = null;
                foreach (var member in memberInfos)
                {
                    var fieldName = member.Name;
                    var fieldRole = FieldRole.None;
                    var propertyName = fieldName;
                    EntityFieldAttribute entityFieldAttribute = (member.GetCustomAttributes(typeof(EntityFieldAttribute), false)?.FirstOrDefault()) as EntityFieldAttribute;
                    fieldRole = entityFieldAttribute?.Role ?? FieldRole.None;
                    fieldName = entityFieldAttribute?.Name ?? fieldName;
                    Type memberType = null;
                    if (member is PropertyInfo propertyInfo)
                    {
                        memberType = propertyInfo.PropertyType;
                    }
                    if (member is FieldInfo fieldInfo)
                    {
                        memberType = fieldInfo.FieldType;
                    }
                    var propertyField = new EntityField()
                    {
                        FieldName = fieldName,
                        PropertyName = propertyName,
                        QueryFormat = entityFieldAttribute?.QueryFormat ?? string.Empty,
                        CacheRole = entityFieldAttribute?.CacheRole ?? FieldCacheRole.None,
                        IsDisableEdit = entityFieldAttribute?.DisableEdit ?? false,
                        IsDisableQuery = entityFieldAttribute?.DisableQuery ?? false,
                        IsPrimaryKey = (fieldRole & FieldRole.PrimaryKey) != 0,
                        IsRefreshDate = (fieldRole & FieldRole.RefreshDate) != 0,
                        IsVersion = (fieldRole & FieldRole.Version) != 0,
                        IsLevel = (fieldRole & FieldRole.Level) != 0,
                        IsSort = (fieldRole & FieldRole.Sort) != 0,
                        IsParent = (fieldRole & FieldRole.Parent) != 0,
                        IsDisplayName = (fieldRole & FieldRole.Display) != 0,
                        DataType = memberType,
                        DbTypeName = entityFieldAttribute?.DbTypeName,
                        MaxLength = entityFieldAttribute?.MaxLength ?? 0,
                        IsFixedLength = entityFieldAttribute?.IsFixedLength ?? false,
                        IsRequired = entityFieldAttribute?.IsRequired ?? false,
                        Comment = entityFieldAttribute?.Description ?? string.Empty
                    };

                    //value provider
                    var valueProvider = GetValueProvider(entityType, member);
                    if (valueProvider != null)
                    {
                        propertyField.ValueProvider = valueProvider;
                    }

                    allFields.Add(propertyField);

                    if (propertyField.IsPrimaryKey)
                    {
                        primaryKeys.Add(propertyName);
                    }
                    else
                    {
                        if ((propertyField.CacheRole & FieldCacheRole.CacheKey) != 0)
                        {
                            cacheKeys.Add(propertyName);
                        }
                        if ((propertyField.CacheRole & FieldCacheRole.CacheKeyPrefix) != 0)
                        {
                            cachePrefixKeys.Add(propertyName);
                        }
                        if ((propertyField.CacheRole & FieldCacheRole.Ignore) != 0)
                        {
                            cacheIgnoreKeys.Add(propertyName);
                        }
                    }
                    if (propertyField.IsVersion)
                    {
                        versionField = propertyName;
                    }
                    if (propertyField.IsRefreshDate)
                    {
                        refreshDateField = propertyName;
                    }

                    //relation config
                    var relationAttributes = member.GetCustomAttributes(typeof(EntityRelationAttribute), false);
                    if (relationAttributes.IsNullOrEmpty())
                    {
                        continue;
                    }
                    if (entityConfig.RelationFields.IsNullOrEmpty())
                    {
                        entityConfig.RelationFields = new Dictionary<Guid, Dictionary<string, string>>();
                    }
                    foreach (var attrObj in relationAttributes)
                    {
                        if (!(attrObj is EntityRelationAttribute relationAttr) || relationAttr.RelationType == null || string.IsNullOrWhiteSpace(relationAttr.RelationField))
                        {
                            continue;
                        }
                        var relationTypeId = relationAttr.RelationType.GUID;
                        entityConfig.RelationFields.TryGetValue(relationTypeId, out var values);
                        values ??= new Dictionary<string, string>();
                        if (values.ContainsKey(propertyName))
                        {
                            continue;
                        }
                        values.Add(propertyName, relationAttr.RelationField);
                        entityConfig.RelationFields[relationTypeId] = values;
                    }
                }
                allFields = allFields.OrderByDescending(c => c.IsPrimaryKey).ThenByDescending(c => cacheKeys.Contains(c.PropertyName)).ToList();
                var allFieldDict = new Dictionary<string, EntityField>(allFields.Count);
                foreach (var field in allFields)
                {
                    allFieldDict[field.PropertyName] = field;
                    var mustQueryField = field.IsPrimaryKey || field.IsVersion;
                    if (mustQueryField)
                    {
                        mustQueryFields.Add(field.PropertyName);
                    }
                    if (mustQueryField || !field.IsDisableQuery)
                    {
                        queryEntityFields.Add(field);
                    }
                    if (!field.IsDisableEdit)
                    {
                        editFields.Add(field);
                    }
                }
                entityConfig.Comment = entityAttribute.Description ?? string.Empty;
                entityConfig.PrimaryKeys = primaryKeys;
                entityConfig.AllFields = allFieldDict;
                entityConfig.VersionField = versionField;
                entityConfig.RefreshDateField = refreshDateField;
                entityConfig.CacheKeys = cacheKeys;
                entityConfig.CachePrefixKeys = cachePrefixKeys;
                entityConfig.CacheIgnoreKeys = cacheIgnoreKeys;
                entityConfig.QueryFields = queryEntityFields.Select(c => c.PropertyName).ToList();
                entityConfig.QueryEntityFields = queryEntityFields;
                entityConfig.EditFields = editFields;
                entityConfig.MustQueryFields = mustQueryFields;
                entityConfig.PredicateType = typeof(Func<,>).MakeGenericType(entityType, booleanType);
                entityConfig.EntityType = entityType;
                EntityConfigurations[typeGuid] = entityConfig;
            }

            #endregion

            #region Get entity configuration

            /// <summary>
            /// Get entity configuration
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <returns>Return entity configuration</returns>
            internal static EntityConfiguration GetEntityConfiguration(Type entityType)
            {
                if (entityType == null)
                {
                    return null;
                }
                var typeGuid = entityType.GUID;
                return GetEntityConfiguration(typeGuid);
            }

            /// <summary>
            /// Get entity configuration
            /// </summary>
            /// <param name="entityTypeId">Entity type id</param>
            /// <returns>Return entity configuration</returns>
            internal static EntityConfiguration GetEntityConfiguration(Guid entityTypeId)
            {
                EntityConfigurations.TryGetValue(entityTypeId, out var entityConfig);
                return entityConfig;
            }

            #endregion

            #region Get all entity configurations

            /// <summary>
            /// Get all entity configurations
            /// </summary>
            /// <returns>Return all entity configurations</returns>
            internal static IEnumerable<EntityConfiguration> GetAllEntityConfigurations()
            {
                return EntityConfigurations.Values;
            }

            #endregion

            #endregion

            #region Entity object name

            #region Configure object name

            /// <summary>
            /// Configure entity object name
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <param name="objectName">Entity object name</param>
            internal static void ConfigureObjectName(Type entityType, string objectName)
            {
                if (entityType == null)
                {
                    return;
                }
                var entityConfiguration = GetEntityConfiguration(entityType);
                if (entityConfiguration != null)
                {
                    entityConfiguration.TableName = objectName;
                }
            }

            #endregion

            #region Get object name

            /// <summary>
            /// Get entity object name
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <returns>Return entity object name</returns>
            internal static string GetEntityObjectName(Type entityType)
            {
                if (entityType == null)
                {
                    return string.Empty;
                }
                var entityConfig = GetEntityConfiguration(entityType);
                return entityConfig?.TableName ?? string.Empty;
            }

            #endregion

            #endregion

            #region Entity fields

            #region Get fields

            /// <summary>
            /// Get fields
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <returns>Return entity fields</returns>
            internal static IEnumerable<string> GetAllFields(Type entityType)
            {
                var entityConfig = GetEntityConfiguration(entityType);
                if (entityConfig?.AllFields == null)
                {
                    return Array.Empty<string>();
                }
                return entityConfig.AllFields.Keys;
            }

            #endregion

            #region Get query fields

            /// <summary>
            /// Get entity query fields
            /// </summary>
            /// <param name="entityTypeAssemblyQualifiedName">Entity type full name</param>
            /// <returns>Return entity query fields</returns>
            internal static IEnumerable<string> GetQueryFields(string entityTypeAssemblyQualifiedName)
            {
                if (string.IsNullOrWhiteSpace(entityTypeAssemblyQualifiedName))
                {
                    return Array.Empty<string>();
                }
                var type = Type.GetType(entityTypeAssemblyQualifiedName);
                return GetQueryFields(type);
            }

            /// <summary>
            /// Get entity query fields
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <returns>Return entity query fields</returns>
            internal static IEnumerable<string> GetQueryFields(Type entityType)
            {
                var entityConfig = GetEntityConfiguration(entityType);
                if (entityConfig?.QueryFields.IsNullOrEmpty() ?? true)
                {
                    return Array.Empty<string>();
                }
                return entityConfig.QueryFields;
            }

            /// <summary>
            /// Get entity query fields
            /// </summary>
            /// <typeparam name="TEntity">Entity type</typeparam>
            /// <returns>Return entity query fields</returns>
            internal static IEnumerable<string> GetQueryFields<TEntity>()
            {
                return GetQueryFields(typeof(TEntity));
            }

            #endregion

            #region Get must query fields

            /// <summary>
            /// Get must query fields
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <returns>Return must query fields</returns>
            internal static IEnumerable<string> GetMustQueryFields(Type entityType)
            {
                var entityConfig = GetEntityConfiguration(entityType);
                if (entityConfig?.MustQueryFields.IsNullOrEmpty() ?? true)
                {
                    return Array.Empty<string>();
                }
                return entityConfig.MustQueryFields;
            }

            #endregion

            #region Get entity field

            /// <summary>
            /// Get entity field
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <param name="propertyName">Property name</param>
            /// <returns>Return the entity field</returns>
            internal static EntityField GetEntityField(Type entityType, string propertyName)
            {
                if (entityType == null || string.IsNullOrWhiteSpace(propertyName))
                {
                    return null;
                }
                EntityField field = null;
                GetEntityConfiguration(entityType)?.AllFields.TryGetValue(propertyName, out field);
                return field;
            }

            #endregion

            #endregion

            #region Primary key

            #region Add primary keys 

            /// <summary>
            /// Add primary keys
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <param name="keyNames">Primary key names</param>
            internal static void AddPrimaryKey(Type entityType, params string[] keyNames)
            {
                if (entityType == null || keyNames.IsNullOrEmpty())
                {
                    return;
                }
                var entityConfig = GetEntityConfiguration(entityType);
                if (entityConfig == null)
                {
                    return;
                }
                if (entityConfig.PrimaryKeys == null)
                {
                    entityConfig.PrimaryKeys = keyNames.ToList();
                }
                else
                {
                    entityConfig.PrimaryKeys = entityConfig.PrimaryKeys.Union(keyNames).ToList();
                }
            }

            /// <summary>
            /// Add primary key
            /// </summary>
            /// <param name="typeAssemblyQualifiedName">Entity type full name</param>
            /// <param name="keyNames">Primary key names</param>
            internal static void AddPrimaryKey(string typeAssemblyQualifiedName, params string[] keyNames)
            {
                if (string.IsNullOrWhiteSpace(typeAssemblyQualifiedName) || keyNames == null || keyNames.Length <= 0)
                {
                    return;
                }
                var type = Type.GetType(typeAssemblyQualifiedName);
                AddPrimaryKey(type);
            }

            /// <summary>
            /// Add primary key
            /// </summary>
            /// <typeparam name="TEntity">Entity type</typeparam>
            /// <param name="fields">Primary key fields</param>
            internal static void AddPrimaryKey<TEntity>(params Expression<Func<TEntity, dynamic>>[] fields)
            {
                if (fields == null)
                {
                    return;
                }
                AddPrimaryKey(typeof(TEntity), fields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body)).ToArray());
            }

            #endregion

            #region Get primary keys

            /// <summary>
            /// Get primary keys
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <returns>Return all primary key field</returns>
            internal static IEnumerable<string> GetPrimaryKeys(Type entityType)
            {
                var entityConfig = GetEntityConfiguration(entityType);
                if (entityConfig?.PrimaryKeys.IsNullOrEmpty() ?? true)
                {
                    return Array.Empty<string>();
                }
                return entityConfig.PrimaryKeys;
            }

            /// <summary>
            /// Determines whether the property is primary key
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <param name="propertyName">Property name</param>
            /// <returns>Return whether is primary key</returns>
            internal static bool IsPrimaryKey(Type entityType, string propertyName)
            {
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    return false;
                }
                var primaryKeys = GetPrimaryKeys(entityType);
                return primaryKeys?.Any(c => c == propertyName) ?? false;
            }

            #endregion

            #endregion

            #region Version field

            #region Set version field

            /// <summary>
            /// Set version field
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <param name="fieldName">Version field name</param>
            internal static void SetVersionField(Type entityType, string fieldName)
            {
                var entityConfig = GetEntityConfiguration(entityType);
                if (entityConfig != null)
                {
                    entityConfig.VersionField = fieldName;
                }
            }

            #endregion

            #region Get version field

            /// <summary>
            /// Get version field
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <returns>Return the version field name</returns>
            internal static string GetVersionField(Type entityType)
            {
                var entityConfig = GetEntityConfiguration(entityType);
                return entityConfig?.VersionField ?? string.Empty;
            }

            #endregion

            #endregion

            #region Refresh date field

            #region Set refresh date field

            /// <summary>
            /// Set refresh date field
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <param name="fieldName">Refresh date field name</param>
            internal static void SetRefreshDateField(Type entityType, string fieldName)
            {
                var entityConfig = GetEntityConfiguration(entityType);
                if (entityConfig != null)
                {
                    entityConfig.RefreshDateField = fieldName;
                }
            }

            #endregion

            #region Get refreshdate field

            /// <summary>
            /// Get refresh date field
            /// </summary>
            /// <param name="entityType">Entity type</param>
            /// <returns>Return the refresh date field name</returns>
            internal static string GetRefreshDateField(Type entityType)
            {
                var entityConfig = GetEntityConfiguration(entityType);
                return entityConfig?.RefreshDateField ?? string.Empty;
            }

            #endregion

            #endregion

            #region Relation fields

            /// <summary>
            /// Get relation fields
            /// </summary>
            /// <param name="sourceEntityType">Source entity type</param>
            /// <param name="relationEntityType">Relation entity type</param>
            /// <returns>Return all relation fields</returns>
            internal static Dictionary<string, string> GetRelationFields(Type sourceEntityType, Type relationEntityType)
            {
                if (sourceEntityType == null || relationEntityType == null)
                {
                    return new Dictionary<string, string>(0);
                }
                var entityConfig = GetEntityConfiguration(sourceEntityType);
                Dictionary<string, string> relationFields = null;
                entityConfig?.RelationFields?.TryGetValue(relationEntityType.GUID, out relationFields);
                return relationFields ?? new Dictionary<string, string>(0);
            }

            #endregion

            #region Get entity property value provider

            public static IEntityPropertyValueProvider GetValueProvider(Type entityType, MemberInfo member)
            {
                ParameterExpression instanceExpression = Expression.Parameter(entityType);

                //getter
                Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
                parameterArray.SetValue(instanceExpression, 0);
                Expression propertyExpression = Expression.PropertyOrField(instanceExpression, member.Name);
                Type funcType = typeof(Func<,>).MakeGenericType(entityType, typeof(object));//function type
                var genericLambdaMethod = ReflectionManager.Expression.LambdaMethod.MakeGenericMethod(funcType);
                var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                {
                        Expression.Convert(propertyExpression,typeof(object)),parameterArray
                });
                Type propertyProviderType = typeof(DefaultEntityPropertyValueProvider<>).MakeGenericType(entityType);
                var propertyProvider = Activator.CreateInstance(propertyProviderType);
                var setGetterMethod = propertyProviderType.GetMethod("SetGetter");
                setGetterMethod.Invoke(propertyProvider, new object[] { lambdaExpression });

                //setter
                lambdaExpression = null;
                Type valueType = typeof(object);
                genericLambdaMethod = ReflectionManager.Expression.LambdaMethod.MakeGenericMethod(typeof(Action<,>).MakeGenericType(entityType, valueType));
                ParameterExpression valueParameter = Expression.Parameter(valueType, "value");
                var parameterExpressionArray = new ParameterExpression[2] { instanceExpression, valueParameter };
                if (member is PropertyInfo propertyInfo)
                {
                    Expression readValueParameter = ExpressionHelper.EnsureCastExpression(valueParameter, propertyInfo.PropertyType);
                    MethodInfo setMethod = propertyInfo.GetSetMethod(true);
                    if (setMethod == null)
                    {
                        throw new ArgumentException("Property does not have a setter.");
                    }

                    Expression readInstanceParameter = ExpressionHelper.EnsureCastExpression(instanceExpression, member.DeclaringType);
                    Expression setExpression = Expression.Call(readInstanceParameter, setMethod, readValueParameter);

                    lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                    {
                        setExpression,parameterExpressionArray
                    });
                }
                else if (member is FieldInfo fieldInfo)
                {
                    Expression sourceExpression = ExpressionHelper.EnsureCastExpression(instanceExpression, fieldInfo.DeclaringType);
                    Expression fieldExpression = Expression.Field(sourceExpression, fieldInfo);

                    Expression valueExpression = ExpressionHelper.EnsureCastExpression(valueParameter, fieldExpression.Type);
                    BinaryExpression assignExpression = Expression.Assign(fieldExpression, valueExpression);

                    lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                    {
                        assignExpression,parameterExpressionArray
                    });
                }
                if (lambdaExpression != null)
                {
                    var setSetterMethod = propertyProviderType.GetMethod("SetSetter");
                    setSetterMethod.Invoke(propertyProvider, new object[] { lambdaExpression });
                }

                return propertyProvider as IEntityPropertyValueProvider;
            }

            #endregion
        }
    }
}
