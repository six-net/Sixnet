// "Company © 2025. All rights reserved."

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
    public partial class SixnetDefaultDataAccess<TEntity> : ISixnetDataAccess<TEntity> where TEntity : SixnetBaseEntity<TEntity>, new()
    {
        #region Insert

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Insert(IEnumerable<TEntity> entities, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Insert(entities, configure);
        }

        /// <summary>
        /// Insert entities and return identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        public List<TIdentity> InsertReturnIdentities<TIdentity>(IEnumerable<TEntity> entities, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.InsertReturnIdentities<TEntity, TIdentity>(entities, configure);
        }

        #endregion

        #region Update

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Update(IEnumerable<TEntity> entities, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Update(entities, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Update(SixnetFieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Update(fieldsAssignment, queryable, configure);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Delete(IEnumerable<TEntity> entities, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Delete(entities, configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Delete(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Delete(queryable, configure);
        }

        #endregion

        #region Query

        /// <summary>
        /// Query entity
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return an entity</returns>
        public TEntity QueryFirst(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryFirst<TEntity>(queryable, configure);
        }

        /// <summary>
        /// Query entities
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity list</returns>
        public List<TEntity> Query(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Query<TEntity>(queryable, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
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
        public List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryMapping(queryable, dataMappingFunc, configure);
        }

        /// <summary>
        /// Query entity paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return entity paging</returns>
        public SixnetPagingInfo<TEntity> QueryPaging(ISixnetQueryable queryable, SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.QueryPaging<TEntity>(queryable, pagingFilter, configure);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Whether has data</returns>
        public bool Exists(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Exists(queryable, configure);
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public int Count(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Count(queryable, configure);
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public TValue Max<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Max<TValue>(queryable, configure);
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public TValue Min<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Min<TValue>(queryable, configure);
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public TValue Sum<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Sum<TValue>(queryable, configure);
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public TValue Avg<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Avg<TValue>(queryable, configure);
        }

        /// <summary>
        /// Query scalar value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public TValue Scalar<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return SixnetDataClientContext.Scalar<TValue>(queryable, configure);
        }

        #endregion
    }
}
