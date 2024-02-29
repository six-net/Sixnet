using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Repository
{
    /// <summary>
    /// Default repository
    /// </summary>
    public partial class DefaultRepository<TModel> : DefaultRootRepository<TModel>
        where TModel : class, ISixnetEntity<TModel>
    {
        #region Function

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Added datas</returns>
        protected override async Task<List<TModel>> AddDataAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.InsertAsync(datas, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Add datas and return identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        protected override async Task<List<TIdentity>> AddDataReturnIdentitiesAsync<TIdentity>(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.InsertReturnIdentitiesAsync<TIdentity>(datas, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="newDatas">New datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated datas</returns>
        protected override async Task<List<TModel>> UpdateDataAsync(IEnumerable<TModel> newDatas, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.UpdateAsync(newDatas, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Update fields
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected override async Task<int> UpdateFieldsAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.UpdateAsync(fieldsAssignment, queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected override async Task<int> DeleteDataAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return 0;
            }
            return await dataAccess.DeleteAsync(datas, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected override async Task<int> DeleteDataAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.DeleteAsync(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get first data
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected override async Task<TModel> GetFirstDataAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.QueryFirstAsync(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        protected override async Task<List<TModel>> GetDataListAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return (await dataAccess.QueryAsync(queryable, configure).ConfigureAwait(false)) ?? new List<TModel>(0);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        protected override async Task<List<TModel>> GetDataListAsync<TFirst, TSecond>(ISixnetQueryable queryable, Func<TFirst, TSecond, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return (await dataAccess.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false)) ?? new List<TModel>(0);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        protected override async Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return (await dataAccess.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false)) ?? new List<TModel>(0);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        protected override async Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return (await dataAccess.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false)) ?? new List<TModel>(0);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        protected override async Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return (await dataAccess.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false)) ?? new List<TModel>(0);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TThird">Third data type</typeparam>
        /// <typeparam name="TFourth">Fourth data type</typeparam>
        /// <typeparam name="TFifth">Fifth data type</typeparam>
        /// <typeparam name="TSixth">Sixth data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        protected override async Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return (await dataAccess.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false)) ?? new List<TModel>(0);
        }

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
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        protected override async Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return (await dataAccess.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false)) ?? new List<TModel>(0);
        }

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data paging</returns>
        protected override async Task<PagingInfo<TModel>> GetDataPagingAsync(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            var entityPaging = await dataAccess.QueryPagingAsync(queryable, pagingFilter, configure).ConfigureAwait(false);
            return entityPaging.ConvertTo<TModel>();
        }

        /// <summary>
        /// Get data list by current datas
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        protected override async Task<List<TModel>> GetDataListByCurrentAsync(IEnumerable<TModel> currentDatas, Action<DataOperationOptions> configure = null)
        {
            if (currentDatas.IsNullOrEmpty())
            {
                return new List<TModel>(0);
            }
            return await dataAccess.QueryByCurrentAsync(currentDatas, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        protected override async Task<bool> ExistsDataAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.ExistsAsync(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get count value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Count value</returns>
        protected override async Task<int> CountValueAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.CountAsync(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        protected override async Task<TValue> MaxValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.MaxAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        protected override async Task<TValue> MinValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.MinAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        protected override async Task<TValue> SumValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.SumAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        protected override async Task<TValue> AvgValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.AvgAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable"></param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        protected override async Task<TValue> ScalarValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await dataAccess.ScalarAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        #endregion
    }
}
