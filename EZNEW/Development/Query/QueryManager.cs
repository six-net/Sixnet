using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EZNEW.Development.Entity;
using EZNEW.Exceptions;
using EZNEW.Paging;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Query manager
    /// </summary>
    public static class QueryManager
    {
        #region Constructor

        static QueryManager()
        {
            AddConditionHandlers = new Dictionary<Guid, Func<DefaultQuery, ICondition, ICondition>>(2)
            {
                { typeof(Criterion).GUID,AddCriterionHandler},
                { typeof(DefaultQuery).GUID,AddGroupQueryHandler}
            };
        }

        #endregion

        #region Fields

        /// <summary>
        /// Add condition handlers
        /// </summary>
        internal static readonly Dictionary<Guid, Func<DefaultQuery, ICondition, ICondition>> AddConditionHandlers = null;

        /// <summary>
        /// Boolean type
        /// </summary>
        static readonly Type BooleanType = typeof(bool);

        /// <summary>
        /// Func<> type
        /// </summary>
        static readonly Type FuncType = typeof(Func<>);

        /// <summary>
        /// Value:Entity type
        /// Key:Query model type guid
        /// </summary>
        static readonly Dictionary<Guid, Type> QueryModelRelationEntities = new Dictionary<Guid, Type>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a method to generate global condition
        /// </summary>
        private static readonly List<Func<GlobalConditionContext, IQuery>> GetGlobalConditionDelegates = new List<Func<GlobalConditionContext, IQuery>>();

        /// <summary>
        /// Indicates whether enable query object cloning,default is true
        /// </summary>
        public static bool EnableQueryCloning = true;

        /// <summary>
        /// Indicates whether filter obsolete data
        /// </summary>
        public static bool FilterObsoleteData { get; set; } = true;

        #endregion

        #region Methods

        #region Create 

        /// <summary>
        /// Create a new query instance
        /// </summary>
        /// <returns>Return query object</returns>
        public static IQuery Create()
        {
            return new DefaultQuery();
        }

        /// <summary>
        /// Create a new query instance
        /// </summary>
        /// <param name="filter">Paging filter</param>
        /// <returns>Return query object</returns>
        public static IQuery Create(PagingFilter filter)
        {
            var query = Create();
            query.SetPaging(filter);
            return query;
        }

        /// <summary>
        /// Create a new query instance
        /// </summary>
        /// <typeparam name="T">Query model</typeparam>
        /// <returns>Return query object</returns>
        public static IQuery Create<T>() where T : IQueryModel<T>
        {
            var query = Create();
            var entityType = GetQueryModelRelationEntityType<T>();
            if (entityType == null)
            {
                throw new EZNEWException(string.Format("query model:{0} didn't relate any entity", typeof(T).FullName));
            }
            query.SetEntityType(entityType);
            return query;
        }

        /// <summary>
        /// Create a new query instance
        /// </summary>
        /// <typeparam name="T">Query model</typeparam>
        /// <returns>Return query object</returns>
        public static IQuery Create<T>(PagingFilter filter) where T : IQueryModel<T>
        {
            var query = Create<T>();
            query.SetPaging(filter);
            if (filter != null)
            {
                query.QuerySize = filter.QuerySize;
            }
            return query;
        }

        /// <summary>
        /// Create a new query instance
        /// </summary>
        /// <typeparam name="T">Query model</typeparam>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return query object</returns>
        public static IQuery Create<T>(Expression<Func<T, bool>> conditionExpression) where T : IQueryModel<T>
        {
            IQuery query = Create<T>();
            if (conditionExpression != null)
            {
                query.And(conditionExpression);
            }
            return query;
        }

        /// <summary>
        /// Create a new query instance by entity
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <returns>Return query object</returns>
        internal static IQuery CreateByEntity<TEntity>()
        {
            var query = Create();
            query.SetEntityType(typeof(TEntity));
            return query;
        }

        #endregion

        #region Append entity identity condition

        /// <summary>
        /// Append entity identity condition to original IQuery object
        /// it will create new IQuery object if the original is null
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="originalQuery">Original query</param>
        /// <param name="exclude">Exclude</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery AppendEntityIdentityCondition<T>(IEnumerable<T> datas, IQuery originalQuery = null, bool exclude = false) where T : BaseEntity<T>, new()
        {
            return AppendEntityIdentityCore(typeof(T), datas, originalQuery, exclude);
        }

        /// <summary>
        /// Append entity identity condition to original IQuery object
        /// it will create new IQuery object if the original is null
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="originalQuery">Original query</param>
        /// <param name="exclude">Exclude</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery AppendEntityIdentityCondition<T>(T data, IQuery originalQuery = null, bool exclude = false) where T : BaseEntity<T>, new()
        {
            if (data == null)
            {
                return originalQuery;
            }
            originalQuery ??= CreateByEntity<T>();
            Type entityType = typeof(T);
            var keys = EntityManager.GetPrimaryKeys(entityType);
            if (keys.IsNullOrEmpty())
            {
                throw new EZNEWException(string.Format("Type:{0} is not set primary keys", entityType.FullName));
            }
            foreach (var key in keys)
            {
                var criteriaOperator = exclude ? CriterionOperator.NotEqual : CriterionOperator.Equal;
                originalQuery = ConnectionExtensions.And(originalQuery, key, criteriaOperator, data.GetValue(key), null);
            }
            return originalQuery;
        }

        /// <summary>
        /// Append entity identity condition to original IQuery object
        /// it will create new IQuery object if the original is null
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="datas">Datas</param>
        /// <param name="originalQuery">Original query</param>
        /// <param name="exclude">Exclude</param>
        /// <returns></returns>
        internal static IQuery AppendEntityIdentityCore(Type entityType, IEnumerable<object> datas, IQuery originalQuery = null, bool exclude = false)
        {
            if (datas == null || !datas.Any())
            {
                return originalQuery;
            }
            originalQuery ??= Create().SetEntityType(entityType);
            var keys = EntityManager.GetPrimaryKeys(entityType);
            if (keys.IsNullOrEmpty())
            {
                throw new EZNEWException(string.Format("Type:{0} isn't set primary keys", entityType.FullName));
            }
            if (datas.ElementAt(0) is not IEntity entityData)
            {
                throw new EZNEWException("Data must inherit from IEntity");
            }
            var firstData = entityData.GetValue(keys.ElementAt(0));
            var dataType = firstData.GetType();
            dynamic keyValueList = Activator.CreateInstance(typeof(List<>).MakeGenericType(dataType));
            var keyCount = keys.GetCount();
            foreach (var data in datas)
            {
                IEntity entity = data as IEntity;
                if (keyCount == 1)
                {
                    keyValueList.Add(entity.GetValue(keys.ElementAt(0)));
                }
                else
                {
                    IQuery entityQuery = Create();
                    foreach (var key in keys)
                    {
                        entityQuery = ConnectionExtensions.And(entityQuery, key, exclude ? CriterionOperator.NotEqual : CriterionOperator.Equal, entity.GetValue(key));
                    }
                    originalQuery.Or(entityQuery);
                }
            }
            if (keyCount == 1)
            {
                if (exclude)
                {
                    originalQuery = NotInExtensions.NotIn(originalQuery, keys.ElementAt(0), keyValueList);
                }
                else
                {
                    originalQuery = InExtensions.In(originalQuery, keys.ElementAt(0), keyValueList);
                }
            }
            return originalQuery;
        }

        #endregion

        #region Add condition handler

        /// <summary>
        /// Add criterion handler
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <param name="condition">Condition</param>
        static ICondition AddCriterionHandler(DefaultQuery originalQuery, ICondition condition)
        {
            if (originalQuery == null || condition == null)
            {
                return null;
            }
            Criterion criterion = condition as Criterion;
            var valueQuery = criterion.Value as IQuery;
            originalQuery.SetHasFieldConverter(originalQuery.HasFieldConverter || criterion.HasFieldConversion());
            if (valueQuery != null)
            {
                if (valueQuery.GetEntityType() == null)
                {
                    throw new EZNEWException("the IQuery object used for the subquery must set EntityType");
                }
                valueQuery = HandleParameterQueryBeforeUse(valueQuery);
                if (!string.IsNullOrWhiteSpace(criterion.Options?.SubqueryValueFieldName))
                {
                    valueQuery.ClearQueryFields();
                    valueQuery.AddQueryFields(criterion.Options.SubqueryValueFieldName);
                }
                criterion.SetValue(valueQuery);
                originalQuery.AddSubquery(valueQuery);
            }
            else
            {
                bool verifyValueNull = false;
                CriterionOperator verifyValueNullOperator = CriterionOperator.IsNull;
                switch (criterion.Operator)
                {
                    case CriterionOperator.Equal:
                        verifyValueNull = true;
                        break;
                    case CriterionOperator.NotEqual:
                        verifyValueNull = true;
                        verifyValueNullOperator = CriterionOperator.NotNull;
                        break;
                }
                if (criterion.Value == null && verifyValueNull)
                {
                    criterion.Operator = verifyValueNullOperator;
                }
            }
            originalQuery.criteria.Add(criterion);
            //originalQuery.atomicConditionCount++;
            //originalQuery.allConditionFieldNameCollection.Add(criterion.Name);
            return criterion;
        }

        /// <summary>
        /// Add group query handler
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <param name="condition">Condition</param>
        static ICondition AddGroupQueryHandler(DefaultQuery originalQuery, ICondition condition)
        {
            if (originalQuery != null && condition is DefaultQuery groupQuery)
            {
                groupQuery = HandleParameterQueryBeforeUse(groupQuery);
                originalQuery.AddGroupQuery(groupQuery);
                return groupQuery;
            }
            return null;
        }

        #endregion

        #region Global condition

        #region Configure global condition

        /// <summary>
        /// Configure global condition
        /// </summary>
        /// <param name="getGlobalConditionDelegate">Get global condition delegate</param>
        public static void ConfigureGlobalCondition(Func<GlobalConditionContext, IQuery> getGlobalConditionDelegate)
        {
            if (getGlobalConditionDelegate != null)
            {
                GetGlobalConditionDelegates.Add(getGlobalConditionDelegate);
            }
        }

        #endregion

        #region Get global condition

        /// <summary>
        /// Get global condition
        /// </summary>
        /// <param name="conditionContext">Global condition context</param>
        /// <returns>Return global condition</returns>
        internal static IQuery GetGlobalCondition(GlobalConditionContext conditionContext)
        {
            if (conditionContext == null)
            {
                throw new EZNEWException($"{nameof(conditionContext)} is null");
            }
            if (conditionContext.EntityType == null)
            {
                throw new EZNEWException($"{nameof(GlobalConditionContext.EntityType)} is null");
            }
            if (conditionContext.OriginalQuery == null)
            {
                conditionContext.OriginalQuery = Create();
                conditionContext.OriginalQuery.SetEntityType(conditionContext.EntityType);
            }
            IQuery globalCondition = null;
            if (!GetGlobalConditionDelegates.IsNullOrEmpty() && conditionContext.OriginalQuery.AllowSetGlobalCondition())
            {
                foreach (var globalConditionDelegate in GetGlobalConditionDelegates)
                {
                    var globalConditionEntry = globalConditionDelegate?.Invoke(conditionContext);
                    if (globalConditionEntry == null)
                    {
                        continue;
                    }
                    if (globalCondition == null)
                    {
                        globalCondition = globalConditionEntry;
                    }
                    else
                    {
                        globalCondition = globalCondition.AddCondition(globalConditionEntry);
                    }
                }
            }

            //filter obsolete data
            if (FilterObsoleteData)
            {
                var obsoleteField = EntityManager.GetObsoleteField(conditionContext.EntityType);
                if (!string.IsNullOrWhiteSpace(obsoleteField) && !conditionContext.OriginalQuery.IncludeObsoleteData)
                {
                    globalCondition ??= Create().SetEntityType(conditionContext.EntityType);
                    globalCondition = globalCondition.Equal(obsoleteField, false);
                }
            }

            return globalCondition;
        }

        #endregion

        #region Set global condition

        #region Set global condition

        /// <summary>
        /// Set global condition
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="originalQuery">Origin query</param>
        /// <returns>Return the newest query object</returns>
        internal static IQuery SetGlobalCondition(Type entityType, IQuery originalQuery, QueryUsageScene usageScene)
        {
            originalQuery ??= Create();
            originalQuery.SetEntityType(entityType);

            //Global condition context
            GlobalConditionContext globalConditionContext = new GlobalConditionContext()
            {
                UsageSceneEntityType = entityType,
                UsageScene = usageScene,
                Location = QueryLocation.Top,
                EntityType = entityType,
                OriginalQuery = originalQuery
            };
            return SetQueryObjectGlobalCondition(globalConditionContext);
        }

        #endregion

        #region Set query global condition

        static IQuery SetQueryObjectGlobalCondition(GlobalConditionContext globalConditionContext)
        {
            if (globalConditionContext?.OriginalQuery is null)
            {
                throw new ArgumentNullException(nameof(GlobalConditionContext.OriginalQuery));
            }
            if (globalConditionContext?.EntityType is null)
            {
                throw new ArgumentNullException(nameof(GlobalConditionContext.EntityType));
            }

            var originalQuery = globalConditionContext.OriginalQuery;

            //global condition
            var globalCondition = GetGlobalCondition(globalConditionContext);
            if (globalCondition != null && !globalCondition.NoneCondition)
            {
                originalQuery.AddCondition(globalCondition);
            }

            //subquery
            if (!originalQuery.Subqueries.IsNullOrEmpty())
            {
                foreach (var subquery in originalQuery.Subqueries)
                {
                    if (subquery != null)
                    {
                        globalConditionContext.OriginalQuery = subquery;
                        globalConditionContext.Location = QueryLocation.Subuery;
                        globalConditionContext.EntityType = subquery.GetEntityType();
                        SetQueryObjectGlobalCondition(globalConditionContext);
                    }
                }
            }
            //join
            if (!originalQuery.Joins.IsNullOrEmpty())
            {
                foreach (var join in originalQuery.Joins)
                {
                    if (join?.JoinQuery != null)
                    {
                        globalConditionContext.OriginalQuery = join.JoinQuery;
                        globalConditionContext.Location = QueryLocation.Join;
                        globalConditionContext.EntityType = join.JoinQuery.GetEntityType();
                        SetQueryObjectGlobalCondition(globalConditionContext);
                    }
                }
            }
            //combine
            if (!originalQuery.Combines.IsNullOrEmpty())
            {
                foreach (var combine in originalQuery.Combines)
                {
                    if (combine?.Query != null)
                    {
                        globalConditionContext.OriginalQuery = combine.Query;
                        globalConditionContext.Location = QueryLocation.Combine;
                        globalConditionContext.EntityType = combine.Query.GetEntityType();
                        SetQueryObjectGlobalCondition(globalConditionContext);
                    }
                }
            }
            return originalQuery;
        }

        #endregion

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

        #region Query model relation entity

        /// <summary>
        /// Configure query model relation entity type
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void ConfigureQueryModelRelationEntity<TQueryModel, TEntity>() where TQueryModel : IQueryModel<TQueryModel>
        {
            ConfigureQueryModelRelationEntity(typeof(TQueryModel), typeof(TEntity));
        }

        /// <summary>
        /// Get query model relation entity type
        /// </summary>
        /// <param name="queryModelAssemblyQualifiedName">Query model type assembly qualified name</param>
        /// <returns>Return the entity type</returns>
        public static Type GetQueryModelRelationEntityType(string queryModelAssemblyQualifiedName)
        {
            if (string.IsNullOrWhiteSpace(queryModelAssemblyQualifiedName))
            {
                return null;
            }
            var type = Type.GetType(queryModelAssemblyQualifiedName);
            return GetQueryModelRelationEntityType(type);
        }

        /// <summary>
        /// Get query model relation entity type
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <returns>Return entity type</returns>
        public static Type GetQueryModelRelationEntityType<TQueryModel>() where TQueryModel : IQueryModel<TQueryModel>
        {
            return GetQueryModelRelationEntityType(typeof(TQueryModel));
        }

        /// <summary>
        /// Configure query model relation entity type
        /// </summary>
        /// <param name="queryModelTypeGuid">Query model type guid</param>
        /// <param name="entityType">Relation entity type</param>
        public static void ConfigureQueryModelRelationEntity(Guid queryModelTypeGuid, Type entityType)
        {
            if (entityType == null)
            {
                return;
            }
            QueryModelRelationEntities[queryModelTypeGuid] = entityType;
        }

        /// <summary>
        /// Configure query model relation entity type
        /// </summary>
        /// <param name="queryModelType">Query model type</param>
        /// <param name="entityType">Relation entity type</param>
        public static void ConfigureQueryModelRelationEntity(Type queryModelType, Type entityType)
        {
            if (queryModelType == null || entityType == null)
            {
                return;
            }
            ConfigureQueryModelRelationEntity(queryModelType.GUID, entityType);
        }

        /// <summary>
        /// Configure query model relation entity type
        /// </summary>
        /// <param name="queryModelType">Query model type</param>
        public static void ConfigureQueryModelRelationEntity(Type queryModelType)
        {
            if (queryModelType == null)
            {
                return;
            }
            if (QueryModelRelationEntities.ContainsKey(queryModelType.GUID))
            {
                return;
            }
            //var attributes = queryModelType.GetCustomAttributes(typeof(QueryEntityAttribute), true);
            //if (attributes.IsNullOrEmpty())
            //{
            //    return;
            //}
            QueryEntityAttribute configAttribute = queryModelType.GetCustomAttribute<QueryEntityAttribute>(false);
            if (configAttribute == null)
            {
                return;
            }
            var relevanceType = configAttribute.RelevanceType;
            ConfigureQueryModelRelationEntity(queryModelType, relevanceType);
        }

        /// <summary>
        /// Get query model relation entity type
        /// </summary>
        /// <param name="queryModelType">Query model type</param>
        /// <returns>Return entity type</returns>
        public static Type GetQueryModelRelationEntityType(Type queryModelType)
        {
            if (queryModelType == null)
            {
                return null;
            }
            if (EntityManager.EntityConfigurations.ContainsKey(queryModelType.GUID))
            {
                return queryModelType;
            }
            QueryModelRelationEntities.TryGetValue(queryModelType.GUID, out Type entityType);
            return entityType;
        }

        #endregion

        #region Handle query object before use

        /// <summary>
        /// Handle parameter query object before use
        /// </summary>
        /// <param name="parameterQuery">Parameter query</param>
        /// <param name="parameterQueryOption">Parameter query option</param>
        /// <returns>Return the newest query object</returns>
        internal static TQuery HandleParameterQueryBeforeUse<TQuery>(TQuery parameterQuery) where TQuery : IQuery
        {
            if (parameterQuery == null)
            {
                return parameterQuery;
            }
            if (EnableQueryCloning)
            {
                parameterQuery = (TQuery)parameterQuery.Clone();
            }
            return parameterQuery;
        }

        #endregion

        #endregion
    }
}
