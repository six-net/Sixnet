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
        /// <returns>return newest instance</returns>
        IQuery Equal(string fieldName, IQuery subQuery);

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
        /// <returns>return newest instance</returns>
        IQuery NotEqual(string fieldName, IQuery subQuery);

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
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery LessThan<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

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
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery LessThanOrEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

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
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThan<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

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
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <param name="or">connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="convert">criterial convert</param>
        /// <returns>return newest instance</returns>
        IQuery GreaterThanOrEqual<T>(Expression<Func<T, dynamic>> field, dynamic value, bool or = false, ICriteriaConvert convert = null) where T : QueryModel<T>;

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
        /// <returns>return newest instance</returns>
        IQuery In(string fieldName, IQuery subQuery);

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
        /// Not Include
        /// </summary>
        /// <param name="fieldName">field</param>
        /// <param name="subQuery">sub query</param>
        /// <returns>return newest instance</returns>
        IQuery NotIn(string fieldName, IQuery subQuery);

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
        List<string> GetActuallyQueryFields<ET>(bool forcePrimaryKey = true, bool forceVersion = true);

        /// <summary>
        /// get actually query fields
        /// </summary>
        /// <returns></returns>
        List<string> GetActuallyQueryFields(Type entityType, bool forcePrimaryKey = true, bool forceVersion = true);

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
        List<Dictionary<string, dynamic>> GetKeysEqualValue(IEnumerable<string> keys);

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

        #endregion
    }
}
