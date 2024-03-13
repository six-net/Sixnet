using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    /// <typeparam name="TModel">Model type</typeparam>
    public abstract partial class DefaultRootRepository<TModel> : BaseRepository<TModel> where TModel : class, ISixnetEntity<TModel>
    {
        #region Impl

        #region Add

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Added data</returns>
        public sealed override TModel Add(TModel data, Action<DataOperationOptions> configure = null)
        {
            return (Add(new List<TModel>(1) { data }, configure))?.FirstOrDefault();
        }

        /// <summary>
        /// Add datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Added datas</returns>
        public sealed override List<TModel> Add(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            SixnetException.ThrowIf(datas.IsNullOrEmpty(), $"{nameof(datas)} is null or empty");
            foreach (var data in datas)
            {
                data.OnDataAdding();
                SixnetException.ThrowIf(!data.AllowToSave(), $"{typeof(TModel).Name}: {data.GetIdentityValue()} cann't to be add");
            }
            return AddData(datas, configure);
        }

        /// <summary>
        /// Add data and return identity
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identity</returns>
        public sealed override TIdentity AddReturnIdentity<TIdentity>(TModel data, Action<DataOperationOptions> configure = null)
        {
            var identities = AddReturnIdentities<TIdentity>(new List<TModel>(1) { data }, configure);
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
        public sealed override List<TIdentity> AddReturnIdentities<TIdentity>(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            return AddDataReturnIdentities<TIdentity>(datas, configure);
        }

        #endregion

        #region Update

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated data</returns>
        public sealed override TModel Update(TModel data, Action<DataOperationOptions> configure = null)
        {
            return (Update(new List<TModel>(1) { data }, configure))?.FirstOrDefault();
        }

        /// <summary>
        /// Update datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated datas</returns>
        public sealed override List<TModel> Update(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            SixnetException.ThrowIf(datas.IsNullOrEmpty(), $"{nameof(datas)} is null or empty");

            var currentDatas = GetDataListByCurrent(datas, configure);
            foreach (var data in datas)
            {
                data.OnDataUpdating(currentDatas?.FirstOrDefault());
                SixnetException.ThrowIf(!data.AllowToSave(), $"{typeof(TModel).Name}: {data.GetIdentityValue()} cann't to be update");
            }

            return UpdateData(datas, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Update(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return UpdateFields(fieldsAssignment, queryable, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Update(FieldsAssignment fieldsAssignment, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return Update(fieldsAssignment, conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Update(FieldsAssignment fieldsAssignment, Action<DataOperationOptions> configure = null)
        {
            return Update(fieldsAssignment, SixnetQuerier.Create<TModel>(), configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return Update(fieldsAssignmentExpression.GetFieldsAssignment(), queryable, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return Update(fieldsAssignmentExpression, conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<DataOperationOptions> configure = null)
        {
            return Update(fieldsAssignmentExpression, SixnetQuerier.Create<TModel>(), configure);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Delete(TModel data, Action<DataOperationOptions> configure = null)
        {
            return Delete(new TModel[1] { data }, configure);
        }

        /// <summary>
        /// Delete datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Delete(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null)
        {
            SixnetException.ThrowIf(datas.IsNullOrEmpty(), $"{nameof(datas)} is null or empty");

            foreach (var data in datas)
            {
                SixnetException.ThrowIf(!(data?.AllowToDelete() ?? false), $"{typeof(TModel)} data:{data?.GetIdentityValue()} cann't to be delete");
            }
            return DeleteData(datas, configure);
        }

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Delete(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return DeleteData(queryable, configure);
        }

        /// <summary>
        /// Delete object by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Delete(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return Delete(conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public sealed override int Delete(Action<DataOperationOptions> configure = null)
        {
            return Delete(SixnetQuerier.Create<TModel>(), configure);
        }

        #endregion

        #region Get

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public sealed override TModel Get(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return GetFirstData(queryable, configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public sealed override TModel Get(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return Get(conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public sealed override TModel Get(Action<DataOperationOptions> configure = null)
        {
            return Get(SixnetQuerier.Create<TModel>(), configure);
        }

        #endregion

        #region Get list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public sealed override List<TModel> GetList(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return GetDataList(queryable, configure) ?? new List<TModel>(0);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public sealed override List<TModel> GetList(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return GetList(conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public sealed override List<TModel> GetList(Action<DataOperationOptions> configure = null)
        {
            return GetList(SixnetQuerier.Create<TModel>(), configure);
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
        public sealed override List<TModel> GetList<TFirst, TSecond>(ISixnetQueryable queryable, Func<TFirst, TSecond, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataList(queryable, dataMappingFunc, configure) ?? new List<TModel>(0);
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
        public sealed override List<TModel> GetList<TFirst, TSecond, TThird>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataList(queryable, dataMappingFunc, configure) ?? new List<TModel>(0);
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
        public sealed override List<TModel> GetList<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataList(queryable, dataMappingFunc, configure) ?? new List<TModel>(0);
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
        public sealed override List<TModel> GetList<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataList(queryable, dataMappingFunc, configure) ?? new List<TModel>(0);
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
        public sealed override List<TModel> GetList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataList(queryable, dataMappingFunc, configure) ?? new List<TModel>(0);
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
        public sealed override List<TModel> GetList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null)
        {
            return GetDataList(queryable, dataMappingFunc, configure) ?? new List<TModel>(0);
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
        public sealed override PagingInfo<TModel> GetPaging(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            return GetDataPaging(queryable, pagingFilter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override PagingInfo<TModel> GetPaging(ISixnetQueryable queryable, int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            return GetPaging(queryable, PagingFilter.Create(page, pageSize), configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override PagingInfo<TModel> GetPaging(Expression<Func<TModel, bool>> conditionExpression, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            return GetPaging(conditionExpression.GetQueryable<TModel>(), pagingFilter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override PagingInfo<TModel> GetPaging(Expression<Func<TModel, bool>> conditionExpression, int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            return GetPaging(conditionExpression, PagingFilter.Create(page, pageSize), configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override PagingInfo<TModel> GetPaging(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            return GetPaging(SixnetQuerier.Create<TModel>(), pagingFilter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public sealed override PagingInfo<TModel> GetPaging(int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            return GetPaging(SixnetQuerier.Create<TModel>(), page, pageSize, configure);
        }

        #endregion

        #region Exists

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public sealed override bool Exists(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return ExistsData(queryable, configure);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public sealed override bool Exists(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return Exists(conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public sealed override bool Exists(Action<DataOperationOptions> configure = null)
        {
            return Exists(SixnetQuerier.Create<TModel>(), configure);
        }

        #endregion

        #region Count

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public sealed override int Count(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return CountValue(queryable, configure);
        }

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public sealed override int Count(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            return CountValue(conditionExpression.GetQueryable<TModel>(), configure);
        }

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public sealed override int Count(Action<DataOperationOptions> configure = null)
        {
            return Count(SixnetQuerier.Create<TModel>(), configure);
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
        public sealed override TValue Max<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return MaxValue<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public sealed override TValue Max<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return Max(field, null, configure);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public sealed override TValue Max<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var maxQueryable = conditionExpression.GetQueryable<TModel>()
                .Select(field.GetDataField(FieldFormatterNames.MAX));
            return Max<TValue>(maxQueryable, configure);
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
        public sealed override TValue Min<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return MinValue<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public sealed override TValue Min<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return Min(field, null, configure);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public sealed override TValue Min<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var minQueryable = conditionExpression.GetQueryable<TModel>()
                .Select(field.GetDataField(FieldFormatterNames.MIN));
            return Min<TValue>(minQueryable, configure);
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
        public sealed override TValue Sum<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return SumValue<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public sealed override TValue Sum<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return Sum(field, null, configure);
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public sealed override TValue Sum<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression = null, Action<DataOperationOptions> configure = null)
        {
            var sumQueryable = conditionExpression.GetQueryable<TModel>()
                .Select(field.GetDataField(FieldFormatterNames.SUM));
            return Sum<TValue>(sumQueryable, configure);
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
        public sealed override TValue Avg<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return AvgValue<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public sealed override TValue Avg<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return Avg(field, null, configure);
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public sealed override TValue Avg<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var avgQueryable = conditionExpression.GetQueryable<TModel>()
                .Select(field.GetDataField(FieldFormatterNames.AVG));
            return Avg<TValue>(avgQueryable, configure);
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
        public sealed override TValue Scalar<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null)
        {
            return ScalarValue<TValue>(queryable, configure);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public sealed override TValue Scalar<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null)
        {
            return Scalar(field, null, configure);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public sealed override TValue Scalar<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var query = conditionExpression.GetQueryable<TModel>()
                        .Select(field.GetDataField());
            return Scalar<TValue>(query, configure);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public sealed override TValue Scalar<TValue>(DataField field, Action<DataOperationOptions> configure = null)
        {
            return Scalar<TValue>(field, null, configure);
        }

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public sealed override TValue Scalar<TValue>(DataField field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null)
        {
            var query = conditionExpression.GetQueryable<TModel>()
                        .Select(field);
            return Scalar<TValue>(query, configure);
        }

        #endregion

        #region Queryable

        /// <summary>
        /// Get a queryable
        /// </summary>
        /// <returns></returns>
        public override ISixnetQueryable<TModel> AsQueryable()
        {
            return SixnetQuerier.Create<TModel>();
        }

        /// <summary>
        /// Get a queryable
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns></returns>
        public override ISixnetQueryable<TModel> AsQueryable(Expression<Func<TModel, bool>> conditionExpression)
        {
            return AsQueryable().Where(conditionExpression);
        }

        /// <summary>
        /// Get a queryable
        /// </summary>
        /// <param name="currentQueryable">Current queryable</param>
        /// <returns></returns>
        public override ISixnetQueryable<TModel> AsQueryable(ISixnetQueryable currentQueryable)
        {
            return SixnetQuerier.Create<TModel>(currentQueryable);
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
        protected abstract List<TModel> AddData(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Add datas and return identiies
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        protected abstract List<TIdentity> AddDataReturnIdentities<TIdentity>(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="newDatas">New datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated datas</returns>
        protected abstract List<TModel> UpdateData(IEnumerable<TModel> newDatas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update columns
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected abstract int UpdateFields(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected abstract int DeleteData(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        protected abstract int DeleteData(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get first data
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected abstract TModel GetFirstData(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        protected abstract List<TModel> GetDataList(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        protected abstract List<TModel> GetDataList<TFirst, TSecond>(ISixnetQueryable queryable, Func<TFirst, TSecond, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract List<TModel> GetDataList<TFirst, TSecond, TThird>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract List<TModel> GetDataList<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract List<TModel> GetDataList<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract List<TModel> GetDataList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        protected abstract List<TModel> GetDataList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data paging</returns>
        protected abstract PagingInfo<TModel> GetDataPaging(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list by current datas
        /// </summary>
        /// <param name="currentDatas">Current datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns></returns>
        protected abstract List<TModel> GetDataListByCurrent(IEnumerable<TModel> currentDatas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Whether has data</returns>
        protected abstract bool ExistsData(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get count value
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns>Count value</returns>
        protected abstract int CountValue(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Max value</returns>
        protected abstract TValue MaxValue<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Min value</returns>
        protected abstract TValue MinValue<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Sum value</returns>
        protected abstract TValue SumValue<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Average value</returns>
        protected abstract TValue AvgValue<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <returns>Value</returns>
        protected abstract TValue ScalarValue<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion
    }
}
