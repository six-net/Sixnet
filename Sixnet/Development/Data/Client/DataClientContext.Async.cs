using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Development.Work;
using Sixnet.Model.Paging;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Client
{
    /// <summary>
    /// Data client context
    /// </summary>
    internal static partial class DataClientContext
    {
        #region Insert

        /// <summary>
        /// Insert
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Inserted datas</returns>
        public static async Task<List<T>> InsertAsync<T>(IEnumerable<T> datas, Action<DataOperationOptions> configure = null) where T : class
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.InsertAsync(datas, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(false))
            {
                var newEntities = await dataClient.InsertAsync(datas, options).ConfigureAwait(false);
                dataClient.Commit();
                return newEntities;
            }
        }

        /// <summary>
        /// Insert and return identities
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static async Task<List<TIdentity>> InsertReturnIdentitiesAsync<T, TIdentity>(IEnumerable<T> datas, Action<DataOperationOptions> configure = null) where T : class
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.InsertReturnIdentitiesAsync<T, TIdentity>(datas, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(false))
            {
                var newIdentities = await dataClient.InsertReturnIdentitiesAsync<T, TIdentity>(datas, options).ConfigureAwait(false);
                dataClient.Commit();
                return newIdentities;
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated datas</returns>
        public static async Task<List<T>> UpdateAsync<T>(IEnumerable<T> datas, Action<DataOperationOptions> configure = null) where T : class, ISixnetEntity<T>
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.UpdateAsync(datas, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(false))
            {
                var newEntities = await dataClient.UpdateAsync(datas, options).ConfigureAwait(false);
                dataClient.Commit();
                return newEntities;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public static async Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.UpdateAsync(fieldsAssignment, queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(false))
            {
                var affectedNum = await dataClient.UpdateAsync(fieldsAssignment, queryable, options).ConfigureAwait(false);
                dataClient.Commit();
                return affectedNum;
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public static async Task<int> DeleteAsync<T>(IEnumerable<T> datas, Action<DataOperationOptions> configure = null) where T : class, ISixnetEntity<T>
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.DeleteAsync(datas, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(false))
            {
                var affectedNum = await dataClient.DeleteAsync(datas, options).ConfigureAwait(false);
                dataClient.Commit();
                return affectedNum;
            }
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public static async Task<int> DeleteAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.DeleteAsync(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(false))
            {
                var affectedNum = await dataClient.DeleteAsync(queryable, options).ConfigureAwait(false);
                dataClient.Commit();
                return affectedNum;
            }
        }

        #endregion

        #region Query

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return an data</returns>
        public static async Task<T> QueryFirstAsync<T>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryFirstAsync<T>(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryFirstAsync<T>(queryable, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return data list</returns>
        public static async Task<List<T>> QueryAsync<T>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryAsync<T>(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryAsync<T>(queryable, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Query by current
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static async Task<List<T>> QueryByCurrentAsync<T>(IEnumerable<T> currentDatas, Action<DataOperationOptions> configure = null) where T : class, ISixnetEntity<T>
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryByCurrentAsync<T>(currentDatas, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryByCurrentAsync<T>(currentDatas, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return data paging</returns>
        public static async Task<PagingInfo<T>> QueryPagingAsync<T>(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryPagingAsync<T>(queryable, pagingFilter, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryPagingAsync<T>(queryable, pagingFilter, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
        }

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
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
        }

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
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
        }

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
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public static async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
        }

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
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public async static Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.QueryMappingAsync(queryable, dataMappingFunc, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Whether has data</returns>
        public static async Task<bool> ExistsAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.ExistsAsync(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.ExistsAsync(queryable, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static async Task<int> CountAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.CountAsync(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.CountAsync(queryable, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static async Task<TValue> MaxAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.MaxAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.MaxAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static async Task<TValue> MinAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.MinAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.MinAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static async Task<TValue> SumAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.SumAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.SumAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static async Task<TValue> AvgAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.AvgAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.AvgAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Query scalar value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public static async Task<TValue> ScalarAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return await UnitOfWork.Current.DataClient.ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
            using (var dataClient = GetDataClient(true))
            {
                return await dataClient.ScalarAsync<TValue>(queryable, options).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
