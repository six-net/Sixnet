using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Repository
{
    /// <summary>
    /// Defines default root repository
    /// </summary>
    public abstract partial class DefaultRootRepository<TModel> : BaseRepository<TModel> where TModel : class, IEntity<TModel>
    {
        #region Impl

        #region Add

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Added data</returns>
        public sealed override async Task<TModel> AddAsync(TModel data, Action<DataOperationOptions> configure = null)
        {
            return (await AddAsync(new List<TModel>(1) { data }, configure).ConfigureAwait(false))?.FirstOrDefault();
        }

        /// <summary>
        /// Add datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Added datas</returns>
        public sealed override Task<List<TModel>> AddAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            SixnetException.ThrowIf(datas.IsNullOrEmpty(), $"{nameof(datas)} is null or empty");
            foreach (var data in datas)
            {
                data.OnDataAdding();
                SixnetException.ThrowIf(!data.AllowToSave(), $"{typeof(TModel).Name}: {data.GetIdentityValue()} cann't to be add");
            }
            return AddDataAsync(datas, configure);
        }

        /// <summary>
        /// Add data and return identity
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identity</returns>
        public sealed override async Task<TIdentity> AddReturnIdentityAsync<TIdentity>(TModel data, Action<DataOperationOptions> configure = null)
        {
            var identities = await AddReturnIdentitiesAsync<TIdentity>(new List<TModel>(1) { data }, configure).ConfigureAwait(false);
            if (!identities.IsNullOrEmpty())
            {
                return identities.FirstOrDefault();
            }
            return default;
        }

        /// <summary>
        /// Add datas and return identities
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        public sealed override Task<List<TIdentity>> AddReturnIdentitiesAsync<TIdentity>(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            return AddDataReturnIdentitiesAsync<TIdentity>(datas, configure);
        }

        #endregion

        #region Update

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated data</returns>
        public sealed override async Task<TModel> UpdateAsync(TModel data, Action<DataOperationOptions> configure = null)
        {
            return (await UpdateAsync(new List<TModel>(1) { data }, configure).ConfigureAwait(false))?.FirstOrDefault();
        }

        /// <summary>
        /// Update datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated datas</returns>
        public sealed override async Task<List<TModel>> UpdateAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            SixnetException.ThrowIf(datas.IsNullOrEmpty(), $"{nameof(datas)} is null or empty");

            var currentDatas = await GetDataListByCurrentAsync(datas, configure).ConfigureAwait(false);
            foreach (var data in datas)
            {
                data.OnDataUpdating(currentDatas?.FirstOrDefault());
                SixnetException.ThrowIf(!data.AllowToSave(), $"{typeof(TModel).Name}: {data.GetIdentityValue()} cann't to be update");
            }

            return await UpdateDataAsync(datas, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return UpdateFieldsAsync(fieldsAssignment, queryable, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return UpdateAsync(fieldsAssignment, conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> UpdateAsync(FieldsAssignment fieldsAssignment, Action<DataOperationOptions> configure = null)
        {
            return UpdateAsync(fieldsAssignment, SixnetQueryable.Create<TModel>(), configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> UpdateAsync(Expression<Func<TModel, bool>> fieldsAssignmentExpression, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return UpdateAsync(fieldsAssignmentExpression.GetFieldsAssignment(), queryable, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> UpdateAsync(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return UpdateAsync(fieldsAssignmentExpression.GetFieldsAssignment(), conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> UpdateAsync(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<DataOperationOptions> configure = null)
        {
            return UpdateAsync(fieldsAssignmentExpression, SixnetQueryable.Create<TModel>(), configure);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> DeleteAsync(TModel data, Action<DataOperationOptions> configure = null)
        {
            return DeleteAsync(new TModel[1] { data }, configure);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="datas">Entity datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> DeleteAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            SixnetException.ThrowIf(datas.IsNullOrEmpty(), $"{nameof(datas)} is null or empty");

            foreach (var data in datas)
            {
                SixnetException.ThrowIf(!(data?.AllowToDelete() ?? false), $"{typeof(TModel)} data:{data?.GetIdentityValue()} cann't to be delete");
            }
            return DeleteDataAsync(datas, configure);
        }

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> DeleteAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DeleteDataAsync(queryable, configure);
        }

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> DeleteAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return DeleteAsync(SixnetQueryable.Create(conditionExpression), configure);
        }

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override Task<int> DeleteAsync(Action<DataOperationOptions> configure = null)
        {
            return DeleteAsync(SixnetQueryable.Create<TModel>(), configure);
        }

        #endregion

        #region Get

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public sealed override Task<TModel> GetAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return GetFirstDataAsync(queryable, configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public sealed override Task<TModel> GetAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return GetAsync(conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public sealed override Task<TModel> GetAsync(Action<DataOperationOptions> configure = null)
        {
            return GetAsync(SixnetQueryable.Create<TModel>(), configure);
        }

        #endregion

        #region Get list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public sealed override Task<List<TModel>> GetListAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return GetDataListAsync(queryable, configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public sealed override Task<List<TModel>> GetListAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return GetListAsync(conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public sealed override Task<List<TModel>> GetListAsync(Action<DataOperationOptions> configure = null)
        {
            return GetListAsync(SixnetQueryable.Create<TModel>(), configure);
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
        public sealed override Task<List<TModel>> GetListAsync<TFirst, TSecond>(ISixnetQueryable queryable, Func<TFirst, TSecond, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataListAsync(queryable, dataMappingFunc, configure);
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
        public sealed override Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataListAsync(queryable, dataMappingFunc, configure);
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
        public sealed override Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataListAsync(queryable, dataMappingFunc, configure);
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
        public sealed override Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataListAsync(queryable, dataMappingFunc, configure);
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
        public sealed override Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataListAsync(queryable, dataMappingFunc, configure);
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
        public sealed override Task<List<TModel>> GetListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataListAsync(queryable, dataMappingFunc, configure);
        }

        #endregion

        #region Get paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override Task<PagingInfo<TModel>> GetPagingAsync(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            return GetDataPagingAsync(queryable, pagingFilter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override Task<PagingInfo<TModel>> GetPagingAsync(ISixnetQueryable queryable, int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            return GetPagingAsync(queryable, PagingFilter.Create(page, pageSize), configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override Task<PagingInfo<TModel>> GetPagingAsync(Expression<Func<TModel, bool>> conditionExpression, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            return GetPagingAsync(SixnetQueryable.Create(conditionExpression), pagingFilter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override Task<PagingInfo<TModel>> GetPagingAsync(Expression<Func<TModel, bool>> conditionExpression, int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            return GetPagingAsync(SixnetQueryable.Create(conditionExpression), page, pageSize, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override Task<PagingInfo<TModel>> GetPagingAsync(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            return GetPagingAsync(SixnetQueryable.Create<TModel>(), pagingFilter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override Task<PagingInfo<TModel>> GetPagingAsync(int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            return GetPagingAsync(SixnetQueryable.Create<TModel>(), page, pageSize, configure);
        }

        #endregion

        #region Exists

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public sealed override Task<bool> ExistsAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return ExistsDataAsync(queryable, configure);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public sealed override Task<bool> ExistsAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return ExistsAsync(SixnetQueryable.Create(conditionExpression), configure);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public sealed override Task<bool> ExistsAsync(Action<DataOperationOptions> configure = null)
        {
            return ExistsAsync(SixnetQueryable.Create<TModel>(), configure);
        }

        #endregion

        #region Count

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public sealed override Task<int> CountAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return CountValueAsync(queryable, configure);
        }

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public sealed override Task<int> CountAsync(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return CountAsync(SixnetQueryable.Create(conditionExpression), configure);
        }

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public sealed override Task<int> CountAsync(Action<DataOperationOptions> configure = null)
        {
            return CountAsync(SixnetQueryable.Create<TModel>(), configure);
        }

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public sealed override Task<TValue> MaxAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return MaxValueAsync<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public sealed override Task<TValue> MaxAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return MaxAsync(field, null, configure);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public sealed override Task<TValue> MaxAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var maxQueryable = conditionExpression.GetQueryable<TModel>()
                .Select(field.GetDataField(FieldFormatterNames.MAX));
            return MaxAsync<TValue>(maxQueryable, configure);
        }

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public sealed override Task<TValue> MinAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return MinValueAsync<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public sealed override Task<TValue> MinAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return MinAsync(field, null, configure);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public sealed override Task<TValue> MinAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var minQueryable = conditionExpression.GetQueryable<TModel>()
                .Select(field.GetDataField(FieldFormatterNames.MIN));
            return MinAsync<TValue>(minQueryable, configure);
        }

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public sealed override Task<TValue> SumAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return SumValueAsync<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public sealed override Task<TValue> SumAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return SumAsync(field, null, configure);
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public sealed override Task<TValue> SumAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression = null, Action<DataOperationOptions> configure = null)
        {
            var sumQueryable = conditionExpression.GetQueryable<TModel>()
                .Select(field.GetDataField(FieldFormatterNames.SUM));
            return SumAsync<TValue>(sumQueryable, configure);
        }

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public sealed override Task<TValue> AvgAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return AvgValueAsync<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public sealed override Task<TValue> AvgAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return AvgAsync(field, null, configure);
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public sealed override Task<TValue> AvgAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var avgQueryable = conditionExpression.GetQueryable<TModel>()
                .Select(field.GetDataField(FieldFormatterNames.AVG));
            return AvgAsync<TValue>(avgQueryable, configure);
        }

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public sealed override Task<TValue> ScalarAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return ScalarValueAsync<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public sealed override Task<TValue> ScalarAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return ScalarAsync(field, null, configure);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public sealed override Task<TValue> ScalarAsync<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var query = conditionExpression.GetQueryable<TModel>()
                        .Select(field.GetDataField());
            return ScalarAsync<TValue>(query, configure);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public sealed override Task<TValue> ScalarAsync<TValue>(PropertyField field, Action<DataOperationOptions> configure = null)
        {
            return ScalarAsync<TValue>(field, null, configure);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public sealed override Task<TValue> ScalarAsync<TValue>(PropertyField field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var query = conditionExpression.GetQueryable<TModel>()
                        .Select(field);
            return ScalarAsync<TValue>(query, configure);
        }

        #endregion

        #endregion

        #region Functions

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Added datas</returns>
        protected abstract Task<List<TModel>> AddDataAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Add datas and return identiies
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        protected abstract Task<List<TIdentity>> AddDataReturnIdentitiesAsync<TIdentity>(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="newDatas">New datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated datas</returns>
        protected abstract Task<List<TModel>> UpdateDataAsync(IEnumerable<TModel> newDatas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update columns
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected abstract Task<int> UpdateFieldsAsync(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected abstract Task<int> DeleteDataAsync(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected abstract Task<int> DeleteDataAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get first data
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected abstract Task<TModel> GetFirstDataAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        protected abstract Task<List<TModel>> GetDataListAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        protected abstract Task<List<TModel>> GetDataListAsync<TFirst, TSecond>(ISixnetQueryable queryable, Func<TFirst, TSecond, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract Task<List<TModel>> GetDataListAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data paging</returns>
        protected abstract Task<PagingInfo<TModel>> GetDataPagingAsync(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list by current datas
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        protected abstract Task<List<TModel>> GetDataListByCurrentAsync(IEnumerable<TModel> currentDatas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Whether has data</returns>
        protected abstract Task<bool> ExistsDataAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get count value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Count value</returns>
        protected abstract Task<int> CountValueAsync(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Max value</returns>
        protected abstract Task<TValue> MaxValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Min value</returns>
        protected abstract Task<TValue> MinValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Sum value</returns>
        protected abstract Task<TValue> SumValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Average value</returns>
        protected abstract Task<TValue> AvgValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Value</returns>
        protected abstract Task<TValue> ScalarValueAsync<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion
    }
}
