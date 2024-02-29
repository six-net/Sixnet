using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;
using System.Threading.Tasks;
using Sixnet.Development.Work;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Data access contract
    /// </summary>
    public partial interface ISixnetDataAccess<TEntity>
    {
        #region Insert

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Inserted entities</returns>
        Task<List<TEntity>> InsertAsync(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Insert entities and return identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        Task<List<TIdentity>> InsertReturnIdentitiesAsync<TIdentity>(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null);

        #endregion

        #region Update

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated entities</returns>
        Task<List<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion

        #region Query

        /// <summary>
        /// Query entity
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return an entity</returns>
        Task<TEntity> QueryFirstAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Query entities
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity list</returns>
        Task<List<TEntity>> QueryAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> QueryMappingAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Query by current
        /// </summary>
        /// <param name="currentEntities">Current entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        Task<List<TEntity>> QueryByCurrentAsync(IEnumerable<TEntity> currentEntities, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Query entity paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity paging</returns>
        Task<PagingInfo<TEntity>> QueryPagingAsync(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Whether has data</returns>
        Task<bool> ExistsAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        Task<int> CountAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        Task<TValue> MaxAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        Task<TValue> MinAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        Task<TValue> SumAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        Task<TValue> AvgAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Query scalar value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        Task<TValue> ScalarAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion
    }
}
