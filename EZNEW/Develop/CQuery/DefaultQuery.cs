using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EZNEW.Develop.CQuery.CriteriaConverter;
using EZNEW.Develop.Entity;
using EZNEW.Fault;
using EZNEW.Develop.DataAccess;
using EZNEW.Paging;
using System.Linq.Expressions;
using EZNEW.ExpressionUtil;

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
        internal Dictionary<string, bool> loadPropertyDictionary = new Dictionary<string, bool>();

        /// <summary>
        /// Equal criterias
        /// </summary>
        internal List<Criteria> equalCriteriaCollection = new List<Criteria>();

        /// <summary>
        /// Join items
        /// </summary>
        internal List<JoinItem> joinItemCollection = new List<JoinItem>();

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
        /// Atomic condition count
        /// </summary>
        internal int atomicConditionCount = 0;

        /// <summary>
        /// All condition field names
        /// </summary>
        internal List<string> allConditionFieldNameCollection = new List<string>();

        /// <summary>
        /// Entity type assembly qualified name
        /// </summary>
        internal string entityTypeAssemblyQualifiedName = string.Empty;
        [NonSerialized]
        internal Dictionary<Guid, dynamic> queryExpressionDictionary = new Dictionary<Guid, dynamic>();
        [NonSerialized]
        internal CancellationToken? cancellationToken = default;
        [NonSerialized]
        internal Type entityType = null;

        #endregion

        #region Constructor

        internal DefaultQuery()
        {
        }

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
        public dynamic QueryTextParameters { get; internal set; } = null;

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
        public IEnumerable<KeyValuePair<string, bool>> LoadPropertys => loadPropertyDictionary;

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
        /// Gets whether is a none condition object
        /// </summary>
        public bool NoneCondition => criteriaCollection.IsNullOrEmpty() && JoinItems.IsNullOrEmpty();

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

        #region And

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery And(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null)
        {
            AddCriteria(QueryOperator.AND, fieldName, @operator, value, converter);
            return this;
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="eachFieldConnectOperator">each field codition connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery And(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null, params string[] fieldNames)
        {
            if (fieldNames.IsNullOrEmpty())
            {
                return this;
            }
            IQuery groupQuery = QueryManager.Create();
            foreach (string field in fieldNames)
            {
                switch (eachFieldConnectOperator)
                {
                    case QueryOperator.AND:
                    default:
                        groupQuery.And(field, @operator, value, converter);
                        break;
                    case QueryOperator.OR:
                        groupQuery.Or(field, @operator, value, converter);
                        break;
                }
            }
            AddQueryItem(QueryOperator.AND, groupQuery);
            return this;
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="criteria">Criteria</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery And<TQueryModel>(Expression<Func<TQueryModel, bool>> criteria) where TQueryModel : QueryModel<TQueryModel>
        {
            var expressQuery = GetExpressionQuery(QueryOperator.AND, criteria.Body);
            if (expressQuery != null)
            {
                AddQueryItem(expressQuery.Item1, expressQuery.Item2);
            }
            return this;
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery And<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            return And(ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, converter);
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="eachFieldConnectOperator">Each field connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery And<TQueryModel>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : QueryModel<TQueryModel>
        {
            if (fields.IsNullOrEmpty())
            {
                return this;
            }
            IEnumerable<string> fieldNames = fields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body));
            return And(eachFieldConnectOperator, @operator, value, converter, fieldNames.ToArray());
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery And(IQuery groupQuery)
        {
            AddQueryItem(QueryOperator.AND, groupQuery);
            return this;
        }

        #endregion

        #region OR

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Or(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null)
        {
            AddCriteria(QueryOperator.OR, fieldName, @operator, value, converter);
            return this;
        }

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <param name="eachFieldConnectOperator">Each field connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fieldNames">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Or(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null, params string[] fieldNames)
        {
            if (fieldNames.IsNullOrEmpty())
            {
                return this;
            }
            IQuery groupQuery = QueryManager.Create();
            foreach (string field in fieldNames)
            {
                switch (eachFieldConnectOperator)
                {
                    case QueryOperator.AND:
                    default:
                        groupQuery.And(field, @operator, value, converter);
                        break;
                    case QueryOperator.OR:
                        groupQuery.Or(field, @operator, value, converter);
                        break;
                }
            }
            AddQueryItem(QueryOperator.OR, groupQuery);
            return this;
        }

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="criteria">Criteria</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Or<TQueryModel>(Expression<Func<TQueryModel, bool>> criteria) where TQueryModel : QueryModel<TQueryModel>
        {
            var expressQuery = GetExpressionQuery(QueryOperator.OR, criteria.Body);
            if (expressQuery != null)
            {
                AddQueryItem(expressQuery.Item1, expressQuery.Item2);
            }
            return this;
        }

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Or<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            return Or(ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, converter);
        }

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="eachFieldConnectOperator">Each field connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Or<TQueryModel>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : QueryModel<TQueryModel>
        {
            if (fields.IsNullOrEmpty())
            {
                return this;
            }
            IEnumerable<string> fieldNames = fields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body));
            return Or(eachFieldConnectOperator, @operator, value, converter, fieldNames.ToArray());
        }

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Or(IQuery groupQuery)
        {
            AddQueryItem(QueryOperator.OR, groupQuery);
            return this;
        }

        #endregion

        #region Equal

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Equal(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.Equal, value, converter);
            return this;
        }

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Equal(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.Equal, subquery, null, new QueryParameterOption()
                {
                    QueryFieldName = subqueryFieldName
                });
            }
            return this;
        }

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Equal<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.Equal, value, converter);
            return this;
        }

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Equal<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            return or ? Or(field, CriteriaOperator.Equal, subquery) : And(field, CriteriaOperator.Equal, subquery);
        }

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Equal<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return this;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return Equal(fieldName, subquery, subFieldName, or);
        }

        #endregion

        #region Not Equal

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqual(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotEqual, value, converter);
            return this;
        }

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqual(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotEqual, subquery, null, new QueryParameterOption()
                {
                    QueryFieldName = subqueryFieldName
                });
            }
            return this;
        }

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotEqual, value, converter);
            return this;
        }

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            return or ? Or(field, CriteriaOperator.NotEqual, subquery) : And(field, CriteriaOperator.NotEqual, subquery);
        }

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqual<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return this;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return NotEqual(fieldName, subquery, subFieldName, or);
        }

        #endregion

        #region LessThan

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThan(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.LessThan, value, converter);
            return this;
        }

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThan(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.LessThan, subquery, null, new QueryParameterOption()
                {
                    QueryFieldName = subqueryFieldName
                });
            }
            return this;
        }

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThan<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.LessThan, value, converter);
            return this;
        }

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThan<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            return or ? Or(field, CriteriaOperator.LessThan, subquery) : And(field, CriteriaOperator.LessThan, subquery);
        }

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThan<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return this;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return LessThan(fieldName, subquery, subFieldName, or);
        }

        #endregion

        #region LessThanOrEqual

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.LessThanOrEqual, value, converter);
            return this;
        }

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqual(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.LessThanOrEqual, subquery, null, new QueryParameterOption()
                {
                    QueryFieldName = subqueryFieldName
                });
            }
            return this;
        }

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.LessThanOrEqual, value, converter);
            return this;
        }

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            return or ? Or(field, CriteriaOperator.LessThanOrEqual, subquery) : And(field, CriteriaOperator.LessThanOrEqual, subquery);
        }

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqual<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return this;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return LessThanOrEqual(fieldName, subquery, subFieldName, or);
        }

        #endregion

        #region GreaterThan

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThan(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.GreaterThan, value, converter);
            return this;
        }

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThan(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.GreaterThan, subquery, null, new QueryParameterOption()
                {
                    QueryFieldName = subqueryFieldName
                });
            }
            return this;
        }

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThan<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.GreaterThan, value, converter);
            return this;
        }

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThan<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            return or ? Or(field, CriteriaOperator.GreaterThan, subquery) : And(field, CriteriaOperator.GreaterThan, subquery);
        }

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThan<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return this;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return GreaterThan(fieldName, subquery, subFieldName, or);
        }

        #endregion

        #region GreaterThanOrEqual

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqual(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.GreaterThanOrEqual, subquery, null, new QueryParameterOption()
                {
                    QueryFieldName = subqueryFieldName
                });
            }
            return this;
        }

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.GreaterThanOrEqual, value, converter);
            return this;
        }

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.GreaterThanOrEqual, value, converter);
            return this;
        }

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            return or ? Or(field, CriteriaOperator.GreaterThanOrEqual, subquery) : And(field, CriteriaOperator.GreaterThanOrEqual, subquery);
        }

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqual<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return this;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return GreaterThanOrEqual(fieldName, subquery, subFieldName, or);
        }

        #endregion

        #region In

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery In(string fieldName, IEnumerable value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.In, value, converter);
            return this;
        }

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery In(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.In, subquery, null, new QueryParameterOption()
                {
                    QueryFieldName = subqueryFieldName
                });
            }
            return this;
        }

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam> 
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery In<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.In, value, converter);
            return this;
        }

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery In<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            return or ? Or(field, CriteriaOperator.In, subquery) : And(field, CriteriaOperator.In, subquery);
        }

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery In<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return this;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return In(fieldName, subquery, subFieldName, or);
        }

        #endregion

        #region Not In

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotIn(string fieldName, IEnumerable value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotIn, value, converter);
            return this;
        }

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotIn(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotIn, subquery, null, new QueryParameterOption()
                {
                    QueryFieldName = subqueryFieldName
                });
            }
            return this;
        }

        /// <summary>
        /// Not Include
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotIn<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotIn, value, converter);
            return this;
        }

        /// <summary>
        /// Not Include
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotIn<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            return or ? Or(field, CriteriaOperator.NotIn, subquery) : And<TQueryModel>(field, CriteriaOperator.NotIn, subquery);
        }

        /// <summary>
        /// Not Include
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotIn<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return this;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return NotIn(fieldName, subquery, subFieldName, or);
        }

        #endregion

        #region Like

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Like(string fieldName, string value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.Like, value, converter);
            return this;
        }

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Like<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.Like, value, converter);
            return this;
        }

        #endregion

        #region NotLike

        /// <summary>
        /// Not Like Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotLike, value, converter);
            return this;
        }

        /// <summary>
        /// Not Like Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotLike, value, converter);
            return this;
        }

        #endregion

        #region BeginLike

        /// <summary>
        /// Begin Like Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery BeginLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.BeginLike, value, converter);
            return this;
        }

        /// <summary>
        /// Begin Like Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery BeginLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.BeginLike, value, converter);
            return this;
        }

        #endregion

        #region NotBeginLike

        /// <summary>
        /// Not Begin Like Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotBeginLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotBeginLike, value, converter);
            return this;
        }

        /// <summary>
        /// Not Begin Like Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotBeginLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotBeginLike, value, converter);
            return this;
        }

        #endregion

        #region EndLike

        /// <summary>
        /// End Like Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EndLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.EndLike, value, converter);
            return this;
        }

        /// <summary>
        /// End Like Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EndLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.EndLike, value, converter);
            return this;
        }

        #endregion

        #region NotEndLike

        /// <summary>
        /// Not End Like Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEndLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotEndLike, value, converter);
            return this;
        }

        /// <summary>
        /// Not End Like Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEndLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotEndLike, value, converter);
            return this;
        }

        #endregion

        #region IsNull

        /// <summary>
        /// Field is null
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery IsNull(string fieldName, bool or = false)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.IsNull, null);
            return this;
        }

        /// <summary>
        /// Field is null
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery IsNull<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.IsNull, null);
            return this;
        }

        #endregion

        #region NotNull

        /// <summary>
        /// Field is not null
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotNull(string fieldName, bool or = false)
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotNull, null);
            return this;
        }

        /// <summary>
        /// Field is not null
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotNull<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, bool or = false) where TQueryModel : QueryModel<TQueryModel>
        {
            AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotNull, null);
            return this;
        }

        #endregion

        #region Sort Condition

        #region ASC

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="converter">Field converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Asc(string field, ICriteriaConverter converter = null)
        {
            AddOrderItem(field, false, converter);
            return this;
        }

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="fieldName">Field name</param>
        /// <param name="converter">Field converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Asc<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddOrderItem(ExpressionHelper.GetExpressionPropertyName(field.Body), false, converter);
            return this;
        }

        #endregion

        #region DESC

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="converter">Field converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Desc(string field, ICriteriaConverter converter = null)
        {
            AddOrderItem(field, true, converter);
            return this;
        }

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="converter">Field converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Desc<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>
        {
            AddOrderItem(ExpressionHelper.GetExpressionPropertyName(field.Body), true, converter);
            return this;
        }

        #endregion 

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

        #endregion

        #region Fields

        /// <summary>
        /// Add special fields need to query
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery AddQueryFields(params string[] fields)
        {
            queryFieldCollection.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Add special fields need to query
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="fieldExpressions">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery AddQueryFields<TQueryModel>(params Expression<Func<TQueryModel, dynamic>>[] fieldExpressions) where TQueryModel : QueryModel<TQueryModel>
        {
            if (!fieldExpressions.IsNullOrEmpty())
            {
                foreach (var expression in fieldExpressions)
                {
                    AddQueryFields(ExpressionHelper.GetExpressionPropertyName(expression.Body));
                }
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
            return this;
        }

        /// <summary>
        /// Add special fields that don't query
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery AddNotQueryFields(params string[] fields)
        {
            notQueryFieldCollection.AddRange(fields);
            return this;
        }

        /// <summary>
        /// Add special fields that don't query
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="fieldExpressions">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery AddNotQueryFields<TQueryModel>(params Expression<Func<TQueryModel, dynamic>>[] fieldExpressions) where TQueryModel : QueryModel<TQueryModel>
        {
            if (!fieldExpressions.IsNullOrEmpty())
            {
                foreach (var expression in fieldExpressions)
                {
                    AddNotQueryFields(ExpressionHelper.GetExpressionPropertyName(expression.Body));
                }
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
            return this;
        }

        /// <summary>
        /// Get actually query fields
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="forcePrimaryKey">Whether force return primary key</param>
        /// <param name="forceVersionKey">Whether fore return version key</param>
        /// <returns>Return the newest IQuery object</returns>
        public IEnumerable<EntityField> GetActuallyQueryFields<TEntity>(bool forcePrimaryKey = true, bool forceVersionKey = true)
        {
            return GetActuallyQueryFields(typeof(TEntity), forcePrimaryKey, forceVersionKey);
        }

        /// <summary>
        /// Get actually query fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="forcePrimaryKey">Whether force return primary key</param>
        /// <param name="forceVersionKey">Whether fore return version key</param>
        /// <returns>Return the newest IQuery object</returns>
        public IEnumerable<EntityField> GetActuallyQueryFields(Type entityType, bool forcePrimaryKey = true, bool forceVersionKey = true)
        {
            if (!queryFieldCollection.IsNullOrEmpty())
            {
                return EntityManager.GetQueryFields(entityType, queryFieldCollection, forcePrimaryKey, forceVersionKey);
            }
            IEnumerable<EntityField> allFields = EntityManager.GetQueryFields(entityType);
            if (!notQueryFieldCollection.IsNullOrEmpty())
            {
                allFields = allFields.Except(notQueryFieldCollection.Select<string, EntityField>(c => c));
                if (forcePrimaryKey)
                {
                    allFields = allFields.Union(EntityManager.GetPrimaryKeys(entityType));
                }
                if (forceVersionKey)
                {
                    var versionField = EntityManager.GetVersionField(entityType);
                    if (!string.IsNullOrWhiteSpace(versionField))
                    {
                        allFields = allFields.Union(new List<EntityField>(1) { versionField });
                    }
                }
            }
            return allFields;
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
            if (parameters != null && !parameters.GetType().IsSerializable)
            {
                parameters = parameters.ObjectToDcitionary();
            }
            QueryTextParameters = parameters;
            QueryType = QueryCommandType.Text;
            return this;
        }

        #endregion

        #region Load Propertys

        /// <summary>
        /// Set load data propertys
        /// </summary>
        /// <param name="properties">Allow load data properties</param>
        public void SetLoadPropertys(Dictionary<string, bool> properties)
        {
            if (properties == null)
            {
                return;
            }
            foreach (var property in properties)
            {
                loadPropertyDictionary[property.Key] = property.Value;
            }
        }

        /// <summary>
        /// Set load data propertys
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <param name="allowLoad">allow load</param>
        /// <param name="propertys">propertys</param>
        public void SetLoadPropertys<T>(bool allowLoad, params Expression<Func<T, dynamic>>[] propertys)
        {
            if (propertys == null)
            {
                return;
            }
            Dictionary<string, bool> propertyDic = new Dictionary<string, bool>(propertys.Length);
            foreach (var property in propertys)
            {
                propertyDic.Add(ExpressionHelper.GetExpressionPropertyName(property.Body), allowLoad);
            }
            SetLoadPropertys(propertyDic);
        }

        /// <summary>
        /// Property is allow load data
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        /// <returns>allow load data</returns>
        public bool AllowLoad(string propertyName)
        {
            return !string.IsNullOrWhiteSpace(propertyName) && loadPropertyDictionary != null && loadPropertyDictionary.ContainsKey(propertyName) && loadPropertyDictionary[propertyName];
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
            queryExpressionDictionary ??= new Dictionary<Guid, dynamic>();
            if (queryExpressionDictionary.ContainsKey(modelType.GUID))
            {
                return queryExpressionDictionary[modelType.GUID];
            }
            if (IsComplexQuery)
            {
                Func<T, bool> falseFunc = (data) => false;
                queryExpressionDictionary.Add(modelType.GUID, falseFunc);
                return falseFunc;
            }
            if (criteriaCollection.IsNullOrEmpty())
            {
                Func<T, bool> trueFunc = (data) => true;
                queryExpressionDictionary.Add(modelType.GUID, trueFunc);
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
            var genericLambdaMethod = QueryManager.LambdaMethod.MakeGenericMethod(funcType);
            var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
            {
                conditionExpression,parameterArray
            });
            Func<T, bool> func = ((Expression<Func<T, bool>>)lambdaExpression).Compile();
            queryExpressionDictionary.Add(modelType.GUID, func);
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
            //if (query.Criterias == null || query.Criterias.Count <= 0)
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
                    Expression beginLikeExpression = Expression.Call(property, QueryManager.StringIndexOfMethod, valueExpression);
                    property = Expression.Equal(beginLikeExpression, Expression.Constant(0));
                    break;
                case CriteriaOperator.Like:
                    Expression likeExpression = Expression.Call(property, QueryManager.StringIndexOfMethod, valueExpression);
                    property = Expression.GreaterThanOrEqual(likeExpression, Expression.Constant(0));
                    break;
                case CriteriaOperator.EndLike:
                    property = Expression.Call(property, QueryManager.EndWithMethod, valueExpression);
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
                    var inMethod = QueryManager.CollectionContainsMethod.MakeGenericMethod(valueType);
                    property = Expression.Call(inMethod, valueExpression, property);
                    break;
                case CriteriaOperator.NotIn:
                    Type notInType = criteriaValue.GetType();
                    if (notInType != null && notInType.GenericTypeArguments != null)
                    {
                        notInType = notInType.GenericTypeArguments[0];
                    }
                    else
                    {
                        notInType = typeof(object);
                    }
                    var notInMethod = QueryManager.CollectionContainsMethod.MakeGenericMethod(notInType);
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
        public IQuery SetRecurve<TQueryModel>(Expression<Func<TQueryModel, dynamic>> key, Expression<Func<TQueryModel, dynamic>> relationKey, RecurveDirection direction = RecurveDirection.Down) where TQueryModel : QueryModel<TQueryModel>
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
        /// Copy a IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        public IQuery Copy()
        {
            var newQuery = CopyWithoutConditions() as DefaultQuery;
            newQuery.criteriaCollection = new List<Tuple<QueryOperator, IQueryItem>>(criteriaCollection);
            newQuery.equalCriteriaCollection = new List<Criteria>(equalCriteriaCollection);
            newQuery.subqueryCollection = new List<IQuery>(Subqueries);
            newQuery.hasSubquery = hasSubquery;
            newQuery.RecurveCriteria = RecurveCriteria;
            newQuery.hasRecurveCriteria = hasRecurveCriteria;
            newQuery.joinItemCollection = new List<JoinItem>(JoinItems);
            newQuery.hasJoin = hasJoin;
            newQuery.atomicConditionCount = atomicConditionCount;
            newQuery.allConditionFieldNameCollection = new List<string>(allConditionFieldNameCollection);
            newQuery.alreadySetGlobalCondition = alreadySetGlobalCondition;
            newQuery.IsObsolete = IsObsolete;
            return newQuery;
        }

        /// <summary>
        /// Deep copy a IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        public IQuery DeepCopy()
        {
            var newQuery = this.DeepClone(ObjectCloneMethod.Binary);
            newQuery.entityType = entityType;
            newQuery.cancellationToken = cancellationToken;
            return newQuery;
        }

        /// <summary>
        /// Copy a IQuery object without conditions
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        public IQuery CopyWithoutConditions()
        {
            var newQuery = new DefaultQuery
            {
                sortCollection = new List<SortCriteria>(sortCollection),
                queryFieldCollection = new List<string>(queryFieldCollection),
                notQueryFieldCollection = new List<string>(notQueryFieldCollection),
                loadPropertyDictionary = new Dictionary<string, bool>(loadPropertyDictionary),
                queryExpressionDictionary = new Dictionary<Guid, dynamic>(queryExpressionDictionary),
                entityType = entityType,
                entityTypeAssemblyQualifiedName = entityTypeAssemblyQualifiedName,
                PagingInfo = PagingInfo,
                QueryText = QueryText,
                QueryTextParameters = QueryTextParameters,
                QueryType = QueryType,
                QuerySize = QuerySize,
                MustReturnValueOnSuccess = MustReturnValueOnSuccess,
                IsolationLevel = IsolationLevel,
                cancellationToken = cancellationToken
            };
            return newQuery;
        }

        #endregion

        #region Join

        #region Join Util

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
            joinQuery = QueryManager.HandleParameterQueryBeforeUse(joinQuery, null);
            joinItemCollection.Add(new JoinItem()
            {
                JoinType = joinType,
                Operator = joinOperator,
                JoinFields = joinFields,
                JoinQuery = joinQuery,
                Sort = joinSort++
            });
            hasJoin = true;
            return this;
        }

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Join(string sourceField, string targetField, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery)
        {
            return Join(new Dictionary<string, string>(1)
                {
                    { sourceField,targetField }
                }, joinType, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Join<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return Join(sourceFieldName, targetFieldName, joinType, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery Join(JoinType joinType, JoinOperator joinOperator, IQuery joinQuery)
        {
            return Join(string.Empty, string.Empty, joinType, joinOperator, joinQuery);
        }

        #endregion

        #region Inner Join

        #region InnerJoin helper

        /// <summary>
        /// Add a inner join query
        /// </summary>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery InnerJoin(JoinOperator joinOperator, params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var joinQuery in joinQuerys)
            {
                InnerJoin(string.Empty, string.Empty, joinOperator, joinQuery);
            }
            return this;
        }

        /// <summary>
        /// Add a inner join query
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery InnerJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return Join(sourceField, targetField, JoinType.InnerJoin, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a inner join query
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery InnerJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return InnerJoin(joinField, joinField, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a inner join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery InnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return InnerJoin(sourceFieldName, targetFieldName, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a inner join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery InnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return InnerJoin(joinFieldName, joinFieldName, joinOperator, joinQuery);
        }

        #endregion

        #region Equal InnerJoin

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualInnerJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return InnerJoin(sourceField, targetField, JoinOperator.Equal, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualInnerJoin(string joinField, IQuery joinQuery)
        {
            return EqualInnerJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return EqualInnerJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return EqualInnerJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualInnerJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                EqualInnerJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region NotEqual InnerJoin

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualInnerJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return InnerJoin(sourceField, targetField, JoinOperator.NotEqual, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualInnerJoin(string joinField, IQuery joinQuery)
        {
            return NotEqualInnerJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return NotEqualInnerJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return NotEqualInnerJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualInnerJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                NotEqualInnerJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region LessThanOrEqual InnerJoin

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualInnerJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return InnerJoin(sourceField, targetField, JoinOperator.LessThanOrEqual, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualInnerJoin(string joinField, IQuery joinQuery)
        {
            return LessThanOrEqualInnerJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LessThanOrEqualInnerJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LessThanOrEqualInnerJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualInnerJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                LessThanOrEqualInnerJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region LessThan InnerJoin

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanInnerJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return InnerJoin(sourceField, targetField, JoinOperator.LessThan, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanInnerJoin(string joinField, IQuery joinQuery)
        {
            return LessThanInnerJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LessThanInnerJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LessThanInnerJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanInnerJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                LessThanInnerJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region GreaterThan InnerJoin

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanInnerJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return InnerJoin(sourceField, targetField, JoinOperator.GreaterThan, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanInnerJoin(string joinField, IQuery joinQuery)
        {
            return GreaterThanInnerJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return GreaterThanInnerJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return GreaterThanInnerJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanInnerJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                GreaterThanInnerJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region GreaterThanOrEqual InnerJoin

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualInnerJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return InnerJoin(sourceField, targetField, JoinOperator.GreaterThanOrEqual, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualInnerJoin(string joinField, IQuery joinQuery)
        {
            return GreaterThanOrEqualInnerJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return GreaterThanOrEqualInnerJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return GreaterThanOrEqualInnerJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualInnerJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                GreaterThanOrEqualInnerJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #endregion

        #region Left Join

        #region Left Join helper

        /// <summary>
        /// Add a left join query
        /// </summary>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LeftJoin(JoinOperator joinOperator, params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var joinQuery in joinQuerys)
            {
                LeftJoin(string.Empty, string.Empty, joinOperator, joinQuery);
            }
            return this;
        }

        /// <summary>
        /// Add a left join query
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LeftJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return Join(sourceField, targetField, JoinType.LeftJoin, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a left join query
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LeftJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return LeftJoin(joinField, joinField, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a left join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LeftJoin(sourceFieldName, targetFieldName, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a left join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LeftJoin(joinFieldName, joinFieldName, joinOperator, joinQuery);
        }

        #endregion

        #region Equal LeftJoin

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualLeftJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return LeftJoin(sourceField, targetField, JoinOperator.Equal, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualLeftJoin(string joinField, IQuery joinQuery)
        {
            return EqualLeftJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return EqualLeftJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return EqualLeftJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualLeftJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                EqualLeftJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region NotEqual LeftJoin

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualLeftJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return LeftJoin(sourceField, targetField, JoinOperator.NotEqual, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualLeftJoin(string joinField, IQuery joinQuery)
        {
            return NotEqualLeftJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return NotEqualLeftJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return NotEqualLeftJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualLeftJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                NotEqualLeftJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region LessThanOrEqual LeftJoin

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualLeftJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return LeftJoin(sourceField, targetField, JoinOperator.LessThanOrEqual, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualLeftJoin(string joinField, IQuery joinQuery)
        {
            return LessThanOrEqualLeftJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LessThanOrEqualLeftJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LessThanOrEqualLeftJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualLeftJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                LessThanOrEqualLeftJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region LessThan LeftJoin

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanLeftJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return LeftJoin(sourceField, targetField, JoinOperator.LessThan, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanLeftJoin(string joinField, IQuery joinQuery)
        {
            return LessThanLeftJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LessThanLeftJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LessThanLeftJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanLeftJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                LessThanLeftJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region GreaterThan LeftJoin

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanLeftJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return LeftJoin(sourceField, targetField, JoinOperator.GreaterThan, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanLeftJoin(string joinField, IQuery joinQuery)
        {
            return GreaterThanLeftJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return GreaterThanLeftJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return GreaterThanLeftJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanLeftJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                GreaterThanLeftJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region GreaterThanOrEqual LeftJoin

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualLeftJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return LeftJoin(sourceField, targetField, JoinOperator.GreaterThanOrEqual, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualLeftJoin(string joinField, IQuery joinQuery)
        {
            return GreaterThanOrEqualLeftJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return GreaterThanOrEqualLeftJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return GreaterThanOrEqualLeftJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualLeftJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                GreaterThanOrEqualLeftJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #endregion

        #region Right Join

        #region Right Join helper

        /// <summary>
        /// Add a right join
        /// </summary>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery RightJoin(JoinOperator joinOperator, params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var joinQuery in joinQuerys)
            {
                RightJoin(string.Empty, string.Empty, joinOperator, joinQuery);
            }
            return this;
        }

        /// <summary>
        /// Add a right join
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery RightJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return Join(sourceField, targetField, JoinType.RightJoin, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a right join
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery RightJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return RightJoin(joinField, joinField, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a right join
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery RightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return RightJoin(sourceFieldName, targetFieldName, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a right join
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery RightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return RightJoin(joinFieldName, joinFieldName, joinOperator, joinQuery);
        }

        #endregion

        #region Equal RightJoin

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualRightJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return RightJoin(sourceField, targetField, JoinOperator.Equal, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualRightJoin(string joinField, IQuery joinQuery)
        {
            return EqualRightJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return EqualRightJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return EqualRightJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualRightJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                EqualRightJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region NotEqual RightJoin

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualRightJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return RightJoin(sourceField, targetField, JoinOperator.NotEqual, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualRightJoin(string joinField, IQuery joinQuery)
        {
            return NotEqualRightJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return NotEqualRightJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return NotEqualRightJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualRightJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                NotEqualRightJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region LessThanOrEqual RightJoin

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualRightJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return RightJoin(sourceField, targetField, JoinOperator.LessThanOrEqual, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualRightJoin(string joinField, IQuery joinQuery)
        {
            return LessThanOrEqualRightJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LessThanOrEqualRightJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LessThanOrEqualRightJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualRightJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                LessThanOrEqualRightJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region LessThan RightJoin

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanRightJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return RightJoin(sourceField, targetField, JoinOperator.LessThan, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanRightJoin(string joinField, IQuery joinQuery)
        {
            return LessThanRightJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LessThanRightJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LessThanRightJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanRightJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                LessThanRightJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region GreaterThan RightJoin

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanRightJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return RightJoin(sourceField, targetField, JoinOperator.GreaterThan, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanRightJoin(string joinField, IQuery joinQuery)
        {
            return GreaterThanRightJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return GreaterThanRightJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return GreaterThanRightJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanRightJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                GreaterThanRightJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region GreaterThanOrEqual RightJoin

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualRightJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return RightJoin(sourceField, targetField, JoinOperator.GreaterThanOrEqual, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualRightJoin(string joinField, IQuery joinQuery)
        {
            return GreaterThanOrEqualRightJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return GreaterThanOrEqualRightJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return GreaterThanOrEqualRightJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualRightJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                GreaterThanOrEqualRightJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #endregion

        #region Full Join

        #region Full Join helper

        /// <summary>
        /// Add a full join
        /// </summary>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery FullJoin(JoinOperator joinOperator, params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var joinQuery in joinQuerys)
            {
                FullJoin(string.Empty, string.Empty, joinOperator, joinQuery);
            }
            return this;
        }

        /// <summary>
        /// Add a full join
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery FullJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return Join(sourceField, targetField, JoinType.FullJoin, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a full join
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery FullJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return FullJoin(joinField, joinField, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a full join
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery FullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return FullJoin(sourceFieldName, targetFieldName, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a full join
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery FullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return FullJoin(joinFieldName, joinFieldName, joinOperator, joinQuery);
        }

        #endregion

        #region Equal FullJoin

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualFullJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return FullJoin(sourceField, targetField, JoinOperator.Equal, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualFullJoin(string joinField, IQuery joinQuery)
        {
            return EqualFullJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return EqualFullJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return EqualFullJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery EqualFullJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                EqualFullJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region NotEqual FullJoin

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualFullJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return FullJoin(sourceField, targetField, JoinOperator.NotEqual, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualFullJoin(string joinField, IQuery joinQuery)
        {
            return NotEqualFullJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return NotEqualFullJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return NotEqualFullJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery NotEqualFullJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                NotEqualFullJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region LessThanOrEqual FullJoin

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualFullJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return FullJoin(sourceField, targetField, JoinOperator.LessThanOrEqual, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualFullJoin(string joinField, IQuery joinQuery)
        {
            return LessThanOrEqualFullJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LessThanOrEqualFullJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LessThanOrEqualFullJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanOrEqualFullJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                LessThanOrEqualFullJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region LessThan FullJoin

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanFullJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return FullJoin(sourceField, targetField, JoinOperator.LessThan, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanFullJoin(string joinField, IQuery joinQuery)
        {
            return LessThanFullJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LessThanFullJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LessThanFullJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery LessThanFullJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                LessThanFullJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region GreaterThan FullJoin

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanFullJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return FullJoin(sourceField, targetField, JoinOperator.GreaterThan, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanFullJoin(string joinField, IQuery joinQuery)
        {
            return GreaterThanFullJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return GreaterThanFullJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return GreaterThanFullJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanFullJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                GreaterThanFullJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #region GreaterThanOrEqual FullJoin

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualFullJoin(string sourceField, string targetField, IQuery joinQuery)
        {
            return FullJoin(sourceField, targetField, JoinOperator.GreaterThanOrEqual, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualFullJoin(string joinField, IQuery joinQuery)
        {
            return GreaterThanOrEqualFullJoin(joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return GreaterThanOrEqualFullJoin(sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return GreaterThanOrEqualFullJoin(joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery GreaterThanOrEqualFullJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                GreaterThanOrEqualFullJoin(string.Empty, string.Empty, query);
            }
            return this;
        }

        #endregion

        #endregion

        #region CrossJoin

        /// <summary>
        /// Add a cross join
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public IQuery CrossJoin(params IQuery[] joinQuerys)
        {
            if (joinQuerys.IsNullOrEmpty())
            {
                return this;
            }
            foreach (var query in joinQuerys)
            {
                Join(string.Empty, string.Empty, JoinType.CrossJoin, JoinOperator.Equal, query);
            }
            return this;
        }

        #endregion

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
            if (queryOperator == QueryOperator.OR)
            {
                Or(globalCondition);
            }
            else
            {
                And(globalCondition);
            }
            return this;
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
            return this.cancellationToken;
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
        /// Add a criteria
        /// </summary>
        /// <param name="queryOperator">Connect operator</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="criteriaOperator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Converter</param>
        /// <param name="queryOption">query parameter option</param>
        void AddCriteria(QueryOperator queryOperator, string fieldName, CriteriaOperator criteriaOperator, dynamic value, ICriteriaConverter converter = null, QueryParameterOption queryOption = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return;
            }
            Criteria newCriteria = Criteria.CreateNewCriteria(fieldName, criteriaOperator, value);
            newCriteria.Converter = converter;
            AddQueryItem(queryOperator, newCriteria, queryOption);
        }

        /// <summary>
        /// Add a IQueryItem
        /// </summary>
        /// <param name="queryOperator">Connect operator</param>
        /// <param name="queryItem">query item</param>
        /// <param name="queryOption">query parameter option</param>
        void AddQueryItem(QueryOperator queryOperator, IQueryItem queryItem, QueryParameterOption queryOption = null)
        {
            #region invoke handler

            var queryItemTypeId = queryItem?.GetType().GUID ?? Guid.Empty;
            Func<DefaultQuery, IQueryItem, QueryParameterOption, IQueryItem> handler = null;
            QueryManager.AddQueryItemHandlers?.TryGetValue(queryItemTypeId, out handler);
            if (handler != null)
            {
                queryItem = handler(this, queryItem, queryOption);
            }

            #endregion

            if (queryItem == null)
            {
                return;
            }

            //clear data
            queryExpressionDictionary?.Clear();
            criteriaCollection.Add(new Tuple<QueryOperator, IQueryItem>(queryOperator, queryItem));
        }

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
        /// Gets whether IQuery is complex query object
        /// </summary>
        /// <returns>Return whether query object is a complex query object</returns>
        bool GetIsComplexQuery()
        {
            return HasSubquery || HasRecurveCriteria || HasJoin;
        }

        /// <summary>
        /// Add order item
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Sort by desc</param>
        /// <param name="converter">Field converter</param>
        void AddOrderItem(string fieldName, bool desc = false, ICriteriaConverter converter = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return;
            }
            sortCollection.Add(new SortCriteria()
            {
                Name = fieldName,
                Desc = desc,
                Converter = converter
            });
        }

        /// <summary>
        /// Get a query item by expression
        /// </summary>
        /// <param name="queryOperator">Connect operator</param>
        /// <param name="expression">Condition expression</param>
        /// <returns>IQueryItem object</returns>
        Tuple<QueryOperator, IQueryItem> GetExpressionQuery(QueryOperator queryOperator, Expression expression)
        {
            var nodeType = expression.NodeType;
            ExpressionType queryNodeType = queryOperator == QueryOperator.OR ? ExpressionType.OrElse : ExpressionType.AndAlso;
            if (ExpressionHelper.IsCompareNodeType(nodeType))
            {
                return GetSingleExpressionQueryItem(queryNodeType, expression);
            }
            else if (ExpressionHelper.IsBoolNodeType(nodeType))
            {
                BinaryExpression binExpression = expression as BinaryExpression;
                if (binExpression == null)
                {
                    throw new EZNEWException("expression is error");
                }
                DefaultQuery query = new DefaultQuery();
                var leftQuery = GetExpressionQuery(queryOperator, binExpression.Left);
                if (leftQuery != null)
                {
                    query.AddQueryItem(leftQuery.Item1, leftQuery.Item2);
                }
                QueryOperator rightQueryOperator = nodeType == ExpressionType.OrElse ? QueryOperator.OR : QueryOperator.AND;
                var rightQuery = GetExpressionQuery(rightQueryOperator, binExpression.Right);
                if (rightQuery != null)
                {
                    query.AddQueryItem(rightQuery.Item1, rightQuery.Item2);
                }
                return new Tuple<QueryOperator, IQueryItem>(queryOperator, query);
            }
            else if (nodeType == ExpressionType.Call)
            {
                return GetCallExpressionQueryItem(queryOperator, expression);
            }
            else if (nodeType == ExpressionType.Not)
            {
                UnaryExpression unaryExpress = expression as UnaryExpression;
                if (unaryExpress != null && unaryExpress.Operand is MethodCallExpression)
                {
                    return GetCallExpressionQueryItem(queryOperator, unaryExpress.Operand, true);
                }
            }
            return null;
        }

        /// <summary>
        /// Get a query item by method call expression
        /// </summary>
        /// <param name="expressionType">Expression node type</param>
        /// <param name="expression">Expression</param>
        /// <param name="negation">Whether is negation</param>
        /// <returns></returns>
        Tuple<QueryOperator, IQueryItem> GetCallExpressionQueryItem(QueryOperator queryOperator, Expression expression, bool negation = false)
        {
            MethodCallExpression callExpression = expression as MethodCallExpression;
            MemberExpression memberArg = null;
            Expression parameterExpression = null;
            if (callExpression.Object != null)
            {
                memberArg = callExpression.Object as MemberExpression;
                parameterExpression = callExpression.Arguments[0];
            }
            else if (callExpression.Arguments.Count == 2)
            {
                memberArg = callExpression.Arguments[0] as MemberExpression;
                parameterExpression = callExpression.Arguments[1];
            }
            if (memberArg == null || parameterExpression == null)
            {
                return null;
            }
            Criteria criteria = null;
            var dataType = memberArg.Type;
            if (dataType == typeof(string))
            {
                criteria = GetStringCallExpressionQueryItem(callExpression.Method.Name, memberArg, parameterExpression, negation);
            }
            else if (typeof(IEnumerable).IsAssignableFrom(memberArg.Type))
            {
                criteria = GetIEnumerableCallExpressionQueryItem(callExpression.Method.Name, parameterExpression, memberArg, negation);
            }
            if (criteria != null)
            {
                return new Tuple<QueryOperator, IQueryItem>(queryOperator, criteria);
            }
            return null;
        }

        /// <summary>
        /// Get a query item by method call expression with string type
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="memberArg">Expression</param>
        /// <param name="negation">Whether is negation</param>
        /// <returns>criteria</returns>
        Criteria GetStringCallExpressionQueryItem(string methodName, Expression memberArg, Expression parameter, bool negation = false)
        {
            Criteria criteria = null;
            CriteriaOperator criteriaOperator = CriteriaOperator.Like;
            //parameter name
            string parameterName = string.Empty;
            if (memberArg is ParameterExpression)
            {
                parameterName = (memberArg as ParameterExpression)?.Name;
            }
            else if (memberArg is MemberExpression)
            {
                parameterName = ExpressionHelper.GetExpressionPropertyName(memberArg as MemberExpression);
            }
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return null;
            }
            string value = Expression.Lambda(parameter)?.Compile().DynamicInvoke()?.ToString();
            switch (methodName)
            {
                case "EndsWith":
                    criteriaOperator = negation ? CriteriaOperator.NotEndLike : CriteriaOperator.EndLike;
                    criteria = Criteria.CreateNewCriteria(parameterName, criteriaOperator, value);
                    break;
                case "StartsWith":
                    criteriaOperator = negation ? CriteriaOperator.NotBeginLike : CriteriaOperator.BeginLike;
                    criteria = Criteria.CreateNewCriteria(parameterName, criteriaOperator, value);
                    break;
                case "Contains":
                    criteriaOperator = negation ? CriteriaOperator.NotLike : CriteriaOperator.Like;
                    criteria = Criteria.CreateNewCriteria(parameterName, criteriaOperator, value);
                    break;
            }
            return criteria;
        }

        /// <summary>
        /// Get a query item by method call expression with IEnumerable
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="memberArg">MemberArg</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="negation">Whether is negation</param>
        /// <returns>Return a criteria</returns>
        Criteria GetIEnumerableCallExpressionQueryItem(string methodName, Expression memberArg, Expression parameter, bool negation = false)
        {
            Criteria criteria = null;
            CriteriaOperator criteriaOperator = CriteriaOperator.In;
            IEnumerable values = null;
            string parameterName = string.Empty;
            if (memberArg is ParameterExpression)
            {
                parameterName = (memberArg as ParameterExpression)?.Name;
            }
            else if (memberArg is MemberExpression)
            {
                parameterName = ExpressionHelper.GetExpressionPropertyName(memberArg as MemberExpression);
            }
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new EZNEWException($"conditional expression is not well-formed");
            }
            switch (methodName)
            {
                case "Contains":
                    values = Expression.Lambda(parameter)?.Compile().DynamicInvoke() as IEnumerable;
                    if (values == null)
                    {
                        throw new EZNEWException($"the value of the collection type is null or empty");
                    }
                    var type = values.GetType();
                    if (!type.IsSerializable && type.IsGenericType)
                    {
                        var argLength = type.GenericTypeArguments.Length;
                        var toListMethod = QueryManager.CollectionToListMethod.MakeGenericMethod(type.GenericTypeArguments[argLength - 1]);
                        values = toListMethod.Invoke(null, new object[1] { values }) as IEnumerable;
                    }
                    criteriaOperator = negation ? CriteriaOperator.NotIn : CriteriaOperator.In;
                    criteria = Criteria.CreateNewCriteria(parameterName, criteriaOperator, values);
                    break;
            }
            return criteria;
        }

        /// <summary>
        /// Get a query item
        /// </summary>
        /// <param name="expressionType">Expression node type</param>
        /// <param name="expression">Expression</param>
        /// <returns>IQueryItem object</returns>
        Tuple<QueryOperator, IQueryItem> GetSingleExpressionQueryItem(ExpressionType expressionType, Expression expression)
        {
            if (expression == null)
            {
                throw new EZNEWException("expression is null");
            }
            BinaryExpression binaryExpression = expression as BinaryExpression;
            if (binaryExpression == null)
            {
                throw new EZNEWException("expression is error");
            }
            QueryOperator qOperator = expressionType == ExpressionType.OrElse ? QueryOperator.OR : QueryOperator.AND;
            Tuple<Expression, Expression> nameAndValue = GetNameAndValueExpression(binaryExpression.Left, binaryExpression.Right);
            if (nameAndValue == null)
            {
                return null;
            }
            string name = ExpressionHelper.GetExpressionPropertyName(nameAndValue.Item1);
            object value = nameAndValue.Item2;
            if (string.IsNullOrEmpty(name) || value == null)
            {
                return null;
            }
            CriteriaOperator cOperator = GetCriteriaOperator(binaryExpression.NodeType);
            return new Tuple<QueryOperator, IQueryItem>(qOperator, Criteria.CreateNewCriteria(name, cOperator, value));
        }

        /// <summary>
        /// Get field and value expression
        /// </summary>
        /// <param name="firstExpression">First expression</param>
        /// <param name="secondExpression">Eecond expression</param>
        /// <returns>Return field and value expression</returns>
        Tuple<Expression, Expression> GetNameAndValueExpression(Expression firstExpression, Expression secondExpression)
        {
            Tuple<Expression, Expression> result = null;
            bool firstIsNameExp = IsNameExpression(firstExpression);
            bool secondIsNameExp = IsNameExpression(secondExpression);
            if (!firstIsNameExp && !secondIsNameExp)
            {
                return result;
            }
            if (firstIsNameExp && secondIsNameExp)
            {
                Expression firstChildExp = ExpressionHelper.GetLastChildExpression(firstExpression);
                Expression secondChildExp = ExpressionHelper.GetLastChildExpression(secondExpression);
                result = firstChildExp.NodeType >= secondChildExp.NodeType ? new Tuple<Expression, Expression>(firstExpression, secondExpression) : new Tuple<Expression, Expression>(secondExpression, firstExpression);
                return result;
            }
            result = firstIsNameExp ? new Tuple<Expression, Expression>(firstExpression, secondExpression) : new Tuple<Expression, Expression>(secondExpression, firstExpression);
            return result;
        }

        /// <summary>
        /// Is field name expression
        /// </summary>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        bool IsNameExpression(Expression expression)
        {
            if (expression == null)
            {
                return false;
            }
            bool result = false;
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    result = true;
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryExpression unaryExp = expression as UnaryExpression;
                    if (unaryExp.Operand.NodeType == ExpressionType.MemberAccess)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        /// <summary>
        /// Get condition operator by expression type
        /// </summary>
        /// <param name="expressType">Expression type</param>
        /// <returns>Return criteria operator</returns>
        CriteriaOperator GetCriteriaOperator(ExpressionType expressType)
        {
            CriteriaOperator cOperator = CriteriaOperator.Equal;
            switch (expressType)
            {
                case ExpressionType.Equal:
                default:
                    cOperator = CriteriaOperator.Equal;
                    break;
                case ExpressionType.NotEqual:
                    cOperator = CriteriaOperator.NotEqual;
                    break;
                case ExpressionType.LessThanOrEqual:
                    cOperator = CriteriaOperator.LessThanOrEqual;
                    break;
                case ExpressionType.LessThan:
                    cOperator = CriteriaOperator.LessThan;
                    break;
                case ExpressionType.GreaterThan:
                    cOperator = CriteriaOperator.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    cOperator = CriteriaOperator.GreaterThanOrEqual;
                    break;
            }
            return cOperator;
        }

        #endregion
    }
}
