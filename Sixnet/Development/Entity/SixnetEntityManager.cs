using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Expressions.Linq;
using Sixnet.Reflection;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Sixnet entity manager
    /// </summary>
    public static class SixnetEntityManager
    {
        #region Fields

        /// <summary>
        /// Value: Entity configuration
        /// Key: Entity type guid
        /// </summary>
        static readonly Dictionary<Guid, EntityConfiguration> _entityConfigurations = new();

        /// <summary>
        /// Defines boolean type
        /// </summary>
        static readonly Type _booleanType = typeof(bool);

        /// <summary>
        /// All field roles
        /// </summary>
        static readonly List<FieldRole> _allFieldRoles = new();

        /// <summary>
        /// Func<> type
        /// </summary>
        static readonly Type _funcType = typeof(Func<>);

        static SixnetEntityManager()
        {
            var fieldRoleValues = Enum.GetValues(typeof(FieldRole));
            foreach (FieldRole roleVal in fieldRoleValues)
            {
                if (roleVal != FieldRole.None)
                {
                    _allFieldRoles.Add(roleVal);
                }
            }
        }

        #endregion

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
            if (_entityConfigurations.ContainsKey(typeGuid))
            {
                return;
            }
            var entityAttribute = entityType.GetCustomAttribute<EntityAttribute>() ?? EntityAttribute.Default;
            IEnumerable<MemberInfo> memberInfos = new List<MemberInfo>(0);
            memberInfos = memberInfos.Union(entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            memberInfos = memberInfos.Union(entityType.GetFields(BindingFlags.Public | BindingFlags.Instance));
            var tableName = string.IsNullOrWhiteSpace(entityAttribute.TableName) ? entityType.Name : entityAttribute.TableName;
            if (!_entityConfigurations.TryGetValue(typeGuid, out EntityConfiguration entityConfig))
            {
                entityConfig = new EntityConfiguration()
                {
                    Group = entityAttribute?.Module ?? string.Empty
                };
            }
            //table name
            if (string.IsNullOrWhiteSpace(entityConfig.TableName))
            {
                entityConfig.TableName = tableName;
            }
            //fields
            var allFields = new List<DataField>();
            var roleFields = new Dictionary<FieldRole, List<DataField>>();

            //cache fields
            var cacheFieldNames = new List<string>();
            var cachePrefixFieldNames = new List<string>();
            var cacheIgnoreFieldNames = new List<string>();

            var necessaryQueryableFields = new List<DataField>();
            var editableFields = new List<DataField>();
            var queryableFields = new List<DataField>();
            foreach (var member in memberInfos)
            {
                var nonDataAttribute = member.GetCustomAttribute<NotEntityFieldAttribute>();
                if (nonDataAttribute != null)
                {
                    continue;
                }
                var fieldName = member.Name;
                var propertyName = fieldName;
                var fieldRole = FieldRole.None;
                var entityFieldAttribute = (member.GetCustomAttributes(typeof(EntityFieldAttribute), false)?.FirstOrDefault()) as EntityFieldAttribute;
                fieldRole = entityFieldAttribute?.Role ?? FieldRole.None;
                fieldName = string.IsNullOrWhiteSpace(entityFieldAttribute?.FieldName) ? fieldName : entityFieldAttribute.FieldName;
                Type memberType = null;
                if (member is PropertyInfo propertyInfo)
                {
                    memberType = propertyInfo.PropertyType;
                }
                if (member is FieldInfo fieldInfo)
                {
                    memberType = fieldInfo.FieldType;
                }
                var propertyField = new DataField()
                {
                    FieldName = fieldName,
                    PropertyName = propertyName,
                    CacheRole = entityFieldAttribute?.CacheRole ?? FieldCacheRole.None,
                    Role = fieldRole,
                    Behavior = entityFieldAttribute?.Behavior ?? FieldBehavior.None,
                    DataType = memberType,
                    DbType = entityFieldAttribute?.DbType,
                    Length = entityFieldAttribute?.Length ?? 0,
                    Description = entityFieldAttribute?.Description ?? string.Empty
                };

                //value provider
                var valueProvider = GetValueProvider(entityType, member);
                if (valueProvider != null)
                {
                    propertyField.ValueProvider = valueProvider;
                }
                allFields.Add(propertyField);

                // field role
                foreach (var roleVal in _allFieldRoles)
                {
                    if (propertyField.InRole(roleVal))
                    {
                        AddRoleField(roleFields, roleVal, propertyField);
                    }
                }

                // cache role
                if (!propertyField.InRole(FieldRole.PrimaryKey))
                {
                    if (propertyField.InCacheRole(FieldCacheRole.CacheKey))
                    {
                        cacheFieldNames.Add(propertyName);
                    }
                    if (propertyField.InCacheRole(FieldCacheRole.CacheKeyPrefix))
                    {
                        cachePrefixFieldNames.Add(propertyName);
                    }
                    if (propertyField.InCacheRole(FieldCacheRole.Ignore))
                    {
                        cacheIgnoreFieldNames.Add(propertyName);
                    }
                }

                //relation config
                var relationAttributes = member.GetCustomAttributes<EntityRelationFieldAttribute>(false);
                if (relationAttributes.IsNullOrEmpty())
                {
                    continue;
                }
                if (entityConfig.RelationFields.IsNullOrEmpty())
                {
                    entityConfig.RelationFields = new Dictionary<Guid, Dictionary<string, EntityRelationFieldAttribute>>();
                }
                foreach (var relationAttrObj in relationAttributes)
                {
                    if (relationAttrObj is EntityRelationFieldAttribute relationAttr && relationAttr.RelationType != null && !string.IsNullOrWhiteSpace(relationAttr.RelationField))
                    {
                        var relationTypeId = relationAttr.RelationType.GUID;
                        entityConfig.RelationFields.TryGetValue(relationTypeId, out var values);
                        values ??= new Dictionary<string, EntityRelationFieldAttribute>();
                        if (!values.ContainsKey(propertyName))
                        {
                            values.Add(propertyName, relationAttr);
                            entityConfig.RelationFields[relationTypeId] = values;
                        }
                    }
                }
            }
            allFields = allFields.OrderByDescending(f => f.InRole(FieldRole.PrimaryKey))
                        .ThenByDescending(c => cacheFieldNames.Contains(c.PropertyName))
                        .ToList();
            var allFieldDict = new Dictionary<string, DataField>(allFields.Count);
            foreach (var field in allFields)
            {
                allFieldDict[field.PropertyName] = field;
                var necessaryQueryField = field.IsNecessaryField();
                if (necessaryQueryField)
                {
                    necessaryQueryableFields.Add(field);
                }
                if (necessaryQueryField || !field.AllowBehavior(FieldBehavior.NotQuery))
                {
                    queryableFields.Add(field);
                }
                if (!field.AllowBehavior(FieldBehavior.NotInsert))
                {
                    editableFields.Add(field);
                }
            }
            entityConfig.Description = entityAttribute.Description ?? string.Empty;
            entityConfig.AllFields = allFieldDict;
            entityConfig.CacheFieldNames = cacheFieldNames;
            entityConfig.CachePrefixFieldNames = cachePrefixFieldNames;
            entityConfig.CacheIgnoreFieldNames = cacheIgnoreFieldNames;
            entityConfig.QueryableFields = queryableFields;
            entityConfig.EditableFields = editableFields;
            entityConfig.NecessaryQueryableFields = necessaryQueryableFields;
            entityConfig.PredicateType = typeof(Func<,>).MakeGenericType(entityType, _booleanType);
            entityConfig.EntityType = entityType;
            entityConfig.EnableCache = entityAttribute.EnableCache;
            entityConfig.Style = entityAttribute.Style;
            entityConfig.RoleFields = roleFields;
            entityConfig.SplitTableType = entityAttribute.SplitTableType;
            entityConfig.SplitTableProviderName = entityAttribute.SplitTableProviderName;
            _entityConfigurations[typeGuid] = entityConfig;
        }

        /// <summary>
        /// Add role field
        /// </summary>
        /// <param name="allRoleFields"></param>
        /// <param name="fieldRole"></param>
        /// <param name="field"></param>
        static void AddRoleField(Dictionary<FieldRole, List<DataField>> allRoleFields, FieldRole fieldRole, DataField field)
        {
            if (allRoleFields.TryGetValue(fieldRole, out List<DataField> fields))
            {
                fields.Add(field);
            }
            else
            {
                allRoleFields[fieldRole] = new List<DataField>() { field };
            }
        }

        #endregion

        #region Get entity config

        /// <summary>
        /// Get entity config
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static EntityConfiguration GetEntityConfig(Type entityType)
        {
            if (entityType == null)
            {
                return null;
            }
            var typeGuid = entityType.GUID;
            return GetEntityConfig(typeGuid);
        }

        /// <summary>
        /// Get entity config
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns></returns>
        public static EntityConfiguration GetEntityConfig<TEntity>()
        {
            return GetEntityConfig(typeof(TEntity));
        }

        /// <summary>
        /// Get entity config
        /// </summary>
        /// <param name="entityTypeId">Entity type id</param>
        /// <returns></returns>
        public static EntityConfiguration GetEntityConfig(Guid entityTypeId)
        {
            _entityConfigurations.TryGetValue(entityTypeId, out var entityConfig);
            return entityConfig;
        }

        #endregion

        #region Get all entity configs

        /// <summary>
        /// Get all entity configs
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<EntityConfiguration> GetAllEntityConfigs()
        {
            return _entityConfigurations.Values;
        }

        #endregion

        #endregion

        #region Entity field

        #region Queryable fields

        /// <summary>
        /// Get entity queryable fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static List<DataField> GetQueryableFields(Type entityType)
        {
            var entityConfig = GetEntityConfig(entityType);
            return entityConfig?.QueryableFields ?? new List<DataField>(0);
        }

        #endregion

        #region Necessary fields

        /// <summary>
        /// Get necessary fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static List<DataField> GetNecessaryFields(Type entityType)
        {
            var entityConfig = GetEntityConfig(entityType);
            return entityConfig?.NecessaryQueryableFields ?? new List<DataField>(0);
        }

        #endregion

        #region Get entity field

        /// <summary>
        /// Get entity field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public static DataField GetField(Type entityType, string propertyName)
        {
            if (entityType == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }
            DataField field = null;
            GetEntityConfig(entityType)?.AllFields.TryGetValue(propertyName, out field);
            return field;
        }

        /// <summary>
        /// Get entity field by role
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static DataField GetField(Type entityType, FieldRole fieldRole)
        {
            var fieldName = GetFieldName(entityType, fieldRole);
            return GetField(entityType, fieldName);
        }

        #endregion

        #region Get fields

        /// <summary>
        /// Get entity role fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<DataField> GetFields(Type entityType, FieldRole fieldRole)
        {
            if (entityType == null || fieldRole == FieldRole.None)
            {
                return new List<DataField>(0);
            }
            var entityConfig = GetEntityConfig(entityType);
            List<DataField> fields = null;
            entityConfig?.RoleFields?.TryGetValue(fieldRole, out fields);
            return fields ?? new List<DataField>(0);
        }

        /// <summary>
        /// Get entity role fields
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<DataField> GetFields<TEntity>(FieldRole fieldRole)
        {
            return GetFields(typeof(TEntity), fieldRole);
        }

        /// <summary>
        /// Get entity role field names
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<string> GetFieldNames(Type entityType, FieldRole fieldRole)
        {
            return GetFields(entityType, fieldRole)?.Select(f => f.PropertyName).ToList() ?? new List<string>(0);
        }

        /// <summary>
        /// Get entity role field names
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<string> GetFieldNames<TEntity>(FieldRole fieldRole)
        {
            return GetFieldNames(typeof(TEntity), fieldRole);
        }

        /// <summary>
        /// Get entity role field name
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static string GetFieldName(Type entityType, FieldRole fieldRole)
        {
            return GetFieldNames(entityType, fieldRole)?.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Get entity role field name
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static string GetFieldName<TEntity>(FieldRole fieldRole)
        {
            return GetFieldNames(typeof(TEntity), fieldRole)?.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Primary key fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static List<DataField> GetPrimaryKeyFields(Type entityType)
        {
            return GetFields(entityType, FieldRole.PrimaryKey);
        }

        /// <summary>
        /// Get primary key property names
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return all primary key property names</returns>
        public static IEnumerable<string> GetPrimaryKeyNames(Type entityType)
        {
            return GetFieldNames(entityType, FieldRole.PrimaryKey);
        }

        /// <summary>
        /// Get primary key names
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return all primary key field names</returns>
        public static IEnumerable<string> GetPrimaryKeyNames<TEntity>()
        {
            return GetPrimaryKeyNames(typeof(TEntity));
        }

        /// <summary>
        /// Determines whether the property is primary key
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Whether is primary key</returns>
        public static bool IsPrimaryKey(Type entityType, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }
            var primaryKeys = GetPrimaryKeyNames(entityType);
            return primaryKeys?.Any(c => c == propertyName) ?? false;
        }

        #endregion

        #region Relation fields

        /// <summary>
        /// Get relation field names
        /// </summary>
        /// <param name="sourceEntityType">Source entity type</param>
        /// <param name="relationEntityType">Relation entity type</param>
        /// <returns>Return all relation fields</returns>
        public static Dictionary<string, string> GetRelationFieldNames(Type sourceEntityType, Type relationEntityType)
        {
            if (sourceEntityType == null || relationEntityType == null)
            {
                return new Dictionary<string, string>(0);
            }

            //source => target
            var sourceRelationFields = GetSingleRelationFieldNames(sourceEntityType, relationEntityType);
            sourceRelationFields ??= new Dictionary<string, string>(0);

            //tart => source
            var targetRelationFields = GetSingleRelationFieldNames(relationEntityType, sourceEntityType);
            if (!targetRelationFields.IsNullOrEmpty())
            {
                foreach (var targetKeyItem in targetRelationFields)
                {
                    sourceRelationFields[targetKeyItem.Value] = targetKeyItem.Key;
                }
            }
            return sourceRelationFields;
        }

        /// <summary>
        /// Get single relation field names
        /// </summary>
        /// <param name="sourceEntityType">Source entity type</param>
        /// <param name="relationEntityType"></param>
        /// <returns></returns>
        static Dictionary<string, string> GetSingleRelationFieldNames(Type sourceEntityType, Type relationEntityType)
        {
            if (sourceEntityType == null || relationEntityType == null)
            {
                return new Dictionary<string, string>(0);
            }
            var sourceEntityConfig = GetEntityConfig(sourceEntityType);
            Dictionary<string, EntityRelationFieldAttribute> sourceRelationFields = null;
            sourceEntityConfig?.RelationFields?.TryGetValue(relationEntityType.GUID, out sourceRelationFields);
            return sourceRelationFields?.ToDictionary(c => c.Key, c => c.Value.RelationField) ?? new Dictionary<string, string>(0);
        }

        #endregion

        #endregion

        #region Table name

        #region Get table name

        /// <summary>
        /// Get entity table name
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity table name</returns>
        public static string GetTableName(Type entityType)
        {
            if (entityType == null)
            {
                return string.Empty;
            }
            var entityConfig = GetEntityConfig(entityType);
            return entityConfig?.TableName ?? string.Empty;
        }

        #endregion

        #endregion

        #region Value provider

        /// <summary>
        /// Get entity value provider
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="member">Member</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static ISixnetEntityPropertyValueProvider GetValueProvider(Type entityType, MemberInfo member)
        {
            ParameterExpression instanceExpression = Expression.Parameter(entityType);

            //getter
            Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
            parameterArray.SetValue(instanceExpression, 0);
            Expression propertyExpression = Expression.PropertyOrField(instanceExpression, member.Name);
            Type funcType = typeof(Func<,>).MakeGenericType(entityType, typeof(object));//function type
            var genericLambdaMethod = SixnetReflecter.Expression.LambdaMethod.MakeGenericMethod(funcType);
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
            genericLambdaMethod = SixnetReflecter.Expression.LambdaMethod.MakeGenericMethod(typeof(Action<,>).MakeGenericType(entityType, valueType));
            ParameterExpression valueParameter = Expression.Parameter(valueType, "value");
            var parameterExpressionArray = new ParameterExpression[2] { instanceExpression, valueParameter };
            if (member is PropertyInfo propertyInfo)
            {
                Expression readValueParameter = SixnetExpressionHelper.EnsureCastExpression(valueParameter, propertyInfo.PropertyType);
                MethodInfo setMethod = propertyInfo.GetSetMethod(true);
                if (setMethod == null)
                {
                    throw new ArgumentException("Property does not have a setter.");
                }

                Expression readInstanceParameter = SixnetExpressionHelper.EnsureCastExpression(instanceExpression, member.DeclaringType);
                Expression setExpression = Expression.Call(readInstanceParameter, setMethod, readValueParameter);

                lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                {
                        setExpression,parameterExpressionArray
                });
            }
            else if (member is FieldInfo fieldInfo)
            {
                Expression sourceExpression = SixnetExpressionHelper.EnsureCastExpression(instanceExpression, fieldInfo.DeclaringType);
                Expression fieldExpression = Expression.Field(sourceExpression, fieldInfo);

                Expression valueExpression = SixnetExpressionHelper.EnsureCastExpression(valueParameter, fieldExpression.Type);
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

            return propertyProvider as ISixnetEntityPropertyValueProvider;
        }

        #endregion

        #region Get predicate type

        /// <summary>
        /// Get predicate type
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return predicate type</returns>
        internal static Type GetEntityPredicateType(Type entityType)
        {
            if (entityType == null)
            {
                return null;
            }
            var entityConfig = GetEntityConfig(entityType);
            return entityConfig?.PredicateType ?? _funcType.MakeGenericType(entityType, _booleanType);
        }

        #endregion
    }
}
