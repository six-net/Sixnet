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
        #region Property

        ///// <summary>
        ///// Query Object Name(usually,it's the table name)
        ///// </summary>
        //string ObjectName
        //{
        //    get;
        //}

        /// <summary>
        /// query model type
        /// </summary>
        Type QueryModelType
        {
            get; set;
        }

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
        /// Allow Load Propertys
        /// </summary>
        Dictionary<string, bool> LoadPropertys
        {
            get;
        }

        /// <summary>
        /// Has Sub Query
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
        /// Recurve Criteria
        /// </summary>
        RecurveCriteria RecurveCriteria
        {
            get;
        }

        /// <summary>
        /// Verify Result Method
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

        #endregion

        #region Method

        #region And

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="criteria">criteria</param>
        /// <returns>return newest instance</returns>
        IQuery And<T>(Expression<Func<T, bool>> criteria) where T : QueryModel<T>;

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field expression</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery And<T>(Expression<Func<T, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="fields">field type</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery And<T>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params Expression<Func<T, dynamic>>[] fields) where T : QueryModel<T>;

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery And(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null);

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <param name="fieldNames">field collection</param>
        /// <returns>return newest instance</returns>
        IQuery And(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params string[] fieldNames);

        /// <summary>
        /// Connect a condition with 'and'
        /// </summary>
        /// <param name="subQuery">sub query instance</param>
        /// <returns>return newest instance</returns>
        IQuery And(IQuery subQuery);

        #endregion

        #region OR

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <typeparam name="T">datatype</typeparam>
        /// <param name="criteria">criteria</param>
        /// <returns>return newest instance</returns>
        IQuery Or<T>(Expression<Func<T, bool>> criteria) where T : QueryModel<T>;

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <typeparam name="T">datatype</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Or<T>(Expression<Func<T, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <param name="fields">field type</param>
        /// <returns>return newest instance</returns>
        IQuery Or<T>(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params Expression<Func<T, dynamic>>[] fields) where T : QueryModel<T>;

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <param name="eachFieldConnectOperator">each field connect operator</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <param name="fieldNames">field collection</param>
        /// <returns>return newest instance</returns>
        IQuery Or(QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null, params string[] fieldNames);

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="operator">condition operator</param>
        /// <param name="value">value</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Or(string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConvert convert = null);

        /// <summary>
        /// Connect a condition with 'or'
        /// </summary>
        /// <param name="subQuery">sub query instance</param>
        /// <returns>return newest instance</returns>
        IQuery Or(IQuery subQuery);

        #endregion

        #region Equal

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Equal(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryFieldName">sub query field</param>
        /// <returns>return newest instance</returns>
        IQuery Equal(string fieldName, IQuery subQuery, string subQueryFieldName = "");

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Equal<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest instance</returns>
        IQuery Equal<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// Equal Condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery Equal<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region Not Equal

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery NotEqual(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryFieldName">sub query field</param>
        /// <returns>return newest instance</returns>
        IQuery NotEqual(string fieldName, IQuery subQuery, string subQueryFieldName = "");

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery NotEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest instance</returns>
        IQuery NotEqual<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery NotEqual<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region LessThan

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery LessThan(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest instance</returns>
        IQuery LessThan(string fieldName, IQuery subQuery, string subQueryField = "");

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery LessThan<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest instance</returns>
        IQuery LessThan<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// Less Than Condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery LessThan<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region LessThanOrEqual

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery LessThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest instance</returns>
        IQuery LessThanOrEqual(string fieldName, IQuery subQuery, string subQueryField = "");

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest instance</returns>
        IQuery LessThanOrEqual<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery LessThanOrEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Less Than Or Equal Condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery LessThanOrEqual<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region GreaterThan

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThan(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThan(string fieldName, IQuery subQuery, string subQueryField = "");

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThan<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThan<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// Greater Than Condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery GreaterThan<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region GreaterThanOrEqual

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThanOrEqual(string fieldName, dynamic value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThanOrEqual(string fieldName, IQuery subQuery, string subQueryField = "");

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThanOrEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThanOrEqual<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// Greater Than Or Equal Condition
        /// </summary>
        /// <typeparam name="SourceQueryModel">source query model</typeparam>
        /// <typeparam name="SubQueryModel">sub query model</typeparam>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryfield">sub query field</param>
        /// <returns></returns>
        IQuery GreaterThanOrEqual<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryfield) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region IN

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery In(string fieldName, IEnumerable value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryFieldName">sub query field name</param>
        /// <returns>return newest instance</returns>
        IQuery In(string fieldName, IQuery subQuery, string subQueryFieldName = "");

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery In<T>(Expression<Func<T, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest instance</returns>
        IQuery In<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest instance</returns>
        IQuery In<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryField) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region Not In

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery NotIn(string fieldName, IEnumerable value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryFieldName">sub query field name</param>
        /// <returns>return newest instance</returns>
        IQuery NotIn(string fieldName, IQuery subQuery, string subQueryFieldName = "");

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery NotIn<T>(Expression<Func<T, dynamic>> field, IEnumerable value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Not Include
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest instance</returns>
        IQuery NotIn<T>(Expression<Func<T, dynamic>> field, IQuery subQuery) where T : QueryModel<T>;

        /// <summary>
        /// Include Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="subQuery">sub query</param>
        /// <param name="subQueryField">sub query field</param>
        /// <returns>return newest instance</returns>
        IQuery NotIn<SourceQueryModel, SubQueryModel>(Expression<Func<SourceQueryModel, dynamic>> field, IQuery subQuery, Expression<Func<SubQueryModel, dynamic>> subQueryField) where SourceQueryModel : QueryModel<SourceQueryModel> where SubQueryModel : QueryModel<SubQueryModel>;

        #endregion

        #region Like

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Like(string fieldName, string value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Like<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        #endregion

        #region BeginLike

        /// <summary>
        /// Begin Like Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery BeginLike(string fieldName, string value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// Begin Like Condition
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery BeginLike<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        #endregion

        #region EndLike

        /// <summary>
        /// End Like Condition
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery EndLike(string fieldName, string value, bool or = false, ICriteriaConvert convert = null);

        /// <summary>
        /// EndLike
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery EndLike<T>(Expression<Func<T, dynamic>> field, string value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

        #endregion

        #region ASC

        /// <summary>
        /// Order By ASC
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Asc<T>(Expression<Func<T, dynamic>> field, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Order By ASC
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Asc(string fieldName, ICriteriaConvert convert = null);

        #endregion

        #region DESC

        /// <summary>
        /// Order By DESC
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="field">field</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Desc<T>(Expression<Func<T, dynamic>> field, ICriteriaConvert convert = null) where T : QueryModel<T>;

        /// <summary>
        /// Order By DESC
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery Desc(string fieldName, ICriteriaConvert convert = null);

        #endregion

        #region Fields

        /// <summary>
        /// Add Special Fields Need To Query
        /// </summary>
        /// <param name="fields">fields</param>
        /// <returns>return newest instance</returns>
        IQuery AddQueryFields(params string[] fields);

        /// <summary>
        /// Add Special Fields Need To Query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="fieldExpression">field expression</param>
        /// <returns>return newest instance</returns>
        IQuery AddQueryFields<T>(Expression<Func<T, dynamic>> fieldExpression) where T : QueryModel<T>;

        /// <summary>
        /// Add Special Fields That don't Query
        /// </summary>
        /// <param name="fields">fields</param>
        /// <returns>return newest instance</returns>
        IQuery AddNotQueryFields(params string[] fields);

        /// <summary>
        /// Add Special Fields That don't Query
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <typeparam name="TProperty">field type</typeparam>
        /// <param name="fieldExpression">field expression</param>
        /// <returns>return newest instance</returns>
        IQuery AddNotQueryFields<T>(Expression<Func<T, dynamic>> fieldExpression) where T : QueryModel<T>;

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

        #region QueryText

        /// <summary>
        /// Set QueryText
        /// </summary>
        /// <param name="queryText">query text</param>
        /// <param name="parameters">parameters</param>
        /// <returns>return newest instance</returns>
        IQuery SetQueryText(string queryText, object parameters = null);

        #endregion

        #region Load Propertys

        /// <summary>
        /// Set Load Propertys
        /// </summary>
        /// <param name="propertys">load propertys</param>
        void SetLoadPropertys(Dictionary<string, bool> propertys);

        /// <summary>
        /// Set Load Propertys
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

        #region Get Special Keys Equal Values

        /// <summary>
        /// Get Special Keys Equal Values
        /// </summary>
        /// <param name="keys">keys</param>
        /// <returns></returns>
        Dictionary<string, List<dynamic>> GetKeysEqualValue(IEnumerable<string> keys);

        #endregion

        #region Get Expression

        /// <summary>
        /// Get Query Expression
        /// </summary>
        /// <typeparam name="T">Data Type</typeparam>
        /// <returns></returns>
        Func<T, bool> GetQueryExpression<T>();

        #endregion

        #region Order Datas

        /// <summary>
        /// Order Datas
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        IEnumerable<T> Order<T>(IEnumerable<T> datas);

        #endregion

        #region Recurve

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

        #region Obsolete

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

        #region Inner Join

        #region InnerJoin helper

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

        #region Equal InnerJoin

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

        #region NotEqual InnerJoin

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

        #region LessThanOrEqual InnerJoin

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

        #region LessThan InnerJoin

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

        #region GreaterThan InnerJoin

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

        #region GreaterThanOrEqual InnerJoin

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

        #region Left Join

        #region Left Join Helper

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

        #region Equal LeftJoin

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

        #region NotEqual LeftJoin

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

        #region LessThanOrEqual LeftJoin

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

        #region LessThan LeftJoin

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

        #region GreaterThan LeftJoin

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

        #region GreaterThanOrEqual LeftJoin

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

        #region Right Join

        #region Right Join Helper

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

        #region Equal RightJoin

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

        #region NotEqual RightJoin

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

        #region LessThanOrEqual RightJoin

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

        #region LessThan RightJoin

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

        #region GreaterThan RightJoin

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

        #region GreaterThanOrEqual RightJoin

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

        #region Full Join

        #region Full Join Helper

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

        #region Equal FullJoin

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

        #region NotEqual FullJoin

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

        #region LessThanOrEqual FullJoin

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

        #region LessThan FullJoin

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

        #region GreaterThan FullJoin

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

        #region GreaterThanOrEqual FullJoin

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

        #region CrossJoin

        /// <summary>
        /// join query
        /// </summary>
        /// <param name="joinQuerys">join query</param>
        /// <returns></returns>
        IQuery CrossJoin(params IQuery[] joinQuerys);

        #endregion

        #endregion

        #endregion
    }
}
