using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
    public partial interface IDataAccessClient
    {
        #region Query

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        Task<List<T>> QueryAsync<T>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Query by current datas
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<List<T>> QueryByCurrentAsync<T>(IEnumerable<T> currentDatas, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<T> QueryFirstAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        Task<T> QueryFirstAsync<T>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        Task<PagingInfo<T>> QueryPagingAsync<T>(Expression<Func<T, bool>> conditionExpression, PagingFilter pagingFilter, DataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        Task<PagingInfo<T>> QueryPagingAsync<T>(Expression<Func<T, bool>> conditionExpression, int page, int pageSize, DataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Dynamic object paging</returns>
        Task<PagingInfo<T>> QueryPagingAsync<T>(ISixnetQueryable queryable, PagingFilter pagingFilter, DataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="options">Options</param>
        /// <returns>Dynamic object paging</returns>
        Task<PagingInfo<T>> QueryPagingAsync<T>(ISixnetQueryable queryable, int page, int pageSize, DataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, DataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, DataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, DataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, DataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, DataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, DataOperationOptions options = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return whether the data exists or not</returns>
        Task<bool> ExistsAsync(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> CountAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> CountAsync(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> MaxAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> MaxAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> MinAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> MinAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> SumAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> SumAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> AvgAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> AvgAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Aggregate value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        Task<TValue> ScalarAsync<TValue>(ISixnetQueryable queryable, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="queries">queries</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        Task<DataSet> QueryMultipleAsync(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

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
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

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
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

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
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

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
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IEnumerable<ISixnetQueryable> queries, DataOperationOptions options = null);

        #endregion

        #region Insert

        /// <summary>
        /// Insert datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<List<T>> InsertAsync<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class;

        /// <summary>
        /// Insert and return identities
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<List<TIdentity>> InsertReturnIdentitiesAsync<T, TIdentity>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class;

        /// <summary>
        /// Insert data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<T> InsertAsync<T>(T data, DataOperationOptions options = null) where T : class;

        /// <summary>
        /// Insert and return identity
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TIdentity> InsertReturnIdentityAsync<T, TIdentity>(T data, DataOperationOptions options = null) where T : class;

        #endregion

        #region Update

        /// <summary>
        /// Update datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<List<T>> UpdateAsync<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Update data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<T> UpdateAsync<T>(T data, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> UpdateAsync<T>(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> UpdateAsync<T>(FieldsAssignment fieldsAssignment, Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, DataOperationOptions options = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync<T>(IEnumerable<T> datas, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync<T>(T data, DataOperationOptions options = null) where T : class, IEntity<T>;

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> conditionExpression, DataOperationOptions options = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync(ISixnetQueryable queryable, DataOperationOptions options = null);

        #endregion

        #region Execution

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="commands">Data commands</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(IEnumerable<DataCommand> commands, DataOperationOptions options = null);

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="dataTable">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task BulkInsertAsync(DataTable dataTable, IBulkInsertionOptions options = null);

        #endregion

        #region Migration

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="migrationInfo">Migration info</param>
        /// <param name="options">Options</param>
        Task MigrateAsync(MigrationInfo migrationInfo, DataOperationOptions options = null);

        #endregion

        #region Get tables

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="options">Data operation options</param>
        /// <returns></returns>
        Task<List<DatabaseTableInfo>> GetTablesAsync(DataOperationOptions options = null);

        #endregion
    }
}
