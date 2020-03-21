using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;
using EZNEW.Framework.Paging;
using EZNEW.Develop.CQuery.CriteriaConvert;
using EZNEW.Develop.Entity;
using EZNEW.Develop.DataAccess;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// all iquery instance and criteria inherit this interface
    /// </summary>
    public interface IQueryItem
    {

    }

    /// <summary>
    /// query info interface
    /// </summary>
    public interface IQuery : IQueryItem
    {
        #region property

        /// <summary>
        /// entity type
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// all criterias or other IQuery items
        /// </summary>
        List<Tuple<QueryOperator, IQueryItem>> Criterias
        {
            get;
        }

        /// <summary>
        /// orders
        /// </summary>
        List<OrderCriteria> Orders
        {
            get;
        }

        /// <summary>
        /// specific query fields(it's priority greater than NoQueryFields)
        /// </summary>
        List<string> QueryFields
        {
            get;
        }

        /// <summary>
        /// specific not query fields(it's priority less than QueryFields)
        /// </summary>
        List<string> NotQueryFields
        {
            get;
        }

        /// <summary>
        /// paging
        /// </summary>
        PagingFilter PagingInfo
        {
            get; set;
        }

        /// <summary>
        /// query text
        /// </summary>
        string QueryText
        {
            get;
        }

        /// <summary>
        /// query text parameter
        /// </summary>
        dynamic QueryTextParameters
        {
            get;
        }

        /// <summary>
        /// query command type
        /// </summary>
        QueryCommandType QueryType
        {
            get;
        }

        /// <summary>
        /// return size
        /// </summary>
        int QuerySize
        {
            get; set;
        }

        /// <summary>
        /// allow load propertys
        /// </summary>
        Dictionary<string, bool> LoadPropertys
        {
            get;
        }

        /// <summary>
        /// has sub query
        /// </summary>
        bool HasSubQuery
        {
            get;
        }

        /// <summary>
        /// has recurve criteria
        /// </summary>
        bool HasRecurveCriteria { get; }

        /// <summary>
        /// has join
        /// </summary>
        bool HasJoin { get; }

        /// <summary>
        /// complex query
        /// </summary>
        bool IsComplexQuery { get; }

        /// <summary>
        /// recurve criteria
        /// </summary>
        RecurveCriteria RecurveCriteria
        {
            get;
        }

        /// <summary>
        /// verify result method
        /// </summary>
        Func<int, bool> VerifyResult { get; set; }

        /// <summary>
        /// direct return if query is obsolete
        /// </summary>
        bool IsObsolete { get; }

        /// <summary>
        /// join items
        /// </summary>
        List<JoinItem> JoinItems { get; }

        /// <summary>
        /// none condition
        /// </summary>
        bool NoneCondition { get; }

        /// <summary>
        /// atomic condition count
        /// </summary>
        int AtomicConditionCount { get; }

        /// <summary>
        /// all condition field names
        /// </summary>
        List<string> AllConditionFieldNames { get; }

        /// <summary>
        /// all subqueries
        /// </summary>
        List<IQuery> Subqueries { get; }

        /// <summary>
        /// data isolation level
        /// </summary>
        DataIsolationLevel? IsolationLevel { get; set; }

        #endregion

        #region method

        #region and

        /// <summary>
        /// connect condition by 'and'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="criteria">criteria</param>
        /// <returns>return newest query object</returns>
        IQuery And<T>(Expression<Func<T, bool>> criteria) where T : QueryModel<T>;

        /// <summary>
        /// connect condition by 'and'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field expression</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery And<T>(Expression<Func<T, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// connect condition by 'and'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="fields">field type</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery And<T>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params Expression<Func<T, dynamic>>[] fields) where T : QueryModel<T>;

        /// <summary>
        /// connect condition by 'and'
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery And(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null);

        /// <summary>
        /// connect condition by 'and'
        /// </summary>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <param name="fieldNames">field collection</param>
        /// <returns>return newest query object</returns>
        IQuery And(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params string[] fieldNames);

        /// <summary>
        /// connect condition by 'and'
        /// </summary>
        /// <param name="groupQuery">group query object</param>
        /// <returns>return newest query object</returns>
        IQuery And(IQuery groupQuery);

        #endregion

        #region or

        /// <summary>
        /// connect condition by 'or'
        /// </summary>
        /// <typeparam name="T">datatype</typeparam>
        /// <param name="criteria">criteria</param>
        /// <returns>return newest query object</returns>
        IQuery Or<T>(Expression<Func<T, bool>> criteria) where T : QueryModel<T>;

        /// <summary>
        /// connect condition by 'or'
        /// </summary>
        /// <typeparam name="T">datatype</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Or<T>(Expression<Func<T, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// connect condition by 'or'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <param name="fields">field type</param>
        /// <returns>return newest query object</returns>
        IQuery Or<T>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params Expression<Func<T, dynamic>>[] fields) where T : QueryModel<T>;

        /// <summary>
        /// connect condition by 'or'
        /// </summary>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <param name="fieldNames">field collection</param>
        /// <returns>return newest query object</returns>
        IQuery Or(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params string[] fieldNames);

        /// <summary>
        /// connect condition by 'or'
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Or(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null);

        /// <summary>
        /// connect condition by 'or'
        /// </summary>
        /// <param name="groupQuery">group query object</param>
        /// <returns>return newest query object</returns>
        IQuery Or(IQuery groupQuery);

        #endregion

        #region equal

        /// <summary>
        /// equal condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Equal(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// equal condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryFieldName">sub query field</param>
        /// <returns>return newest query object</returns>
        IQuery Equal(string fieldName, IQuery subQuery, string subQueryFieldName = "");

        /// <summary>
        /// equal condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Equal<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// equal condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest query object</returns>
        IQuery Equal<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// equal condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery Equal<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region not equal

        /// <summary>
        /// not equal condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery NotEqual(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// not equal condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryFieldName">sub query field</param>
        /// <returns>return newest query object</returns>
        IQuery NotEqual(string fieldName, IQuery subQuery, string subQueryFieldName = "");

        /// <summary>
        /// not equal condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery NotEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// not equal condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest query object</returns>
        IQuery NotEqual<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// not equal condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery NotEqual<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region less than

        /// <summary>
        /// less than condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery LessThan(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// less than condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest query object</returns>
        IQuery LessThan(string fieldName, IQuery subQuery, string subQueryField = "");

        /// <summary>
        /// less than condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery LessThan<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// less than condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest query object</returns>
        IQuery LessThan<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// less than condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery LessThan<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region less than or equal

        /// <summary>
        /// less than or equal condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery LessThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// less than or equal condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest query object</returns>
        IQuery LessThanOrEqual(string fieldName, IQuery subQuery, string subQueryField = "");

        /// <summary>
        /// less than or equal condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest query object</returns>
        IQuery LessThanOrEqual<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// less than or equal condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery LessThanOrEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// less than or equal condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery LessThanOrEqual<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region greater than

        /// <summary>
        /// greater than condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery GreaterThan(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// greater than condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest query object</returns>
        IQuery GreaterThan(string fieldName, IQuery subQuery, string subQueryField = "");

        /// <summary>
        /// greater than condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery GreaterThan<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// greater than condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest query object</returns>
        IQuery GreaterThan<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// greater than condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery GreaterThan<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region greater than or equal

        /// <summary>
        /// greater than or equal condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery GreaterThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// greater than or equal condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest query object</returns>
        IQuery GreaterThanOrEqual(string fieldName, IQuery subQuery, string subQueryField = "");

        /// <summary>
        /// greater than or equal condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery GreaterThanOrEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// greater than or equal condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest query object</returns>
        IQuery GreaterThanOrEqual<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// greater than or equal condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqual<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region in

        /// <summary>
        /// include value condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery In(string fieldName, IEnumerable value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// include value condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryFieldName">sub query field name</param>
        /// <returns>return newest query object</returns>
        IQuery In(string fieldName, IQuery subQuery, string subQueryFieldName = "");

        /// <summary>
        /// include value condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery In<T>(Expression<Func<T, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// include value condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest query object</returns>
        IQuery In<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// include value condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest query object</returns>
        IQuery In<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryField) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region not in

        /// <summary>
        /// not include condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery NotIn(string fieldName, IEnumerable value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// not include condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryFieldName">sub query field name</param>
        /// <returns>return newest query object</returns>
        IQuery NotIn(string fieldName, IQuery subQuery, string subQueryFieldName = "");

        /// <summary>
        /// not include condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery NotIn<T>(Expression<Func<T, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// not include condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest query object</returns>
        IQuery NotIn<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// not include condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest query object</returns>
        IQuery NotIn<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryField) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region like

        /// <summary>
        /// like condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Like(string fieldName, string value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// like condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Like<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        #endregion

        #region not like

        /// <summary>
        /// not like condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery NotLike(string fieldName, string value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// not like condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery NotLike<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        #endregion

        #region begin like

        /// <summary>
        /// begin like condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery BeginLike(string fieldName, string value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// begin like condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery BeginLike<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        #endregion

        #region not begin like

        /// <summary>
        /// not begin like condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery NotBeginLike(string fieldName, string value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// not begin like condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery NotBeginLike<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        #endregion

        #region end like

        /// <summary>
        /// end like condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery EndLike(string fieldName, string value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// end like condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery EndLike<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        #endregion

        #region not end like

        /// <summary>
        /// not end like condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery NotEndLike(string fieldName, string value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// not end like condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery NotEndLike<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        #endregion

        #region is null

        /// <summary>
        /// field is null
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery IsNull(string fieldName, bool or = false);

        /// <summary>
        /// field is null
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery IsNull<T>(Expression<Func<T, dynamic>> field, bool or = false) where T : QueryModel<T>;

        #endregion

        #region not null

        /// <summary>
        /// field is not null
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery NotNull(string fieldName, bool or = false);

        /// <summary>
        /// field is not null
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>return newest query object</returns>
        IQuery NotNull<T>(Expression<Func<T, dynamic>> field, bool or = false) where T : QueryModel<T>;

        #endregion

        #region asc

        /// <summary>
        /// order by asc
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Asc<T>(Expression<Func<T, dynamic>> field, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// order by asc
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Asc(string fieldName, ICriteriaConvert convert = null);

        #endregion

        #region desc

        /// <summary>
        /// order by desc
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Desc<T>(Expression<Func<T, dynamic>> field, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// order by desc
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest query object</returns>
        IQuery Desc(string fieldName, ICriteriaConvert convert = null);

        #endregion

        #region fields

        /// <summary>
        /// add special fields need to query
        /// </summary>
        /// <param name="fields">fields</param>
        /// <returns>return newest query object</returns>
        IQuery AddQueryFields(params string[] fields);

        /// <summary>
        /// add special fields need to query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="fieldExpressions">field expression</param>
        /// <returns>return newest query object</returns>
        IQuery AddQueryFields<T>(params Expression<Func<T, dynamic>>[] fieldExpressions) where T : QueryModel<T>;

        /// <summary>
        /// add special fields that don't query
        /// </summary>
        /// <param name="fields">fields</param>
        /// <returns>return newest query object</returns>
        IQuery AddNotQueryFields(params string[] fields);

        /// <summary>
        /// add special fields that don't query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="fieldExpressions">field expression</param>
        /// <returns>return newest query object</returns>
        IQuery AddNotQueryFields<T>(params Expression<Func<T, dynamic>>[] fieldExpressions) where T : QueryModel<T>;

        /// <summary>
        /// get actually query fields
        /// </summary>
        /// <returns></returns>
        List<EntityField> GetActuallyQueryFields<ET>(bool forcePrimaryKey = true, bool forceVersion = true);

        /// <summary>
        /// get actually query fields
        /// </summary>
        /// <returns></returns>
        List<EntityField> GetActuallyQueryFields(Type entityType, bool forcePrimaryKey = true, bool forceVersion = true);

        #endregion

        #region query text

        /// <summary>
        /// set query text
        /// </summary>
        /// <param name="queryText">query text</param>
        /// <param name="parameters">parameters</param>
        /// <returns>return newest query object</returns>
        IQuery SetQueryText(string queryText, object parameters = null);

        #endregion

        #region load propertys

        /// <summary>
        /// set load propertys
        /// </summary>
        /// <param name="propertys">load propertys</param>
        void SetLoadPropertys(Dictionary<string, bool> propertys);

        /// <summary>
        /// set load propertys
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <param name="allowLoad">allow load</param>
        /// <param name="propertys">propertys</param>
        void SetLoadPropertys<T>(bool allowLoad, params Expression<Func<T, dynamic>>[] propertys);

        /// <summary>
        /// property is allow load data
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        /// <returns>allow load data</returns>
        bool AllowLoad(string propertyName);

        /// <summary>
        /// property is allow load data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">propertyName</param>
        /// <returns>allow load data</returns>
        bool AllowLoad<T>(Expression<Func<T, dynamic>> property);

        #endregion

        #region get special keys equal values

        /// <summary>
        /// get special keys equal values
        /// </summary>
        /// <param name="keys">keys</param>
        /// <returns></returns>
        Dictionary<string, List<dynamic>> GetKeysEqualValue(IEnumerable<string> keys);

        #endregion

        #region get expression

        /// <summary>
        /// get query expression
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <returns></returns>
        Func<T, bool> GetQueryExpression<T>();

        #endregion

        #region order datas

        /// <summary>
        /// order datas
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        IEnumerable<T> Order<T>(IEnumerable<T> datas);

        #endregion

        #region recurve

        /// <summary>
        /// set recurve criteria
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="relationKey">relation key</param>
        /// <param name="direction">recurve direction</param>
        /// <returns></returns>
        IQuery SetRecurve(string key, string relationKey, RecurveDirection direction = RecurveDirection.Down);

        /// <summary>
        /// set recurve criteria
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="relationKey">relation key</param>
        /// <param name="direction">recurve direction</param>
        /// <returns></returns>
        IQuery SetRecurve<T>(Expression<Func<T, dynamic>> key, Expression<Func<T, dynamic>> relationKey, RecurveDirection direction = RecurveDirection.Down);

        #endregion

        #region obsolete

        /// <summary>
        /// obsolete query
        /// </summary>
        void Obsolete();

        /// <summary>
        /// cancel obsolete
        /// </summary>
        void Activate();

        #endregion

        #region clone

        /// <summary>
        /// clone
        /// </summary>
        /// <returns></returns>
        IQuery Clone();

        #endregion

        #region join

        #region join util

        /// <summary>
        /// join
        /// </summary>
        /// <param name="joinFields">join fields</param>
        /// <param name="joinType">join type</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery Join(Dictionary<string, string> joinFields, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinType">join type</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery Join(string sourceField, string targetField, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinType">join type</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery Join<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinType">join type</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery Join(JoinType joinType, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region inner join

        #region inner join helper

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuerys">join querys</param>
        /// <returns>return newest query object instance</returns>
        IQuery InnerJoin(JoinOperator joinOperator, params IQuery[] joinQuerys);

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery InnerJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery InnerJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery InnerJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery InnerJoin<T>(Expression<Func<T, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region equal inner join

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualInnerJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualInnerJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery EqualInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region not equal inner join

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualInnerJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualInnerJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery NotEqualInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region less than or equal inner join

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualInnerJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualInnerJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region less than inner join

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanInnerJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanInnerJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery LessThanInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region greater than inner join

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanInnerJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanInnerJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery GreaterThanInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #region greater than or equal inner join

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualInnerJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// inner join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualInnerJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualInnerJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualInnerJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualInnerJoin(params IQuery[] joinQuerys);

        #endregion

        #endregion

        #region left join

        #region left join helper

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuerys">join querys</param>
        /// <returns>return newest query object instance</returns>
        IQuery LeftJoin(JoinOperator joinOperator, params IQuery[] joinQuerys);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LeftJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LeftJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// left query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LeftJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// left query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LeftJoin<T>(Expression<Func<T, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region equal left join

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualLeftJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualLeftJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery EqualLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region not equal left join

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// left query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualLeftJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// left query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualLeftJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// left query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery NotEqualLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region less than or equal left join

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualLeftJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualLeftJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region less than left join

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanLeftJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanLeftJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery LessThanLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region greater than left join

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanLeftJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanLeftJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery GreaterThanLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #region greater than or equal left join

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualLeftJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualLeftJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualLeftJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualLeftJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// left join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualLeftJoin(params IQuery[] joinQuerys);

        #endregion

        #endregion

        #region right join

        #region right join helper

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuerys">join querys</param>
        /// <returns>return newest query object instance</returns>
        IQuery RightJoin(JoinOperator joinOperator, params IQuery[] joinQuerys);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery RightJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery RightJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// right query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery RightJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// right query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery RightJoin<T>(Expression<Func<T, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region equal right join

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualRightJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualRightJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery EqualRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region not equal right join

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// right query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualRightJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// right query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualRightJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// right query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery NotEqualRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region less than or equal right join

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualRightJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualRightJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region less than right join

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanRightJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanRightJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery LessThanRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region greater than right join

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanRightJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanRightJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery GreaterThanRightJoin(params IQuery[] joinQuerys);

        #endregion

        #region greater than or equal right join

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualRightJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualRightJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualRightJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualRightJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// right join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualRightJoin(params IQuery[] joinQuerys);

        #endregion

        #endregion

        #region full join

        #region full join helper

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuerys">join querys</param>
        /// <returns>return newest query object instance</returns>
        IQuery FullJoin(JoinOperator joinOperator, params IQuery[] joinQuerys);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery FullJoin(string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery FullJoin(string joinField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// full query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery FullJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// full query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinOperator">join operator</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery FullJoin<T>(Expression<Func<T, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery);

        #endregion

        #region equal full join

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualFullJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery EqualFullJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery EqualFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region not equal full join

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// full query
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualFullJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// full query
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery NotEqualFullJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// full query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery NotEqualFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region less than or equal full join

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualFullJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualFullJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery LessThanOrEqualFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region less than full join

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanFullJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery LessThanFullJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery LessThanFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region greater than full join

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanFullJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanFullJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery GreaterThanFullJoin(params IQuery[] joinQuerys);

        #endregion

        #region greater than or equal full join

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualFullJoin(string sourceField, string targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualFullJoin(string joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="sourceField">source field</param>
        /// <param name="targetField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualFullJoin<Source, Target>(Expression<Func<Source, dynamic>> sourceField, Expression<Func<Target, dynamic>> targetField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinField">target field</param>
        /// <param name="joinQuery">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualFullJoin<T>(Expression<Func<T, dynamic>> joinField, IQuery joinQuery);

        /// <summary>
        /// full join
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqualFullJoin(params IQuery[] joinQuerys);

        #endregion

        #endregion

        #region cross join

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery CrossJoin(params IQuery[] joinQuerys);

        #endregion

        #endregion

        #region entity type

        /// <summary>
        /// set entity type
        /// </summary>
        /// <param name="entityType">entity type</param>
        void SetEntityType(Type entityType);

        #endregion

        #region global condition

        /// <summary>
        /// set global condition
        /// </summary>
        /// <param name="globalCondition">global condition</param>
        /// <param name="queryOperator">query operator</param>
        /// <returns></returns>
        IQuery SetGlobalCondition(IQuery globalCondition, QueryOperator queryOperator);

        /// <summary>
        /// allow set global condition
        /// </summary>
        /// <returns></returns>
        bool AllowSetGlobalCondition();

        #endregion

        #endregion
    }
}
