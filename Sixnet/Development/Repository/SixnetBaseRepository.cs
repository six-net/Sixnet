// "Company © 2025. All rights reserved."

using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Repository
{
    /// <summary>
    /// Defines base repository
    /// </summary>
    public abstract partial class SixnetBaseRepository<TModel> : ISixnetRepository<TModel>
    {
        #region Add

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Add(TModel data, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Add data and return identity
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identity</returns>
        public abstract TIdentity AddReturnIdentity<TIdentity>(TModel data, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Add datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Add(IEnumerable<TModel> datas, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Add datas and return identiies
        /// </summary>
        /// <typeparam name="TIdentity">Identity type</typeparam>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options</param>
        /// <returns>Identities</returns>
        public abstract List<TIdentity> AddReturnIdentities<TIdentity>(IEnumerable<TModel> datas, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Update

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Update(TModel data, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Update datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Update(IEnumerable<TModel> datas, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Update(SixnetFieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Update(SixnetFieldsAssignment fieldsAssignment, Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment">Fields assignment</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Update(SixnetFieldsAssignment fieldsAssignment, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Delete(TModel data, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Delete datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Delete(IEnumerable<TModel> datas, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Delete data by condition
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Delete(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Delete by condition
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Delete(Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public abstract int Delete(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Get

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public abstract TModel Get(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public abstract TModel Get(Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public abstract TModel Get(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Get list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public abstract List<TModel> GetList(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public abstract List<TModel> GetList(Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public abstract List<TModel> GetList(Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TFirst">First data type</typeparam>
        /// <typeparam name="TSecond">Second data type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="dataMappingFunc">Data mapping function</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Return the datas</returns>
        public abstract List<TModel> GetList<TFirst, TSecond>(ISixnetQueryable queryable, Func<TFirst, TSecond, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

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
        public abstract List<TModel> GetList<TFirst, TSecond, TThird>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

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
        public abstract List<TModel> GetList<TFirst, TSecond, TThird, TFourth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

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
        public abstract List<TModel> GetList<TFirst, TSecond, TThird, TFourth, TFifth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

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
        public abstract List<TModel> GetList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

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
        public abstract List<TModel> GetList<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(ISixnetQueryable queryable, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TModel> dataMappingFunc, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Get paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract SixnetPagingInfo<TModel> GetPaging(ISixnetQueryable queryable, SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract SixnetPagingInfo<TModel> GetPaging(ISixnetQueryable queryable, int page, int pageSize, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract SixnetPagingInfo<TModel> GetPaging(Expression<Func<TModel, bool>> conditionExpression, SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract SixnetPagingInfo<TModel> GetPaging(Expression<Func<TModel, bool>> conditionExpression, int page, int pageSize, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract SixnetPagingInfo<TModel> GetPaging(SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public abstract SixnetPagingInfo<TModel> GetPaging(int page, int pageSize, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Exists

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public abstract bool Exists(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public abstract bool Exists(Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Whether has data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Whether has data</returns>
        public abstract bool Exists(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Count

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public abstract int Count(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public abstract int Count(Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data num</returns>
        public abstract int Count(Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public abstract TValue Max<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public abstract TValue Max<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public abstract TValue Max<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public abstract TValue Min<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public abstract TValue Min<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public abstract TValue Min<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public abstract TValue Sum<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public abstract TValue Sum<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public abstract TValue Sum<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression = null, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public abstract TValue Avg<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public abstract TValue Avg<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public abstract TValue Avg<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Scalar

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract TValue Scalar<TValue>(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract TValue Scalar<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract TValue Scalar<TValue>(Expression<Func<TModel, TValue>> field, Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract TValue Scalar<TValue>(SixnetDataField field, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get scalar value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Value</returns>
        public abstract TValue Scalar<TValue>(SixnetDataField field, Expression<Func<TModel, bool>> conditionExpression, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Queryable

        /// <summary>
        /// Get a queryable
        /// </summary>
        /// <returns></returns>
        public abstract ISixnetQueryable<TModel> AsQueryable();

        /// <summary>
        /// Get a queryable
        /// </summary>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns></returns>
        public abstract ISixnetQueryable<TModel> AsQueryable(Expression<Func<TModel, bool>> conditionExpression);

        /// <summary>
        /// Get a queryable
        /// </summary>
        /// <param name="currentQueryable">Current queryable</param>
        /// <returns></returns>
        public abstract ISixnetQueryable<TModel> AsQueryable(ISixnetQueryable currentQueryable);

        #endregion
    }
}
