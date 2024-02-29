using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        internal static readonly Dictionary<Guid, EntityConfiguration> EntityConfigurations = new();

        /// <summary>
        /// Defines boolean type
        /// </summary>
        static readonly Type BooleanType = typeof(bool);

        /// <summary>
        /// All field roles
        /// </summary>
        static readonly List<FieldRole> AllFieldRoles = new();

        static SixnetEntityManager()
        {
            var fieldRoleValues = Enum.GetValues(typeof(FieldRole));
            foreach (FieldRole roleVal in fieldRoleValues)
            {
                if (roleVal != FieldRole.None)
                {
                    AllFieldRoles.Add(roleVal);
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
            if (EntityConfigurations.ContainsKey(typeGuid))
            {
                return;
            }
            var entityAttribute = entityType.GetCustomAttribute<EntityAttribute>() ?? EntityAttribute.Default;
            IEnumerable<MemberInfo> memberInfos = new List<MemberInfo>(0);
            memberInfos = memberInfos.Union(entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            memberInfos = memberInfos.Union(entityType.GetFields(BindingFlags.Public | BindingFlags.Instance));
            var tableName = string.IsNullOrWhiteSpace(entityAttribute.TableName) ? entityType.Name : entityAttribute.TableName;
            if (!EntityConfigurations.TryGetValue(typeGuid, out EntityConfiguration entityConfig))
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
            var allFields = new List<EntityField>();
            var roleFields = new Dictionary<FieldRole, List<EntityField>>();

            //cache fields
            var cacheFieldNames = new List<string>();
            var cachePrefixFieldNames = new List<string>();
            var cacheIgnoreFieldNames = new List<string>();

            var necessaryQueryableFields = new List<EntityField>();
            var editableFields = new List<EntityField>();
            var queryableFields = new List<EntityField>();
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
                var propertyField = new EntityField()
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
                foreach (var roleVal in AllFieldRoles)
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
            var allFieldDict = new Dictionary<string, EntityField>(allFields.Count);
            foreach (var field in allFields)
            {
                allFieldDict[field.PropertyName] = field;
                var necessaryQueryField = field.IsNecessaryQueryField();
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
            entityConfig.PredicateType = typeof(Func<,>).MakeGenericType(entityType, BooleanType);
            entityConfig.EntityType = entityType;
            entityConfig.EnableCache = entityAttribute.EnableCache;
            entityConfig.Style = entityAttribute.Style;
            entityConfig.RoleFields = roleFields;
            entityConfig.SplitTableType = entityAttribute.SplitTableType;
            entityConfig.SplitTableProviderName = entityAttribute.SplitTableProviderName;
            EntityConfigurations[typeGuid] = entityConfig;
        }

        static void AddRoleField(Dictionary<FieldRole, List<EntityField>> allRoleFields, FieldRole fieldRole, EntityField field)
        {
            if (allRoleFields.TryGetValue(fieldRole, out List<EntityField> fields))
            {
                fields.Add(field);
            }
            else
            {
                allRoleFields[fieldRole] = new List<EntityField>() { field };
            }
        }

        #endregion

        #region Get entity configuration

        /// <summary>
        /// Get entity configuration
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity configuration</returns>
        public static EntityConfiguration GetEntityConfiguration(Type entityType)
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
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return entity configuration</returns>
        public static EntityConfiguration GetEntityConfiguration<TEntity>()
        {
            return GetEntityConfiguration(typeof(TEntity));
        }

        /// <summary>
        /// Get entity configuration
        /// </summary>
        /// <param name="entityTypeId">Entity type id</param>
        /// <returns>Return entity configuration</returns>
        public static EntityConfiguration GetEntityConfiguration(Guid entityTypeId)
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
        public static IEnumerable<EntityConfiguration> GetAllEntityConfigurations()
        {
            return EntityConfigurations.Values;
        }

        #endregion

        #endregion

        #region Table name

        #region Set table name

        /// <summary>
        /// Set entity table name
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="tableName">Table object name</param>
        internal static void SetTableName(Type entityType, string tableName)
        {
            if (entityType == null)
            {
                return;
            }
            var entityConfiguration = GetEntityConfiguration(entityType);
            if (entityConfiguration != null)
            {
                entityConfiguration.TableName = tableName;
            }
        }

        #endregion

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
            var entityConfig = GetEntityConfiguration(entityType);
            return entityConfig?.TableName ?? string.Empty;
        }

        #endregion

        #endregion

        #region Entity field

        #region Get all field names

        /// <summary>
        /// Get all field names
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return all field names</returns>
        public static IEnumerable<string> GetAllFieldNames(Type entityType)
        {
            var entityConfig = GetEntityConfiguration(entityType);
            if (entityConfig?.AllFields.IsNullOrEmpty() ?? true)
            {
                return Array.Empty<string>();
            }
            return entityConfig.AllFields.Keys;
        }

        #endregion

        #region Queryable fields

        /// <summary>
        /// Get entity queryable fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static List<EntityField> GetQueryableFields(Type entityType)
        {
            var entityConfig = GetEntityConfiguration(entityType);
            return entityConfig?.QueryableFields ?? new List<EntityField>(0);
        }

        #endregion

        #region Necessary queryable fields

        /// <summary>
        /// Get necessary queryable fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return necessary queryable fields</returns>
        public static List<EntityField> GetNecessaryQueryableFields(Type entityType)
        {
            var entityConfig = GetEntityConfiguration(entityType);
            return entityConfig?.NecessaryQueryableFields ?? new List<EntityField>(0);
        }

        #endregion

        #region Get entity default field

        /// <summary>
        /// Get entity default field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return default field</returns>
        public static EntityField GetDefaultField(Type entityType)
        {
            var entityConfig = GetEntityConfiguration(entityType);
            if (entityConfig != null)
            {
                entityConfig.RoleFields.TryGetValue(FieldRole.PrimaryKey, out var primaryKeys);
                if (primaryKeys.IsNullOrEmpty())
                {
                    primaryKeys = entityConfig.QueryableFields;
                }
                return primaryKeys?.FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// Get entity default field
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Return default field</returns>
        public static EntityField GetDefaultField<TEntity>()
        {
            return GetDefaultField(typeof(TEntity));
        }

        #endregion

        #region Get entity field

        /// <summary>
        /// Get entity field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return the entity field</returns>
        public static EntityField GetField(Type entityType, string propertyName)
        {
            if (entityType == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }
            EntityField field = null;
            GetEntityConfiguration(entityType)?.AllFields.TryGetValue(propertyName, out field);
            return field;
        }

        /// <summary>
        /// Get entity role field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static EntityField GetRoleField(Type entityType, FieldRole fieldRole)
        {
            var fieldName = GetRoleFieldName(entityType, fieldRole);
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
        public static List<EntityField> GetRoleFields(Type entityType, FieldRole fieldRole)
        {
            if (entityType == null || fieldRole == FieldRole.None)
            {
                return new List<EntityField>(0);
            }
            var entityConfig = GetEntityConfiguration(entityType);
            List<EntityField> fields = null;
            entityConfig?.RoleFields?.TryGetValue(fieldRole, out fields);
            return fields ?? new List<EntityField>(0);
        }

        /// <summary>
        /// Get entity role fields
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<EntityField> GetRoleFields<TEntity>(FieldRole fieldRole)
        {
            return GetRoleFields(typeof(TEntity), fieldRole);
        }

        /// <summary>
        /// Get entity role field names
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<string> GetRoleFieldNames(Type entityType, FieldRole fieldRole)
        {
            return GetRoleFields(entityType, fieldRole)?.Select(f => f.PropertyName).ToList() ?? new List<string>(0);
        }

        /// <summary>
        /// Get entity role field names
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<string> GetRoleFieldNames<TEntity>(FieldRole fieldRole)
        {
            return GetRoleFieldNames(typeof(TEntity), fieldRole);
        }

        /// <summary>
        /// Get entity role field names
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static string GetRoleFieldName(Type entityType, FieldRole fieldRole)
        {
            return GetRoleFieldNames(entityType, fieldRole)?.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Get entity role field name
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static string GetRoleFieldName<TEntity>(FieldRole fieldRole)
        {
            return GetRoleFieldNames(typeof(TEntity), fieldRole)?.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Primary key fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static List<EntityField> GetPrimaryKeyFields(Type entityType)
        {
            return GetRoleFields(entityType, FieldRole.PrimaryKey);
        }

        /// <summary>
        /// Get primary key property names
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return all primary key property names</returns>
        public static IEnumerable<string> GetPrimaryKeyNames(Type entityType)
        {
            return GetRoleFieldNames(entityType, FieldRole.PrimaryKey);
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
            var sourceEntityConfig = GetEntityConfiguration(sourceEntityType);
            Dictionary<string, EntityRelationFieldAttribute> sourceRelationFields = null;
            sourceEntityConfig?.RelationFields?.TryGetValue(relationEntityType.GUID, out sourceRelationFields);
            return sourceRelationFields?.ToDictionary(c => c.Key, c => c.Value.RelationField) ?? new Dictionary<string, string>(0);
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
    }
}
