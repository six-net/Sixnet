using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Default data access
    /// </summary>
    public partial class DefaultDataAccess<TEntity> : ISixnetDataAccess<TEntity> where TEntity : SixnetBaseEntity<TEntity>, new()
    {
        #region Insert

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Inserted entities</returns>
        public async Task<List<TEntity>> InsertAsync(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.InsertAsync(entities, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Insert entities and return identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        public async Task<List<TIdentity>> InsertReturnIdentitiesAsync<TIdentity>(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.InsertReturnIdentitiesAsync<TEntity, TIdentity>(entities, configure).ConfigureAwait(false);
        }

        #endregion

        #region Update

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated entities</returns>
        public async Task<List<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.UpdateAsync(entities, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public async Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.UpdateAsync(fieldsAssignment, queryable, configure).ConfigureAwait(false);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.DeleteAsync(entities, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public async Task<int> DeleteAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.DeleteAsync(queryable, configure).ConfigureAwait(false);
        }

        #endregion

        #region Query

        /// <summary>
        /// Query entity
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return an entity</returns>
        public async Task<TEntity> QueryFirstAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryFirstAsync<TEntity>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Query entities
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity list</returns>
        public async Task<List<TEntity>> QueryAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryAsync<TEntity>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Query by current
        /// </summary>
        /// <param name="currentEntities">Current entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public async Task<List<TEntity>> QueryByCurrentAsync(IEnumerable<TEntity> currentEntities, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryByCurrentAsync<TEntity>(currentEntities, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Query entity paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity paging</returns>
        public async Task<PagingInfo<TEntity>> QueryPagingAsync(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryPagingAsync<TEntity>(queryable, pagingFilter, configure).ConfigureAwait(false);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false);
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
        public async Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.QueryMappingAsync(queryable, dataMappingFunc, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Whether has data</returns>
        public async Task<bool> ExistsAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.ExistsAsync(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public async Task<int> CountAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.CountAsync(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public async Task<TValue> MaxAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.MaxAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public async Task<TValue> MinAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.MinAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public async Task<TValue> SumAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.SumAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public async Task<TValue> AvgAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.AvgAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Query scalar value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public async Task<TValue> ScalarAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return await DataClientContext.ScalarAsync<TValue>(queryable, configure).ConfigureAwait(false);
        }

        #endregion
    }
}
