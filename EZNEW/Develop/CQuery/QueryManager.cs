using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using EZNEW.Develop.Entity;
using EZNEW.Fault;
using EZNEW.Paging;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Query manager
    /// </summary>
    public static class QueryManager
    {
        #region Fields

        /// <summary>
        /// Lambda method info
        /// </summary>
        internal static MethodInfo LambdaMethod = null;

        /// <summary>
        /// String index method info
        /// </summary>
        internal static MethodInfo StringIndexOfMethod = null;

        /// <summary>
        /// String end with method info
        /// </summary>
        internal static MethodInfo EndWithMethod = null;

        /// <summary>
        /// Collection contains method info
        /// </summary>
        internal static MethodInfo CollectionContainsMethod = null;

        /// <summary>
        /// Collection to list method info
        /// </summary>
        internal static MethodInfo CollectionToListMethod = null;

        /// <summary>
        /// Add query item handlers
        /// </summary>
        internal static readonly Dictionary<Guid, Func<DefaultQuery, IQueryItem, QueryParameterOption, IQueryItem>> AddQueryItemHandlers = null;

        /// <summary>
        /// Boolean type
        /// </summary>
        static readonly Type BooleanType = typeof(bool);

        /// <summary>
        /// Func<> type
        /// </summary>
        static readonly Type FuncType = typeof(Func<>);

        /// <summary>
        /// Query model entity
        /// key:query model type guid
        /// </summary>
        static readonly ConcurrentDictionary<Guid, Type> QueryModelEntityRelationDictionary = new ConcurrentDictionary<Guid, Type>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a method to generate global condition
        /// </summary>
        static Func<GlobalConditionFilter, GlobalCondition> GetGlobalConditionProxy { get; set; }

        /// <summary>
        /// Gets or sets a value to determine whether copy parameter IQuery object,default is true
        /// parameter IQuery:subquery,join item
        /// </summary>
        public static bool EnableCopyParameterQueryObject = true;

        #endregion

        #region Constructor

        static QueryManager()
        {
            var baseExpressMethods = typeof(Expression).GetMethods(BindingFlags.Public | BindingFlags.Static);
            LambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
            StringIndexOfMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "IndexOf" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
            EndWithMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "EndsWith" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
            CollectionContainsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(c => c.Name == "Contains" && c.GetParameters().Length == 2);
            CollectionToListMethod = typeof(Enumerable).GetMethods().FirstOrDefault(c => c.Name == "ToList" && c.GetParameters().Length == 1);
            AddQueryItemHandlers = new Dictionary<Guid, Func<DefaultQuery, IQueryItem, QueryParameterOption, IQueryItem>>(2)
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
        public static IQuery Create<T>() where T : QueryModel<T>
        {
            QueryModel<T>.Init();
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
        public static IQuery Create<T>(PagingFilter filter) where T : QueryModel<T>
        {
            var query = Create<T>();
            query.SetPaging(filter);
            return query;
        }

        /// <summary>
        /// Create a new query instance
        /// </summary>
        /// <typeparam name="T">Query model</typeparam>
        /// <param name="criteria">Condition expression</param>
        /// <returns>Return query object</returns>
        public static IQuery Create<T>(Expression<Func<T, bool>> criteria) where T : QueryModel<T>
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
                throw new EZNEWException(string.Format("type:{0} isn't set primary keys", entityType.FullName));
            }
            var firstData = datas.ElementAt(0).GetPropertyValue(keys.ElementAt(0));
            var dataType = firstData.GetType();
            dynamic keyValueList = Activator.CreateInstance(typeof(List<>).MakeGenericType(dataType));
            foreach (T entity in datas)
            {
                if (keys.Count == 1)
                {
                    keyValueList.Add(entity.GetPropertyValue(keys.ElementAt(0)));
                }
                else
                {
                    IQuery entityQuery = Create();
                    foreach (var key in keys)
                    {
                        entityQuery.And(key, exclude ? CriteriaOperator.NotEqual : CriteriaOperator.Equal, entity.GetPropertyValue(key));
                    }
                    originalQuery.Or(entityQuery);
                }
            }
            if (keys.Count == 1)
            {
                if (exclude)
                {
                    originalQuery.NotIn(keys.ElementAt(0), keyValueList);
                }
                else
                {
                    originalQuery.In(keys.ElementAt(0), keyValueList);
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
            originalQuery = originalQuery ?? CreateByEntity<T>();
            Type entityType = typeof(T);
            var keys = EntityManager.GetPrimaryKeys(entityType);
            if (keys.IsNullOrEmpty())
            {
                throw new EZNEWException(string.Format("type:{0} is not set primary keys", entityType.FullName));
            }
            foreach (var key in keys)
            {
                originalQuery.And(key, exclude ? CriteriaOperator.NotEqual : CriteriaOperator.Equal, data.GetPropertyValue(key));
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
        static IQueryItem AddCriteriaQueryItemHandler(DefaultQuery originalQuery, IQueryItem queryItem, QueryParameterOption parameterQueryOption)
        {
            if (originalQuery == null || queryItem == null)
            {
                return null;
            }
            Criteria criteria = queryItem as Criteria;
            var queryValue = criteria.Value as IQuery;
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
        static IQueryItem AddQueryInfoQueryItemHandler(DefaultQuery originalQuery, IQueryItem queryItem, QueryParameterOption parameterQueryOption)
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
            GetGlobalConditionProxy = getGlobalConditionOperation;
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
            if (GetGlobalConditionProxy != null && conditionFilter.OriginalQuery.AllowSetGlobalCondition())
            {
                globalCondition = GetGlobalConditionProxy(conditionFilter);
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
        /// Set query model relation entity type
        /// </summary>
        /// <param name="typeGuid">Query model type guid</param>
        /// <param name="entityType">Relation entity type</param>
        public static void SetQueryModelRelatioEntity(Guid typeGuid, Type entityType)
        {
            if (entityType == null)
            {
                return;
            }
            QueryModelEntityRelationDictionary[typeGuid] = entityType;
        }

        /// <summary>
        /// Set query model relation entity type
        /// </summary>
        /// <param name="queryModelType">Query model type</param>
        /// <param name="entityType">Relation entity type</param>
        public static void SetQueryModelRelatioEntity(Type queryModelType, Type entityType)
        {
            if (queryModelType == null || entityType == null)
            {
                return;
            }
            SetQueryModelRelatioEntity(queryModelType.GUID, entityType);
        }

        /// <summary>
        /// Set query model relation entity type
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public static void SetQueryModelRelatioEntity<TQueryModel, TEntity>() where TQueryModel : QueryModel<TQueryModel>
        {
            SetQueryModelRelatioEntity(typeof(TQueryModel), typeof(TEntity));
        }

        /// <summary>
        /// Config query model relation entity type
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        internal static void ConfigureQueryModelRelationEntity<TQueryModel>() where TQueryModel : QueryModel<TQueryModel>
        {
            SetQueryModelRelationEntity(typeof(TQueryModel));
        }

        /// <summary>
        /// Config query model relation entity type
        /// </summary>
        /// <param name="queryModelType">Query model type</param>
        public static void SetQueryModelRelationEntity(Type queryModelType)
        {
            if (queryModelType == null)
            {
                return;
            }
            if (QueryModelEntityRelationDictionary.ContainsKey(queryModelType.GUID))
            {
                return;
            }
            var attributes = queryModelType.GetCustomAttributes(typeof(QueryEntityAttribute), true);
            if (attributes.IsNullOrEmpty())
            {
                return;
            }
            var configAttribute = attributes[0] as QueryEntityAttribute;
            if (configAttribute == null)
            {
                return;
            }
            var relevanceType = configAttribute.RelevanceType;
            SetQueryModelRelatioEntity(queryModelType, relevanceType);
            EntityManager.ConfigureEntity(relevanceType);
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
            if (queryModelType == null)
            {
                return null;
            }
            QueryModelEntityRelationDictionary.TryGetValue(queryModelType.GUID, out Type entityType);
            if (entityType == null)
            {
                SetQueryModelRelationEntity(queryModelType);
                QueryModelEntityRelationDictionary.TryGetValue(queryModelType.GUID, out entityType);
            }
            return entityType;
        }

        /// <summary>
        /// Get query model relation entity type
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <returns>Return entity type</returns>
        public static Type GetQueryModelRelationEntityType<TQueryModel>() where TQueryModel : QueryModel<TQueryModel>
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
        internal static TQuery HandleParameterQueryBeforeUse<TQuery>(TQuery parameterQuery, QueryParameterOption parameterQueryOption) where TQuery : IQuery
        {
            if (parameterQuery == null)
            {
                return parameterQuery;
            }
            if (EnableCopyParameterQueryObject)
            {
                parameterQuery = (TQuery)parameterQuery.DeepCopy();
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

        #endregion
    }
}
