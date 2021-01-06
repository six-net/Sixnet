using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using EZNEW.Paging;
using EZNEW.Develop.CQuery.CriteriaConverter;
using EZNEW.Develop.DataAccess;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// All iquery instance and criteria inherit this interface
    /// </summary>
    public interface IQueryItem { }

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
        IEnumerable<KeyValuePair<string, object>> QueryTextParameters { get; }

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
        /// Gets whether has combine items
        /// </summary>
        bool HasCombine { get; }

        /// <summary>
        /// Gets whether has criteria converter
        /// </summary>
        bool HasConverter { get; }

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
        /// Gets the combine items
        /// </summary>
        IEnumerable<CombineItem> CombineItems { get; }

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

        #region Sort Condition

        #region Add order

        /// <summary>
        /// Add order
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Sort by desc</param>
        /// <param name="converter">Field converter</param>
        IQuery AddOrder(string fieldName, bool desc = false, ICriteriaConverter converter = null);

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
        IQuery AddQueryFields<TQueryModel>(params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>;

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
        IQuery AddNotQueryFields<TQueryModel>(params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>;

        /// <summary>
        /// Clear not query fields
        /// </summary>
        /// <returns>Return the newest IQuery object</returns>
        IQuery ClearNotQueryFields();

        /// <summary>
        /// Get actually query fields
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="forceMustFields">Whether return the must query fields</param>
        /// <returns>Return the newest IQuery object</returns>
        IEnumerable<string> GetActuallyQueryFields<TEntity>(bool forceMustFields);

        /// <summary>
        /// Get actually query fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="forceMustFields">Whether return the must query fields</param>
        /// <returns>Return the newest IQuery object</returns>
        IEnumerable<string> GetActuallyQueryFields(Type entityType, bool forceMustFields);

        /// <summary>
        /// Get actually query fields
        /// Item1: whether return entity full query fields
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="forceMustFields">Whether return the must query fields</param>
        /// <returns>Return actually query fields</returns>
        Tuple<bool, IEnumerable<string>> GetActuallyQueryFieldsWithSign(Type entityType, bool forceMustFields);

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
        /// Set allow load data properties
        /// </summary>
        /// <param name="properties">Properties</param>
        void SetLoadProperty(Dictionary<string, bool> properties);

        /// <summary>
        /// Set allow load data properties
        /// </summary>
        /// <typeparam name="TQueryModel">Query model type</typeparam>
        /// <param name="allowLoad">Whether allow load data</param>
        /// <param name="properties">Properties</param>
        void SetLoadProperty<TQueryModel>(bool allowLoad, params Expression<Func<TQueryModel, dynamic>>[] properties);

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
        IQuery SetRecurve<TQueryModel>(Expression<Func<TQueryModel, dynamic>> key, Expression<Func<TQueryModel, dynamic>> relationKey, RecurveDirection direction = RecurveDirection.Down) where TQueryModel : IQueryModel<TQueryModel>;

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
        IQuery LightClone();

        /// <summary>
        /// Deep copy a IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        IQuery Clone();

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
        IQuery Join(Dictionary<string, string> joinFields, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery);

        /// <summary>
        /// Add a join item
        /// </summary>
        /// <param name="joinItem">Join item</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Join(JoinItem joinItem);

        #endregion

        #region Combine

        /// <summary>
        /// Add a combine item
        /// </summary>
        /// <param name="combineItem">Combine item</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Combine(CombineItem combineItem);

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

        #region Add criteria

        /// <summary>
        /// Add a criteria
        /// </summary>
        /// <param name="queryOperator">Connect operator</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="criteriaOperator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Converter</param>
        /// <param name="queryOption">query parameter option</param>
        IQuery AddCriteria(QueryOperator queryOperator, string fieldName, CriteriaOperator criteriaOperator, dynamic value, ICriteriaConverter converter = null, QueryParameterOptions queryOption = null);

        #endregion

        #region Add query item

        /// <summary>
        /// Add a IQueryItem
        /// </summary>
        /// <param name="queryOperator">Connect operator</param>
        /// <param name="queryItem">query item</param>
        /// <param name="queryOption">query parameter option</param>
        IQuery AddQueryItem(QueryOperator queryOperator, IQueryItem queryItem, QueryParameterOptions queryOption = null);

        #endregion

        #endregion
    }
}
