using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines queryable contract
    /// </summary>
    public partial interface ISixnetQueryable : ICondition
    {
        #region Properties

        /// <summary>
        /// Get the queryable id
        /// </summary>
        Guid Id { get; }

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
        /// Get the selected fields
        /// </summary>
        IEnumerable<IDataField> SelectedFields { get; }

        /// <summary>
        /// Get the unselected fields
        /// </summary>
        IEnumerable<IDataField> UnselectedFields { get; }

        /// <summary>
        /// Gets the group fields
        /// </summary>
        IEnumerable<IDataField> GroupFields { get; }

        /// <summary>
        /// Gets the script
        /// </summary>
        string Script { get; }

        /// <summary>
        /// Gets the script type
        /// </summary>
        DataScriptType ScriptType { get; }

        /// <summary>
        /// Gets the query text parameter
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> ScriptParameters { get; }

        /// <summary>
        /// Gets the query command type
        /// </summary>
        QueryableExecutionMode ExecutionMode { get; }

        /// <summary>
        /// Gets or sets the skip data count
        /// </summary>
        int SkipCount { get; }

        /// <summary>
        /// Gets or sets the take data count
        /// </summary>
        int TakeCount { get; }

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
        /// Indicates whether has field formatter
        /// </summary>
        bool HasFieldFormatter { get; }

        /// <summary>
        /// Indicates whether is a complex query
        /// Has subquery,recurve,join,field converter
        /// </summary>
        bool IsComplex { get; }

        /// <summary>
        /// Gets the tree info
        /// </summary>
        TreeInfo TreeInfo { get; }

        /// <summary>
        /// Gets the join entries
        /// </summary>
        IEnumerable<JoinEntry> Joins { get; }

        /// <summary>
        /// Gets the combine entries
        /// </summary>
        IEnumerable<CombineEntry> Combines { get; }

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        DataIsolationLevel? IsolationLevel { get; }

        /// <summary>
        /// Whether include archived data
        /// </summary>
        bool IncludeArchivedData { get; }

        /// <summary>
        /// Whether unisolated data
        /// </summary>
        bool UnisolatedData { get; }

        /// <summary>
        /// Gets the from type
        /// </summary>
        QueryableFromType FromType { get; }

        /// <summary>
        /// Gets the target queryable
        /// </summary>
        ISixnetQueryable TargetQueryable { get; }

        /// <summary>
        /// Gets the query output type
        /// </summary>
        QueryableOutputType OutputType { get; }

        /// <summary>
        /// Whether is a group queryable
        /// </summary>
        bool IsGroupQueryable { get; }

        /// <summary>
        /// Gets or sets the queryable
        /// </summary>
        ISixnetQueryable HavingQueryable { get; }

        /// <summary>
        /// Gets the split table behavior
        /// </summary>
        SplitTableBehavior SplitTableBehavior { get; }

        /// <summary>
        /// Whether is distinct
        /// </summary>
        bool IsDistincted { get; }

        #endregion

        #region Methods

        #region Condition

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        ISixnetQueryable Where(ICondition condition);

        /// <summary>
        /// Append group condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="condition">Group condition</param>
        /// <returns></returns>
        ISixnetQueryable WhereIf(bool predicate, ICondition condition);

        #endregion

        #region Sort

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        ISixnetQueryable OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        ISixnetQueryable OrderBy(string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable OrderBy(IDataField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable OrderBy(IEnumerable<IDataField> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        ISixnetQueryable OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        ISixnetQueryable OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable OrderByIf(bool predicate, IDataField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable OrderByIf(bool predicate, IEnumerable<IDataField> fields, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> InnerJoin<TFirst>(Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> InnerJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> InnerJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> InnerJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> LeftJoin<TFirst>(Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="secondQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> LeftJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> LeftJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> LeftJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> RightJoin<TFirst>(Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> RightJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> RightJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> RightJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> FullJoin<TFirst>(Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> FullJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> FullJoinIf<TFirst>(bool predicate, Expression<Func<TFirst, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> FullJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable, Expression<Func<TFirst, bool>> joinExpression = null);

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <returns></returns>
        ISixnetQueryable<TFirst> CrossJoin<TFirst>();

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="firstQueryable">First queryable</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> CrossJoin<TFirst>(ISixnetQueryable<TFirst> firstQueryable);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> CrossJoinIf<TFirst>(bool predicate);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFirst">TFirst</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="firstQueryable">First queryable</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> CrossJoinIf<TFirst>(bool predicate, ISixnetQueryable<TFirst> firstQueryable);

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        ISixnetQueryable Join(params JoinEntry[] joinEntries);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable Select(params IDataField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        ISixnetQueryable Select(params string[] fieldNames);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable Unselect(params IDataField[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        ISixnetQueryable Unselect(params string[] fieldNames);

        /// <summary>
        /// Get the selected fields
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="includeNecessaryFields">Whether include necessary fields</param>
        /// <returns></returns>
        IEnumerable<IDataField> GetSelectedFields(Type modelType, bool includeNecessaryFields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataFieldName">Data field name</param>
        /// <param name="parentFieldName">Parent field name</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        #endregion

        #region Script

        /// <summary>
        /// Set script
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        ISixnetQueryable SetScript(string script, DataScriptType scriptType = DataScriptType.Text, object parameters = null);

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
        ISixnetQueryable GroupBy(params IDataField[] fields);

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
        ISixnetQueryable SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        ISixnetQueryable SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter);

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        ISixnetQueryable Output(QueryableOutputType outputType);

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

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable IncludeArchived();

        #endregion

        #region Isolation

        /// <summary>
        /// Unisolated
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable Unisolated();

        #endregion

        #region Data access

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(FieldsAssignment fieldsAssignment, Action<DataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Delete(Action<DataOperationOptions> configure = null);

        #endregion

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        T First<T>(Action<DataOperationOptions> configure = null);

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        List<T> ToList<T>(Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TReturn> ToList<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> ToList<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> ToList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<T> ToPaging<T>(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<T> ToPaging<T>(int page, int pageSize, Action<DataOperationOptions> configure = null);

        #endregion

        #region Any

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        bool Any(Action<DataOperationOptions> configure = null);

        #endregion

        #region Count

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        int Count(Action<DataOperationOptions> configure = null);

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        TValue Max<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        TValue Min<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        TValue Sum<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        TValue Avg<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        TValue Scalar<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable Negate();

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines model queryable
    /// </summary>
    public partial interface ISixnetModelQueryable<TModel> : ISixnetQueryable
    {
        #region Data access

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        TModel First(Action<DataOperationOptions> configure = null);

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        List<TModel> ToList(Action<DataOperationOptions> configure = null);

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<TModel> ToPaging(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<TModel> ToPaging(int page, int pageSize, Action<DataOperationOptions> configure = null);

        #endregion 

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    public partial interface ISixnetQueryable<TFirst> : ISixnetModelQueryable<TFirst>
    {
        #region Methods

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderBy(string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderBy(IDataField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderBy(IEnumerable<IDataField> fields, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderByIf(bool predicate, IDataField field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fields">Fields</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> OrderByIf(bool predicate, IEnumerable<IDataField> fields, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> InnerJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> InnerJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> InnerJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> InnerJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> LeftJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> LeftJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> LeftJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> LeftJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> RightJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> RightJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> RightJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> RightJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> FullJoin<TSecond>(Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> FullJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> FullJoinIf<TSecond>(bool predicate, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> FullJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable, Expression<Func<TFirst, TSecond, bool>> joinExpression = null);

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> CrossJoin<TSecond>();

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="secondQueryable">Second queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> CrossJoin<TSecond>(ISixnetQueryable<TSecond> secondQueryable);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> CrossJoinIf<TSecond>(bool predicate);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSecond">TSecond</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="secondQueryable">Second queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> CrossJoinIf<TSecond>(bool predicate, ISixnetQueryable<TSecond> secondQueryable);

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Join(params JoinEntry[] joinEntries);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Select(params IDataField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Select(params string[] fieldNames);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Unselect(params IDataField[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Unselect(params string[] fieldNames);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> Select(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> Unselect(params Expression<Func<TFirst, object>>[] fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataFieldName">Data field name</param>
        /// <param name="parentFieldName">Parent field name</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

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
        new ISixnetQueryable<TFirst> GroupBy(params IDataField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> GroupBy(params Expression<Func<TFirst, object>>[] fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> From(ISixnetQueryable targetQueryable);

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
        new ISixnetQueryable<TFirst> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter);

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Output(QueryableOutputType outputType);

        #endregion

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> IncludeArchived();

        #endregion

        #region Isolation

        /// <summary>
        /// Unisolated
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Unisolated();

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst> Negate();

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond> : ISixnetModelQueryable<TFirst>
    {
        #region Methods

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Where(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> OrderBy(string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> OrderBy(Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> OrderByIf(bool predicate, Expression<Func<TSecond, object>> field, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> InnerJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> InnerJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> InnerJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> InnerJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> LeftJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> LeftJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> LeftJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> LeftJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> RightJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> RightJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> RightJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> RightJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> FullJoin<TThird>(Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> FullJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> FullJoinIf<TThird>(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> FullJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable, Expression<Func<TFirst, TSecond, TThird, bool>> joinExpression = null);

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoin<TThird>();

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoin<TThird>(ISixnetQueryable<TThird> thirdQueryable);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoinIf<TThird>(bool predicate);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TThird">TThird</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="thirdQueryable">Third queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> CrossJoinIf<TThird>(bool predicate, ISixnetQueryable<TThird> thirdQueryable);

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Join(params JoinEntry[] joinEntries);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Select(params IDataField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Select(params string[] fieldNames);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Unselect(params string[] fieldNames);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Select(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Unselect(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Select(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Unselect(params Expression<Func<TSecond, object>>[] fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataFieldName">Data field name</param>
        /// <param name="parentFieldName">Parent field name</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

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
        new ISixnetQueryable<TFirst, TSecond> GroupBy(params IDataField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> GroupBy(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> GroupBy(params Expression<Func<TSecond, object>>[] fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> Having(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> From(ISixnetQueryable targetQueryable);

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
        new ISixnetQueryable<TFirst, TSecond> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter);

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Output(QueryableOutputType outputType);

        #endregion

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> IncludeArchived();

        #endregion

        #region Isolation

        /// <summary>
        /// Unisolated
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Unisolated();

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
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond> Negate();

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird> : ISixnetModelQueryable<TFirst>
    {
        #region Methods

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Where(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderBy(Expression<Func<TThird, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> OrderByIf(bool predicate, Expression<Func<TThird, object>> field, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> InnerJoin<TFourth>(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Fourth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> InnerJoin<TFourth>(ISixnetQueryable<TFourth> fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> InnerJoinIf<TFourth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Fourth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> InnerJoinIf<TFourth>(bool predicate, ISixnetQueryable<TFourth> fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LeftJoin<TFourth>(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LeftJoin<TFourth>(ISixnetQueryable<TFourth> fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LeftJoinIf<TFourth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> LeftJoinIf<TFourth>(bool predicate, ISixnetQueryable<TFourth> fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> RightJoin<TFourth>(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> RightJoin<TFourth>(ISixnetQueryable<TFourth> fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> RightJoinIf<TFourth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> RightJoinIf<TFourth>(bool predicate, ISixnetQueryable<TFourth> fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> FullJoin<TFourth>(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> FullJoin<TFourth>(ISixnetQueryable<TFourth> fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> FullJoinIf<TFourth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> FullJoinIf<TFourth>(bool predicate, ISixnetQueryable<TFourth> fourthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> joinExpression = null);

        #endregion

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> CrossJoin<TFourth>();

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> CrossJoin<TFourth>(ISixnetQueryable<TFourth> fourthQueryable);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> CrossJoinIf<TFourth>(bool predicate);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFourth">TFourth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fourthQueryable">Third queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> CrossJoinIf<TFourth>(bool predicate, ISixnetQueryable<TFourth> fourthQueryable);

        #endregion

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Join(params JoinEntry[] joinEntries);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Select(params IDataField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Select(params string[] fieldNames);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Unselect(params string[] fieldNames);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Select(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Unselect(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Select(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Unselect(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Select(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Unselect(params Expression<Func<TThird, object>>[] fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> TreeMatching(string dataField, string parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

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
        new ISixnetQueryable<TFirst, TSecond, TThird> GroupBy(params IDataField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> GroupBy(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> GroupBy(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> GroupBy(params Expression<Func<TThird, object>>[] fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Having(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> From(ISixnetQueryable targetQueryable);

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
        new ISixnetQueryable<TFirst, TSecond, TThird> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter);

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Output(QueryableOutputType outputType);

        #endregion

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> IncludeArchived();

        #endregion

        #region Isolation

        /// <summary>
        /// Unisolated
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Unisolated();

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
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird> Negate();

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    /// <typeparam name="TFourth">TFourth</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird, TFourth> : ISixnetModelQueryable<TFirst>
    {
        #region Methods

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TThird, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TThird, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderBy(Expression<Func<TFourth, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> OrderByIf(bool predicate, Expression<Func<TFourth, object>> field, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoin<TFifth>(ISixnetQueryable<TFifth> fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Fifth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> InnerJoinIf<TFifth>(bool predicate, ISixnetQueryable<TFifth> fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoin<TFifth>(ISixnetQueryable<TFifth> fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> LeftJoinIf<TFifth>(bool predicate, ISixnetQueryable<TFifth> fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoin<TFifth>(ISixnetQueryable<TFifth> fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> RightJoinIf<TFifth>(bool predicate, ISixnetQueryable<TFifth> fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoin<TFifth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoin<TFifth>(ISixnetQueryable<TFifth> fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoinIf<TFifth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> FullJoinIf<TFifth>(bool predicate, ISixnetQueryable<TFifth> fifthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> joinExpression = null);

        #endregion 

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoin<TFifth>();

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoin<TFifth>(ISixnetQueryable<TFifth> fifthQueryable);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoinIf<TFifth>(bool predicate);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TFifth">TFifth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="fifthQueryable">Third queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> CrossJoinIf<TFifth>(bool predicate, ISixnetQueryable<TFifth> fifthQueryable);

        #endregion 

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Join(params JoinEntry[] joinEntries);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Select(params IDataField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Select(params string[] fieldNames);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(params string[] fieldNames);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Select(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Select(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Select(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Select(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unselect(params Expression<Func<TFourth, object>>[] fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataFieldName">Data field name</param>
        /// <param name="parentFieldName">Parent field name</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

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
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(params IDataField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> GroupBy(params Expression<Func<TFourth, object>>[] fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> From(ISixnetQueryable targetQueryable);

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
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter);

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Output(QueryableOutputType outputType);

        #endregion

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> IncludeArchived();

        #endregion

        #region Isolation

        /// <summary>
        /// Unisolated
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Unisolated();

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
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth> Negate();

        #endregion

        #endregion
    }

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
        #region Methods

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TThird, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TThird, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFourth, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFourth, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderBy(Expression<Func<TFifth, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> OrderByIf(bool predicate, Expression<Func<TFifth, object>> field, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> RightJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoin<TSixth>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoinIf<TSixth>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> FullJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> joinExpression = null);

        #endregion 

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoin<TSixth>();

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoin<TSixth>(ISixnetQueryable<TSixth> sixthQueryable);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoinIf<TSixth>(bool predicate);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSixth">TSixth</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="sixthQueryable">Sixth queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> CrossJoinIf<TSixth>(bool predicate, ISixnetQueryable<TSixth> sixthQueryable);

        #endregion 

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Join(params JoinEntry[] joinEntries);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params IDataField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params string[] fieldNames);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params string[] fieldNames);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Select(params Expression<Func<TFifth, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unselect(params Expression<Func<TFifth, object>>[] fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataFieldName">Data field name</param>
        /// <param name="parentFieldName">Parent field name</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> TreeMatching(string dataFieldName, string parentFieldName, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

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
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params IDataField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> GroupBy(params Expression<Func<TFifth, object>>[] fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> From(ISixnetQueryable targetQueryable);

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
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter);

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Output(QueryableOutputType outputType);

        #endregion

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> IncludeArchived();

        #endregion

        #region Isolation

        /// <summary>
        /// Unisolated
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Unisolated();

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
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> Negate();

        #endregion

        #endregion
    }

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
        #region Methods

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TThird, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TThird, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFourth, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFourth, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TFifth, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TFifth, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy(Expression<Func<TSixth, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> OrderByIf(bool predicate, Expression<Func<TSixth, object>> field, bool desc = false);

        #endregion

        #region Join

        #region Inner join

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Inner join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        #endregion

        #region Left join

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Left join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        #endregion

        #region Right join

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Right join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> RightJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        #endregion

        #region Full join

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoin<TSeventh>(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoinIf<TSeventh>(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        /// <summary>
        /// Full join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <param name="joinExpression">Join expression</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> FullJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> joinExpression = null);

        #endregion 

        #region Cross join

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoin<TSeventh>();

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoin<TSeventh>(ISixnetQueryable<TSeventh> seventhQueryable);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoinIf<TSeventh>(bool predicate);

        /// <summary>
        /// Cross join
        /// </summary>
        /// <typeparam name="TSeventh">TSeventh</typeparam>
        /// <param name="predicate">Predicate</param>
        /// <param name="seventhQueryable">Seventh queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> CrossJoinIf<TSeventh>(bool predicate, ISixnetQueryable<TSeventh> seventhQueryable);

        #endregion 

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Join(params JoinEntry[] joinEntries);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params IDataField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params string[] fieldNames);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params string[] fieldNames);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TFifth, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TFifth, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Select(params Expression<Func<TSixth, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unselect(params Expression<Func<TSixth, object>>[] fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> TreeMatching(string dataField, string parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

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
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params IDataField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TFifth, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy(params Expression<Func<TSixth, object>>[] fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> From(ISixnetQueryable targetQueryable);

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
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter);

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Output(QueryableOutputType outputType);

        #endregion

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> IncludeArchived();

        #endregion

        #region Isolation

        /// <summary>
        /// Unisolated
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Unisolated();

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
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> Negate();

        #endregion

        #endregion
    }

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
        #region Methods

        #region Condition

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> WhereIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region Sort

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldNames">Field names</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, IEnumerable<string> fieldNames, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="desc">Whether order by desc</param>
        /// <param name="targetType">Target type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, string fieldName, bool desc = false, Type targetType = null);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFirst, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TSecond, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TThird, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TThird, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFourth, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFourth, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TFifth, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TFifth, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TSixth, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TSixth, object>> field, bool desc = false);

        /// <summary>
        /// Order by field
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy(Expression<Func<TSeventh, object>> field, bool desc = false);

        /// <summary>
        /// Order by fields
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="field">Field</param>
        /// <param name="desc">Whether order by desc</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByIf(bool predicate, Expression<Func<TSeventh, object>> field, bool desc = false);

        #endregion

        #region Join

        #region Add join

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="joinEntries">Join entries</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Join(params JoinEntry[] joinEntries);

        #endregion

        #endregion

        #region Select

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select(params IDataField[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select(params string[] fieldNames);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fieldNames">Field names</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Unselect(params string[] fieldNames);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Unselect(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Unselect(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Unselect(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Unselect(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select(params Expression<Func<TFifth, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Unselect(params Expression<Func<TFifth, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select(params Expression<Func<TSixth, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Unselect(params Expression<Func<TSixth, object>>[] fields);

        /// <summary>
        /// Select fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select(params Expression<Func<TSeventh, object>>[] fields);

        /// <summary>
        /// Unselect fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Unselect(params Expression<Func<TSeventh, object>>[] fields);

        #endregion

        #region Tree

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> TreeMatching(string dataField, string parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree matching
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> TreeMatching(IDataField dataField, IDataField parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

        /// <summary>
        /// Tree match
        /// </summary>
        /// <param name="dataField">Data field</param>
        /// <param name="parentField">Parent field</param>
        /// <param name="direction">Matching direction</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> TreeMatching(Expression<Func<TFirst, object>> dataField, Expression<Func<TFirst, object>> parentField, TreeMatchingDirection direction = TreeMatchingDirection.Down);

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
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params IDataField[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params Expression<Func<TFirst, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params Expression<Func<TSecond, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params Expression<Func<TThird, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params Expression<Func<TFourth, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params Expression<Func<TFifth, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params Expression<Func<TSixth, object>>[] fields);

        /// <summary>
        /// Group by fields
        /// </summary>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy(params Expression<Func<TSeventh, object>>[] fields);

        #endregion

        #region Having

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Having(Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        /// <summary>
        /// Append having condition
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="expression">Expression</param>
        /// <param name="connector">Connector</param>
        /// <returns></returns>
        ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> HavingIf(bool predicate, Expression<Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> expression, CriterionConnector connector = CriterionConnector.And);

        #endregion

        #region From

        /// <summary>
        /// Query from other queryable
        /// </summary>
        /// <param name="targetQueryable">Target queryable</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> From(ISixnetQueryable targetQueryable);

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
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SplitTable(IEnumerable<dynamic> splitValues, SplitTableNameSelectionPattern selectionPattern = SplitTableNameSelectionPattern.Precision);

        /// <summary>
        /// Use split table
        /// </summary>
        /// <param name="splitTableNameFilter">Split table name filter</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> SplitTable(Func<IEnumerable<string>, IEnumerable<string>> splitTableNameFilter);

        #endregion

        #region Output

        /// <summary>
        /// Set output
        /// </summary>
        /// <param name="outputType">Output type</param>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Output(QueryableOutputType outputType);

        #endregion

        #region Archived

        /// <summary>
        /// Include archived
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> IncludeArchived();

        #endregion

        #region Isolation

        /// <summary>
        /// Unisolated
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Unisolated();

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
        List<TReturn> ToList<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion

        #region Negate

        /// <summary>
        /// Negate
        /// </summary>
        /// <returns></returns>
        new ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Negate();

        #endregion

        #endregion
    }
}
