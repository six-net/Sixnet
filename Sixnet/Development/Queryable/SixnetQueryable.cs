using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;
using Sixnet.Session;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Sixnet queryable
    /// </summary>
    public static class SixnetQueryable
    {
        #region Fields

        /// <summary>
        /// Boolean type
        /// </summary>
        static readonly Type BooleanType = typeof(bool);

        /// <summary>
        /// Func<> type
        /// </summary>
        static readonly Type FuncType = typeof(Func<>);

        /// <summary>
        /// Type global filters
        /// </summary>
        static readonly Dictionary<Guid, ICondition> typeGlobalFilters = new();

        /// <summary>
        /// Global filter delegate
        /// </summary>
        static Func<GlobalFilterContext, ISixnetQueryable> globalFilterDelegate;

        /// <summary>
        /// Whether disabled inactive data filter
        /// </summary>
        internal static bool DisabledInactiveDataFilter = false;

        /// <summary>
        /// Whether disabled isolation data filter
        /// </summary>
        private static bool DisabledIsolationDataFilter = false;

        #endregion

        #region Methods

        #region Create 

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable Create(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryable(sourceQueryable).SetModelType(typeof(ExpandoObject));
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst> Create<TFirst>(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryableOne<TFirst>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond> Create<TFirst, TSecond>(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryableTwo<TFirst, TSecond>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird> Create<TFirst, TSecond, TThird>(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryableThree<TFirst, TSecond, TThird>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Create<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryableFour<TFirst, TSecond, TThird, TFourth>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Create<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryableFive<TFirst, TSecond, TThird, TFourth, TFifth>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TSixth"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Create<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryableSix<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(sourceQueryable);
        }

        /// <summary>
        /// Create a new queryable instance
        /// </summary>
        /// <typeparam name="TFirst"></typeparam>
        /// <typeparam name="TSecond"></typeparam>
        /// <typeparam name="TThird"></typeparam>
        /// <typeparam name="TFourth"></typeparam>
        /// <typeparam name="TFifth"></typeparam>
        /// <typeparam name="TSixth"></typeparam>
        /// <typeparam name="TSeventh"></typeparam>
        /// <param name="sourceQueryable">Source queryable</param>
        /// <returns></returns>
        public static ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Create<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable sourceQueryable = null)
        {
            return new DefaultQueryableSeven<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(sourceQueryable);
        }

        /// <summary>
        /// Create a new query instance
        /// </summary>
        /// <typeparam name="T">Query model</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return query object</returns>
        public static ISixnetQueryable Create<T>(Expression<Func<T, bool>> conditionExpression)
        {
            var query = Create<T>();
            if (conditionExpression != null)
            {
                query.Where(conditionExpression);
            }
            return query;
        }

        /// <summary>
        /// Create a new query instance by entity
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Return query object</returns>
        internal static ISixnetQueryable CreateByEntity<TEntity>()
        {
            var query = Create();
            query.SetModelType(typeof(TEntity));
            return query;
        }

        #endregion

        #region Append entity identity queryable

        /// <summary>
        /// Append entity identity condition to original queryable
        /// it will create new queryable object if the original is null
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entities">Entities</param>
        /// <param name="originalQueryable">Original queryable</param>
        /// <param name="exclude">Exclude</param>
        /// <returns>Return the newest queryable</returns>
        public static ISixnetQueryable AppendEntityIdentityCondition<TEntity>(IEnumerable<TEntity> entities, ISixnetQueryable originalQueryable = null, bool exclude = false) where TEntity : class, IEntity<TEntity>
        {
            return AppendEntityIdentityCore(entities, originalQueryable, exclude);
        }

        /// <summary>
        /// Append entity identity condition to original queryable
        /// it will create new queryable object if the original is null
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="originalQueryable">Original queryable</param>
        /// <param name="exclude">Exclude</param>
        /// <returns>Return the newest queryable</returns>
        public static ISixnetQueryable AppendEntityIdentityCondition<TEntity>(TEntity entity, ISixnetQueryable originalQueryable = null, bool exclude = false) where TEntity : class, IEntity<TEntity>
        {
            return AppendEntityIdentityCore(new List<TEntity>() { entity }, originalQueryable, exclude);
        }

        /// <summary>
        /// Append entity identity condition to original queryable object
        /// it will create new queryable if the original is null
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="entities">Datas</param>
        /// <param name="originalQueryable">Original queryable</param>
        /// <param name="exclude">Exclude</param>
        /// <returns></returns>
        internal static ISixnetQueryable AppendEntityIdentityCore<TEntity>(IEnumerable<TEntity> entities, ISixnetQueryable originalQueryable = null, bool exclude = false) where TEntity : class, IEntity<TEntity>
        {
            if (entities.IsNullOrEmpty())
            {
                return originalQueryable;
            }
            var entityType = typeof(TEntity);
            originalQueryable ??= Create().SetModelType(entityType);

            var primaryKeys = EntityManager.GetPrimaryKeyNames(entityType);
            ThrowHelper.ThrowFrameworkErrorIf(primaryKeys.IsNullOrEmpty(), string.Format("Type:{0} isn't set primary keys", entityType.FullName));

            var primaryKeyValues = new List<dynamic>();
            var primaryKeyCount = primaryKeys.GetCount();
            var isSinglePrimaryKey = primaryKeyCount == 1;
            var entityGroupQueryable = Create();
            foreach (var entity in entities)
            {
                if (isSinglePrimaryKey)
                {
                    primaryKeyValues.Add(entity.GetValue(primaryKeys.ElementAt(0)));
                }
                else
                {
                    var entityQueryable = Create();
                    foreach (var key in primaryKeys)
                    {
                        entityQueryable = entityQueryable.Where(Criterion.Create(exclude ? CriterionOperator.NotEqual : CriterionOperator.Equal, PropertyField.Create(key, entityType), ConstantField.Create(entity.GetValue(key))));
                    }
                    entityQueryable.Connector = CriterionConnector.Or;
                    entityGroupQueryable.Where(entityQueryable);
                }
            }
            if (isSinglePrimaryKey)
            {
                var primaryKeyValueCount = primaryKeyValues.Count;
                originalQueryable = exclude
                    ? primaryKeyValueCount > 1
                        ? originalQueryable.Where(Criterion.Create(CriterionOperator.NotIn, PropertyField.Create(primaryKeys.ElementAt(0), entityType), ConstantField.Create(primaryKeyValues)))
                        : originalQueryable.Where(Criterion.Create(CriterionOperator.NotEqual, PropertyField.Create(primaryKeys.ElementAt(0), entityType), ConstantField.Create(primaryKeyValues[0])))
                    : primaryKeyValueCount > 1
                        ? originalQueryable.Where(Criterion.Create(CriterionOperator.In, PropertyField.Create(primaryKeys.ElementAt(0), entityType), ConstantField.Create(primaryKeyValues)))
                        : originalQueryable.Where(Criterion.Create(CriterionOperator.Equal, PropertyField.Create(primaryKeys.ElementAt(0), entityType), ConstantField.Create(primaryKeyValues[0])));
            }
            else
            {
                originalQueryable = originalQueryable.Where(entityGroupQueryable);
            }
            return originalQueryable;
        }

        #endregion

        #region Global filter

        #region Add global filter

        /// <summary>
        /// Add global filter
        /// </summary>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="globalFilter">Global filter</param>
        public static void AddGlobalFilter<TFilter>(ICondition globalFilter)
        {
            typeGlobalFilters[typeof(TFilter).GUID] = globalFilter;
        }

        /// <summary>
        /// Add global filter
        /// </summary>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="globalFilterExpression">Global filter expression</param>
        public static void AddGlobalFilter<TFilter>(Expression<Func<TFilter, bool>> globalFilterExpression)
        {
            var globalFilterCondition = ExpressionHelper.GetQueryable(globalFilterExpression, CriterionConnector.And);
            AddGlobalFilter<TFilter>(globalFilterCondition);
        }

        /// <summary>
        /// Add global filter
        /// </summary>
        /// <param name="getGlobalFilterDelegate">Get global filter delegate</param>
        public static void AddGlobalFilter(Func<GlobalFilterContext, ISixnetQueryable> getGlobalFilterDelegate)
        {
            globalFilterDelegate = getGlobalFilterDelegate;
        }

        #endregion

        #region Get global filter

        /// <summary>
        /// Get global filter
        /// </summary>
        /// <param name="globalFilterContext">Global filter context</param>
        /// <returns></returns>
        internal static ICondition GetGlobalFilter(GlobalFilterContext globalFilterContext)
        {
            SixnetException.ThrowIf(globalFilterContext == null, $"{nameof(globalFilterContext)} is null");

            if (globalFilterContext.OriginalQueryable == null)
            {
                globalFilterContext.OriginalQueryable = Create();
                globalFilterContext.OriginalQueryable.SetModelType(globalFilterContext.ModelType);
            }
            var originalQueryable = globalFilterContext.OriginalQueryable;
            if (originalQueryable.IsGroupQueryable)
            {
                return null;
            }

            // custom global filter
            var globalFilter = globalFilterDelegate?.Invoke(globalFilterContext);

            // filter archived data
            if (!DisabledInactiveDataFilter && !originalQueryable.IncludeArchivedData)
            {
                var inactiveFieldName = EntityManager.GetRoleFieldName(globalFilterContext.ModelType, FieldRole.Archived);
                if (!string.IsNullOrWhiteSpace(inactiveFieldName))
                {
                    globalFilter ??= Create().SetModelType(globalFilterContext.ModelType);
                    globalFilter = globalFilter.Where(Criterion.Create(CriterionOperator.Equal, PropertyField.Create(inactiveFieldName, globalFilterContext.ModelType), ConstantField.Create(false)));
                }
            }

            // filter isolation data
            if (!DisabledIsolationDataFilter && !globalFilterContext.OriginalQueryable.UnisolatedData)
            {
                var isolationFieldName = EntityManager.GetRoleFieldName(globalFilterContext.ModelType, FieldRole.Isolation);
                if (!string.IsNullOrWhiteSpace(isolationFieldName))
                {
                    var isolationField = EntityManager.GetEntityConfiguration(globalFilterContext.ModelType).AllFields[isolationFieldName];
                    var isolationDataId = SessionContext.Current?.IsolationData?.Id;

                    SixnetException.ThrowIf(string.IsNullOrWhiteSpace(isolationDataId), "Not set isolation value");

                    globalFilter = globalFilter.Where(Criterion.Create(CriterionOperator.Equal, PropertyField.Create(isolationFieldName, globalFilterContext.ModelType), ConstantField.Create(isolationDataId.ConvertTo(isolationField.DataType))));
                }
            }

            return globalFilter;
        }

        #endregion

        #region Set global filter

        #region Set global filter

        /// <summary>
        /// Set global filter
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="originalQueryable">Origin queryable</param>
        /// <param name="operationType">Operation type</param>
        /// <returns></returns>
        internal static ISixnetQueryable SetGlobalFilter(Type entityType, ISixnetQueryable originalQueryable, DataOperationType operationType)
        {
            originalQueryable ??= Create();
            originalQueryable.SetModelType(entityType);

            //Global filter context
            var globalFilterContext = new GlobalFilterContext()
            {
                UsageSceneModelType = entityType,
                OperationType = operationType,
                Location = QueryableLocation.Top,
                ModelType = entityType,
                OriginalQueryable = originalQueryable
            };
            return SetQueryableGlobalFilter(globalFilterContext);
        }

        #endregion

        #region Set queryable global filter

        /// <summary>
        /// Set queryable global filter
        /// </summary>
        /// <param name="globalFilterContext">Global filter context</param>
        /// <returns></returns>
        static ISixnetQueryable SetQueryableGlobalFilter(GlobalFilterContext globalFilterContext)
        {
            ThrowHelper.ThrowArgNullIf(globalFilterContext?.OriginalQueryable is null, nameof(GlobalFilterContext.OriginalQueryable));
            var originalQueryable = globalFilterContext.OriginalQueryable;

            // group queryable not set global condition
            if (originalQueryable.IsGroupQueryable)
            {
                return originalQueryable;
            }

            #region condition

            if (!originalQueryable.Conditions.IsNullOrEmpty())
            {
                foreach (var condition in originalQueryable.Conditions)
                {
                    if (condition is ISixnetQueryable conditionQueryable && !conditionQueryable.IsGroupQueryable)
                    {
                        globalFilterContext.OriginalQueryable = conditionQueryable;
                        globalFilterContext.Location = QueryableLocation.Condition;
                        globalFilterContext.ModelType = conditionQueryable.GetModelType();
                        SetQueryableGlobalFilter(globalFilterContext);
                    }
                    if (condition is Criterion criterion)
                    {
                        if (criterion.Left is QueryableField leftQueryableField && leftQueryableField.Queryable != null)
                        {
                            globalFilterContext.OriginalQueryable = leftQueryableField.Queryable;
                            globalFilterContext.Location = QueryableLocation.Subquery;
                            globalFilterContext.ModelType = leftQueryableField.Queryable.GetModelType();
                            SetQueryableGlobalFilter(globalFilterContext);
                        }
                        if (criterion.Right is QueryableField rightQueryableField && rightQueryableField.Queryable != null)
                        {
                            globalFilterContext.OriginalQueryable = rightQueryableField.Queryable;
                            globalFilterContext.Location = QueryableLocation.Subquery;
                            globalFilterContext.ModelType = rightQueryableField.Queryable.GetModelType();
                            SetQueryableGlobalFilter(globalFilterContext);
                        }
                    }
                }
            }

            #endregion

            #region join

            if (!originalQueryable.Joins.IsNullOrEmpty())
            {
                foreach (var join in originalQueryable.Joins)
                {
                    if (join == null)
                    {
                        continue;
                    }
                    if (join.TargetQueryable != null)
                    {
                        globalFilterContext.OriginalQueryable = join.TargetQueryable;
                        globalFilterContext.Location = QueryableLocation.JoinTarget;
                        globalFilterContext.ModelType = join.TargetQueryable.GetModelType();
                        SetQueryableGlobalFilter(globalFilterContext);
                    }
                    if (join.Connection != null)
                    {
                        if (join.Connection is Criterion connectionCriterion)
                        {
                            if (connectionCriterion.Left is QueryableField leftQueryableField && leftQueryableField.Queryable != null)
                            {
                                globalFilterContext.OriginalQueryable = leftQueryableField.Queryable;
                                globalFilterContext.Location = QueryableLocation.JoinConnection;
                                globalFilterContext.ModelType = leftQueryableField.Queryable.GetModelType();
                                SetQueryableGlobalFilter(globalFilterContext);
                            }
                            if (connectionCriterion.Right is QueryableField rightQueryableField && rightQueryableField.Queryable != null)
                            {
                                globalFilterContext.OriginalQueryable = rightQueryableField.Queryable;
                                globalFilterContext.Location = QueryableLocation.JoinConnection;
                                globalFilterContext.ModelType = rightQueryableField.Queryable.GetModelType();
                                SetQueryableGlobalFilter(globalFilterContext);
                            }
                        }
                        if (join.Connection is ISixnetQueryable connectionQueryable)
                        {
                            globalFilterContext.OriginalQueryable = connectionQueryable;
                            globalFilterContext.Location = QueryableLocation.JoinConnection;
                            globalFilterContext.ModelType = connectionQueryable.GetModelType();
                            SetQueryableGlobalFilter(globalFilterContext);
                        }
                    }
                }
            }

            #endregion

            #region combine

            if (!originalQueryable.Combines.IsNullOrEmpty())
            {
                foreach (var combine in originalQueryable.Combines)
                {
                    if (combine?.TargetQueryable != null)
                    {
                        globalFilterContext.OriginalQueryable = combine.TargetQueryable;
                        globalFilterContext.Location = QueryableLocation.Combine;
                        globalFilterContext.ModelType = combine.TargetQueryable.GetModelType();
                        SetQueryableGlobalFilter(globalFilterContext);
                    }
                }
            }

            #endregion

            #region from

            if (originalQueryable.FromType == QueryableFromType.Queryable && originalQueryable.TargetQueryable != null)
            {
                globalFilterContext.OriginalQueryable = originalQueryable.TargetQueryable;
                globalFilterContext.Location = QueryableLocation.From;
                globalFilterContext.ModelType = originalQueryable.TargetQueryable.GetModelType();
                SetQueryableGlobalFilter(globalFilterContext);
            }

            #endregion

            #region root

            var globalCondition = GetGlobalFilter(globalFilterContext);
            if (globalCondition != null)
            {
                originalQueryable.Where(globalCondition);
            }

            #endregion

            return originalQueryable;
        }

        #endregion

        #endregion

        #region Inactive data filter

        /// <summary>
        /// Disable inactive data global filter
        /// </summary>
        public static void DisableInactiveDataGlobalFilter()
        {
            DisabledInactiveDataFilter = true;
        }

        #endregion

        #region Isolation data filter

        /// <summary>
        /// Disable isolation data global filter
        /// </summary>
        public static void DisableIsolationDataGlobalFilter()
        {
            DisabledIsolationDataFilter = true;
        }

        #endregion

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
            var entityConfig = EntityManager.GetEntityConfiguration(entityType);
            return entityConfig?.PredicateType ?? FuncType.MakeGenericType(entityType, BooleanType);
        }

        #endregion

        #region Func

        /// <summary>
        /// Max func
        /// </summary>
        /// <param name="value">Original value</param>
        /// <returns></returns>
        public static T Max<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Min func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Min<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Avg func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Avg<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Count func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Count<T>(T value)
        {
            return 0;
        }

        /// <summary>
        /// Sum func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Sum<T>(T value)
        {
            return value;
        }

        /// <summary>
        /// Json value func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T JsonValue<T>(object value, string path)
        {
            return default;
        }

        /// <summary>
        /// Json object func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T JsonObject<T>(object value, string path)
        {
            return default;
        }

        /// <summary>
        /// Is null
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static bool IsNull(object value)
        {
            return true;
        }

        /// <summary>
        /// Not null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool NotNull(object value)
        {
            return true;
        }

        #endregion

        #endregion
    }
}
