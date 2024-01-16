using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Client
{
    /// <summary>
    /// Defines data access client contract
    /// </summary>
    public partial interface IDataAccessClient : IDisposable
    {
        #region Query

        /// <summary>
        /// Query by current
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        List<T> QueryByCurrent<T>(IEnumerable<T> currentDatas, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        List<T> Query<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        List<T> Query<T>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        T QueryFirst<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        T QueryFirst<T>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        PagingInfo<T> QueryPaging<T>(Expression<Func<T, bool>> conditionExpression, PagingFilter pagingFilter, DataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        PagingInfo<T> QueryPaging<T>(Expression<Func<T, bool>> conditionExpression, int page, int pageSize, DataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        PagingInfo<T> QueryPaging<T>(ISixnetQueryable queryable, PagingFilter pagingFilter, DataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        PagingInfo<T> QueryPaging<T>(ISixnetQueryable queryable, int page, int pageSize, DataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, DataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, DataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, DataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, DataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, DataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TSeventh">Seventh data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, DataOperationOptions options = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        bool Exists<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return whether the data exists or not</returns>
        bool Exists(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        int Count<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        int Count(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        TValue Max<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        TValue Max<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        TValue Min<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        TValue Min<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        TValue Sum<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        TValue Sum<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        TValue Avg<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        TValue Avg<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        TValue Scalar<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="queries">queries</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        DataSet QueryMultiple(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Tuple<List<TFirst>, List<TSecond>> QueryMultiple<TFirst, TSecond>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Tuple<List<TFirst>, List<TSecond>, List<TThird>> QueryMultiple<TFirst, TSecond, TThird>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>> QueryMultiple<TFirst, TSecond, TThird, TFourth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <typeparam name="TSeventh">Seventh data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>> QueryMultiple<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        #endregion

        #region Insert

        /// <summary>
        /// Insert datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        List<T> Insert<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class;

        /// <summary>
        /// Insert data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        T Insert<T>(T data, DataOperationOptions options = null) where T : class;

        /// <summary>
        /// Insert and return identities
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        List<TIdentity> InsertReturnIdentities<T, TIdentity>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class;

        /// <summary>
        /// Insert and return identity
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        TIdentity InsertReturnIdentity<T, TIdentity>(T data, DataOperationOptions options = null) where T : class;

        #endregion

        #region Update

        /// <summary>
        /// Update datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        List<T> Update<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Update data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        T Update<T>(T data, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        int Update<T>(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        int Update<T>(FieldsAssignment fieldsAssignment, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        int Update(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, DataOperationOptions options = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        int Delete<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        int Delete<T>(T data, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        int Delete<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        int Delete(ISixnetQueryable queryable, DataOperationOptions options = null);

        #endregion

        #region Execution

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="commands">Data commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        int Execute(IEnumerable<DataCommand> commands, DataOperationOptions options = null);

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="command">Data command</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        int Execute(DataCommand command, DataOperationOptions options = null);

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="scriptType">Script type</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        int Execute(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, DataOperationOptions options = null);

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="dataTable">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        void BulkInsert(DataTable dataTable, IBulkInsertionOptions options = null);

        #endregion

        #region Migrate

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="migrationInfo">Migration info</param>
        /// <param name="options">Data operation options</param>
        void Migrate(MigrationInfo migrationInfo, DataOperationOptions options = null);

        #endregion

        #region Get tables

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="options">Data operation options</param>
        /// <returns></returns>
        List<DatabaseTableInfo> GetTables(DataOperationOptions options = null);

        #endregion
    }
}
