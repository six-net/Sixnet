using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Queryable;
using Sixnet.Development.Work;
using Sixnet.Model.Paging;

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
        List<TEntity> Insert(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Insert entities and return identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        List<TIdentity> InsertReturnIdentities<TIdentity>(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null);

        #endregion

        #region Update

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated entities</returns>
        List<TEntity> Update(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Delete(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Delete(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion

        #region Query

        /// <summary>
        /// Query entity
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return an entity</returns>
        TEntity QueryFirst(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Query entities
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity list</returns>
        List<TEntity> Query(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Query by current
        /// </summary>
        /// <param name="currentEntities">Current entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        List<TEntity> QueryByCurrent(IEnumerable<TEntity> currentEntities, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Query entity paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity paging</returns>
        PagingInfo<TEntity> QueryPaging(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Whether has data</returns>
        bool Exists(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        int Count(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        TValue Max<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        TValue Min<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        TValue Sum<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        TValue Avg<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Query scalar value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        TValue Scalar<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion
    }
}
