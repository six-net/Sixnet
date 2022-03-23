using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using EZNEW.Paging;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Entity;
using EZNEW.Data;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines query object contract
    /// </summary>
    public interface IQuery : ICondition
    {
        #region Properties

        /// <summary>
        /// Gets all conditions
        /// </summary>
        IEnumerable<ICondition> Conditions { get; }

        /// <summary>
        /// Gets all criterion
        /// </summary>
        IEnumerable<Criterion> Criteria { get; }

        /// <summary>
        /// Gets all sorts
        /// </summary>
        IEnumerable<SortEntry> Sorts { get; }

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
        string Text { get; }

        /// <summary>
        /// Gets the query text parameter
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> TextParameters { get; }

        /// <summary>
        /// Gets the query command type
        /// </summary>
        QueryExecutionMode ExecutionMode { get; }

        /// <summary>
        /// Gets or sets query data size
        /// </summary>
        int QuerySize { get; set; }

        /// <summary>
        /// Gets all of data properties allow to lazy load
        /// </summary>
        IEnumerable<KeyValuePair<string, bool>> LoadProperties { get; }

        /// <summary>
        /// Indicates whether has subquery
        /// </summary>
        bool HasSubquery { get; }

        /// <summary>
        /// Indicates whether has recurve
        /// </summary>
        bool HasRecurve { get; }

        /// <summary>
        /// Indicates whether has join
        /// </summary>
        bool HasJoin { get; }

        /// <summary>
        /// Indicates whether has combine
        /// </summary>
        bool HasCombine { get; }

        /// <summary>
        /// Indicates whether has field converter
        /// </summary>
        bool HasFieldConverter { get; }

        /// <summary>
        /// Indicates whether is a complex query
        /// Has subquery,recurve,join,field converter
        /// </summary>
        bool IsComplex { get; }

        /// <summary>
        /// Gets the recurve condition
        /// </summary>
        Recurve Recurve { get; }

        /// <summary>
        /// Indicates whether must affect data
        /// </summary>
        bool MustAffectData { get; set; }

        /// <summary>
        /// Indicates whether is obsolete
        /// </summary>
        bool IsObsolete { get; }

        /// <summary>
        /// Gets the join entries
        /// </summary>
        IEnumerable<JoinEntry> Joins { get; }

        /// <summary>
        /// Gets the combine entries
        /// </summary>
        IEnumerable<CombineEntry> Combines { get; }

        /// <summary>
        /// Indicates there is no conditions
        /// </summary>
        bool NoneCondition { get; }

        /// <summary>
        /// Gets all subqueries
        /// </summary>
        IEnumerable<IQuery> Subqueries { get; }

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        DataIsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Indicates whether query obsolete data
        /// </summary>
        bool IncludeObsoleteData { get; set; }

        #endregion

        #region Methods

        #region Sort

        #region Add sort

        /// <summary>
        /// Add sort
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="desc">Sort by desc</param>
        /// <param name="sortOptions">Sort options</param>
        IQuery AddSort(params SortEntry[] sortEntries);

        #endregion

        #region Clear sort

        /// <summary>
        /// Clear sort
        /// </summary>
        /// <returns>Return the newest IQuery object</returns>
        IQuery ClearSort();

        #endregion

        #region Sort datas

        /// <summary>
        /// Sort datas
        /// </summary>
        /// <typeparam name="TEntity">Entity data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="useDefaultFieldToSort">Whether use default field to sort</param>
        /// <returns>Return the sorted data set</returns>
        IEnumerable<TEntity> SortData<TEntity>(IEnumerable<TEntity> datas, bool useDefaultFieldToSort = false) where TEntity : BaseEntity<TEntity>, new();

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
        /// <param name="forceNecessaryFields">Whether include necessary fields</param>
        /// <returns>Return actually query fields</returns>
        Tuple<bool, IEnumerable<string>> GetActuallyQueryFieldsWithSign(Type entityType, bool forceNecessaryFields);

        #endregion

        #region Text

        /// <summary>
        /// Set text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery SetText(string text, object parameters = null);

        #endregion

        #region Load Property

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

        #region Get parameters

        /// <summary>
        /// Get equal parameters
        /// </summary>
        /// <param name="parameterNames">Parameter names</param>
        /// <returns>Return parameter and values</returns>
        Dictionary<string, List<dynamic>> GetEqualParameters(IEnumerable<string> parameterNames = null);

        #endregion

        #region Get validation function

        /// <summary>
        /// Get validation function
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <returns>Return a function delegate</returns>
        Func<T, bool> GetValidationFunction<T>();

        #endregion

        #region Recurve

        /// <summary>
        /// Set recurve
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="relationField">Relation field</param>
        /// <param name="direction">Recurve direction</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery SetRecurve(string dataField, string relationField, RecurveDirection direction = RecurveDirection.Down);

        /// <summary>
        /// set recurve
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="dataField">Data field</param>
        /// <param name="relationField">Relation field</param>
        /// <param name="direction">Recurve direction</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery SetRecurve<TQueryModel>(Expression<Func<TQueryModel, dynamic>> dataField, Expression<Func<TQueryModel, dynamic>> relationField, RecurveDirection direction = RecurveDirection.Down) where TQueryModel : IQueryModel<TQueryModel>;

        #endregion

        #region Obsolete

        /// <summary>
        /// Obsolete
        /// </summary>
        void Obsolete();

        /// <summary>
        /// Activate
        /// </summary>
        void Activate();

        #endregion

        #region Clone

        /// <summary>
        /// Clone an IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        IQuery LightClone();

        /// <summary>
        /// Deep clone an IQuery object
        /// </summary>
        /// <returns>Return the replicated Query object</returns>
        IQuery Clone();

        #endregion

        #region Join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Join(JoinEntry joinEntry);

        #endregion

        #region Combine

        /// <summary>
        /// Add combine
        /// </summary>
        /// <param name="combineEntry">Combine entry</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery Combine(CombineEntry combineEntry);

        #endregion

        #region GlobalCondition

        /// <summary>
        /// Set global condition
        /// </summary>
        /// <param name="globalCondition">Global condition</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery SetGlobalCondition(IQuery globalCondition);

        /// <summary>
        /// Indicates whether allow set global condition
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
        /// Set paging info
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        IQuery SetPaging(PagingFilter pagingFilter);

        /// <summary>
        /// Set paging info
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns></returns>
        IQuery SetPaging(int pageIndex, int pageSize = 20);

        #endregion

        #region Add criterion

        /// <summary>
        /// Add criterion
        /// </summary>
        /// <param name="connector">Connector</param>
        /// <param name="field">Field</param>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        IQuery AddCriterion(CriterionConnector connector, FieldInfo field, CriterionOperator criterionOperator, dynamic value, CriterionOptions criterionOptions = null);

        #endregion

        #region Add condition

        /// <summary>
        /// Add condition
        /// </summary>
        /// <param name="condition">Condition</param>
        IQuery AddCondition(ICondition condition);

        #endregion

        #endregion
    }
}
