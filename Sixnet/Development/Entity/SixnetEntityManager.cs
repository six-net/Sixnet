// "Company © 2025. All rights reserved."

using System.Reflection;

using Sixnet.Development.Data.Database;
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
        static readonly Dictionary<Guid, SixnetEntityConfiguration> _entityConfigurations = new();

        /// <summary>
        /// Defines boolean type
        /// </summary>
        static readonly Type _booleanType = typeof(bool);

        /// <summary>
        /// All field roles
        /// </summary>
        static readonly List<SixnetFieldRole> _allFieldRoles = new();

        /// <summary>
        /// Func<> type
        /// </summary>
        static readonly Type _funcType = typeof(Func<>);

        static SixnetEntityManager()
        {
            var fieldRoleValues = Enum.GetValues(typeof(SixnetFieldRole));
            foreach (SixnetFieldRole roleVal in fieldRoleValues)
            {
                if (roleVal != SixnetFieldRole.None)
                {
                    _allFieldRoles.Add(roleVal);
                }
            }
            ConfigureEntity(typeof(SixnetAppUpdateRecordEntity));
            ConfigureEntity(typeof(SixnetLocalizationEntity));
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
            var entityAttribute = entityType.GetCustomAttribute<SixnetEntityAttribute>(false);
            if (entityAttribute == null)
            {
                return;
            }
            IEnumerable<MemberInfo> memberInfos = new List<MemberInfo>(0);
            memberInfos = memberInfos.Union(entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
            memberInfos = memberInfos.Union(entityType.GetFields(BindingFlags.Public | BindingFlags.Instance));
            var tableName = string.IsNullOrWhiteSpace(entityAttribute.TableName) ? entityType.Name : entityAttribute.TableName;
            if (!_entityConfigurations.TryGetValue(typeGuid, out SixnetEntityConfiguration entityConfig))
            {
                entityConfig = new SixnetEntityConfiguration()
                {
                    Group = entityAttribute?.Module ?? string.Empty,
                    Schema = entityAttribute?.Schema ?? string.Empty,
                };
            }
            //table name
            if (string.IsNullOrWhiteSpace(entityConfig.TableName))
            {
                entityConfig.TableName = tableName;
            }
            //fields
            var allFields = new List<SixnetDataField>();
            var roleFields = new Dictionary<SixnetFieldRole, List<SixnetDataField>>();

            //cache fields
            var cacheFieldNames = new List<string>();
            var cachePrefixFieldNames = new List<string>();
            var cacheIgnoreFieldNames = new List<string>();

            var necessaryQueryableFields = new List<SixnetDataField>();
            var editableFields = new List<SixnetDataField>();
            var queryableFields = new List<SixnetDataField>();
            foreach (var member in memberInfos)
            {
                var nonDataAttribute = member.GetCustomAttribute<SixnetNotEntityFieldAttribute>();
                if (nonDataAttribute != null)
                {
                    continue;
                }
                var fieldName = member.Name;
                var propertyName = fieldName;
                var fieldRole = SixnetFieldRole.None;
                var entityFieldAttribute = (member.GetCustomAttributes(typeof(SixnetEntityFieldAttribute), false)?.FirstOrDefault()) as SixnetEntityFieldAttribute;
                fieldRole = entityFieldAttribute?.Role ?? SixnetFieldRole.None;
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
                // file object name
                var fileObjectName = entityFieldAttribute?.FileObjectName;
                if (string.IsNullOrWhiteSpace(fileObjectName))
                {
                    fileObjectName = $"{entityType.Name}.{propertyName}".ToLower();
                }
                var propertyField = new SixnetDataField()
                {
                    FieldName = fieldName,
                    PropertyName = propertyName,
                    CacheRole = entityFieldAttribute?.CacheRole ?? SixnetFieldCacheRole.None,
                    Role = fieldRole,
                    Behavior = entityFieldAttribute?.Behavior ?? SixnetFieldBehavior.None,
                    DataType = memberType,
                    DbType = entityFieldAttribute?.DbType,
                    Length = entityFieldAttribute?.Length ?? 0,
                    Description = entityFieldAttribute?.Description ?? string.Empty,
                    FileObjectName = fileObjectName,
                    ModelType = entityType,
                    DbFeature = entityFieldAttribute?.DbFeature ?? SixnetFieldDbFeature.None,
                    FormatSetting = entityFieldAttribute?.FormatSetting,
                    Precision = entityFieldAttribute?.Precision ?? 0,
                    DefaultValue = entityFieldAttribute?.DefaultValue ?? string.Empty,
                    IncrementValue = entityFieldAttribute?.IncrementValue ?? 0,
                    IndexSequence = entityFieldAttribute?.IndexSequence ?? 0
                };
                var fieldStartValue = entityFieldAttribute?.StartValue ?? 0;
                if (propertyField.InRole(SixnetFieldRole.PrimaryKey) && fieldStartValue == 0)
                {
                    fieldStartValue = entityAttribute.PrimaryKeyStartValue;
                }
                propertyField.StartValue = fieldStartValue;

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
                if (!propertyField.InRole(SixnetFieldRole.PrimaryKey))
                {
                    if (propertyField.InCacheRole(SixnetFieldCacheRole.CacheKey))
                    {
                        cacheFieldNames.Add(propertyName);
                    }
                    if (propertyField.InCacheRole(SixnetFieldCacheRole.CacheKeyPrefix))
                    {
                        cachePrefixFieldNames.Add(propertyName);
                    }
                    if (propertyField.InCacheRole(SixnetFieldCacheRole.Ignore))
                    {
                        cacheIgnoreFieldNames.Add(propertyName);
                    }
                }

                //relation config
                var relationAttributes = member.GetCustomAttributes<SixnetEntityRelationFieldAttribute>(false);
                if (relationAttributes.IsNullOrEmpty())
                {
                    continue;
                }
                if (entityConfig.RelationFields.IsNullOrEmpty())
                {
                    entityConfig.RelationFields = new Dictionary<Guid, Dictionary<string, SixnetEntityRelationFieldAttribute>>();
                }
                foreach (var relationAttrObj in relationAttributes)
                {
                    if (relationAttrObj is SixnetEntityRelationFieldAttribute relationAttr && relationAttr.RelationType != null && !string.IsNullOrWhiteSpace(relationAttr.RelationField))
                    {
                        var relationTypeId = relationAttr.RelationType.GUID;
                        entityConfig.RelationFields.TryGetValue(relationTypeId, out var values);
                        values ??= new Dictionary<string, SixnetEntityRelationFieldAttribute>();
                        if (!values.ContainsKey(propertyName))
                        {
                            values.Add(propertyName, relationAttr);
                            entityConfig.RelationFields[relationTypeId] = values;
                        }
                    }
                }
            }

            allFields = allFields.OrderByDescending(f => f.InRole(SixnetFieldRole.PrimaryKey))
                        .ThenByDescending(c => cacheFieldNames.Contains(c.PropertyName))
                        .ToList();
            var allFieldDict = new Dictionary<string, SixnetDataField>(allFields.Count);
            foreach (var field in allFields)
            {
                allFieldDict[field.PropertyName] = field;
                var necessaryQueryField = field.IsNecessaryField();
                if (necessaryQueryField)
                {
                    necessaryQueryableFields.Add(field);
                }
                if (necessaryQueryField || !field.AllowBehavior(SixnetFieldBehavior.NotQuery))
                {
                    queryableFields.Add(field);
                }
                if (!field.AllowBehavior(SixnetFieldBehavior.NotInsert))
                {
                    editableFields.Add(field);
                }
            }
            entityConfig.IsSystem = entityAttribute.IsSystem;
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
            entityConfig.AutoExpansionSplitNum = entityAttribute.AutoExpansionSplitNum;
            _entityConfigurations[typeGuid] = entityConfig;
        }

        /// <summary>
        /// Add role field
        /// </summary>
        /// <param name="allRoleFields"></param>
        /// <param name="fieldRole"></param>
        /// <param name="field"></param>
        static void AddRoleField(Dictionary<SixnetFieldRole, List<SixnetDataField>> allRoleFields, SixnetFieldRole fieldRole, SixnetDataField field)
        {
            if (allRoleFields.TryGetValue(fieldRole, out List<SixnetDataField> fields))
            {
                fields.Add(field);
            }
            else
            {
                allRoleFields[fieldRole] = new List<SixnetDataField>() { field };
            }
        }

        #endregion

        #region Get entity config

        /// <summary>
        /// Get entity config
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static SixnetEntityConfiguration GetEntityConfig(Type entityType)
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
        public static SixnetEntityConfiguration GetEntityConfig<TEntity>()
        {
            return GetEntityConfig(typeof(TEntity));
        }

        /// <summary>
        /// Get entity config
        /// </summary>
        /// <param name="entityTypeId">Entity type id</param>
        /// <returns></returns>
        public static SixnetEntityConfiguration GetEntityConfig(Guid entityTypeId)
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
        public static IEnumerable<SixnetEntityConfiguration> GetAllEntityConfigs()
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
        public static List<SixnetDataField> GetQueryableFields(Type entityType)
        {
            var entityConfig = GetEntityConfig(entityType);
            return entityConfig?.QueryableFields ?? new List<SixnetDataField>(0);
        }

        #endregion

        #region Necessary fields

        /// <summary>
        /// Get necessary fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static List<SixnetDataField> GetNecessaryFields(Type entityType)
        {
            var entityConfig = GetEntityConfig(entityType);
            return entityConfig?.NecessaryQueryableFields ?? new List<SixnetDataField>(0);
        }

        #endregion

        #region Get entity field

        /// <summary>
        /// Get entity field
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public static SixnetDataField GetField(Type entityType, string propertyName)
        {
            if (entityType == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }
            SixnetDataField field = null;
            GetEntityConfig(entityType)?.AllFields.TryGetValue(propertyName, out field);
            return field;
        }

        /// <summary>
        /// Get entity field by role
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static SixnetDataField GetField(Type entityType, SixnetFieldRole fieldRole)
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
        public static List<SixnetDataField> GetFields(Type entityType, SixnetFieldRole fieldRole)
        {
            if (entityType == null || fieldRole == SixnetFieldRole.None)
            {
                return new List<SixnetDataField>(0);
            }
            var entityConfig = GetEntityConfig(entityType);
            List<SixnetDataField> fields = null;
            entityConfig?.RoleFields?.TryGetValue(fieldRole, out fields);
            return fields ?? new List<SixnetDataField>(0);
        }

        /// <summary>
        /// Get entity role fields
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<SixnetDataField> GetFields<TEntity>(SixnetFieldRole fieldRole)
        {
            return GetFields(typeof(TEntity), fieldRole);
        }

        /// <summary>
        /// Get entity role field names
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<string> GetFieldNames(Type entityType, SixnetFieldRole fieldRole)
        {
            return GetFields(entityType, fieldRole)?.Select(f => f.PropertyName).ToList() ?? new List<string>(0);
        }

        /// <summary>
        /// Get entity role field names
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static List<string> GetFieldNames<TEntity>(SixnetFieldRole fieldRole)
        {
            return GetFieldNames(typeof(TEntity), fieldRole);
        }

        /// <summary>
        /// Get entity role field name
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static string GetFieldName(Type entityType, SixnetFieldRole fieldRole)
        {
            return GetFieldNames(entityType, fieldRole)?.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Get entity role field name
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public static string GetFieldName<TEntity>(SixnetFieldRole fieldRole)
        {
            return GetFieldNames(typeof(TEntity), fieldRole)?.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Primary key fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public static List<SixnetDataField> GetPrimaryKeyFields(Type entityType)
        {
            return GetFields(entityType, SixnetFieldRole.PrimaryKey);
        }

        /// <summary>
        /// Get primary key property names
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return all primary key property names</returns>
        public static IEnumerable<string> GetPrimaryKeyNames(Type entityType)
        {
            return GetFieldNames(entityType, SixnetFieldRole.PrimaryKey);
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
            Dictionary<string, SixnetEntityRelationFieldAttribute> sourceRelationFields = null;
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
            var parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
            parameterArray.SetValue(instanceExpression, 0);
            var propertyExpression = Expression.PropertyOrField(instanceExpression, member.Name);
            var funcType = typeof(Func<,>).MakeGenericType(entityType, typeof(object));//function type
            var genericLambdaMethod = SixnetReflecter.Expression.LambdaMethod.MakeGenericMethod(funcType);
            var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
            {
                Expression.Convert(propertyExpression,typeof(object)),parameterArray
            });
            var propertyProviderType = typeof(SixnetDefaultEntityPropertyValueProvider<>).MakeGenericType(entityType);
            var propertyProvider = Activator.CreateInstance(propertyProviderType);
            var setGetterMethod = propertyProviderType.GetMethod("SetGetter");
            setGetterMethod.Invoke(propertyProvider, new object[] { lambdaExpression });

            //setter
            lambdaExpression = null;
            var valueType = typeof(object);
            genericLambdaMethod = SixnetReflecter.Expression.LambdaMethod.MakeGenericMethod(typeof(Action<,>).MakeGenericType(entityType, valueType));
            var valueParameter = Expression.Parameter(valueType, "value");
            var parameterExpressionArray = new ParameterExpression[2] { instanceExpression, valueParameter };
            if (member is PropertyInfo propertyInfo)
            {
                var readValueParameter = SixnetExpressionHelper.EnsureCastExpression(valueParameter, propertyInfo.PropertyType);
                var setMethod = propertyInfo.GetSetMethod(true) ?? throw new ArgumentException("Property does not have a setter.");
                var readInstanceParameter = SixnetExpressionHelper.EnsureCastExpression(instanceExpression, member.DeclaringType);
                var setExpression = Expression.Call(readInstanceParameter, setMethod, readValueParameter);

                lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                {
                        setExpression,parameterExpressionArray
                });
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
