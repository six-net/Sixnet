using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Linq.Expressions;
using EZNEW.Expressions;
using EZNEW.Reflection;
using EZNEW.Exceptions;
using EZNEW.Development.DataAccess;
using EZNEW.Paging;
using EZNEW.Development.Entity;
using System.Collections;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines default implement for IQuery
    /// </summary>
    [Serializable]
    internal class DefaultQuery : IQuery
    {
        #region Fields

        /// <summary>
        /// All condition
        /// </summary>
        internal List<ICondition> conditionCollection = new List<ICondition>();

        /// <summary>
        /// Criteria
        /// </summary>
        internal List<Criterion> criteria = new List<Criterion>();

        /// <summary>
        /// Sorts
        /// </summary>
        internal List<SortEntry> sortCollection = new List<SortEntry>();

        /// <summary>
        /// Query fields
        /// </summary>
        internal List<string> queryFieldCollection = new List<string>();

        /// <summary>
        /// Not query fields
        /// </summary>
        internal List<string> notQueryFieldCollection = new List<string>();

        /// <summary>
        /// Load data properties
        /// </summary>
        internal Dictionary<string, bool> loadProperties = new Dictionary<string, bool>();

        /// <summary>
        /// Join collection
        /// </summary>
        internal List<JoinEntry> joinCollection = new List<JoinEntry>();

        /// <summary>
        /// Combine collection
        /// </summary>
        internal List<CombineEntry> combineCollection = new List<CombineEntry>();

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
        internal bool hasRecurve = false;

        /// <summary>
        /// Has combine
        /// </summary>
        internal bool hasCombine = false;

        /// <summary>
        /// Has field converter
        /// </summary>
        internal bool hasFieldConverter = false;

        /// <summary>
        /// Actually query fields
        /// </summary>
        Tuple<bool, IEnumerable<string>> actuallyQueryFields = null;

        /// <summary>
        /// Entity type assembly qualified name
        /// </summary>
        internal string entityTypeAssemblyQualifiedName = string.Empty;
        [NonSerialized]
        internal Dictionary<Guid, dynamic> validationFuncDict = new Dictionary<Guid, dynamic>();
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
        /// Gets all condition
        /// </summary>
        public IEnumerable<ICondition> Conditions => conditionCollection;

        /// <summary>
        /// Gets all criterion
        /// </summary>
        public IEnumerable<Criterion> Criteria => criteria;

        /// <summary>
        /// Gets all sort
        /// </summary>
        public IEnumerable<SortEntry> Sorts => sortCollection;

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
        /// Gets the text
        /// </summary>
        public string Text { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the text parameters
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> TextParameters { get; internal set; } = null;

        /// <summary>
        /// Gets the query execution mode
        /// </summary>
        public QueryExecutionMode ExecutionMode { get; internal set; } = QueryExecutionMode.QueryObject;

        /// <summary>
        /// Gets or sets query data size
        /// </summary>
        public int QuerySize { get; set; } = 0;

        /// <summary>
        /// Gets all of data properties allow to lazy load
        /// </summary>
        public IEnumerable<KeyValuePair<string, bool>> LoadProperties => loadProperties;

        /// <summary>
        /// Indicates whether has subquery
        /// </summary>
        public bool HasSubquery => hasSubquery;

        /// <summary>
        /// Indicates whether has recurve
        /// </summary>
        public bool HasRecurve => hasRecurve;

        /// <summary>
        /// Indicates whether has join
        /// </summary>
        public bool HasJoin => hasJoin;

        /// <summary>
        /// Indicates whether has combine
        /// </summary>
        public bool HasCombine => hasCombine;

        /// <summary>
        /// Indicates whether has field converter
        /// </summary>
        public bool HasFieldConverter => hasFieldConverter;

        /// <summary>
        /// Indicates whether is a complex query
        /// Include subquery,recurve,join
        /// </summary>
        public bool IsComplex => GetIsComplexQuery();

        /// <summary>
        /// Gets the recurve
        /// </summary>
        public Recurve Recurve { get; private set; }

        /// <summary>
        /// Indicates whether must affect data
        /// </summary>
        public bool MustAffectData { get; set; }

        /// <summary>
        /// Indicates whether is obsolete
        /// </summary>
        public bool IsObsolete { get; internal set; } = false;

        /// <summary>
        /// Gets the joins
        /// </summary>
        public IEnumerable<JoinEntry> Joins => joinCollection;

        /// <summary>
        /// Gets the combines
        /// </summary>
        public IEnumerable<CombineEntry> Combines => combineCollection;

        /// <summary>
        /// Indecats whether are no conditions included
        /// </summary>
        public bool NoneCondition => conditionCollection.IsNullOrEmpty() && Joins.IsNullOrEmpty() && combineCollection.IsNullOrEmpty();

        ///// <summary>
        ///// Gets the atomic condition count
        ///// </summary>
        //public int AtomicConditionCount => atomicConditionCount;

        ///// <summary>
        ///// Gets all condition field names
        ///// </summary>
        //public IEnumerable<string> AllConditionFieldNames => allConditionFieldNameCollection;

        /// <summary>
        /// Gets all subqueries
        /// </summary>
        public IEnumerable<IQuery> Subqueries => subqueryCollection;

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Indicates whether query obsolete data
        /// </summary>
        public bool IncludeObsoleteData { get; set; }

        /// <summary>
        /// Gets or sets the connection operator
        /// </summary>
        public CriterionConnector Connector { get; set; } = CriterionConnector.And;

        #endregion

        #region Methods

        #region Condition

        /// <summary>
        /// Add criterion
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <param name="field">Field</param>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        public IQuery AddCriterion(CriterionConnector connector, FieldInfo field, CriterionOperator criterionOperator, dynamic value, CriterionOptions criterionOptions = null)
        {
            if (string.IsNullOrWhiteSpace(field?.Name))
            {
                throw new EZNEWException($"Field name for [{value}] is null or empty.");
            }
            Criterion newCriterion = Criterion.Create(field, criterionOperator, value, connector, criterionOptions);
            return AddCondition(newCriterion);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <param name="condition">Condition</param>
        public IQuery AddCondition(ICondition condition)
        {
            #region invoke handler

            var queryItemTypeId = condition?.GetType().GUID ?? Guid.Empty;
            Func<DefaultQuery, ICondition, ICondition> handler = null;
            QueryManager.AddConditionHandlers?.TryGetValue(queryItemTypeId, out handler);
            if (handler != null)
            {
                condition = handler(this, condition);
            }

            #endregion

            if (condition != null)
            {
                //clear data
                validationFuncDict?.Clear();
                conditionCollection.Add(condition);
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
        public IQuery ClearSort()
        {
            sortCollection.Clear();
            return this;
        }

        #endregion

        #region Add sort

        ///// <summary>
        ///// Add sort
        ///// </summary>
        ///// <param name="fieldName">Field name</param>
        ///// <param name="desc">Sort by desc</param>
        ///// <param name="sortOptions">Sort options</param>
        //public IQuery AddSort(string fieldName, bool desc = false, SortOptions sortOptions = null)
        //{
        //    return AddSort(FieldInfo.Create(fieldName), desc, sortOptions);
        //}

        ///// <summary>
        ///// Add sort
        ///// </summary>
        ///// <param name="field">Field name</param>
        ///// <param name="desc">Sort by desc</param>
        ///// <param name="sortOptions">Sort options</param>
        //public IQuery AddSort(FieldInfo field, bool desc = false, SortOptions sortOptions = null)
        //{
        //    if (!string.IsNullOrWhiteSpace(field?.Name))
        //    {
        //        sortCollection.Add(new SortEntry()
        //        {
        //            Field = field,
        //            Desc = desc,
        //            Options = sortOptions
        //        });
        //    }
        //    return this;
        //}

        /// <summary>
        /// Add sort
        /// </summary>
        /// <param name="sortEntries">Sort entries</param>
        /// <returns></returns>
        public IQuery AddSort(params SortEntry[] sortEntries)
        {
            if (sortEntries.IsNullOrEmpty())
            {
                return this;
            }
            sortCollection.AddRange(sortEntries);
            return this;
        }

        #endregion

        #region Sort data

        /// <summary>
        /// Sort data
        /// </summary>
        /// <typeparam name="TEntity">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="useDefaultFieldToSort">Whether use default field to sort</param>
        /// <returns>Return the sorted data set</returns>
        public IEnumerable<TEntity> SortData<TEntity>(IEnumerable<TEntity> datas, bool useDefaultFieldToSort = false) where TEntity : BaseEntity<TEntity>, new()
        {
            if (datas.IsNullOrEmpty())
            {
                return datas;
            }
            IOrderedEnumerable<TEntity> sortedDatas = null;
            var entityType = typeof(TEntity);
            if (!sortCollection.IsNullOrEmpty())
            {
                foreach (var sortEntry in sortCollection)
                {
                    if (sortEntry == null)
                    {
                        continue;
                    }
                    var field = EntityManager.GetEntityField(entityType, sortEntry.Field?.Name);
                    if (field == null)
                    {
                        throw new EZNEWException($"{entityType.FullName}=>{sortEntry.Field}:Entity field config not find.");
                    }
                    Func<TEntity, object> getDataFunc = field.ValueProvider.Get;//ExpressionHelper.GetPropertyOrFieldFunction<TEntity>(sortEntry.Name);
                    if (getDataFunc == null)
                    {
                        continue;
                    }
                    if (sortedDatas == null)
                    {
                        sortedDatas = sortEntry.Desc ? datas.OrderByDescending(getDataFunc) : datas.OrderBy(getDataFunc);
                    }
                    else
                    {
                        sortedDatas = sortEntry.Desc ? sortedDatas.ThenByDescending(getDataFunc) : sortedDatas.ThenBy(getDataFunc);
                    }
                }
            }
            else if (useDefaultFieldToSort)
            {
                var defaultField = EntityManager.GetDefaultField(typeof(TEntity));
                if (defaultField?.ValueProvider == null)
                {
                    return datas;
                }
                sortedDatas = datas.OrderByDescending(defaultField.ValueProvider.Get);
            }
            return sortedDatas ?? datas;
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
            if (!fields.IsNullOrEmpty())
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
            if (!fields.IsNullOrEmpty())
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
        /// <param name="forceNecessaryFields">Whether include necessary fields</param>
        /// <returns>Return actually query fields</returns>
        public Tuple<bool, IEnumerable<string>> GetActuallyQueryFieldsWithSign(Type entityType, bool forceNecessaryFields)
        {
            if (actuallyQueryFields == null)
            {
                bool fullQuery = true;
                var allowQueryFields = QueryFields;
                if (!allowQueryFields.IsNullOrEmpty())
                {
                    fullQuery = false;
                    var mustQueryFields = EntityManager.GetMustQueryFields(entityType);
                    if (forceNecessaryFields && !mustQueryFields.IsNullOrEmpty())
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
                    if (forceNecessaryFields && !mustQueryFields.IsNullOrEmpty())
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

        #region Text

        /// <summary>
        /// Set text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery SetText(string text, object parameters = null)
        {
            Text = text;
            TextParameters = parameters?.ObjectToDcitionary();
            ExecutionMode = QueryExecutionMode.Text;
            return this;
        }

        #endregion

        #region Load Property

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

        #region Get parameters

        /// <summary>
        /// Get equal parameters
        /// </summary>
        /// <param name="parameterNames">Parameter names</param>
        /// <returns>Return parameter values</returns>
        public Dictionary<string, List<dynamic>> GetEqualParameters(IEnumerable<string> parameterNames = null)
        {
            if (ExecutionMode == QueryExecutionMode.Text || criteria.IsNullOrEmpty())
            {
                return new Dictionary<string, List<dynamic>>();
            }
            var equalCriteria = criteria.Where(c => c.IsEqual);
            return GetCriteriaValues(equalCriteria, parameterNames);
        }

        /// <summary>
        /// Get criteria values
        /// </summary>
        /// <param name="criteria">Criteria</param>
        /// <param name="names">Names</param>
        /// <returns>Return parameter values</returns>
        Dictionary<string, List<dynamic>> GetCriteriaValues(IEnumerable<Criterion> criteria, IEnumerable<string> names = null)
        {
            if (criteria.IsNullOrEmpty())
            {
                return new Dictionary<string, List<dynamic>>(0);
            }
            var criteriaGroup = criteria.GroupBy(c => c.Field.Name);
            if (names != null && !names.IsNullOrEmpty())
            {
                criteriaGroup = criteriaGroup.Where(c => names.Contains(c.Key));
            }
            var criteriaDict = criteriaGroup.ToDictionary(c => c.Key, c => c.ToList());
            var parameters = new Dictionary<string, List<dynamic>>(criteriaDict.Count);
            foreach (var criteriaItem in criteriaDict)
            {
                if (criteriaItem.Value.IsNullOrEmpty())
                {
                    continue;
                }
                List<dynamic> values = new List<dynamic>();
                foreach (var criterion in criteriaItem.Value)
                {
                    var criterionValue = criterion.Value;
                    if (criterionValue == null)
                    {
                        continue;
                    }
                    if (criterionValue is IEnumerable)
                    {
                        foreach (var valueEntry in criterionValue)
                        {
                            values.Add(valueEntry);
                        }
                    }
                    else
                    {
                        values.Add(criterionValue);
                    }
                }
                if (values.IsNullOrEmpty())
                {
                    continue;
                }
                parameters[criteriaItem.Key] = values;
            }
            return parameters;
        }

        #endregion

        #region Get validation function

        /// <summary>
        /// Get validation function
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Return a validation function</returns>
        public Func<T, bool> GetValidationFunction<T>()
        {
            Type modelType = typeof(T);
            validationFuncDict ??= new Dictionary<Guid, dynamic>();
            if (validationFuncDict.ContainsKey(modelType.GUID))
            {
                return validationFuncDict[modelType.GUID];
            }
            if (IsComplex)
            {
                Func<T, bool> falseFunc = (data) => false;
                validationFuncDict.Add(modelType.GUID, falseFunc);
                return falseFunc;
            }
            if (conditionCollection.IsNullOrEmpty())
            {
                Func<T, bool> trueFunc = (data) => true;
                validationFuncDict.Add(modelType.GUID, trueFunc);
                return trueFunc;
            }
            Type funcType = QueryManager.GetEntityPredicateType(modelType);
            ParameterExpression parExp = Expression.Parameter(modelType);
            Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
            parameterArray.SetValue(parExp, 0);
            Expression conditionExpression = null;
            foreach (var conditionEntry in conditionCollection)
            {
                Expression childExpression = GenerateExpression(parExp, conditionEntry);
                if (childExpression == null)
                {
                    continue;
                }
                if (conditionExpression == null)
                {
                    conditionExpression = childExpression;
                    continue;
                }
                if (conditionEntry.Connector == CriterionConnector.And)
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
            validationFuncDict.Add(modelType.GUID, func);
            return func;
        }

        /// <summary>
        /// Generate expressionn
        /// </summary>
        /// <param name="parameter">Parameter expression</param>
        /// <param name="condition">Condition</param>
        /// <returns>Return query expression</returns>
        Expression GenerateExpression(Expression parameter, ICondition condition)
        {
            if (condition is Criterion)
            {
                return GenerateSingleExpression(parameter, condition as Criterion);
            }
            IQuery query = condition as IQuery;
            if (query?.Conditions.IsNullOrEmpty() ?? true)
            {
                return null;
            }
            var conditionCount = query.Conditions.Count();
            var firstCondition = query.Conditions.First();
            if (conditionCount == 1 && firstCondition is Criterion firstCriterion)
            {
                return GenerateSingleExpression(parameter, firstCriterion);
            }
            Expression conditionExpression = null;
            foreach (var conditionEntry in query.Conditions)
            {
                var childExpression = GenerateExpression(parameter, conditionEntry);
                if (conditionExpression == null)
                {
                    conditionExpression = childExpression;
                    continue;
                }
                if (conditionEntry.Connector == CriterionConnector.And)
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
        /// <param name="criterion">Criterion</param>
        /// <returns>Return query expression</returns>
        Expression GenerateSingleExpression(Expression parameter, Criterion criterion)
        {
            object criteriaValue = criterion.Value;
            Expression valueExpression = Expression.Constant(criteriaValue, criteriaValue?.GetType() ?? typeof(object));
            Expression property = string.IsNullOrWhiteSpace(criterion.Field?.Name) ? null : Expression.PropertyOrField(parameter, criterion.Field.Name);
            switch (criterion.Operator)
            {
                case CriterionOperator.False:
                    property = Expression.Constant(false, typeof(bool));
                    break;
                case CriterionOperator.True:
                    property = Expression.Constant(true, typeof(bool));
                    break;
                case CriterionOperator.Equal:
                case CriterionOperator.IsNull:
                    if (criteriaValue == null && !property.Type.AllowNull())
                    {
                        property = Expression.Constant(false, typeof(bool));
                    }
                    else
                    {
                        property = Expression.Equal(property, valueExpression);
                    }
                    break;
                case CriterionOperator.NotEqual:
                case CriterionOperator.NotNull:
                    if (criteriaValue == null && !property.Type.AllowNull())
                    {
                        property = Expression.Constant(true, typeof(bool));
                    }
                    else
                    {
                        property = Expression.NotEqual(property, valueExpression);
                    }
                    break;
                case CriterionOperator.GreaterThan:
                    property = Expression.GreaterThan(property, valueExpression);
                    break;
                case CriterionOperator.GreaterThanOrEqual:
                    property = Expression.GreaterThanOrEqual(property, valueExpression);
                    break;
                case CriterionOperator.LessThan:
                    property = Expression.LessThan(property, valueExpression);
                    break;
                case CriterionOperator.LessThanOrEqual:
                    property = Expression.LessThanOrEqual(property, valueExpression);
                    break;
                case CriterionOperator.BeginLike:
                    Expression beginLikeExpression = Expression.Call(property, ReflectionManager.String.StringIndexOfMethod, valueExpression);
                    property = Expression.Equal(beginLikeExpression, Expression.Constant(0));
                    break;
                case CriterionOperator.Like:
                    Expression likeExpression = Expression.Call(property, ReflectionManager.String.StringIndexOfMethod, valueExpression);
                    property = Expression.GreaterThanOrEqual(likeExpression, Expression.Constant(0));
                    break;
                case CriterionOperator.EndLike:
                    property = Expression.Call(property, ReflectionManager.String.EndWithMethod, valueExpression);
                    break;
                case CriterionOperator.In:
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
                case CriterionOperator.NotIn:
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
            return property;
        }

        #endregion

        #region Recurve

        /// <summary>
        /// Set recurve
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="relationField">Relation field</param>
        /// <param name="direction">Recurve direction</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery SetRecurve(string dataField, string relationField, RecurveDirection direction = RecurveDirection.Down)
        {
            if (string.IsNullOrWhiteSpace(dataField) || string.IsNullOrWhiteSpace(relationField))
            {
                throw new EZNEWException($"{nameof(dataField)} or {nameof(relationField)} is null or empty");
            }
            if (dataField == relationField)
            {
                throw new EZNEWException($"{nameof(dataField)} and {nameof(relationField)} can not be the same value");
            }
            Recurve = new Recurve()
            {
                DataField = dataField,
                RelationField = relationField,
                Direction = direction
            };
            hasRecurve = true;
            return this;
        }

        /// <summary>
        /// Set recurve criteria
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="dataField">Data field</param>
        /// <param name="relationField">Relation field</param>
        /// <param name="direction">Recurve direction</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery SetRecurve<TQueryModel>(Expression<Func<TQueryModel, dynamic>> dataField, Expression<Func<TQueryModel, dynamic>> relationField, RecurveDirection direction = RecurveDirection.Down) where TQueryModel : IQueryModel<TQueryModel>
        {
            return SetRecurve(ExpressionHelper.GetExpressionPropertyName(dataField), ExpressionHelper.GetExpressionPropertyName(relationField), direction);
        }

        #endregion

        #region Obsolete

        /// <summary>
        /// Obsolete
        /// </summary>
        public void Obsolete()
        {
            IsObsolete = true;
        }

        /// <summary>
        /// Activate
        /// </summary>
        public void Activate()
        {
            IsObsolete = false;
        }

        #endregion

        #region Clone

        /// <summary>
        /// Light clone an IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        public IQuery LightClone()
        {
            var newQuery = CloneValueMember();
            newQuery.Recurve = Recurve;
            newQuery.conditionCollection = new List<ICondition>(conditionCollection);
            newQuery.criteria = new List<Criterion>(criteria);
            newQuery.sortCollection = new List<SortEntry>(sortCollection);
            newQuery.queryFieldCollection = new List<string>(queryFieldCollection);
            newQuery.notQueryFieldCollection = new List<string>(notQueryFieldCollection);
            newQuery.loadProperties = new Dictionary<string, bool>(loadProperties);
            newQuery.joinCollection = new List<JoinEntry>(joinCollection);
            newQuery.combineCollection = new List<CombineEntry>(combineCollection);
            newQuery.subqueryCollection = new List<IQuery>(subqueryCollection);
            newQuery.actuallyQueryFields = actuallyQueryFields == null ? null : new Tuple<bool, IEnumerable<string>>(actuallyQueryFields.Item1, actuallyQueryFields.Item2);
            newQuery.PagingInfo = PagingInfo == null ? null : new PagingFilter() { Page = PagingInfo.Page, PageSize = PagingInfo.PageSize, QuerySize = PagingInfo.QuerySize };
            newQuery.validationFuncDict = new Dictionary<Guid, dynamic>(validationFuncDict);
            newQuery.TextParameters = TextParameters?.ToDictionary(c => c.Key, c => c.Value);
            return newQuery;
        }

        /// <summary>
        /// Clone a IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        public IQuery Clone()
        {
            var newQuery = CloneValueMember();
            newQuery.conditionCollection = conditionCollection.Select(c => c?.Clone()).ToList();
            newQuery.sortCollection = sortCollection.Select(c => c?.Clone()).ToList();
            newQuery.queryFieldCollection = new List<string>(queryFieldCollection);
            newQuery.notQueryFieldCollection = new List<string>(notQueryFieldCollection);
            newQuery.loadProperties = new Dictionary<string, bool>(loadProperties);
            newQuery.criteria = criteria.Select(c => c.Clone()).ToList();
            newQuery.joinCollection = joinCollection.Select(c => c.Clone()).ToList();
            newQuery.combineCollection = combineCollection.Select(c => c.Clone()).ToList();
            newQuery.subqueryCollection = subqueryCollection.Select(c => c.Clone()).ToList();
            newQuery.actuallyQueryFields = actuallyQueryFields == null ? null : new Tuple<bool, IEnumerable<string>>(actuallyQueryFields.Item1, actuallyQueryFields.Item2?.Select(c => c));
            newQuery.PagingInfo = PagingInfo == null ? null : new PagingFilter() { Page = PagingInfo.Page, PageSize = PagingInfo.PageSize, QuerySize = PagingInfo.QuerySize };
            newQuery.validationFuncDict = new Dictionary<Guid, dynamic>(validationFuncDict);
            newQuery.TextParameters = TextParameters?.ToDictionary(c => c.Key, c => c.Value);
            newQuery.Recurve = Recurve?.Clone();
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
                hasRecurve = hasRecurve,
                hasCombine = hasCombine,
                hasFieldConverter = hasFieldConverter,
                entityTypeAssemblyQualifiedName = entityTypeAssemblyQualifiedName,
                cancellationToken = cancellationToken,
                entityType = entityType,
                Text = Text,
                ExecutionMode = ExecutionMode,
                QuerySize = QuerySize,
                MustAffectData = MustAffectData,
                IsObsolete = IsObsolete,
                IsolationLevel = IsolationLevel,
                IncludeObsoleteData = IncludeObsoleteData,
                Connector = Connector
            };
            return newQuery;
        }

        #endregion

        #region Join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Join(JoinEntry joinEntry)
        {
            if (joinEntry?.JoinObjectFilter == null)
            {
                return this;
            }
            joinEntry.JoinObjectFilter = QueryManager.HandleParameterQueryBeforeUse(joinEntry.JoinObjectFilter);
            if (joinEntry.JoinObjectExtraFilter != null)
            {
                joinEntry.JoinObjectExtraFilter = QueryManager.HandleParameterQueryBeforeUse(joinEntry.JoinObjectExtraFilter);
            }
            joinEntry.Sort = joinSort++;
            joinCollection.Add(joinEntry);
            hasJoin = true;
            hasFieldConverter |= joinEntry.JoinObjectFilter.HasFieldConverter;
            return this;
        }

        #endregion

        #region Combine

        /// <summary>
        /// Add combine
        /// </summary>
        /// <param name="combineEntry">Combine entry</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Combine(CombineEntry combineEntry)
        {
            if (combineEntry == null)
            {
                return this;
            }
            combineEntry.Query = QueryManager.HandleParameterQueryBeforeUse(combineEntry.Query);
            combineCollection.Add(combineEntry);
            hasCombine = true;
            return this;
        }

        #endregion

        #region GlobalCondition

        /// <summary>
        /// Set global condition
        /// </summary>
        /// <param name="globalCondition">Global condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery SetGlobalCondition(IQuery globalCondition)
        {
            if (alreadySetGlobalCondition || globalCondition == null || globalCondition.NoneCondition)
            {
                return this;
            }
            alreadySetGlobalCondition = true;
            return AddCondition(globalCondition);
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

        #region Cancellation token

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

        /// <summary>
        /// Set paging info
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        public IQuery SetPaging(int pageIndex, int pageSize = 20)
        {
            return SetPaging(new PagingFilter()
            {
                Page = pageIndex,
                PageSize = pageSize
            });
        }

        #endregion

        #endregion

        #region Util

        /// <summary>
        /// Set has subquery
        /// </summary>
        /// <param name="hasSubquery">Has subquery</param>
        internal void SetHasSubquery(bool hasSubquery)
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
        /// Set has recurve
        /// </summary>
        /// <param name="hasRecurve">Has recurve</param>
        internal void SetHasRecurve(bool hasRecurve)
        {
            this.hasRecurve = hasRecurve;
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
        /// <param name="hasFieldConverter">Has converter</param>
        internal void SetHasFieldConverter(bool hasFieldConverter)
        {
            this.hasFieldConverter = hasFieldConverter;
        }

        /// <summary>
        /// Add subquery
        /// </summary>
        /// <param name="subquery">Subquery</param>
        /// <returns></returns>
        internal DefaultQuery AddSubquery(IQuery subquery)
        {
            if (subquery != null)
            {
                subqueryCollection.Add(subquery);
                SetHasSubquery(true);
                SetHasJoin(HasJoin || subquery.HasJoin);
                SetHasRecurve(HasRecurve || subquery.HasRecurve);
                SetHasFieldConverter(HasFieldConverter || subquery.HasFieldConverter);
            }
            return this;
        }

        /// <summary>
        /// Add group query
        /// </summary>
        /// <param name="groupQuery">Group query</param>
        /// <returns></returns>
        internal DefaultQuery AddGroupQuery(DefaultQuery groupQuery)
        {
            if (groupQuery != null)
            {
                groupQuery.SetEntityType(entityType);
                SetHasSubquery(HasSubquery || groupQuery.HasSubquery);
                SetHasJoin(HasJoin || groupQuery.HasJoin);
                SetHasRecurve(HasRecurve || groupQuery.HasRecurve);
                criteria.AddRange(groupQuery.criteria);
                alreadySetGlobalCondition |= groupQuery.alreadySetGlobalCondition;
                SetHasFieldConverter(HasFieldConverter || groupQuery.HasFieldConverter);
            }
            return this;
        }

        /// <summary>
        /// Gets whether IQuery is complex query object
        /// </summary>
        /// <returns>Return whether query object is a complex query object</returns>
        bool GetIsComplexQuery()
        {
            return hasSubquery || hasRecurve || hasJoin || hasCombine || hasFieldConverter;
        }

        #endregion
    }
}
