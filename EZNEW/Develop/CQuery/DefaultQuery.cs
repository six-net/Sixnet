using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Linq.Expressions;
using EZNEW.ExpressionUtil;
using EZNEW.Reflection;
using EZNEW.Develop.CQuery.CriteriaConverter;
using EZNEW.Fault;
using EZNEW.Develop.DataAccess;
using EZNEW.Paging;
using EZNEW.Develop.Entity;
using EZNEW.Cache;
using EZNEW.Data.Cache;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// IQuery default implement
    /// </summary>
    [Serializable]
    internal class DefaultQuery : IQuery
    {
        #region Fields

        /// <summary>
        /// All criterias
        /// </summary>
        internal List<Tuple<QueryOperator, IQueryItem>> criteriaCollection = new List<Tuple<QueryOperator, IQueryItem>>();

        /// <summary>
        /// Orders
        /// </summary>
        internal List<SortCriteria> sortCollection = new List<SortCriteria>();

        /// <summary>
        /// Query fields
        /// </summary>
        internal List<string> queryFieldCollection = new List<string>();

        /// <summary>
        /// Not query fields
        /// </summary>
        internal List<string> notQueryFieldCollection = new List<string>();

        /// <summary>
        /// Load data property
        /// </summary>
        internal Dictionary<string, bool> loadProperties = new Dictionary<string, bool>();

        /// <summary>
        /// Equal criterias
        /// </summary>
        internal List<Criteria> equalCriteriaCollection = new List<Criteria>();

        /// <summary>
        /// Join items
        /// </summary>
        internal List<JoinItem> joinItemCollection = new List<JoinItem>();

        /// <summary>
        /// Combine items
        /// </summary>
        internal List<CombineItem> combineItemCollection = new List<CombineItem>();

        /// <summary>
        /// Subqueries
        /// </summary>
        internal List<IQuery> subqueryCollection = new List<IQuery>();

        /// <summary>
        /// Join sort
        /// </summary>
        internal int joinSort = 0;

        /// <summary>
        /// Already set global condition
        /// </summary>
        internal bool alreadySetGlobalCondition = false;

        /// <summary>
        /// Has subquery
        /// </summary>
        internal bool hasSubquery = false;

        /// <summary>
        /// Has join
        /// </summary>
        internal bool hasJoin = false;

        /// <summary>
        /// Has recurve
        /// </summary>
        internal bool hasRecurveCriteria = false;

        /// <summary>
        /// Has combine
        /// </summary>
        internal bool hasCombine = false;

        /// <summary>
        /// Has converter
        /// </summary>
        internal bool hasConverter = false;

        /// <summary>
        /// Atomic condition count
        /// </summary>
        internal int atomicConditionCount = 0;

        /// <summary>
        /// All condition field names
        /// </summary>
        internal List<string> allConditionFieldNameCollection = new List<string>();

        /// <summary>
        /// Actually query fields
        /// </summary>
        Tuple<bool, IEnumerable<string>> actuallyQueryFields = null;

        /// <summary>
        /// Entity type assembly qualified name
        /// </summary>
        internal string entityTypeAssemblyQualifiedName = string.Empty;
        [NonSerialized]
        internal Dictionary<Guid, dynamic> queryExpressionCaches = new Dictionary<Guid, dynamic>();
        [NonSerialized]
        internal CancellationToken? cancellationToken = default;
        [NonSerialized]
        internal Type entityType = null;

        #endregion

        #region Constructor

        internal DefaultQuery() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets all criterias or other IQuery items
        /// </summary>
        public IEnumerable<Tuple<QueryOperator, IQueryItem>> Criterias => criteriaCollection;

        /// <summary>
        /// Gets the order items
        /// </summary>
        public IEnumerable<SortCriteria> Orders => sortCollection;

        /// <summary>
        /// Gets the specific query fields(it's priority greater than NoQueryFields)
        /// </summary>
        public IEnumerable<string> QueryFields => queryFieldCollection;

        /// <summary>
        /// Gets the specific not query fields(it's priority less than QueryFields)
        /// </summary>
        public IEnumerable<string> NotQueryFields => notQueryFieldCollection;

        /// <summary>
        /// Gets the paging info
        /// </summary>
        public PagingFilter PagingInfo { get; private set; } = null;

        /// <summary>
        /// Gets the query text
        /// </summary>
        public string QueryText { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the query text parameter
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> QueryTextParameters { get; internal set; } = null;

        /// <summary>
        /// Gets the query command type
        /// </summary>
        public QueryCommandType QueryType { get; internal set; } = QueryCommandType.QueryObject;

        /// <summary>
        /// Gets or sets query data size
        /// </summary>
        public int QuerySize { get; set; } = 0;

        /// <summary>
        /// Gets all of data properties allow to lazy load
        /// </summary>
        public IEnumerable<KeyValuePair<string, bool>> LoadPropertys => loadProperties;

        /// <summary>
        /// Gets whether has subquery
        /// </summary>
        public bool HasSubquery => hasSubquery;

        /// <summary>
        /// Gets whether has recurve criteria
        /// </summary>
        public bool HasRecurveCriteria => hasRecurveCriteria;

        /// <summary>
        /// Gets whether has join item
        /// </summary>
        public bool HasJoin => hasJoin;

        /// <summary>
        /// Gets whether has combine items
        /// </summary>
        public bool HasCombine => hasCombine;

        /// <summary>
        /// Gets whether has criteria converter
        /// </summary>
        public bool HasConverter => hasConverter;

        /// <summary>
        /// Gets whether is a complex query
        /// Include subquery,recurve criteria,join item
        /// </summary>
        public bool IsComplexQuery => GetIsComplexQuery();

        /// <summary>
        /// Gets the recurve criteria
        /// </summary>
        public RecurveCriteria RecurveCriteria { get; private set; }

        /// <summary>
        /// Gets or sets whether must return value for success
        /// </summary>
        public bool MustReturnValueOnSuccess { get; set; }

        /// <summary>
        /// Gets whether the query object is obsolete
        /// </summary>
        public bool IsObsolete { get; internal set; } = false;

        /// <summary>
        /// Gets the join items
        /// </summary>
        public IEnumerable<JoinItem> JoinItems => joinItemCollection;

        /// <summary>
        /// Gets the combine items
        /// </summary>
        public IEnumerable<CombineItem> CombineItems => combineItemCollection;

        /// <summary>
        /// Gets whether is a none condition object
        /// </summary>
        public bool NoneCondition => criteriaCollection.IsNullOrEmpty() && JoinItems.IsNullOrEmpty() && combineItemCollection.IsNullOrEmpty();

        /// <summary>
        /// Gets the atomic condition count
        /// </summary>
        public int AtomicConditionCount => atomicConditionCount;

        /// <summary>
        /// Gets all condition field names
        /// </summary>
        public IEnumerable<string> AllConditionFieldNames => allConditionFieldNameCollection;

        /// <summary>
        /// Gets all subqueries
        /// </summary>
        public IEnumerable<IQuery> Subqueries => subqueryCollection;

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }

        #endregion

        #region Functions

        #region Condition

        /// <summary>
        /// Add a criteria
        /// </summary>
        /// <param name="queryOperator">Connect operator</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="criteriaOperator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Converter</param>
        /// <param name="queryOption">query parameter option</param>
        public IQuery AddCriteria(QueryOperator queryOperator, string fieldName, CriteriaOperator criteriaOperator, dynamic value, ICriteriaConverter converter = null, QueryParameterOptions queryOption = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return this;
            }
            Criteria newCriteria = Criteria.CreateNewCriteria(fieldName, criteriaOperator, value);
            newCriteria.Converter = converter;
            return AddQueryItem(queryOperator, newCriteria, queryOption);
        }

        /// <summary>
        /// Add a IQueryItem
        /// </summary>
        /// <param name="queryOperator">Connect operator</param>
        /// <param name="queryItem">query item</param>
        /// <param name="queryOption">query parameter option</param>
        public IQuery AddQueryItem(QueryOperator queryOperator, IQueryItem queryItem, QueryParameterOptions queryOption = null)
        {
            #region invoke handler

            var queryItemTypeId = queryItem?.GetType().GUID ?? Guid.Empty;
            Func<DefaultQuery, IQueryItem, QueryParameterOptions, IQueryItem> handler = null;
            QueryManager.AddQueryItemHandlers?.TryGetValue(queryItemTypeId, out handler);
            if (handler != null)
            {
                queryItem = handler(this, queryItem, queryOption);
            }

            #endregion

            if (queryItem != null)
            {
                //clear data
                queryExpressionCaches?.Clear();
                criteriaCollection.Add(new Tuple<QueryOperator, IQueryItem>(queryOperator, queryItem));
            }
            return this;
        }

        #endregion

        #region Sort

        #region Clear sort

        /// <summary>
        /// Clear sort condition
        /// </summary>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery ClearOrder()
        {
            sortCollection.Clear();
            return this;
        }

        #endregion

        #region Add order

        /// <summary>
        /// Add order
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Sort by desc</param>
        /// <param name="converter">Field converter</param>
        public IQuery AddOrder(string fieldName, bool desc = false, ICriteriaConverter converter = null)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                sortCollection.Add(new SortCriteria()
                {
                    Name = fieldName,
                    Desc = desc,
                    Converter = converter
                });
            }
            return this;
        }

        #endregion

        #endregion

        #region Fields

        /// <summary>
        /// Add special fields need to query
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery AddQueryFields(params string[] fields)
        {
            if (fields?.Length > 0)
            {
                queryFieldCollection.AddRange(fields);
                ClearFieldsCache();
            }
            return this;
        }

        /// <summary>
        /// Add special fields need to query
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="fieldExpressions">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery AddQueryFields<TQueryModel>(params Expression<Func<TQueryModel, dynamic>>[] fieldExpressions) where TQueryModel : IQueryModel<TQueryModel>
        {
            if (!fieldExpressions.IsNullOrEmpty())
            {
                foreach (var expression in fieldExpressions)
                {
                    AddQueryFields(ExpressionHelper.GetExpressionPropertyName(expression.Body));
                }
                ClearFieldsCache();
            }
            return this;
        }

        /// <summary>
        /// Clear query fields
        /// </summary>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery ClearQueryFields()
        {
            queryFieldCollection?.Clear();
            ClearFieldsCache();
            return this;
        }

        /// <summary>
        /// Add special fields that don't query
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery AddNotQueryFields(params string[] fields)
        {
            if (fields?.Length > 0)
            {
                notQueryFieldCollection.AddRange(fields);
                ClearFieldsCache();
            }
            return this;
        }

        /// <summary>
        /// Add special fields that don't query
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="fieldExpressions">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery AddNotQueryFields<TQueryModel>(params Expression<Func<TQueryModel, dynamic>>[] fieldExpressions) where TQueryModel : IQueryModel<TQueryModel>
        {
            if (!fieldExpressions.IsNullOrEmpty())
            {
                foreach (var expression in fieldExpressions)
                {
                    AddNotQueryFields(ExpressionHelper.GetExpressionPropertyName(expression.Body));
                }
                ClearFieldsCache();
            }
            return this;
        }

        /// <summary>
        /// Clear not query fields
        /// </summary>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery ClearNotQueryFields()
        {
            notQueryFieldCollection?.Clear();
            ClearFieldsCache();
            return this;
        }

        /// <summary>
        /// Get actually query fields
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="forceMustFields">Whether return the must query fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IEnumerable<string> GetActuallyQueryFields<TEntity>(bool forceMustFields)
        {
            return GetActuallyQueryFields(typeof(TEntity), forceMustFields);
        }

        /// <summary>
        /// Get actually query fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="forceMustFields">Whether return the must query fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IEnumerable<string> GetActuallyQueryFields(Type entityType, bool forceMustFields)
        {
            return GetActuallyQueryFieldsWithSign(entityType, forceMustFields).Item2;
        }

        /// <summary>
        /// Get actually query fields
        /// Item1: whether return entity full query fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="forceMustFields">Whether return the must query fields</param>
        /// <returns>Return actually query fields</returns>
        public Tuple<bool, IEnumerable<string>> GetActuallyQueryFieldsWithSign(Type entityType, bool forceMustFields)
        {
            if (actuallyQueryFields == null)
            {
                bool fullQuery = true;
                var allowQueryFields = QueryFields;
                if (!allowQueryFields.IsNullOrEmpty())
                {
                    fullQuery = false;
                    var mustQueryFields = EntityManager.GetMustQueryFields(entityType);
                    if (forceMustFields && !mustQueryFields.IsNullOrEmpty())
                    {
                        allowQueryFields = mustQueryFields.Union(allowQueryFields);
                    }
                    return actuallyQueryFields = new Tuple<bool, IEnumerable<string>>(fullQuery, allowQueryFields);
                }
                IEnumerable<string> allFields = EntityManager.GetQueryFields(entityType);
                var notQueryFields = NotQueryFields;
                if (!notQueryFields.IsNullOrEmpty())
                {
                    fullQuery = false;
                    allFields = allFields.Except(notQueryFields);
                    var mustQueryFields = EntityManager.GetMustQueryFields(entityType);
                    if (forceMustFields && !mustQueryFields.IsNullOrEmpty())
                    {
                        allFields = mustQueryFields.Union(allFields);
                    }
                }
                return actuallyQueryFields = new Tuple<bool, IEnumerable<string>>(fullQuery, allFields);
            }
            return actuallyQueryFields;
        }

        /// <summary>
        /// Clear fields cache
        /// </summary>
        void ClearFieldsCache()
        {
            actuallyQueryFields = null;
        }

        #endregion

        #region QueryText

        /// <summary>
        /// Set query text
        /// </summary>
        /// <param name="queryText">Query text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery SetQueryText(string queryText, object parameters = null)
        {
            QueryText = queryText;
            QueryTextParameters = parameters?.ObjectToDcitionary();
            QueryType = QueryCommandType.Text;
            return this;
        }

        #endregion

        #region Load Propertys

        /// <summary>
        /// Set load data propertys
        /// </summary>
        /// <param name="properties">Allow load data properties</param>
        public void SetLoadProperty(Dictionary<string, bool> properties)
        {
            if (properties == null)
            {
                return;
            }
            foreach (var property in properties)
            {
                loadProperties[property.Key] = property.Value;
            }
        }

        /// <summary>
        /// Set load data propertys
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <param name="allowLoad">allow load</param>
        /// <param name="properties">properties</param>
        public void SetLoadProperty<T>(bool allowLoad, params Expression<Func<T, dynamic>>[] properties)
        {
            if (properties == null)
            {
                return;
            }
            Dictionary<string, bool> loadPropertyValues = new Dictionary<string, bool>(properties.Length);
            foreach (var property in properties)
            {
                loadPropertyValues.Add(ExpressionHelper.GetExpressionPropertyName(property.Body), allowLoad);
            }
            SetLoadProperty(loadPropertyValues);
        }

        /// <summary>
        /// Property is allow load data
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        /// <returns>allow load data</returns>
        public bool AllowLoad(string propertyName)
        {
            return !string.IsNullOrWhiteSpace(propertyName) && loadProperties != null && loadProperties.ContainsKey(propertyName) && loadProperties[propertyName];
        }

        /// <summary>
        /// Property is allow load data
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="property">Property</param>
        /// <returns>Return whether property allow load data</returns>
        public bool AllowLoad<TEntity>(Expression<Func<TEntity, dynamic>> property)
        {
            if (property == null)
            {
                return false;
            }
            string propertyName = ExpressionHelper.GetExpressionPropertyName(property);
            return AllowLoad(propertyName);
        }

        #endregion

        #region Get Special Keys Equal Values

        /// <summary>
        /// Get special keys equal values
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>Return key and values</returns>
        public Dictionary<string, List<dynamic>> GetKeysEqualValue(IEnumerable<string> keys)
        {
            if (QueryType == QueryCommandType.Text || keys.IsNullOrEmpty() || equalCriteriaCollection.IsNullOrEmpty())
            {
                return new Dictionary<string, List<dynamic>>();
            }
            var equalCriteriaDict = equalCriteriaCollection.GroupBy(c => c.Name).ToDictionary(c => c.Key, c => c.ToList());
            var values = new Dictionary<string, List<dynamic>>(equalCriteriaDict.Count);
            foreach (var key in keys)
            {
                equalCriteriaDict.TryGetValue(key, out var keyCriterias);
                if (keyCriterias.IsNullOrEmpty())
                {
                    continue;
                }
                List<dynamic> criteriaValues = new List<dynamic>();
                foreach (var criteria in keyCriterias)
                {
                    var criteriaValue = criteria.GetCriteriaRealValue();
                    if (criteriaValue == null)
                    {
                        continue;
                    }
                    if (criteria.Operator == CriteriaOperator.In)
                    {
                        foreach (var cvalue in criteriaValue)
                        {
                            criteriaValues.Add(cvalue);
                        }
                    }
                    else
                    {
                        criteriaValues.Add(criteriaValue);
                    }
                }
                if (criteriaValues.IsNullOrEmpty())
                {
                    continue;
                }
                values[key] = criteriaValues;
            }
            return values;
        }

        #endregion

        #region Get Expression

        /// <summary>
        /// Get query expression
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Return query expression</returns>
        public Func<T, bool> GetQueryExpression<T>()
        {
            Type modelType = typeof(T);
            queryExpressionCaches ??= new Dictionary<Guid, dynamic>();
            if (queryExpressionCaches.ContainsKey(modelType.GUID))
            {
                return queryExpressionCaches[modelType.GUID];
            }
            if (IsComplexQuery)
            {
                Func<T, bool> falseFunc = (data) => false;
                queryExpressionCaches.Add(modelType.GUID, falseFunc);
                return falseFunc;
            }
            if (criteriaCollection.IsNullOrEmpty())
            {
                Func<T, bool> trueFunc = (data) => true;
                queryExpressionCaches.Add(modelType.GUID, trueFunc);
                return trueFunc;
            }
            Type funcType = QueryManager.GetEntityPredicateType(modelType);//typeof(Func<,>).MakeGenericType(modelType, typeof(bool));
            ParameterExpression parExp = Expression.Parameter(modelType);//parameter model type
            Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
            parameterArray.SetValue(parExp, 0);
            Expression conditionExpression = null;
            foreach (var queryItem in criteriaCollection)
            {
                Expression childExpression = GenerateExpression(parExp, queryItem.Item2);
                if (childExpression == null)
                {
                    continue;
                }
                if (conditionExpression == null)
                {
                    conditionExpression = childExpression;
                    continue;
                }
                if (queryItem.Item1 == QueryOperator.AND)
                {
                    conditionExpression = Expression.AndAlso(conditionExpression, childExpression);
                }
                else
                {
                    conditionExpression = Expression.OrElse(conditionExpression, childExpression);
                }
            }
            if (conditionExpression == null)
            {
                return null;
            }
            var genericLambdaMethod = ReflectionManager.Expression.LambdaMethod.MakeGenericMethod(funcType);
            var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
            {
                conditionExpression,parameterArray
            });
            Func<T, bool> func = ((Expression<Func<T, bool>>)lambdaExpression).Compile();
            queryExpressionCaches.Add(modelType.GUID, func);
            return func;
        }

        /// <summary>
        /// Generate expressionn
        /// </summary>
        /// <param name="parameter">Parameter expression</param>
        /// <param name="queryItem">Query item</param>
        /// <returns>Return query expression</returns>
        Expression GenerateExpression(Expression parameter, IQueryItem queryItem)
        {
            if (queryItem is Criteria)
            {
                return GenerateSingleExpression(parameter, queryItem as Criteria);
            }
            IQuery query = queryItem as IQuery;
            if (query?.Criterias.IsNullOrEmpty() ?? true)
            {
                return null;
            }
            var criteriasCount = query.Criterias.Count();
            var firstCriteria = query.Criterias.First();
            if (criteriasCount == 1 && firstCriteria.Item2 is Criteria)
            {
                return GenerateSingleExpression(parameter, firstCriteria.Item2 as Criteria);
            }
            Expression conditionExpression = null;
            foreach (var criteriaItem in query.Criterias)
            {
                var childExpression = GenerateExpression(parameter, criteriaItem.Item2);
                if (conditionExpression == null)
                {
                    conditionExpression = childExpression;
                    continue;
                }
                if (criteriaItem.Item1 == QueryOperator.AND)
                {
                    conditionExpression = Expression.AndAlso(conditionExpression, childExpression);
                }
                else
                {
                    conditionExpression = Expression.OrElse(conditionExpression, childExpression);
                }
            }
            return conditionExpression;
        }

        /// <summary>
        /// Generate a single expression
        /// </summary>
        /// <param name="parameter">Parameter expression</param>
        /// <param name="criteria">Criteria</param>
        /// <returns>Return query expression</returns>
        Expression GenerateSingleExpression(Expression parameter, Criteria criteria)
        {
            object criteriaValue = criteria.GetCriteriaRealValue();
            Expression valueExpression = Expression.Constant(criteriaValue, criteriaValue?.GetType() ?? typeof(object));
            Expression property = Expression.PropertyOrField(parameter, criteria.Name);
            switch (criteria.Operator)
            {
                case CriteriaOperator.Equal:
                case CriteriaOperator.IsNull:
                    if (criteriaValue == null && !property.Type.AllowNull())
                    {
                        property = Expression.Constant(false, typeof(bool));
                    }
                    else
                    {
                        property = Expression.Equal(property, valueExpression);
                    }
                    break;
                case CriteriaOperator.NotEqual:
                case CriteriaOperator.NotNull:
                    if (criteriaValue == null && !property.Type.AllowNull())
                    {
                        property = Expression.Constant(true, typeof(bool));
                    }
                    else
                    {
                        property = Expression.NotEqual(property, valueExpression);
                    }
                    break;
                case CriteriaOperator.GreaterThan:
                    property = Expression.GreaterThan(property, valueExpression);
                    break;
                case CriteriaOperator.GreaterThanOrEqual:
                    property = Expression.GreaterThanOrEqual(property, valueExpression);
                    break;
                case CriteriaOperator.LessThan:
                    property = Expression.LessThan(property, valueExpression);
                    break;
                case CriteriaOperator.LessThanOrEqual:
                    property = Expression.LessThanOrEqual(property, valueExpression);
                    break;
                case CriteriaOperator.BeginLike:
                    Expression beginLikeExpression = Expression.Call(property, ReflectionManager.String.StringIndexOfMethod, valueExpression);
                    property = Expression.Equal(beginLikeExpression, Expression.Constant(0));
                    break;
                case CriteriaOperator.Like:
                    Expression likeExpression = Expression.Call(property, ReflectionManager.String.StringIndexOfMethod, valueExpression);
                    property = Expression.GreaterThanOrEqual(likeExpression, Expression.Constant(0));
                    break;
                case CriteriaOperator.EndLike:
                    property = Expression.Call(property, ReflectionManager.String.EndWithMethod, valueExpression);
                    break;
                case CriteriaOperator.In:
                    Type valueType = criteriaValue.GetType();
                    if (valueType != null && valueType.GenericTypeArguments != null && valueType.GenericTypeArguments.Length > 0)
                    {
                        valueType = valueType.GenericTypeArguments[valueType.GenericTypeArguments.Length - 1];
                    }
                    else if (valueType.IsArray)
                    {
                        Array arrayValue = criteriaValue as Array;
                        if (arrayValue != null && arrayValue.Length > 0)
                        {
                            valueType = arrayValue.GetValue(0).GetType();
                        }
                        else
                        {
                            valueType = typeof(object);
                        }
                    }
                    else
                    {
                        valueType = typeof(object);
                    }
                    var inMethod = ReflectionManager.Collections.GetCollectionContainsMethod(valueType);
                    property = Expression.Call(inMethod, valueExpression, property);
                    break;
                case CriteriaOperator.NotIn:
                    Type notInValueType = criteriaValue.GetType();
                    if (notInValueType != null && notInValueType.GenericTypeArguments != null)
                    {
                        notInValueType = notInValueType.GenericTypeArguments[0];
                    }
                    else
                    {
                        notInValueType = typeof(object);
                    }
                    var notInMethod = ReflectionManager.Collections.GetCollectionContainsMethod(notInValueType);
                    property = Expression.Not(Expression.Call(notInMethod, valueExpression, property));
                    break;
                default:
                    property = null;
                    break;
            }
            if (property == null)
            {
                return null;
            }
            return property;
        }

        #endregion

        #region Order Datas

        /// <summary>
        /// Order datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns>Return the sorted data set</returns>
        public IEnumerable<T> Sort<T>(IEnumerable<T> datas)
        {
            if (sortCollection.IsNullOrEmpty() || datas.IsNullOrEmpty())
            {
                return datas;
            }
            IOrderedEnumerable<T> orderedDatas = null;
            foreach (var orderItem in sortCollection)
            {
                var orderFun = ExpressionHelper.GetPropertyOrFieldFunction<T>(orderItem.Name);
                if (orderFun == null)
                {
                    continue;
                }
                if (orderedDatas == null)
                {
                    orderedDatas = orderItem.Desc ? datas.OrderByDescending(orderFun) : datas.OrderBy(orderFun);
                }
                else
                {
                    orderedDatas = orderItem.Desc ? orderedDatas.ThenByDescending(orderFun) : orderedDatas.ThenBy(orderFun);
                }
            }
            return orderedDatas ?? datas;
        }

        #endregion

        #region Recurve

        /// <summary>
        /// Set recurve criteria
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="relationKey">Relation key</param>
        /// <param name="direction">Recurve direction</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery SetRecurve(string key, string relationKey, RecurveDirection direction = RecurveDirection.Down)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(relationKey))
            {
                throw new EZNEWException($"{nameof(key)} or {nameof(relationKey)} is null or empty");
            }
            if (key == relationKey)
            {
                throw new EZNEWException($"{nameof(key)} and {nameof(relationKey)} can not be the same value");
            }
            RecurveCriteria = new RecurveCriteria()
            {
                Key = key,
                RelationKey = relationKey,
                Direction = direction
            };
            hasRecurveCriteria = true;
            return this;
        }

        /// <summary>
        /// Set recurve criteria
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="key">Key</param>
        /// <param name="relationKey">Relation key</param>
        /// <param name="direction">Recurve direction</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery SetRecurve<TQueryModel>(Expression<Func<TQueryModel, dynamic>> key, Expression<Func<TQueryModel, dynamic>> relationKey, RecurveDirection direction = RecurveDirection.Down) where TQueryModel : IQueryModel<TQueryModel>
        {
            return SetRecurve(ExpressionHelper.GetExpressionPropertyName(key), ExpressionHelper.GetExpressionPropertyName(relationKey), direction);
        }

        #endregion

        #region Obsolete

        /// <summary>
        /// Bbsolete query
        /// </summary>
        public void Obsolete()
        {
            IsObsolete = true;
        }

        /// <summary>
        /// Cancel obsolete
        /// </summary>
        public void Activate()
        {
            IsObsolete = false;
        }

        #endregion

        #region Clone

        /// <summary>
        /// Light clone a IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        public IQuery LightClone()
        {
            var newQuery = CloneValueMember();
            newQuery.criteriaCollection = new List<Tuple<QueryOperator, IQueryItem>>(criteriaCollection);
            newQuery.sortCollection = new List<SortCriteria>(sortCollection);
            newQuery.queryFieldCollection = new List<string>(queryFieldCollection);
            newQuery.notQueryFieldCollection = new List<string>(notQueryFieldCollection);
            newQuery.loadProperties = new Dictionary<string, bool>(loadProperties);
            newQuery.equalCriteriaCollection = new List<Criteria>(equalCriteriaCollection);
            newQuery.joinItemCollection = new List<JoinItem>(joinItemCollection);
            newQuery.combineItemCollection = new List<CombineItem>(combineItemCollection);
            newQuery.subqueryCollection = new List<IQuery>(subqueryCollection);
            newQuery.allConditionFieldNameCollection = new List<string>(allConditionFieldNameCollection);
            newQuery.actuallyQueryFields = actuallyQueryFields == null ? null : new Tuple<bool, IEnumerable<string>>(actuallyQueryFields.Item1, actuallyQueryFields.Item2);
            newQuery.PagingInfo = PagingInfo == null ? null : new PagingFilter() { Page = PagingInfo.Page, PageSize = PagingInfo.PageSize, QuerySize = PagingInfo.QuerySize };
            newQuery.queryExpressionCaches = new Dictionary<Guid, dynamic>(queryExpressionCaches);
            newQuery.QueryTextParameters = QueryTextParameters?.ToDictionary(c => c.Key, c => c.Value);
            return newQuery;
        }

        /// <summary>
        /// Clone a IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        public IQuery Clone()
        {
            var newQuery = CloneValueMember();
            newQuery.criteriaCollection = criteriaCollection.Select(c => new Tuple<QueryOperator, IQueryItem>(c.Item1, QueryManager.Clone(c.Item2))).ToList();
            newQuery.sortCollection = sortCollection.Select(c => c?.Clone()).ToList();
            newQuery.queryFieldCollection = new List<string>(queryFieldCollection);
            newQuery.notQueryFieldCollection = new List<string>(notQueryFieldCollection);
            newQuery.loadProperties = new Dictionary<string, bool>(loadProperties);
            newQuery.equalCriteriaCollection = equalCriteriaCollection.Select(c => c.Clone()).ToList();
            newQuery.joinItemCollection = joinItemCollection.Select(c => c.Clone()).ToList();
            newQuery.combineItemCollection = combineItemCollection.Select(c => c.Clone()).ToList();
            newQuery.subqueryCollection = subqueryCollection.Select(c => c.Clone()).ToList();
            newQuery.allConditionFieldNameCollection = new List<string>(allConditionFieldNameCollection);
            newQuery.actuallyQueryFields = actuallyQueryFields == null ? null : new Tuple<bool, IEnumerable<string>>(actuallyQueryFields.Item1, actuallyQueryFields.Item2?.Select(c => c));
            newQuery.PagingInfo = PagingInfo == null ? null : new PagingFilter() { Page = PagingInfo.Page, PageSize = PagingInfo.PageSize, QuerySize = PagingInfo.QuerySize };
            newQuery.queryExpressionCaches = new Dictionary<Guid, dynamic>(queryExpressionCaches);
            newQuery.QueryTextParameters = QueryTextParameters?.ToDictionary(c => c.Key, c => c.Value);
            return newQuery;
        }

        /// <summary>
        /// Clone value member
        /// </summary>
        /// <returns></returns>
        DefaultQuery CloneValueMember()
        {
            var newQuery = new DefaultQuery
            {
                joinSort = joinSort,
                alreadySetGlobalCondition = alreadySetGlobalCondition,
                hasSubquery = hasSubquery,
                hasJoin = hasJoin,
                hasRecurveCriteria = hasRecurveCriteria,
                hasCombine = hasCombine,
                hasConverter = hasConverter,
                atomicConditionCount = atomicConditionCount,
                entityTypeAssemblyQualifiedName = entityTypeAssemblyQualifiedName,
                cancellationToken = cancellationToken,
                entityType = entityType,
                QueryText = QueryText,
                QueryType = QueryType,
                QuerySize = QuerySize,
                RecurveCriteria = RecurveCriteria,
                MustReturnValueOnSuccess = MustReturnValueOnSuccess,
                IsObsolete = IsObsolete,
                IsolationLevel = IsolationLevel
            };
            return newQuery;
        }

        #endregion

        #region Join

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <param name="joinFields">Join fields</param>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Join(Dictionary<string, string> joinFields, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery)
        {
            if (joinQuery.GetEntityType() == null)
            {
                throw new EZNEWException("the IQuery object used for the join operation must set the property EntityType");
            }
            return Join(new JoinItem()
            {
                JoinType = joinType,
                Operator = joinOperator,
                JoinFields = joinFields,
                JoinQuery = joinQuery,
                Sort = joinSort++
            });
        }

        /// <summary>
        /// Add a join item
        /// </summary>
        /// <param name="joinItem">Join item</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Join(JoinItem joinItem)
        {
            if (joinItem?.JoinQuery == null)
            {
                return this;
            }
            joinItem.JoinQuery = QueryManager.HandleParameterQueryBeforeUse(joinItem.JoinQuery, null);
            if (joinItem.ExtraQuery != null)
            {
                joinItem.ExtraQuery = QueryManager.HandleParameterQueryBeforeUse(joinItem.ExtraQuery, null);
            }
            joinItemCollection.Add(joinItem);
            hasJoin = true;
            hasConverter |= joinItem.JoinQuery.HasConverter;
            return this;
        }

        #endregion

        #region Combine

        /// <summary>
        /// Add a combine item
        /// </summary>
        /// <param name="combineItem">Combine item</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Combine(CombineItem combineItem)
        {
            if (combineItem == null)
            {
                return this;
            }
            combineItem.CombineQuery = QueryManager.HandleParameterQueryBeforeUse(combineItem.CombineQuery, null);
            combineItemCollection.Add(combineItem);
            hasCombine = true;
            return this;
        }

        #endregion

        #region GlobalCondition

        /// <summary>
        /// Set global condition
        /// </summary>
        /// <param name="globalCondition">Global condition</param>
        /// <param name="queryOperator">Query operator</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery SetGlobalCondition(IQuery globalCondition, QueryOperator queryOperator)
        {
            if (alreadySetGlobalCondition || globalCondition == null || globalCondition.NoneCondition)
            {
                return this;
            }
            alreadySetGlobalCondition = true;
            return AddQueryItem(queryOperator, globalCondition);
        }

        /// <summary>
        /// Whether allow set global condition
        /// </summary>
        /// <returns>allow set global condition</returns>
        public bool AllowSetGlobalCondition()
        {
            return !alreadySetGlobalCondition;
        }

        #endregion

        #region Reset

        /// <summary>
        /// Reset IQuery object
        /// </summary>
        public void Reset()
        {
            alreadySetGlobalCondition = false;
        }

        #endregion

        #region CancellationToken

        /// <summary>
        /// Set cancellation token
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery SetCancellationToken(CancellationToken? cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            return this;
        }

        /// <summary>
        /// Get cancellation token
        /// </summary>
        /// <returns>Cancellation token</returns>
        public CancellationToken? GetCancellationToken()
        {
            return cancellationToken;
        }

        #endregion

        #region Entity Type

        /// <summary>
        /// Get the entity type associated with IQuery
        /// </summary>
        /// <returns>Return the entity type</returns>
        public Type GetEntityType()
        {
            if (entityType == null && !string.IsNullOrWhiteSpace(entityTypeAssemblyQualifiedName))
            {
                entityType = Type.GetType(entityTypeAssemblyQualifiedName);
            }
            return entityType;
        }

        /// <summary>
        /// Set the entity type associated with IQuery
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns>Return the entity type</returns>
        public IQuery SetEntityType(Type entityType)
        {
            if (entityType != null)
            {
                this.entityType = entityType;
                entityTypeAssemblyQualifiedName = entityType.AssemblyQualifiedName;
            }
            return this;
        }

        #endregion

        #region Paging

        /// <summary>
        /// Set paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        public IQuery SetPaging(PagingFilter pagingFilter)
        {
            if (pagingFilter == null)
            {
                return this;
            }
            if (PagingInfo == null)
            {
                PagingInfo = new PagingFilter();
            }
            PagingInfo.Page = pagingFilter.Page;
            PagingInfo.PageSize = pagingFilter.PageSize;
            return this;
        }

        #endregion

        #endregion

        #region Util

        /// <summary>
        /// Set has subquery
        /// </summary>
        /// <param name="hasSubquery">Has subquery</param>
        internal void SetHasSubQuery(bool hasSubquery)
        {
            this.hasSubquery = hasSubquery;
        }

        /// <summary>
        /// Set has join
        /// </summary>
        /// <param name="hasJoin">Has join</param>
        internal void SetHasJoin(bool hasJoin)
        {
            this.hasJoin = hasJoin;
        }

        /// <summary>
        /// Set has recurve criteria
        /// </summary>
        /// <param name="hasRecurveCriteria">Has recurve criteria</param>
        internal void SetHasRecurveCriteria(bool hasRecurveCriteria)
        {
            this.hasRecurveCriteria = hasRecurveCriteria;
        }

        /// <summary>
        /// Set has combine items
        /// </summary>
        /// <param name="hasCombine">Has combine</param>
        internal void SetHasCombine(bool hasCombine)
        {
            this.hasCombine = hasCombine;
        }

        /// <summary>
        /// Set has converter
        /// </summary>
        /// <param name="hasConverter">Has converter</param>
        internal void SetHasConverter(bool hasConverter)
        {
            this.hasConverter = hasConverter;
        }

        /// <summary>
        /// Gets whether IQuery is complex query object
        /// </summary>
        /// <returns>Return whether query object is a complex query object</returns>
        bool GetIsComplexQuery()
        {
            return hasSubquery || hasRecurveCriteria || hasJoin || hasCombine || hasConverter;
        }

        #endregion
    }
}
