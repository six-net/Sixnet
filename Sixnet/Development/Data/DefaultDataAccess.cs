using System;
using System.Collections.Generic;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Defines base data access
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
        public List<TEntity> Insert(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Insert(entities, configure);
        }

        /// <summary>
        /// Insert entities and return identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        public List<TIdentity> InsertReturnIdentities<TIdentity>(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.InsertReturnIdentities<TEntity, TIdentity>(entities, configure);
        }

        #endregion

        #region Update

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated entities</returns>
        public List<TEntity> Update(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Update(entities, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Update(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Update(fieldsAssignment, queryable, configure);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Delete(IEnumerable<TEntity> entities, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Delete(entities, configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Delete(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Delete(queryable, configure);
        }

        #endregion

        #region Query

        /// <summary>
        /// Query entity
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return an entity</returns>
        public TEntity QueryFirst(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryFirst<TEntity>(queryable, configure);
        }

        /// <summary>
        /// Query entities
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity list</returns>
        public List<TEntity> Query(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Query<TEntity>(queryable, configure);
        }

        /// <summary>
        /// Query by current
        /// </summary>
        /// <param name="currentEntities">Current entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public List<TEntity> QueryByCurrent(IEnumerable<TEntity> currentEntities, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryByCurrent(currentEntities, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
        }

        /// <summary>
        /// Query entity paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity paging</returns>
        public PagingInfo<TEntity> QueryPaging(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.QueryPaging<TEntity>(queryable, pagingFilter, configure);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Whether has data</returns>
        public bool Exists(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Exists(queryable, configure);
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public int Count(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Count(queryable, configure);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public TValue Max<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Max<TValue>(queryable, configure);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public TValue Min<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Min<TValue>(queryable, configure);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public TValue Sum<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Sum<TValue>(queryable, configure);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public TValue Avg<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Avg<TValue>(queryable, configure);
        }

        /// <summary>
        /// Query scalar value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public TValue Scalar<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DataClientContext.Scalar<TValue>(queryable, configure);
        }

        #endregion
    }
}
