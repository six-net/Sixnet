using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections;
using System.Threading;
using EZNEW.Paging;
using EZNEW.Develop.CQuery.CriteriaConverter;
using EZNEW.Develop.Entity;
using EZNEW.Develop.DataAccess;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// All iquery instance and criteria inherit this interface
    /// </summary>
    public interface IQueryItem
    {
    }

    /// <summary>
    /// Query info contract
    /// </summary>
    public interface IQuery : IQueryItem
    {
        #region Properties

        /// <summary>
        /// Gets the all criterias or other IQuery items
        /// </summary>
        IEnumerable<Tuple<QueryOperator, IQueryItem>> Criterias { get; }

        /// <summary>
        /// Gets the order items
        /// </summary>
        IEnumerable<SortCriteria> Orders { get; }

        /// <summary>
        /// Gets the specific query fields(it's priority greater than NoQueryFields)
        /// </summary>
        IEnumerable<string> QueryFields { get; }

        /// <summary>
        /// Gets the specific not query fields(it's priority less than QueryFields)
        /// </summary>
        IEnumerable<string> NotQueryFields { get; }

        /// <summary>
        /// Gets the paging info
        /// </summary>
        PagingFilter PagingInfo { get; }

        /// <summary>
        /// Gets the query text
        /// </summary>
        string QueryText { get; }

        /// <summary>
        /// Gets the query text parameter
        /// </summary>
        dynamic QueryTextParameters { get; }

        /// <summary>
        /// Gets the query command type
        /// </summary>
        QueryCommandType QueryType { get; }

        /// <summary>
        /// Gets or sets query data size
        /// </summary>
        int QuerySize { get; set; }

        /// <summary>
        /// Gets all of data properties allow to lazy load
        /// </summary>
        IEnumerable<KeyValuePair<string, bool>> LoadPropertys { get; }

        /// <summary>
        /// Gets whether has subquery
        /// </summary>
        bool HasSubquery { get; }

        /// <summary>
        /// Gets whether has recurve criteria
        /// </summary>
        bool HasRecurveCriteria { get; }

        /// <summary>
        /// Gets whether has join item
        /// </summary>
        bool HasJoin { get; }

        /// <summary>
        /// Gets whether is a complex query
        /// Include subquery,recurve criteria,join item
        /// </summary>
        bool IsComplexQuery { get; }

        /// <summary>
        /// Gets the recurve criteria
        /// </summary>
        RecurveCriteria RecurveCriteria { get; }

        /// <summary>
        /// Gets or sets whether must return value for success
        /// </summary>
        bool MustReturnValueOnSuccess { get; set; }

        /// <summary>
        /// Gets whether the query object is obsolete
        /// </summary>
        bool IsObsolete { get; }

        /// <summary>
        /// Gets the join items
        /// </summary>
        IEnumerable<JoinItem> JoinItems { get; }

        /// <summary>
        /// Gets whether is a none condition object
        /// </summary>
        bool NoneCondition { get; }

        /// <summary>
        /// Gets the atomic condition count
        /// </summary>
        int AtomicConditionCount { get; }

        /// <summary>
        /// Gets all condition field names
        /// </summary>
        IEnumerable<string> AllConditionFieldNames { get; }

        /// <summary>
        /// Gets all subqueries
        /// </summary>
        IEnumerable<IQuery> Subqueries { get; }

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        DataIsolationLevel? IsolationLevel { get; set; }

        #endregion

        #region Methods

        #region And

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery And(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null);

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="eachFieldConnectOperator">each field codition connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery And(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null, params string[] fieldNames);

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="criteria">Criteria</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery And<TQueryModel>(Expression<Func<TQueryModel, bool>> criteria) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="field">Field expression</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery And<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="eachFieldConnectOperator">Each field connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery And<TQueryModel>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery And(IQuery groupQuery);

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
        IQuery Or(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null);

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <param name="eachFieldConnectOperator">Each field connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fieldNames">field collection</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Or(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null, params string[] fieldNames);

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="criteria">Criteria</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Or<TQueryModel>(Expression<Func<TQueryModel, bool>> criteria) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Or<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="eachFieldConnectOperator">Each field connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Or<TQueryModel>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Add a condition with 'or'
        /// </summary>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Or(IQuery groupQuery);

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
        IQuery Equal(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Equal(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false);

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Equal<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Equal<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Equal<SourceQueryModel, SubqueryQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<SubqueryQueryModel, dynamic>> subqueryField, bool or = false) where SourceQueryModel : QueryModel<SourceQueryModel> where SubqueryQueryModel : QueryModel<SubqueryQueryModel>;

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
        IQuery NotEqual(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqual(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false);

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery NotEqual<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>;

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
        IQuery LessThan(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThan(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false);

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThan<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThan<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery LessThan<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>;

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
        IQuery LessThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqual(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false);

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery LessThanOrEqual<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>;

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
        IQuery GreaterThan(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThan(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false);

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThan<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThan<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery GreaterThan<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>;

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
        IQuery GreaterThanOrEqual(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false);

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqual<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery GreaterThanOrEqual<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>;

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
        IQuery In(string fieldName, IEnumerable value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery In(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false);

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery In<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery In<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery In<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>;

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
        IQuery NotIn(string fieldName, IEnumerable value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotIn(string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false);

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotIn<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotIn<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery NotIn<TSourceQueryModel, TSubqueryQueryModel>(Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : QueryModel<TSourceQueryModel> where TSubqueryQueryModel : QueryModel<TSubqueryQueryModel>;

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
        IQuery Like(string fieldName, string value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Like<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery NotLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Not Like Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery BeginLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Begin Like Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery BeginLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery NotBeginLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Not Begin Like Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotBeginLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery EndLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// End Like Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EndLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

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
        IQuery NotEndLike(string fieldName, string value, bool or = false, ICriteriaConverter converter = null);

        /// <summary>
        /// Not End Like Condition
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEndLike<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        #endregion

        #region IsNull

        /// <summary>
        /// Field is null
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery IsNull(string fieldName, bool or = false);

        /// <summary>
        /// Field is null
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery IsNull<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

        #endregion

        #region NotNull

        /// <summary>
        /// Field is not null
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotNull(string fieldName, bool or = false);

        /// <summary>
        /// Field is not null
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotNull<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, bool or = false) where TQueryModel : QueryModel<TQueryModel>;

        #endregion

        #region Sort Condition

        #region ASC

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Asc(string fieldName, ICriteriaConverter converter = null);

        /// <summary>
        ///Order by asc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Asc<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        #endregion

        #region DESC

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Desc(string fieldName, ICriteriaConverter converter = null);

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="field">Field</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Desc<TQueryModel>(Expression<Func<TQueryModel, dynamic>> field, ICriteriaConverter converter = null) where TQueryModel : QueryModel<TQueryModel>;

        #endregion

        #region Clear Order

        /// <summary>
        /// Clear order condition
        /// </summary>
        /// <returns>Return the newest IQuery object</returns>
        IQuery ClearOrder();

        #endregion

        #endregion

        #region Fields

        /// <summary>
        /// Add special fields need to query
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery AddQueryFields(params string[] fields);

        /// <summary>
        /// Add special fields need to query
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery AddQueryFields<TQueryModel>(params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Clear query fields
        /// </summary>
        /// <returns></returns>
        IQuery ClearQueryFields();

        /// <summary>
        /// Add special fields that don't query
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery AddNotQueryFields(params string[] fields);

        /// <summary>
        /// Add special fields that don't query
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery AddNotQueryFields<TQueryModel>(params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : QueryModel<TQueryModel>;

        /// <summary>
        /// Clear not query fields
        /// </summary>
        /// <returns>Return the newest IQuery object</returns>
        IQuery ClearNotQueryFields();

        /// <summary>
        /// Get actually query fields
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="forcePrimaryKey">Force return primary key</param>
        /// <param name="forceVersionKey">Force return version key</param>
        /// <returns>Return actually query fields</returns>
        IEnumerable<EntityField> GetActuallyQueryFields<TEntity>(bool forcePrimaryKey = true, bool forceVersionKey = true);

        /// <summary>
        /// Get actually query fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="forcePrimaryKey">Force return primary key</param>
        /// <param name="forceVersionKey">Force return version key</param>
        /// <returns>Return actially entity fields</returns>
        IEnumerable<EntityField> GetActuallyQueryFields(Type entityType, bool forcePrimaryKey = true, bool forceVersionKey = true);

        #endregion

        #region QueryText

        /// <summary>
        /// Set query text
        /// </summary>
        /// <param name="queryText">Query text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery SetQueryText(string queryText, object parameters = null);

        #endregion

        #region Load Propertys

        /// <summary>
        /// Set load data propertys
        /// </summary>
        /// <param name="propertys">Propertys</param>
        void SetLoadPropertys(Dictionary<string, bool> propertys);

        /// <summary>
        /// Set load data propertys
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="allowLoad">Whether allow load data</param>
        /// <param name="propertys">Propertys</param>
        void SetLoadPropertys<TQueryModel>(bool allowLoad, params Expression<Func<TQueryModel, dynamic>>[] propertys);

        /// <summary>
        /// Property is allow load data
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return whether property allow load data</returns>
        bool AllowLoad(string propertyName);

        /// <summary>
        /// Property is allow load data
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="property">Property</param>
        /// <returns>Return whether property allow load data</returns>
        bool AllowLoad<TQueryModel>(Expression<Func<TQueryModel, dynamic>> property);

        #endregion

        #region Get Special Keys Equal Values

        /// <summary>
        /// Get special keys equal values
        /// </summary>
        /// <param name="keys">keys</param>
        /// <returns>Return key and values</returns>
        Dictionary<string, List<dynamic>> GetKeysEqualValue(IEnumerable<string> keys);

        #endregion

        #region Get Expression

        /// <summary>
        /// Get expression
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Return expression</returns>
        Func<T, bool> GetQueryExpression<T>();

        #endregion

        #region Order Datas

        /// <summary>
        /// Order datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <returns>Return the sorted data set</returns>
        IEnumerable<T> Sort<T>(IEnumerable<T> datas);

        #endregion

        #region Recurve

        /// <summary>
        /// Set recurve criteria
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="relationKey">Relation key</param>
        /// <param name="direction">Recurve direction</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery SetRecurve(string key, string relationKey, RecurveDirection direction = RecurveDirection.Down);

        /// <summary>
        /// set recurve criteria
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="key">Key</param>
        /// <param name="relationKey">Relation key</param>
        /// <param name="direction">Recurve direction</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery SetRecurve<TQueryModel>(Expression<Func<TQueryModel, dynamic>> key, Expression<Func<TQueryModel, dynamic>> relationKey, RecurveDirection direction = RecurveDirection.Down) where TQueryModel : QueryModel<TQueryModel>;

        #endregion

        #region Obsolete

        /// <summary>
        /// Bbsolete query
        /// </summary>
        void Obsolete();

        /// <summary>
        /// Cancel obsolete
        /// </summary>
        void Activate();

        #endregion

        #region Clone

        /// <summary>
        /// Copy a IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        IQuery Copy();

        /// <summary>
        /// Deep copy a IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        IQuery DeepCopy();

        /// <summary>
        /// Copy a IQuery object without conditions
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        IQuery CopyWithoutConditions();

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
        IQuery Join(Dictionary<string, string> joinFields, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Join(string sourceField, string targetField, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery);

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
        IQuery Join<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Join(JoinType joinType, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region Inner Join

        #region InnerJoin helper

        /// <summary>
        /// Add a inner join query
        /// </summary>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery InnerJoin(JoinOperator joinOperator, params IQuery[] joinQuerys);

        /// <summary>
        /// Add a inner join query
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery InnerJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a inner join query
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery InnerJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery);

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
        IQuery InnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a inner join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery InnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region Equal InnerJoin

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the Equal operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region NotEqual InnerJoin

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region LessThanOrEqual InnerJoin

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region LessThan InnerJoin

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the LessThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region GreaterThan InnerJoin

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region GreaterThanOrEqual InnerJoin

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualInnerJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualInnerJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a inner join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualInnerJoin(params IQuery[] joinQuerys);

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
        IQuery LeftJoin(JoinOperator joinOperator, params IQuery[] joinQuerys);

        /// <summary>
        /// Add a left join query
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LeftJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a left join query
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LeftJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery);

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
        IQuery LeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a left join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region Equal LeftJoin

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the Equal operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region NotEqual LeftJoin

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the NotEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region LessThanOrEqual LeftJoin

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region LessThan LeftJoin

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the LessThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region GreaterThan LeftJoin

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region GreaterThanOrEqual LeftJoin

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualLeftJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualLeftJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a left join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualLeftJoin(params IQuery[] joinQuerys);

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
        IQuery RightJoin(JoinOperator joinOperator, params IQuery[] joinQuerys);

        /// <summary>
        /// Add a right join
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery RightJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a right join
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery RightJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery);

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
        IQuery RightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a right join
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery RightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region Equal RightJoin

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the Equal operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region NotEqual RightJoin

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the NotEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region LessThanOrEqual RightJoin

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region LessThan RightJoin

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the LessThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region GreaterThan RightJoin

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region GreaterThanOrEqual RightJoin

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualRightJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualRightJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a right join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualRightJoin(params IQuery[] joinQuerys);

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
        IQuery FullJoin(JoinOperator joinOperator, params IQuery[] joinQuerys);

        /// <summary>
        /// Add a full join
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery FullJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a full join
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery FullJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery);

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
        IQuery FullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a full join
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery FullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region Equal FullJoin

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the Equal operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery EqualFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region NotEqual FullJoin

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the NotEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery NotEqualFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region LessThanOrEqual FullJoin

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanOrEqualFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region LessThan FullJoin

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the LessThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery LessThanFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region GreaterThan FullJoin

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the GreaterThan operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region GreaterThanOrEqual FullJoin

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualFullJoin<TSource, TTarget>(Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualFullJoin<TSource>(Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// Add a full join by using the GreaterThanOrEqual operation
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery GreaterThanOrEqualFullJoin(params IQuery[] joinQuerys);

        #endregion

        #endregion

        #region CrossJoin

        /// <summary>
        /// Add a cross join
        /// </summary>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery CrossJoin(params IQuery[] joinQuerys);

        #endregion

        #endregion

        #region GlobalCondition

        /// <summary>
        /// Set global condition
        /// </summary>
        /// <param name="globalCondition">Global condition</param>
        /// <param name="queryOperator">Query operator</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery SetGlobalCondition(IQuery globalCondition, QueryOperator queryOperator);

        /// <summary>
        /// Whether allow set global condition
        /// </summary>
        /// <returns>Return whether allow set global condition</returns>
        bool AllowSetGlobalCondition();

        #endregion

        #region Reset

        /// <summary>
        /// Reset IQuery object
        /// </summary>
        void Reset();

        #endregion

        #region CancellationToken

        /// <summary>
        /// Set cancellation token
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery SetCancellationToken(CancellationToken? cancellationToken);

        /// <summary>
        /// Get cancellation token
        /// </summary>
        /// <returns>Cancellation Token</returns>
        CancellationToken? GetCancellationToken();

        #endregion

        #region Entity Type

        /// <summary>
        /// Get the entity type associated with IQuery
        /// </summary>
        /// <returns>Return entity type</returns>
        Type GetEntityType();

        /// <summary>
        /// Set the entity type associated with IQuery
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>Return entity type</returns>
        IQuery SetEntityType(Type entityType);

        #endregion

        #region Paging

        /// <summary>
        /// Set paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        IQuery SetPaging(PagingFilter pagingFilter);

        #endregion

        #endregion
    }
}
