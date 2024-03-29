﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Repository
{
    public partial interface IRepository
    {
        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(FieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(FieldsAssignment fieldsAssignment, Action<DataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Delete(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Delete(Action<DataOperationOptions> configure = null);

        #endregion

        #region Exists

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        bool Exists(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        bool Exists(Action<DataOperationOptions> configure = null);

        #endregion

        #region Count

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        int Count(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        int Count(Action<DataOperationOptions> configure = null);

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        TValue Max<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        TValue Min<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        TValue Sum<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        TValue Avg<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        TValue Scalar<TValue>(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        #endregion
    }

    /// <summary>
    /// Defines repository contract
    /// </summary>
    /// <typeparam name="TModel">Model type</typeparam>
    public partial interface IRepository<TModel> : IRepository
    {
        #region Add

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Added data</returns>
        TModel Add(TModel data, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Add data and return identity
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identity</returns>
        TIdentity AddReturnIdentity<TIdentity>(TModel data, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Add datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Added datas</returns>
        List<TModel> Add(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Add datas and return identiies
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        List<TIdentity> AddReturnIdentities<TIdentity>(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        #endregion

        #region Update

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated data</returns>
        TModel Update(TModel data, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Updated datas</returns>
        List<TModel> Update(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(FieldsAssignment fieldsAssignment, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Delete(TModel data, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Delete(IEnumerable<TModel> datas, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        int Delete(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Get

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        TModel Get(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        TModel Get(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        TModel Get(Action<DataOperationOptions> configure = null);

        #endregion

        #region Get list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        List<TModel> GetList(ISixnetQueryable queryable, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        List<TModel> GetList(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        List<TModel> GetList(Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        List<TModel> GetList<TFirst, TSecond>(ISixnetQueryable queryable, Func<TFirst, TSecond, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TModel> GetList<TFirst, TSecond, TThird>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TModel> GetList<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TModel> GetList<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TModel> GetList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

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
        List<TModel> GetList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TModel> dataMappingFunc, Action<DataOperationOptions> configure = null);

        #endregion

        #region Get paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<TModel> GetPaging(ISixnetQueryable queryable, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<TModel> GetPaging(ISixnetQueryable queryable, int page, int pageSize, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<TModel> GetPaging(Expression<Func<TModel, bool>> conditionExpression, PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<TModel> GetPaging(Expression<Func<TModel, bool>> conditionExpression, int page, int pageSize, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<TModel> GetPaging(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        PagingInfo<TModel> GetPaging(int page, int pageSize, Action<DataOperationOptions> configure = null);

        #endregion

        #region Exists

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        bool Exists(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Count

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        int Count(Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        TValue Max<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        TValue Max<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        TValue Min<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        TValue Min<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        TValue Sum<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        TValue Sum<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        TValue Avg<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        TValue Avg<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        TValue Scalar<TValue>(Expression<Func<TModel, TValue>> field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        TValue Scalar<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        TValue Scalar<TValue>(PropertyField field, Action<DataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Conditionv expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        TValue Scalar<TValue>(PropertyField field, Expression<Func<TModel, bool>> conditionExpression, Action<DataOperationOptions> configure = null);

        #endregion

        #region Queryable

        /// <summary>
        /// Get a queryable
        /// </summary>
        /// <returns></returns>
        ISixnetQueryable<TModel> AsQueryable();

        /// <summary>
        /// Get a queryable
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns></returns>
        ISixnetQueryable<TModel> AsQueryable(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get a queryable
        /// </summary>
        /// <param name="currentQueryable">Current queryable</param>
        /// <returns></returns>
        ISixnetQueryable<TModel> AsQueryable(ISixnetQueryable currentQueryable);

        #endregion
    }
}
