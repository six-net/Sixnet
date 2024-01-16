using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines queryable contract
    /// </summary>
    public partial interface ISixnetQueryable
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
        Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, Action<DataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        Task<int> DeleteAsync(Action<DataOperationOptions> configure = null);

        #endregion

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        Task<T> FirstAsync<T>(Action<DataOperationOptions> configure = null);

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        Task<List<T>> ToListAsync<T>(Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> ToListAsync<TFirst, TSecond, TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TReturn>(Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TFourth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        Task<List<TReturn>> ToListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        Task<PagingInfo<T>> ToPagingAsync<T>(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        Task<PagingInfo<T>> ToPagingAsync<T>(int page, int pageSize, Action<DataOperationOptions> configure = null);

        #endregion

        #region Any

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        Task<bool> AnyAsync(Action<DataOperationOptions> configure = null);

        #endregion

        #region Count

        /// <summary>
        /// Count data num
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        Task<int> CountAsync(Action<DataOperationOptions> configure = null);

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        Task<TValue> MaxAsync<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        Task<TValue> MinAsync<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        Task<TValue> SumAsync<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        Task<TValue> AvgAsync<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        Task<TValue> ScalarAsync<TValue>(Action<DataOperationOptions> configure = null);

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines model queryable
    /// </summary>
    public partial interface ISixnetModelQueryable<TModel> : ISixnetQueryable
    {
        #region Data access

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        Task<TModel> FirstAsync(Action<DataOperationOptions> configure = null);

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        Task<List<TModel>> ToListAsync(Action<DataOperationOptions> configure = null);

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        Task<PagingInfo<TModel>> ToPagingAsync(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        Task<PagingInfo<TModel>> ToPagingAsync(int page, int pageSize, Action<DataOperationOptions> configure = null);

        #endregion 

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        Task<int> UpdateAsync(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond> : ISixnetModelQueryable<TFirst>
    {
        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> ToListAsync<TReturn>(Func<TFirst, TSecond, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird> : ISixnetModelQueryable<TFirst>
    {
        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> ToListAsync<TReturn>(Func<TFirst, TSecond, TThird, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    /// <typeparam name="TFourth">TFourth</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird, TFourth> : ISixnetModelQueryable<TFirst>
    {
        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> ToListAsync<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    /// <typeparam name="TFourth">TFourth</typeparam>
    /// <typeparam name="TFifth">TFifth</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth> : ISixnetModelQueryable<TFirst>
    {
        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> ToListAsync<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    /// <typeparam name="TFourth">TFourth</typeparam>
    /// <typeparam name="TFifth">TFifth</typeparam>
    /// <typeparam name="TSixth">TSixth</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth> : ISixnetModelQueryable<TFirst>
    {
        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> ToListAsync<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion
    }

    /// <summary>
    /// Defines queryable contract
    /// </summary>
    /// <typeparam name="TFirst">TFirst</typeparam>
    /// <typeparam name="TSecond">TSecond</typeparam>
    /// <typeparam name="TThird">TThird</typeparam>
    /// <typeparam name="TFourth">TFourth</typeparam>
    /// <typeparam name="TFifth">TFifth</typeparam>
    /// <typeparam name="TSixth">TSixth</typeparam>
    /// <typeparam name="TSeventh">TSeventh</typeparam>
    public partial interface ISixnetQueryable<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> : ISixnetModelQueryable<TFirst>
    {
        #region Data access

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TReturn">Return data type</typeparam>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        Task<List<TReturn>> ToListAsync<TReturn>(Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #endregion
    }
}
