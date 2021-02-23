using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using EZNEW.Develop.Entity;
using EZNEW.Fault;
using EZNEW.Paging;
using EZNEW.Configuration;
using EZNEW.ExpressionUtil;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Query manager
    /// </summary>
    public static class QueryManager
    {
        #region Fields

        /// <summary>
        /// Add query item handlers
        /// </summary>
        internal static readonly Dictionary<Guid, Func<DefaultQuery, IQueryItem, QueryParameterOptions, IQueryItem>> AddQueryItemHandlers = null;

        /// <summary>
        /// Boolean type
        /// </summary>
        static readonly Type BooleanType = typeof(bool);

        /// <summary>
        /// Func<> type
        /// </summary>
        static readonly Type FuncType = typeof(Func<>);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a method to generate global condition
        /// </summary>
        private static readonly List<Func<GlobalConditionFilter, GlobalCondition>> GetGlobalConditionProxys = new List<Func<GlobalConditionFilter, GlobalCondition>>();

        /// <summary>
        /// Gets or sets a value to determine whether copy parameter IQuery object,default is true
        /// parameter IQuery:subquery,join item
        /// </summary>
        public static bool EnableCopyParameterQueryObject = true;

        #endregion

        #region Constructor

        static QueryManager()
        {
            AddQueryItemHandlers = new Dictionary<Guid, Func<DefaultQuery, IQueryItem, QueryParameterOptions, IQueryItem>>(2)
            {
                { typeof(Criteria).GUID,AddCriteriaQueryItemHandler},
                { typeof(DefaultQuery).GUID,AddQueryInfoQueryItemHandler}
            };
        }

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
        /// <param name="criteria">Condition expression</param>
        /// <returns>Return query object</returns>
        public static IQuery Create<T>(Expression<Func<T, bool>> criteria) where T : IQueryModel<T>
        {
            IQuery query = Create<T>();
            if (criteria != null)
            {
                query.And(criteria);
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
            if (datas == null || !datas.Any())
            {
                return originalQuery;
            }
            originalQuery = originalQuery ?? CreateByEntity<T>();
            var entityType = typeof(T);
            var keys = EntityManager.GetPrimaryKeys(entityType);
            if (keys.IsNullOrEmpty())
            {
                throw new EZNEWException(string.Format("Type:{0} isn't set primary keys", entityType.FullName));
            }
            var firstData = datas.ElementAt(0).GetValue(keys.ElementAt(0));
            var dataType = firstData.GetType();
            dynamic keyValueList = Activator.CreateInstance(typeof(List<>).MakeGenericType(dataType));
            var keyCount = keys.GetCount();
            foreach (T entity in datas)
            {
                if (keyCount == 1)
                {
                    keyValueList.Add(entity.GetValue(keys.ElementAt(0)));
                }
                else
                {
                    IQuery entityQuery = Create();
                    foreach (var key in keys)
                    {
                        entityQuery = AndExtensions.And(entityQuery, key, exclude ? CriteriaOperator.NotEqual : CriteriaOperator.Equal, entity.GetValue(key));
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
                var criteriaOperator = exclude ? CriteriaOperator.NotEqual : CriteriaOperator.Equal;
                originalQuery = AndExtensions.And(originalQuery, key, criteriaOperator, data.GetValue(key), null);
            }
            return originalQuery;
        }

        #endregion

        #region Add criteria handler

        /// <summary>
        /// Add criteria query item handler
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <param name="queryItem">Parameter query item</param>
        /// <param name="parameterQueryOption">Parameter query option</param>
        static IQueryItem AddCriteriaQueryItemHandler(DefaultQuery originalQuery, IQueryItem queryItem, QueryParameterOptions parameterQueryOption)
        {
            if (originalQuery == null || queryItem == null)
            {
                return null;
            }
            Criteria criteria = queryItem as Criteria;
            var queryValue = criteria.Value as IQuery;
            originalQuery.SetHasConverter(originalQuery.HasConverter || criteria.Converter != null);
            if (queryValue != null)
            {
                if (queryValue.GetEntityType() == null)
                {
                    throw new EZNEWException("the IQuery object used for the subquery must set the property EntityType");
                }
                queryValue = HandleParameterQueryBeforeUse(queryValue, parameterQueryOption);
                criteria.SetValue(queryValue);
                originalQuery.subqueryCollection.Add(queryValue);
                originalQuery.SetHasSubQuery(true);
                originalQuery.SetHasJoin(originalQuery.HasJoin || queryValue.HasJoin);
                originalQuery.SetHasRecurveCriteria(originalQuery.HasRecurveCriteria || queryValue.HasRecurveCriteria);
                originalQuery.SetHasConverter(originalQuery.HasConverter || queryValue.HasConverter);
            }
            else
            {
                bool equalCriterial = false;
                bool verifyValueNull = false;
                CriteriaOperator verifyValueNullOperator = CriteriaOperator.IsNull;
                switch (criteria.Operator)
                {
                    case CriteriaOperator.Equal:
                        equalCriterial = true;
                        verifyValueNull = true;
                        break;
                    case CriteriaOperator.NotEqual:
                        verifyValueNull = true;
                        verifyValueNullOperator = CriteriaOperator.NotNull;
                        break;
                    case CriteriaOperator.In:
                        equalCriterial = true;
                        break;
                }
                if (criteria.GetCriteriaRealValue() == null && verifyValueNull)
                {
                    equalCriterial = false;
                    criteria.Operator = verifyValueNullOperator;
                }
                if (equalCriterial)
                {
                    originalQuery.equalCriteriaCollection.Add(criteria);
                }
            }
            originalQuery.atomicConditionCount++;
            originalQuery.allConditionFieldNameCollection.Add(criteria.Name);
            return criteria;
        }

        /// <summary>
        /// Add queryInfo query item handler
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <param name="queryItem">Parameter query item</param>
        /// <param name="parameterQueryOption">Parameter query option</param>
        static IQueryItem AddQueryInfoQueryItemHandler(DefaultQuery originalQuery, IQueryItem queryItem, QueryParameterOptions parameterQueryOption)
        {
            if (originalQuery == null || queryItem == null)
            {
                return null;
            }
            DefaultQuery valueQuery = queryItem as DefaultQuery;
            valueQuery = HandleParameterQueryBeforeUse(valueQuery, parameterQueryOption);
            valueQuery.SetEntityType(originalQuery.entityType);
            originalQuery.SetHasSubQuery(originalQuery.HasSubquery || valueQuery.HasSubquery);
            originalQuery.SetHasJoin(originalQuery.HasJoin || valueQuery.HasJoin);
            originalQuery.SetHasRecurveCriteria(originalQuery.HasRecurveCriteria || valueQuery.HasRecurveCriteria);
            originalQuery.equalCriteriaCollection.AddRange(valueQuery.equalCriteriaCollection);
            originalQuery.atomicConditionCount += valueQuery.AtomicConditionCount;
            originalQuery.allConditionFieldNameCollection.AddRange(valueQuery.AllConditionFieldNames);
            originalQuery.alreadySetGlobalCondition |= valueQuery.alreadySetGlobalCondition;
            originalQuery.SetHasConverter(originalQuery.HasConverter || valueQuery.HasConverter);
            return valueQuery;
        }

        #endregion

        #region Global condition

        /// <summary>
        /// Configure global condition
        /// </summary>
        /// <param name="getGlobalConditionOperation">Get global condition operation</param>
        public static void ConfigureGlobalCondition(Func<GlobalConditionFilter, GlobalCondition> getGlobalConditionOperation)
        {
            if (getGlobalConditionOperation != null)
            {
                GetGlobalConditionProxys.Add(getGlobalConditionOperation);
            }
        }

        /// <summary>
        /// Get global condition
        /// </summary>
        /// <param name="conditionFilter">Condition filter</param>
        /// <returns>Return global condition</returns>
        internal static GlobalCondition GetGlobalCondition(GlobalConditionFilter conditionFilter)
        {
            if (conditionFilter == null)
            {
                throw new EZNEWException("GlobalConditionFilter is null");
            }
            if (conditionFilter.EntityType == null)
            {
                throw new EZNEWException("GlobalConditionFilter.EntityType is null");
            }
            if (conditionFilter.OriginalQuery == null)
            {
                conditionFilter.OriginalQuery = Create();
                conditionFilter.OriginalQuery.SetEntityType(conditionFilter.EntityType);
            }
            GlobalCondition globalCondition = null;
            if (!GetGlobalConditionProxys.IsNullOrEmpty() && conditionFilter.OriginalQuery.AllowSetGlobalCondition())
            {
                foreach (var globalConditionProxy in GetGlobalConditionProxys)
                {
                    var nowGlobalCondition = globalConditionProxy(conditionFilter);
                    if (nowGlobalCondition?.Value == null)
                    {
                        continue;
                    }
                    if (globalCondition == null)
                    {
                        globalCondition = nowGlobalCondition;
                    }
                    else
                    {
                        globalCondition.Value = globalCondition.Value.AddQueryItem(nowGlobalCondition.AppendMethod, nowGlobalCondition.Value);
                    }
                }
            }
            return globalCondition;
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
            var entityConfig = EntityManager.GetEntityConfiguration(entityType);
            return entityConfig?.PredicateType ?? FuncType.MakeGenericType(entityType, BooleanType);
        }

        #endregion

        #region Query model relation entity

        /// <summary>
        /// Configure query model relation entity type
        /// </summary>
        /// <param name="queryModelTypeGuid">Query model type guid</param>
        /// <param name="entityType">Relation entity type</param>
        public static void ConfigureQueryModelRelationEntity(Guid queryModelTypeGuid, Type entityType)
        {
            ConfigurationManager.QueryModel.ConfigureQueryModelRelationEntity(queryModelTypeGuid, entityType);
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
        /// <param name="queryModelType">Query model type</param>
        /// <returns>Return entity type</returns>
        public static Type GetQueryModelRelationEntityType(Type queryModelType)
        {
            return ConfigurationManager.QueryModel.GetQueryModelRelationEntityType(queryModelType);
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

        #endregion

        #region Handle query object before use

        /// <summary>
        /// Handle parameter query object before use
        /// </summary>
        /// <param name="parameterQuery">Parameter query</param>
        /// <param name="parameterQueryOption">Parameter query option</param>
        /// <returns>Return the newest query object</returns>
        internal static TQuery HandleParameterQueryBeforeUse<TQuery>(TQuery parameterQuery, QueryParameterOptions parameterQueryOption) where TQuery : IQuery
        {
            if (parameterQuery == null)
            {
                return parameterQuery;
            }
            if (EnableCopyParameterQueryObject)
            {
                parameterQuery = (TQuery)parameterQuery.Clone();
            }
            if (parameterQueryOption != null)
            {
                if (!string.IsNullOrWhiteSpace(parameterQueryOption.QueryFieldName))
                {
                    parameterQuery.ClearQueryFields();
                    parameterQuery.AddQueryFields(parameterQueryOption.QueryFieldName);
                }
            }
            return parameterQuery;
        }

        #endregion

        #region Clone

        /// <summary>
        /// Clone a IQueryItem
        /// </summary>
        /// <param name="originalQueryItem">Originnal query item</param>
        /// <returns></returns>
        public static IQueryItem Clone(IQueryItem originalQueryItem)
        {
            if (originalQueryItem is Criteria criteria)
            {
                return criteria.Clone();
            }
            if (originalQueryItem is IQuery query)
            {
                return query.Clone();
            }
            return null;
        }

        #endregion

        #endregion
    }
}
