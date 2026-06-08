// "Company © 2025. All rights reserved."

using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Repository
{
    /// <summary>
    /// Defines default repository
    /// </summary>
    /// <typeparam name="TModel">Entity</typeparam>
    public partial class SixnetDefaultRepository<TModel> : SixnetDefaultRootRepository<TModel>
        where TModel : class, ISixnetEntity<TModel>
    {
        #region Fields

        /// <summary>
        /// Data access
        /// </summary>
        readonly ISixnetDataAccess<TModel> dataAccess = null;

        #endregion

        #region Constructor

        public SixnetDefaultRepository(ISixnetDataAccess<TModel> dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        #endregion

        #region Function

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected override int AddData(IEnumerable<TModel> datas, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Insert(datas, configure);
        }

        /// <summary>
        /// Add datas and return identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        protected override List<TIdentity> AddDataReturnIdentities<TIdentity>(IEnumerable<TModel> datas, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.InsertReturnIdentities<TIdentity>(datas, configure);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="newDatas">New datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected override int UpdateData(IEnumerable<TModel> newDatas, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Update(newDatas, configure);
        }

        /// <summary>
        /// Update fields
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected override int UpdateFields(SixnetFieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Update(fieldsAssignment, queryable, configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected override int DeleteData(IEnumerable<TModel> datas, Action<SixnetDataOperationOptions> configure = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return 0;
            }
            return dataAccess.Delete(datas, configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected override int DeleteData(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Delete(queryable, configure);
        }

        /// <summary>
        /// Get first data
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected override TModel GetFirstData(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.QueryFirst(queryable, configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        protected override List<TModel> GetDataList(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return (dataAccess.Query(queryable, configure)) ?? new List<TModel>(0);
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
        protected override List<TModel> GetDataList<TFirst, TSecond>(ISixnetQueryable queryable, Func<TFirst, TSecond, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return (dataAccess.QueryMapping(queryable, dataMappingFunc, configure)) ?? new List<TModel>(0);
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
        protected override List<TModel> GetDataList<TFirst, TSecond, TThird>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return (dataAccess.QueryMapping(queryable, dataMappingFunc, configure)) ?? new List<TModel>(0);
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
        protected override List<TModel> GetDataList<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return (dataAccess.QueryMapping(queryable, dataMappingFunc, configure)) ?? new List<TModel>(0);
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
        protected override List<TModel> GetDataList<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return (dataAccess.QueryMapping(queryable, dataMappingFunc, configure)) ?? new List<TModel>(0);
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
        protected override List<TModel> GetDataList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return (dataAccess.QueryMapping(queryable, dataMappingFunc, configure)) ?? new List<TModel>(0);
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
        protected override List<TModel> GetDataList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null)
        {
            return (dataAccess.QueryMapping(queryable, dataMappingFunc, configure)) ?? new List<TModel>(0);
        }

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data paging</returns>
        protected override SixnetPagingInfo<TModel> GetDataPaging(ISixnetQueryable queryable, SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null)
        {
            var entityPaging = dataAccess.QueryPaging(queryable, pagingFilter, configure);
            return entityPaging.ConvertTo<TModel>();
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        protected override bool ExistsData(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Exists(queryable, configure);
        }

        /// <summary>
        /// Get count value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Count value</returns>
        protected override int CountValue(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Count(queryable, configure);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        protected override TValue MaxValue<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Max<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        protected override TValue MinValue<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Min<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        protected override TValue SumValue<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Sum<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        protected override TValue AvgValue<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Scalar<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable"></param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        protected override TValue ScalarValue<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return dataAccess.Scalar<TValue>(queryable, configure);
        }

        #endregion
    }
}
