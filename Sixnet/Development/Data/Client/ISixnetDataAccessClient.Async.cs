// "Company © 2025. All rights reserved."

using System.Data;
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
    public partial interface ISixnetDataAccessClient
    {
        #region Query

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        Task<List<T>> QueryAsync<T>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        Task<List<T>> QueryAsync<T>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<T> QueryFirstAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        Task<T> QueryFirstAsync<T>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query the first data
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns>Data list</returns>
        Task<T> QueryFirstAsync<T>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        Task<PagingInfo<T>> QueryPagingAsync<T>(Expression<Func<T, bool>> conditionExpression, PagingFilter pagingFilter, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        Task<PagingInfo<T>> QueryPagingAsync<T>(Expression<Func<T, bool>> conditionExpression, int page, int pageSize, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="options">Options</param>
        /// <returns>Dynamic object paging</returns>
        Task<PagingInfo<T>> QueryPagingAsync<T>(ISixnetQueryable queryable, PagingFilter pagingFilter, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query paging data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="options">Options</param>
        /// <returns>Dynamic object paging</returns>
        Task<PagingInfo<T>> QueryPagingAsync<T>(ISixnetQueryable queryable, int page, int pageSize, SixnetDataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(string script, object parameters, Func<TFirst, TSecond, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null);

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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, SixnetDataOperationOptions options = null);

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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="options">Options</param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string script, object parameters, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Paging data</returns>
        Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return whether the data exists or not</returns>
        Task<bool> ExistsAsync(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> CountAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> CountAsync(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> MaxAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> MaxAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> MinAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> MinAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> SumAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> SumAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> AvgAsync<T, TValue>(Expression<Func<T, TValue>> field, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TValue> AvgAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Scalar value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        Task<TValue> ScalarAsync<TValue>(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Scalar value
        /// </summary>
        /// <typeparam name="TValue">Data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns>Return the data</returns>
        Task<TValue> ScalarAsync<TValue>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="queries">queries</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        Task<DataSet> QueryMultipleAsync(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns>Return the dataset</returns>
        Task<DataSet> QueryMultipleAsync(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="queries">Queries</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

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
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

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
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

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
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Query multiple data
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

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
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(IEnumerable<ISixnetQueryable> queries, SixnetDataOperationOptions options = null);

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
        /// <param name="script">Script</param>
        /// <param name="parameters">Parameters</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<Tuple<List<TFirst>, List<TSecond>, List<TThird>, List<TFourth>, List<TFifth>, List<TSixth>, List<TSeventh>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string script, object parameters = null, DataScriptType scriptType = DataScriptType.Text, SixnetDataOperationOptions options = null);

        #endregion

        #region Insert

        /// <summary>
        /// Insert datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> InsertAsync<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>;

        /// <summary>
        /// Insert and return identities
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<List<TIdentity>> InsertReturnIdentitiesAsync<T, TIdentity>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>;

        /// <summary>
        /// Insert data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> InsertAsync<T>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>;

        /// <summary>
        /// Insert and return identity
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<TIdentity> InsertReturnIdentityAsync<T, TIdentity>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>;

        #endregion

        #region Update

        /// <summary>
        /// Update datas
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>;

        /// <summary>
        /// Update data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>;

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> UpdateAsync<T>(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> UpdateAsync<T>(FieldsAssignment fieldsAssignment, Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync<T>(IEnumerable<T> datas, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>;

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync<T>(T data, SixnetDataOperationOptions options = null) where T : class, ISixnetEntity<T>;

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync<T>(Expression<Func<T, bool>> conditionExpression, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="options">Options</param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync(ISixnetQueryable queryable, SixnetDataOperationOptions options = null);

        #endregion

        #region Execution

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="commands">Data commands</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(IEnumerable<SixnetDataCommand> commands);

        /// <summary>
        /// Execution
        /// </summary>
        /// <param name="command">Data command</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task<int> ExecuteAsync(SixnetDataCommand command);

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <param name="dataTable">Data table</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        Task BulkInsertAsync(DataTable dataTable, ISixnetBulkInsertionOptions options = null);

        #endregion

        #region Migration

        /// <summary>
        /// Migrate
        /// </summary>
        /// <param name="migrationInfo">Migration info</param>
        /// <param name="options">Data operation options</param>
        Task MigrateAsync(MigrationInfo migrationInfo, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Create all entity tables
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task CreateAllEntityTablesAsync(SixnetDataOperationOptions options = null);

        /// <summary>
        /// Delete all entity tables
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task DeleteAllEntityTablesAsync(SixnetDataOperationOptions options = null);

        /// <summary>
        /// Create entity table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="options"></param>
        Task CreateTableAsync<TEntity>(SixnetDataOperationOptions options = null) where TEntity : ISixnetEntity;

        /// <summary>
        /// Delete entity table
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="options"></param>
        Task DeleteTableAsync<TEntity>(SixnetDataOperationOptions options = null) where TEntity : ISixnetEntity;

        /// <summary>
        /// Add field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="options"></param>
        Task AddFieldAsync<TEntity>(Expression<Func<TEntity, object>> field, SixnetDataOperationOptions options = null) where TEntity : ISixnetEntity;

        /// <summary>
        /// Delete field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        Task DeleteFieldAsync<TEntity>(Expression<Func<TEntity, object>> field, SixnetDataOperationOptions options = null) where TEntity : ISixnetEntity;

        /// <summary>
        /// Alter field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="options"></param>
        Task AlterFieldAsync<TEntity>(Expression<Func<TEntity, object>> field, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Alter field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <param name="options"></param>
        Task AlterFieldAsync<TEntity>(Expression<Func<TEntity, object>> field, Action<DataField> configureField = null, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Alter field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="field"></param>
        /// <param name="options"></param>
        Task AlterFieldAsync<TEntity>(string fieldName, DataField field, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Create table
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="options">Options</param>
        Task CreateTableAsync(Type entityType, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Delete table
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        Task DeleteTableAsync(List<Type> entityTypes, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Add field
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        Task AddFieldAsync(Type entityType, List<DataField> fields, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Delete field
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        Task DeleteFieldAsync(Type entityType, List<DataField> fields, SixnetDataOperationOptions options = null);

        /// <summary>
        /// Alter fields
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="fields"></param>
        /// <param name="options"></param>
        Task AlterFieldAsync(Type entityType, Dictionary<string, DataField> fields, SixnetDataOperationOptions options);

        #endregion

        #region Get tables

        /// <summary>
        /// Get tables
        /// </summary>
        /// <param name="options">Data operation options</param>
        /// <returns></returns>
        Task<List<SixnetDataTable>> GetTablesAsync(SixnetDataOperationOptions options = null);

        #endregion
    }
}
