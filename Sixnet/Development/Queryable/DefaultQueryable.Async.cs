using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Repository;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines default implement for ISixnetQueryable
    /// </summary>
    internal partial class DefaultQueryable
    {
        #region Data access

        #region Update

        /// <summary>
        /// Update data
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="configure">Configure options</param>
        /// <returns>Affected rows</returns>
        public async Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return await repository.UpdateAsync(fieldsAssignment, this, configure).ConfigureAwait(false);
            }
            return await DataClientContext.UpdateAsync(fieldsAssignment, this, configure).ConfigureAwait(false);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return await repository.DeleteAsync(this, configure).ConfigureAwait(false);
            }
            return await DataClientContext.DeleteAsync(this, configure).ConfigureAwait(false);
        }

        #endregion

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public async Task<T> FirstAsync<T>(Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is IRepository<T> repository)
            {
                return await repository.GetAsync(this, configure).ConfigureAwait(false);
            }
            return await DataClientContext.QueryFirstAsync<T>(this, configure).ConfigureAwait(false);
        }

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public async Task<List<T>> ToListAsync<T>(Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is IRepository<T> repository)
            {
                return await repository.GetListAsync(this, configure).ConfigureAwait(false);
            }
            return await DataClientContext.QueryAsync<T>(this, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public async Task<List<TReturn>> ToListAsync<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(this, dataMappingFunc, configure).ConfigureAwait(false);
        }

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
        public async Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(this, dataMappingFunc, configure).ConfigureAwait(false);
        }

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
        public async Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(this, dataMappingFunc, configure).ConfigureAwait(false);
        }

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
        public async Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(this, dataMappingFunc, configure).ConfigureAwait(false);
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
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public async Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(this, dataMappingFunc, configure).ConfigureAwait(false);
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
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public async Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(this, dataMappingFunc, configure).ConfigureAwait(false);
        }

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public async Task<PagingInfo<T>> ToPagingAsync<T>(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is IRepository<T> repository)
            {
                return await repository.GetPagingAsync(this, pagingFilter, configure).ConfigureAwait(false);
            }
            return await DataClientContext.QueryPagingAsync<T>(this, pagingFilter, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public async Task<PagingInfo<T>> ToPagingAsync<T>(int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            return await ToPagingAsync<T>(PagingFilter.Create(page, pageSize), configure).ConfigureAwait(false);
        }

        #endregion

        #region Any

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public async Task<bool> AnyAsync(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return await repository.ExistsAsync(this, configure).ConfigureAwait(false);
            }
            return await DataClientContext.ExistsAsync(this, configure).ConfigureAwait(false);
        }

        #endregion

        #region Count

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public async Task<int> CountAsync(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return await repository.CountAsync(this, configure).ConfigureAwait(false);
            }
            return await DataClientContext.CountAsync(this, configure).ConfigureAwait(false);
        }

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public async Task<TValue> MaxAsync<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return await repository.MaxAsync<TValue>(this, configure).ConfigureAwait(false);
            }
            return await repository.MaxAsync<TValue>(this, configure).ConfigureAwait(false);
        }

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public async Task<TValue> MinAsync<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return await repository.MinAsync<TValue>(this, configure).ConfigureAwait(false);
            }
            return await repository.MinAsync<TValue>(this, configure).ConfigureAwait(false);
        }

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public async Task<TValue> SumAsync<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return await repository.SumAsync<TValue>(this, configure).ConfigureAwait(false);
            }
            return await repository.SumAsync<TValue>(this, configure).ConfigureAwait(false);
        }

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public async Task<TValue> AvgAsync<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return await repository.AvgAsync<TValue>(this, configure).ConfigureAwait(false);
            }
            return await repository.AvgAsync<TValue>(this, configure).ConfigureAwait(false);
        }

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public async Task<TValue> ScalarAsync<TValue>(Action<DataOperationOptions> configure = null)
        {
            var repository = queryableContext.Repository;
            if (repository != null)
            {
                return await repository.ScalarAsync<TValue>(this, configure).ConfigureAwait(false);
            }
            return await repository.ScalarAsync<TValue>(this, configure).ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}
