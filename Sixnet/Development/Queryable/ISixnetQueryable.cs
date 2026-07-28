// "Company © 2025. All rights reserved."

using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Queryable
{
    #region Base queryable

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    public partial interface ISixnetQueryable : ISixnetCondition
    {
        #region Properties

        /// <summary>
        /// Get the queryable info
        /// </summary>
        SixnetQueryableInfo Info { get; }

        #endregion

        #region Methods

        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        ISixnetQueryable Where(ISixnetCondition condition);

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        ISixnetQueryable WhereIf(bool predicate, ISixnetCondition condition);

        #endregion

        #region Sort

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable OrderBy(ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable OrderBy(IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable OrderByIf(bool predicate, ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false);

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable SelectFields(params ISixnetField[] fields);

        /// <summary>
        /// Get selected fields
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="includeNecessary">Whether include necessary fields</param>
        /// <returns></returns>
        IEnumerable<ISixnetField> GetSelectedFields(Type modelType, bool includeNecessary);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        #endregion

        #region Script

        /// <summary>
        /// Set script
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        ISixnetQueryable SetScript(string script, SixnetDataScriptType scriptType = SixnetDataScriptType.Text, object parameters = null);

        #endregion

        #region Validation

        /// <summary>
        /// Validation function
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <returns></returns>
        Func<T, bool> GetValidationFunction<T>();

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        ISixnetQueryable LightClone();

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        ISixnetQueryable Clone();

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        ISixnetQueryable UnionAll(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        ISixnetQueryable UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        ISixnetQueryable Union(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        ISixnetQueryable Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        ISixnetQueryable Except(ISixnetQueryable exceptQueryable);

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptQueryable">Except expression</param>
        /// <returns></returns>
        ISixnetQueryable Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null);

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        ISixnetQueryable Intersect(ISixnetQueryable intersectQueryable);

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectQueryable">Intersect expression</param>
        /// <returns></returns>
        ISixnetQueryable Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null);

        #endregion

        #endregion

        #region Model type

        /// <summary>
        /// Get the data type
        /// </summary>
        /// <returns></returns>
        Type GetModelType();

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        ISixnetQueryable SetModelType(Type modelType);

        #endregion

        #region Take

        /// <summary>
        /// Set take data count
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        ISixnetQueryable Take(int count, int skip = 0);

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable Distinct();

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        ISixnetQueryable GroupBy(params string[] fieldNames);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable GroupBy(params ISixnetField[] fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        ISixnetQueryable Having(ISixnetQueryable queryable);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        ISixnetQueryable HavingIf(bool predicate, ISixnetQueryable queryable);

        #endregion

        #region From

        /// <summary>
        /// From other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        ISixnetQueryable From(ISixnetQueryable targetQueryable);

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        ISixnetQueryable From(params string[] tableNames);

        /// <summary>
        /// As a data source
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable AsSource();

        /// <summary>
        /// As a data source
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable<TSource> AsSource<TSource>();

        /// <summary>
        /// As a temp table
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable AsTempTable();

        /// <summary>
        /// As a temp table
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable<TTable> AsTempTable<TTable>();

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        ISixnetQueryable SplitTable(dynamic splitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        ISixnetQueryable SplitTable(dynamic startSplitValue, dynamic endSplitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        ISixnetQueryable SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        ISixnetQueryable SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter);

        #endregion

        #region Subquery

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        bool Contains(object value);

        /// <summary>
        /// Not contains
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns></returns>
        bool NotContains(object value);

        /// <summary>
        /// Equal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Equal(object value);

        /// <summary>
        /// Not equal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool NotEqual(object value);

        /// <summary>
        /// Less than or equal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool LessThanOrEqual(object value);

        /// <summary>
        /// Less than
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool LessThan(object value);

        /// <summary>
        /// Greater than
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool GreaterThan(object value);

        /// <summary>
        /// Greater than or equal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool GreaterThanOrEqual(object value);

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable IncludeArchived();

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable IgnoreIsolation();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        ISixnetQueryable IgnoreFilter<TFilter>();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        ISixnetQueryable IgnoreFilter(Type filterType);

        /// <summary>
        /// Has ignored filter
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        bool HasIgnoredFilter(SixnetFieldRole fieldRole);

        /// <summary>
        /// Has ignored type filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        bool HasIgnoredFilter<TFilter>();

        /// <summary>
        /// Has ignored type filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        bool HasIgnoredFilter(Type filterType);

        #endregion

        #region Data access

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(SixnetFieldsAssignment fieldsAssignment, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Delete(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        T First<T>(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        List<T> ToList<T>(Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TSeventh">Seventh data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        SixnetPagingInfo<T> ToPaging<T>(SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        SixnetPagingInfo<T> ToPaging<T>(int page, int pageSize, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Any

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        bool Any(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Count

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        int Count(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        TValue Max<TValue>(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        TValue Min<TValue>(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        TValue Sum<TValue>(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        TValue Avg<TValue>(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        TValue Scalar<TValue>(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable Negate();

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable ReadOnly();

        #endregion

        #endregion
    }

    #endregion

    #region Model queryable

    /// <summary>
    /// Defines model queryable
    /// </summary>
    public partial interface ISixnetModelQueryable<TModel> : ISixnetQueryable
    {
        #region From

        /// <summary>
        /// As a data source
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TModel> AsSource();

        /// <summary>
        /// As a temp table
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TModel> AsTempTable();

        #endregion

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        TModel First(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        List<TModel> ToList(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        SixnetPagingInfo<TModel> ToPaging(SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        SixnetPagingInfo<TModel> ToPaging(int page, int pageSize, Action<SixnetDataOperationOptions> configure = null);

        #endregion 

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field </param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        TValue Max<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field </param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        TValue Min<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field </param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        TValue Sum<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field </param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        TValue Avg<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null);

        #endregion
    }

    #endregion

    #region One

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    public partial interface ISixnetQueryable<TFirst> : ISixnetModelQueryable<TFirst>
    {
        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Where(ISixnetCondition condition);

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> WhereIf(bool predicate, ISixnetCondition condition);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> Where(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderBy(ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderBy(IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderByIf(bool predicate, ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> OrderBy(Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> OrderByIf(bool predicate, Expression<Func<TFirst, object>> fields, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> InnerJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> InnerJoin<TSecond>(ISixnetQueryable secondQueryable, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> InnerJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> InnerJoinIf<TSecond>(bool predicate, ISixnetQueryable secondQueryable, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> LeftJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> LeftJoin<TSecond>(ISixnetQueryable secondQueryable, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> LeftJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> LeftJoinIf<TSecond>(bool predicate, ISixnetQueryable secondQueryable, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> RightJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> RightJoin<TSecond>(ISixnetQueryable secondQueryable, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> RightJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> RightJoinIf<TSecond>(bool predicate, ISixnetQueryable secondQueryable, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> FullJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> FullJoin<TSecond>(ISixnetQueryable secondQueryable, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> FullJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> FullJoinIf<TSecond>(bool predicate, ISixnetQueryable secondQueryable, Expression<Func<TFirst, TSecond, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> CrossJoin<TSecond>(Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> CrossJoin<TSecond>(ISixnetQueryable secondQueryable, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> CrossJoinIf<TSecond>(bool predicate, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> CrossJoinIf<TSecond>(bool predicate, ISixnetQueryable secondQueryable, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Join(SixnetJoinEntry joinEntry, Action<SixnetJoinEntry> configure = null);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> SelectFields(params ISixnetField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> SelectFields<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TResult>> fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> Tree(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst> LightClone();

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst> Clone();

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> UnionAll(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Union(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Except(ISixnetQueryable exceptQueryable);

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptQueryable">Except expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null);

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Intersect(ISixnetQueryable intersectQueryable);

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectQueryable">Intersect expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null);

        #endregion

        #endregion

        #region Model type

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> SetModelType(Type modelType);

        #endregion

        #region Take

        /// <summary>
        /// Take
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Take(int count, int skip = 0);

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Distinct();

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> GroupBy(params string[] fieldNames);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> GroupBy(params ISixnetField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> GroupBy(Expression<Func<TFirst, object>> fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> Having(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> From(ISixnetQueryable targetQueryable);

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> From(params string[] tableNames);

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> SplitTable(dynamic splitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> SplitTable(dynamic startSplitValue, dynamic endSplitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter);

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> IncludeArchived();

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> IgnoreIsolation();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> IgnoreFilter<TFilter>();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> IgnoreFilter(Type filterType);

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Negate();

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> ReadOnly();

        #endregion
    }

    #endregion

    #region Two

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond> : ISixnetModelQueryable<TFirst>
    {
        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Where(ISixnetCondition condition);

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> WhereIf(bool predicate, ISixnetCondition condition);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Where(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Where(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> OrderBy(ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> OrderBy(IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> OrderBy(Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> OrderBy(Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> InnerJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> InnerJoin<TThird>(ISixnetQueryable thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> InnerJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> InnerJoinIf<TThird>(bool predicate, ISixnetQueryable thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> LeftJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> LeftJoin<TThird>(ISixnetQueryable thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> LeftJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> LeftJoinIf<TThird>(bool predicate, ISixnetQueryable thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> RightJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> RightJoin<TThird>(ISixnetQueryable thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> RightJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> RightJoinIf<TThird>(bool predicate, ISixnetQueryable thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> FullJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> FullJoin<TThird>(ISixnetQueryable thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> FullJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> FullJoinIf<TThird>(bool predicate, ISixnetQueryable thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoin<TThird>(Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoin<TThird>(ISixnetQueryable thirdQueryable, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoinIf<TThird>(bool predicate, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoinIf<TThird>(bool predicate, ISixnetQueryable thirdQueryable, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Join(SixnetJoinEntry joinEntry, Action<SixnetJoinEntry> configure = null);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> SelectFields(params ISixnetField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> SelectFields<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Tree(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Tree(Expression<Func<TFirst, TSecond, object>> dataField, Expression<Func<TFirst, TSecond, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond> LightClone();

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond> Clone();

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> UnionAll(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Union(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Except(ISixnetQueryable exceptQueryable);

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptExpression">Except expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null);

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Intersect(ISixnetQueryable intersectQueryable);

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectQueryable">Intersect expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null);

        #endregion

        #endregion

        #region Model type

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> SetModelType(Type modelType);

        #endregion

        #region Take

        /// <summary>
        /// Take
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Take(int count, int skip = 0);

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Distinct();

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> GroupBy(params string[] fieldNames);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> GroupBy(params ISixnetField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> GroupBy(Expression<Func<TFirst, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> GroupBy(Expression<Func<TFirst, TSecond, object>> fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Having(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Having(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> From(ISixnetQueryable targetQueryable);

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> From(params string[] tableNames);

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> SplitTable(dynamic splitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> SplitTable(dynamic startSplitValue, dynamic endSplitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter);

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> IncludeArchived();

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> IgnoreIsolation();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> IgnoreFilter<TFilter>();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> IgnoreFilter(Type filterType);

        #endregion

        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Negate();

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> ReadOnly();

        #endregion
    }

    #endregion

    #region Three

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird> : ISixnetModelQueryable<TFirst>
    {
        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Where(ISixnetCondition condition);

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> WhereIf(bool predicate, ISixnetCondition condition);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Where(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Where(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> InnerJoin<TFourth>(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Fourth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> InnerJoin<TFourth>(ISixnetQueryable fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> InnerJoinIf<TFourth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Fourth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> InnerJoinIf<TFourth>(bool predicate, ISixnetQueryable fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LeftJoin<TFourth>(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LeftJoin<TFourth>(ISixnetQueryable fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LeftJoinIf<TFourth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LeftJoinIf<TFourth>(bool predicate, ISixnetQueryable fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> RightJoin<TFourth>(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> RightJoin<TFourth>(ISixnetQueryable fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> RightJoinIf<TFourth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> RightJoinIf<TFourth>(bool predicate, ISixnetQueryable fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> FullJoin<TFourth>(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> FullJoin<TFourth>(ISixnetQueryable fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> FullJoinIf<TFourth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> FullJoinIf<TFourth>(bool predicate, ISixnetQueryable fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> CrossJoin<TFourth>(Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> CrossJoin<TFourth>(ISixnetQueryable fourthQueryable, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> CrossJoinIf<TFourth>(bool predicate, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> CrossJoinIf<TFourth>(bool predicate, ISixnetQueryable fourthQueryable, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Join(SixnetJoinEntry joinEntry, Action<SixnetJoinEntry> configure = null);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> SelectFields(params ISixnetField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> SelectFields<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Tree(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Tree(Expression<Func<TFirst, TSecond, object>> dataField, Expression<Func<TFirst, TSecond, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Tree(Expression<Func<TFirst, TSecond, TThird, object>> dataField, Expression<Func<TFirst, TSecond, TThird, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> LightClone();

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Clone();

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> UnionAll(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Union(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Except(ISixnetQueryable exceptQueryable);

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptExpression">Except expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null);

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Intersect(ISixnetQueryable intersectQueryable);

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectQueryable">Intersect expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null);

        #endregion

        #endregion

        #region Model type

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> SetModelType(Type modelType);

        #endregion

        #region Take

        /// <summary>
        /// Take
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Take(int count, int skip = 0);

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Distinct();

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> GroupBy(params string[] fieldNames);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> GroupBy(params ISixnetField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> GroupBy(Expression<Func<TFirst, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> GroupBy(Expression<Func<TFirst, TSecond, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> GroupBy(Expression<Func<TFirst, TSecond, TThird, object>> fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Having(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Having(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> From(ISixnetQueryable targetQueryable);

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> From(params string[] tableNames);

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> SplitTable(dynamic splitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> SplitTable(dynamic startSplitValue, dynamic endSplitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter);

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> IncludeArchived();

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> IgnoreIsolation();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> IgnoreFilter<TFilter>();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> IgnoreFilter(Type filterType);

        #endregion

        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Negate();

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> ReadOnly();

        #endregion
    }

    #endregion

    #region Four

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    /// <typeparam name="TFourth">TFourth</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird, TFourth> : ISixnetModelQueryable<TFirst>
    {
        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(ISixnetCondition condition);

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, ISixnetCondition condition);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoin<TFifth>(ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoin<TFifth>(ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoin<TFifth>(ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoin<TFifth>(ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion 

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoin<TFifth>(Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoin<TFifth>(ISixnetQueryable fifthQueryable, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoinIf<TFifth>(bool predicate, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoinIf<TFifth>(bool predicate, ISixnetQueryable fifthQueryable, Action<SixnetJoinEntry> configure = null);

        #endregion 

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Join(SixnetJoinEntry joinEntry, Action<SixnetJoinEntry> configure = null);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields(params ISixnetField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(Expression<Func<TFirst, TSecond, object>> dataField, Expression<Func<TFirst, TSecond, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(Expression<Func<TFirst, TSecond, TThird, object>> dataField, Expression<Func<TFirst, TSecond, TThird, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LightClone();

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Clone();

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> UnionAll(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Union(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Except(ISixnetQueryable exceptQueryable);

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptExpression">Except expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null);

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Intersect(ISixnetQueryable intersectQueryable);

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectQueryable">Intersect expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null);

        #endregion

        #endregion

        #region Model type

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SetModelType(Type modelType);

        #endregion

        #region Take

        /// <summary>
        /// Take
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Take(int count, int skip = 0);

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Distinct();

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(params string[] fieldNames);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(params ISixnetField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(Expression<Func<TFirst, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(Expression<Func<TFirst, TSecond, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(Expression<Func<TFirst, TSecond, TThird, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> From(ISixnetQueryable targetQueryable);

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> From(params string[] tableNames);

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(dynamic splitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(dynamic startSplitValue, dynamic endSplitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter);

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> IncludeArchived();

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> IgnoreIsolation();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> IgnoreFilter<TFilter>();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> IgnoreFilter(Type filterType);

        #endregion

        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Negate();

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> ReadOnly();

        #endregion
    }

    #endregion

    #region Five

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    /// <typeparam name="TFourth">TFourth</typeparam>
    /// <typeparam name="TFifth">TFifth</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> : ISixnetModelQueryable<TFirst>
    {
        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(ISixnetCondition condition);

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, ISixnetCondition condition);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> fields, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoin<TSixth>(ISixnetQueryable sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoinIf<TSixth>(bool predicate, ISixnetQueryable sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoin<TSixth>(ISixnetQueryable sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoinIf<TSixth>(bool predicate, ISixnetQueryable sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoin<TSixth>(ISixnetQueryable sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoinIf<TSixth>(bool predicate, ISixnetQueryable sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoin<TSixth>(ISixnetQueryable sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoinIf<TSixth>(bool predicate, ISixnetQueryable sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion 

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoin<TSixth>(Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoin<TSixth>(ISixnetQueryable sixthQueryable, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoinIf<TSixth>(bool predicate, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoinIf<TSixth>(bool predicate, ISixnetQueryable sixthQueryable, Action<SixnetJoinEntry> configure = null);

        #endregion 

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Join(SixnetJoinEntry joinEntry, Action<SixnetJoinEntry> configure = null);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SelectFields(params ISixnetField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SelectFields<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult>> fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Tree(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Tree(Expression<Func<TFirst, TSecond, object>> dataField, Expression<Func<TFirst, TSecond, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Tree(Expression<Func<TFirst, TSecond, TThird, object>> dataField, Expression<Func<TFirst, TSecond, TThird, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LightClone();

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Clone();

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> UnionAll(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Union(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Except(ISixnetQueryable exceptQueryable);

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptExpression">Except expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null);

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Intersect(ISixnetQueryable intersectQueryable);

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectQueryable">Intersect expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null);

        #endregion

        #endregion

        #region Model type

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SetModelType(Type modelType);

        #endregion

        #region Take

        /// <summary>
        /// Take
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Take(int count, int skip = 0);

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Distinct();

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params string[] fieldNames);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params ISixnetField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(Expression<Func<TFirst, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(Expression<Func<TFirst, TSecond, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(Expression<Func<TFirst, TSecond, TThird, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> From(ISixnetQueryable targetQueryable);

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> From(params string[] tableNames);

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(dynamic splitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(dynamic startSplitValue, dynamic endSplitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter);

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> IncludeArchived();

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> IgnoreIsolation();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> IgnoreFilter<TFilter>();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> IgnoreFilter(Type filterType);

        #endregion

        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Negate();

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> ReadOnly();

        #endregion
    }

    #endregion

    #region Six

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    /// <typeparam name="TFourth">TFourth</typeparam>
    /// <typeparam name="TFifth">TFifth</typeparam>
    /// <typeparam name="TSixth">TSixth</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> : ISixnetModelQueryable<TFirst>
    {
        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(ISixnetCondition condition);

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, ISixnetCondition condition);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, object>> fields, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoin<TSeventh>(ISixnetQueryable seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoinIf<TSeventh>(bool predicate, ISixnetQueryable seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoin<TSeventh>(ISixnetQueryable seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoinIf<TSeventh>(bool predicate, ISixnetQueryable seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoin<TSeventh>(ISixnetQueryable seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoinIf<TSeventh>(bool predicate, ISixnetQueryable seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoin<TSeventh>(ISixnetQueryable seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="connection">Connection expression</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoinIf<TSeventh>(bool predicate, ISixnetQueryable seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> connection = null, Action<SixnetJoinEntry> configure = null);

        #endregion 

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoin<TSeventh>(Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoin<TSeventh>(ISixnetQueryable seventhQueryable, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoinIf<TSeventh>(bool predicate, Action<SixnetJoinEntry> configure = null);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoinIf<TSeventh>(bool predicate, ISixnetQueryable seventhQueryable, Action<SixnetJoinEntry> configure = null);

        #endregion 

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Join(SixnetJoinEntry joinEntry, Action<SixnetJoinEntry> configure = null);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SelectFields(params ISixnetField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SelectFields<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>> fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Tree(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Tree(Expression<Func<TFirst, TSecond, object>> dataField, Expression<Func<TFirst, TSecond, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Tree(Expression<Func<TFirst, TSecond, TThird, object>> dataField, Expression<Func<TFirst, TSecond, TThird, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LightClone();

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Clone();

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> UnionAll(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Union(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Except(ISixnetQueryable exceptQueryable);

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptExpression">Except expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null);

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Intersect(ISixnetQueryable intersectQueryable);

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectQueryable">Intersect expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null);

        #endregion

        #endregion

        #region Model type

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SetModelType(Type modelType);

        #endregion

        #region Take

        /// <summary>
        /// Take
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Take(int count, int skip = 0);

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Distinct();

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params string[] fieldNames);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params ISixnetField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(Expression<Func<TFirst, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(Expression<Func<TFirst, TSecond, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(Expression<Func<TFirst, TSecond, TThird, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, object>> fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> From(ISixnetQueryable targetQueryable);

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> From(params string[] tableNames);

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(dynamic splitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(dynamic startSplitValue, dynamic endSplitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter);

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> IncludeArchived();

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> IgnoreIsolation();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> IgnoreFilter<TFilter>();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> IgnoreFilter(Type filterType);

        #endregion

        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Negate();

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> ReadOnly();

        #endregion
    }

    #endregion

    #region Seven

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    /// <typeparam name="TFourth">TFourth</typeparam>
    /// <typeparam name="TFifth">TFifth</typeparam>
    /// <typeparam name="TSixth">TSixth</typeparam>
    /// <typeparam name="TSeventh">TSeventh</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> : ISixnetModelQueryable<TFirst>
    {
        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(ISixnetCondition condition);

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, ISixnetCondition condition);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, ISixnetField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, IEnumerable<ISixnetField> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFirst, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> fields, bool desc = false);

        #endregion

        #region Join

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntry">Join entry</param>
        /// <param name="configure">Configure join</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Join(SixnetJoinEntry joinEntry, Action<SixnetJoinEntry> configure = null);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SelectFields(params ISixnetField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SelectFields<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>> fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SelectFields<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>> fields);

        /// <summary>
        /// Select fields as a data source
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsSource<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TResult>> fields);

        /// <summary>
        /// Select fields as a temp table
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TResult> SelectAsTempTable<TResult>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TResult>> fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Tree(ISixnetField dataField, ISixnetField parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Tree(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Tree(Expression<Func<TFirst, TSecond, object>> dataField, Expression<Func<TFirst, TSecond, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Tree(Expression<Func<TFirst, TSecond, TThird, object>> dataField, Expression<Func<TFirst, TSecond, TThird, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Tree(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> dataField, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSeventh, object>> parentField, SixnetTreeMatchingDirection direction = SixnetTreeMatchingDirection.Down);

        #endregion

        #region Clone

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LightClone();

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Return the replicated Queryable</returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Clone();

        #endregion

        #region Combine

        #region UnionAll

        /// <summary>
        /// Union all
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> UnionAll(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union all
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> UnionAll<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Union

        /// <summary>
        /// Union
        /// </summary>
        /// <param name="unionQueryable">Union queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Union(ISixnetQueryable unionQueryable);

        /// <summary>
        /// Union
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="unionExpression">Union expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Union<TTarget>(Expression<Func<TTarget, bool>> unionExpression = null);

        #endregion

        #region Except

        /// <summary>
        /// Except
        /// </summary>
        /// <param name="exceptQueryable">Except queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Except(ISixnetQueryable exceptQueryable);

        /// <summary>
        /// Except
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="exceptExpression">Except expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Except<TTarget>(Expression<Func<TTarget, bool>> exceptExpression = null);

        #endregion

        #region Intersect

        /// <summary>
        /// Intersect
        /// </summary>
        /// <param name="intersectQueryable">Intersect queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Intersect(ISixnetQueryable intersectQueryable);

        /// <summary>
        /// Intersect
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="intersectQueryable">Intersect expression</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Intersect<TTarget>(Expression<Func<TTarget, bool>> intersectExpression = null);

        #endregion

        #endregion

        #region Model type

        /// <summary>
        /// Set model type
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SetModelType(Type modelType);

        #endregion

        #region Take

        /// <summary>
        /// Take
        /// </summary>
        /// <param name="count">Take data count</param>
        /// <param name="skip">Skip data count</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Take(int count, int skip = 0);

        #endregion

        #region Distinct

        /// <summary>
        /// Distinct
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Distinct();

        #endregion

        #region Group

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params string[] fieldNames);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params ISixnetField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(Expression<Func<TFirst, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(Expression<Func<TFirst, TSecond, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(Expression<Func<TFirst, TSecond, TThird, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, object>> fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> expression, SixnetCriterionConnector connector = SixnetCriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> From(ISixnetQueryable targetQueryable);

        /// <summary>
        /// From specify table
        /// </summary>
        /// <param name="tableNames">Table names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> From(params string[] tableNames);

        #endregion

        #region Split table

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValue">Split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SplitTable(dynamic splitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="startSplitValue">Start split value</param>
        /// <param name="endSplitValue">End split value</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SplitTable(dynamic startSplitValue, dynamic endSplitValue);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitValues">Split values</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SplitTable(IEnumerable<dynamic> splitValues, SixnetSplitTableNameSelectionPattern selectionPattern = SixnetSplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SplitTable(Func<IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>, IEnumerable<SixnetDatabaseObjectName>> splitTableNameFilter);

        #endregion

        #region Filter

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> IncludeArchived();

        /// <summary>
        /// Ignore data isolation
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> IgnoreIsolation();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> IgnoreFilter<TFilter>();

        /// <summary>
        /// Ignore filter
        /// </summary>
        /// <param name="filterType">Filter type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> IgnoreFilter(Type filterType);

        #endregion

        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Negate();

        #endregion

        #region Read only

        /// <summary>
        /// Mark as read only
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> ReadOnly();

        #endregion
    }

    #endregion
}
