using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Framework.Extension;
using EZNEW.Develop.Entity;
using EZNEW.Framework.Fault;
using System.Reflection;
using System.Collections.Concurrent;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// query factory
    /// </summary>
    public static class QueryFactory
    {
        #region fields

        internal static MethodInfo LambdaMethod = null;
        internal static MethodInfo StringIndexOfMethod = null;
        internal static MethodInfo EndWithMethod = null;
        internal static MethodInfo CollectionContainsMethod = null;
        internal static MethodInfo CollectionToListMethod = null;
        internal static Dictionary<Guid, Action<QueryInfo, IQueryItem>> AddQueryItemHandlers = null;
        static Type booleanType = typeof(bool);
        static Type funcType = typeof(Func<>);
        /// <summary>
        /// query model entity
        /// key:query model type guid
        /// </summary>
        static ConcurrentDictionary<Guid, Type> queryModelEntityRelations = new ConcurrentDictionary<Guid, Type>();

        #endregion

        #region propertys

        /// <summary>
        /// global query filter
        /// </summary>
        public static Func<GlobalConditionFilter, GlobalConditionFilterResult> GetGlobalCondition { get; set; } 

        #endregion

        #region constructor

        static QueryFactory()
        {
            var baseExpressMethods = typeof(Expression).GetMethods(BindingFlags.Public | BindingFlags.Static);
            LambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
            StringIndexOfMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "IndexOf" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
            EndWithMethod = typeof(string).GetMethods().FirstOrDefault(c => c.Name == "EndsWith" && c.GetParameters().Length == 1 && c.GetParameters()[0].ParameterType.FullName == typeof(string).FullName);
            CollectionContainsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(c => c.Name == "Contains" && c.GetParameters().Length == 2);
            CollectionToListMethod = typeof(Enumerable).GetMethods().FirstOrDefault(c => c.Name == "ToList" && c.GetParameters().Length == 1);
            AddQueryItemHandlers = new Dictionary<Guid, Action<QueryInfo, IQueryItem>>()
            {
                { typeof(Criteria).GUID,new Action<QueryInfo,IQueryItem>(AddCriteriaQueryItemHandler)},
                { typeof(QueryInfo).GUID,new Action<QueryInfo,IQueryItem>(AddQueryInfoQueryItemHandler)}
            };
        }

        #endregion

        #region methods

        #region create 

        /// <summary>
        /// create a new query instance
        /// </summary>
        /// <returns>IQuery object</returns>
        public static IQuery Create()
        {
            return new QueryInfo();
        }

        /// <summary>
        /// create a new query instance
        /// </summary>
        /// <param name="filter">pagingfilter</param>
        /// <returns>IQuery object</returns>
        public static IQuery Create(PagingFilter filter)
        {
            var query = Create();
            query.SetPaging(filter);
            return query;
        }

        /// <summary>
        /// create a new query instance
        /// </summary>
        /// <typeparam name="T">query model</typeparam>
        /// <returns>IQuery object</returns>
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
        /// create a new query instance
        /// </summary>
        /// <typeparam name="T">query model</typeparam>
        /// <returns>IQuery object</returns>
        public static IQuery Create<T>(PagingFilter filter) where T : QueryModel<T>
        {
            var query = Create<T>();
            query.SetPaging(filter);
            return query;
        }

        /// <summary>
        /// create a new query instance
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="criteria">condition expression</param>
        /// <returns>IQuery object</returns>
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
        /// create a new query instance by entity
        /// </summary>
        /// <typeparam name="ET"></typeparam>
        /// <returns></returns>
        static IQuery CreateByEntity<ET>()
        {
            var query = Create();
            query.SetEntityType(typeof(ET));
            return query;
        }

        #endregion

        #region append condition

        /// <summary>
        /// append entity identity condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas">datas</param>
        /// <param name="originalQuery">original query</param>
        /// <param name="exclude">exclude</param>
        /// <returns></returns>
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
        /// append entity identity condition
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="originalQuery"></param>
        /// <param name="exclude"></param>
        /// <returns></returns>
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

        #region handler

        /// <summary>
        /// add criteria query item handler
        /// </summary>
        /// <param name="queryItem"></param>
        static void AddCriteriaQueryItemHandler(QueryInfo query, IQueryItem queryItem)
        {
            Criteria criteria = queryItem as Criteria;
            var queryValue = criteria.Value as IQuery;
            if (queryValue != null)
            {
                if (queryValue.GetEntityType() == null)
                {
                    throw new EZNEWException("the IQuery object used for the subquery must set the property EntityType");
                }
                query.Subqueries.Add(queryValue);
                query.SetHasSubQuery(true);
                query.SetHasJoin(query.HasJoin || queryValue.HasJoin);
                query.SetHasRecurveCriteria(query.HasRecurveCriteria || queryValue.HasRecurveCriteria);
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
                    query.equalCriteriaList.Add(criteria);
                }
            }
            query.atomicConditionCount++;
            query.allConditionFieldNames.Add(criteria.Name);
        }

        /// <summary>
        /// add queryInfo query item handler
        /// </summary>
        /// <param name="queryItem"></param>
        static void AddQueryInfoQueryItemHandler(QueryInfo query, IQueryItem queryItem)
        {
            QueryInfo valueQuery = queryItem as QueryInfo;
            valueQuery.SetEntityType(query.entityType);
            query.SetHasSubQuery(query.HasSubQuery || valueQuery.HasSubQuery);
            query.SetHasJoin(query.HasJoin || valueQuery.HasJoin);
            query.SetHasRecurveCriteria(query.HasRecurveCriteria || valueQuery.HasRecurveCriteria);
            query.equalCriteriaList.AddRange(valueQuery.equalCriteriaList);
            query.atomicConditionCount += valueQuery.AtomicConditionCount;
            query.allConditionFieldNames.AddRange(valueQuery.AllConditionFieldNames);
            query.alreadySetGlobalCondition |= valueQuery.alreadySetGlobalCondition;
        }

        #endregion

        #region get predicate type

        /// <summary>
        /// get predicate type
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns></returns>
        internal static Type GetEntityPredicateType(Type entityType)
        {
            if (entityType == null)
            {
                return null;
            }
            var entityConfig = EntityManager.GetEntityConfig(entityType);
            return entityConfig?.PredicateType ?? funcType.MakeGenericType(entityType, booleanType);
        }

        #endregion

        #region set query model relation entity

        /// <summary>
        /// set query model relation entity type
        /// </summary>
        /// <param name="typeGuid">query model type guid</param>
        /// <param name="entityType">relation type</param>
        public static void SetQueryModelRelatioEntity(Guid typeGuid, Type entityType)
        {
            if (entityType == null)
            {
                return;
            }
            queryModelEntityRelations[typeGuid] = entityType;
        }

        /// <summary>
        /// set query model relation entity type
        /// </summary>
        /// <param name="queryModelType">query model type</param>
        /// <param name="entityType">relation entity type</param>
        public static void SetQueryModelRelatioEntity(Type queryModelType, Type entityType)
        {
            if (queryModelType == null || entityType == null)
            {
                return;
            }
            SetQueryModelRelatioEntity(queryModelType.GUID, entityType);
        }

        /// <summary>
        /// set query model relation entity type
        /// </summary>
        /// <typeparam name="QT">query model type</typeparam>
        /// <typeparam name="RT">entity type</typeparam>
        public static void SetQueryModelRelatioEntity<QT, RT>() where QT : QueryModel<QT>
        {
            SetQueryModelRelatioEntity(typeof(QT), typeof(RT));
        }

        /// <summary>
        /// config query model relation entity type
        /// </summary>
        /// <typeparam name="QT">query model type</typeparam>
        internal static void ConfigQueryModelRelationEntity<QT>()
        {
            SetQueryModelRelationEntity(typeof(QT));
        }

        /// <summary>
        /// config query model relation entity type
        /// </summary>
        /// <param name="queryModelType">query model type</param>
        public static void SetQueryModelRelationEntity(Type queryModelType)
        {
            if (queryModelType == null)
            {
                return;
            }
            if (queryModelEntityRelations.ContainsKey(queryModelType.GUID))
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
            EntityManager.ConfigEntity(relevanceType);
        }

        #endregion

        #region get query model relation entity

        /// <summary>
        /// get query model relation entity type
        /// </summary>
        /// <param name="queryModelAssemblyQualifiedName">query model type assembly qualified name</param>
        /// <returns></returns>
        public static Type GetQueryModelRelationEntityType(string queryModelAssemblyQualifiedName)
        {
            if (queryModelAssemblyQualifiedName.IsNullOrEmpty())
            {
                return null;
            }
            var type = Type.GetType(queryModelAssemblyQualifiedName);
            return GetQueryModelRelationEntityType(type);
        }

        /// <summary>
        /// get query model relation entity type
        /// </summary>
        /// <param name="queryModelType">query model type</param>
        /// <returns></returns>
        public static Type GetQueryModelRelationEntityType(Type queryModelType)
        {
            if (queryModelType == null)
            {
                return null;
            }
            queryModelEntityRelations.TryGetValue(queryModelType.GUID, out Type entityType);
            if (entityType == null)
            {
                SetQueryModelRelationEntity(queryModelType);
                queryModelEntityRelations.TryGetValue(queryModelType.GUID, out entityType);
            }
            return entityType;
        }

        /// <summary>
        /// get query model relation entity type
        /// </summary>
        /// <typeparam name="T">query model type</typeparam>
        /// <returns></returns>
        public static Type GetQueryModelRelationEntityType<T>()
        {
            return GetQueryModelRelationEntityType(typeof(T));
        }

        #endregion

        #region append global condition

        /// <summary>
        /// global condition filter
        /// </summary>
        /// <param name="conditionFilter">condition filter</param>
        /// <returns>filter result</returns>
        public static GlobalConditionFilterResult GlobalConditionFilter(GlobalConditionFilter conditionFilter)
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
                conditionFilter.OriginalQuery = QueryFactory.Create();
                conditionFilter.OriginalQuery.SetEntityType(conditionFilter.EntityType);
            }
            GlobalConditionFilterResult globalConditionResult = null;
            if (GetGlobalCondition != null && conditionFilter.OriginalQuery.AllowSetGlobalCondition())
            {
                globalConditionResult = GetGlobalCondition(conditionFilter);
            }
            return globalConditionResult;
        }

        #endregion

        #endregion
    }
}
