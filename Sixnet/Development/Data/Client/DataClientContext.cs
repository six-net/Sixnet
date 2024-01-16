using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Development.Work;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Client
{
    /// <summary>
    /// Data client context
    /// </summary>
    internal static partial class DataClientContext
    {
        #region Insert

        /// <summary>
        /// Insert datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Inserted datas</returns>
        public static List<T> Insert<T>(IEnumerable<T> datas, Action<DataOperationOptions> configure = null) where T : class
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Insert(datas, options);
            }
            using (var dataClient = GetDataClient(false))
            {
                var newDatas = dataClient.Insert(datas, options);
                dataClient.Commit();
                return newDatas;
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
        public static List<TIdentity> InsertReturnIdentities<T, TIdentity>(IEnumerable<T> datas, Action<DataOperationOptions> configure = null) where T : class
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.InsertReturnIdentities<T, TIdentity>(datas, options);
            }
            using (var dataClient = GetDataClient(false))
            {
                var newIdentities = dataClient.InsertReturnIdentities<T, TIdentity>(datas, options);
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
        public static List<T> Update<T>(IEnumerable<T> datas, Action<DataOperationOptions> configure = null) where T : class, IEntity<T>
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Update(datas, options);
            }
            using (var dataClient = GetDataClient(false))
            {
                var newDatas = dataClient.Update(datas, options);
                dataClient.Commit();
                return newDatas;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public static int Update(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Update(fieldsAssignment, queryable, options);
            }
            using (var dataClient = GetDataClient(false))
            {
                var affectedNum = dataClient.Update(fieldsAssignment, queryable, options);
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
        public static int Delete<T>(IEnumerable<T> datas, Action<DataOperationOptions> configure = null) where T : class, IEntity<T>
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Delete(datas, options);
            }
            using (var dataClient = GetDataClient(false))
            {
                var affectedNum = dataClient.Delete(datas, options);
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
        public static int Delete(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Delete(queryable, options);
            }
            using (var dataClient = GetDataClient(false))
            {
                var affectedNum = dataClient.Delete(queryable, options);
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
        public static T QueryFirst<T>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.QueryFirst<T>(queryable, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.QueryFirst<T>(queryable, options);
            }
        }

        /// <summary>
        /// Query datas
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return data list</returns>
        public static List<T> Query<T>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Query<T>(queryable, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.Query<T>(queryable, options);
            }
        }

        /// <summary>
        /// Query by current
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static List<T> QueryByCurrent<T>(IEnumerable<T> currentDatas, Action<DataOperationOptions> configure = null) where T : class, IEntity<T>
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.QueryByCurrent(currentDatas, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.QueryByCurrent(currentDatas, options);
            }
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return data paging</returns>
        public static PagingInfo<T> QueryPaging<T>(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.QueryPaging<T>(queryable, pagingFilter, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.QueryPaging<T>(queryable, pagingFilter, options);
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.QueryMapping(queryable, dataMappingFunc, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.QueryMapping(queryable, dataMappingFunc, options);
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.QueryMapping(queryable, dataMappingFunc, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.QueryMapping(queryable, dataMappingFunc, options);
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.QueryMapping(queryable, dataMappingFunc, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.QueryMapping(queryable, dataMappingFunc, options);
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.QueryMapping(queryable, dataMappingFunc, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.QueryMapping(queryable, dataMappingFunc, options);
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.QueryMapping(queryable, dataMappingFunc, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.QueryMapping(queryable, dataMappingFunc, options);
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
        public static List<TReturn> QueryMapping<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.QueryMapping(queryable, dataMappingFunc, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.QueryMapping(queryable, dataMappingFunc, options);
            }
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Whether has data</returns>
        public static bool Exists(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Exists(queryable, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.Exists(queryable, options);
            }
        }

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static int Count(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Count(queryable, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.Count(queryable, options);
            }
        }

        /// <summary>
        /// Max value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static TValue Max<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Max<TValue>(queryable, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.Max<TValue>(queryable, options);
            }
        }

        /// <summary>
        /// Min value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static TValue Min<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Min<TValue>(queryable, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.Min<TValue>(queryable, options);
            }
        }

        /// <summary>
        /// Sum value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static TValue Sum<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Sum<TValue>(queryable, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.Sum<TValue>(queryable, options);
            }
        }

        /// <summary>
        /// Avg value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        public static TValue Avg<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Avg<TValue>(queryable, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.Avg<TValue>(queryable, options);
            }
        }

        /// <summary>
        /// Query scalar value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public static TValue Scalar<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            var options = GetDataOperationOptions(configure);

            if (UnitOfWork.Current != null)
            {
                return UnitOfWork.Current.DataClient.Scalar<TValue>(queryable, options);
            }
            using (var dataClient = GetDataClient(true))
            {
                return dataClient.Scalar<TValue>(queryable, options);
            }
        }

        #endregion

        #region Util

        #region Get data client

        /// <summary>
        /// Get a data client
        /// </summary>
        /// <returns></returns>
        static IDataClient GetDataClient(bool forQuery)
        {
            return DataManager.GetClient(true, !forQuery);
        }

        #endregion

        #region Get data options

        static DataOperationOptions GetDataOperationOptions(Action<DataOperationOptions> configure = null)
        {
            var options = DataOperationOptions.Create();
            configure?.Invoke(options);
            return options;
        }

        #endregion

        #endregion
    }
}
