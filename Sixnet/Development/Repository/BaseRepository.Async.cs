using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Repository
{
    /// <summary>
    /// Defines base repository
    /// </summary>
    public abstract partial class BaseRepository<TModel> : ISixnetRepository<TModel>
    {
        #region Add

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Added data</returns>
        public abstract Task<TModel> AddAsync(TModel data, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Add data and return identity
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identity</returns>
        public abstract Task<TIdentity> AddReturnIdentityAsync<TIdentity>(TModel data, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Add datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Added datas</returns>
        public abstract Task<List<TModel>> AddAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Add datas and return identiies
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        public abstract Task<List<TIdentity>> AddReturnIdentitiesAsync<TIdentity>(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        #endregion

        #region Update

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated data</returns>
        public abstract Task<TModel> UpdateAsync(TModel data, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated datas</returns>
        public abstract Task<List<TModel>> UpdateAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> UpdateAsync(Expression<Func<TModel, bool>> fieldsAssignmentExpression, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> UpdateAsync(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> UpdateAsync(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> DeleteAsync(TModel data, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> DeleteAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> DeleteAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> DeleteAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract Task<int> DeleteAsync(Action<DataOperationOptions> configure = null);

        #endregion

        #region Get

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public abstract Task<TModel> GetAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public abstract Task<TModel> GetAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public abstract Task<TModel> GetAsync(Action<DataOperationOptions> configure = null);

        #endregion

        #region Get list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public abstract Task<List<TModel>> GetListAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public abstract Task<List<TModel>> GetListAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public abstract Task<List<TModel>> GetListAsync(Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public abstract Task<List<TModel>> GetListAsync<TFirst, TSecond>(ISixnetQueryable queryable, Func<TFirst, TSecond, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        public abstract Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        public abstract Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        public abstract Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        public abstract Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        public abstract Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #region Get paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract Task<PagingInfo<TModel>> GetPagingAsync(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract Task<PagingInfo<TModel>> GetPagingAsync(ISixnetQueryable queryable, int page, int pageSize, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract Task<PagingInfo<TModel>> GetPagingAsync(Expression<Func<TModel, bool>> conditionExpression, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract Task<PagingInfo<TModel>> GetPagingAsync(Expression<Func<TModel, bool>> conditionExpression, int page, int pageSize, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract Task<PagingInfo<TModel>> GetPagingAsync(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract Task<PagingInfo<TModel>> GetPagingAsync(int page, int pageSize, Action<DataOperationOptions> configure = null);

        #endregion

        #region Exists

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public abstract Task<bool> ExistsAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public abstract Task<bool> ExistsAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public abstract Task<bool> ExistsAsync(Action<DataOperationOptions> configure = null);

        #endregion

        #region Count

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public abstract Task<int> CountAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public abstract Task<int> CountAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public abstract Task<int> CountAsync(Action<DataOperationOptions> configure = null);

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public abstract Task<TValue> MaxAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public abstract Task<TValue> MaxAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public abstract Task<TValue> MaxAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public abstract Task<TValue> MinAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public abstract Task<TValue> MinAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public abstract Task<TValue> MinAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public abstract Task<TValue> SumAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public abstract Task<TValue> SumAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public abstract Task<TValue> SumAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression = null, Action<DataOperationOptions> configure = null);

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public abstract Task<TValue> AvgAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public abstract Task<TValue> AvgAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public abstract Task<TValue> AvgAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract Task<TValue> ScalarAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract Task<TValue> ScalarAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract Task<TValue> ScalarAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract Task<TValue> ScalarAsync<TValue>(PropertyField field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract Task<TValue> ScalarAsync<TValue>(PropertyField field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion
    }
}
