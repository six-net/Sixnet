// "Company © 2025. All rights reserved."

using Sixnet.Development.Data;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Repository;
using Sixnet.Expressions.Linq;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default model queryable
    /// </summary>
    internal abstract partial class SixnetDefaultModelQueryable<TModel> : SixnetDefaultQueryable, ISixnetModelQueryable<TModel>
    {
        #region Constructor

        public SixnetDefaultModelQueryable(ISixnetQueryable sourceQueryable = null) : base(sourceQueryable) { }

        public SixnetDefaultModelQueryable(SixnetQueryableInfo sourceQueryableContext = null) : base(sourceQueryableContext) { }

        #endregion

        #region From

        /// <summary>
        /// As a data source
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TModel> AsSource()
        {
            return AsSource<TModel>();
        }

        /// <summary>
        /// As a temp table
        /// </summary>
        /// <returns></returns>
        public new ISixnetQueryable<TModel> AsTempTable()
        {
            return AsTempTable<TModel>();
        }

        #endregion

        #region Data access

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public TModel First(Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<TModel> firstRepository)
            {
                return firstRepository.Get(this, configure);
            }
            return First<TModel>(configure);
        }

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public List<TModel> ToList(Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<TModel> firstRepository)
            {
                return firstRepository.GetList(this, configure);
            }
            return ToList<TModel>(configure);
        }

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public SixnetPagingInfo<TModel> ToPaging(SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<TModel> firstRepository)
            {
                return firstRepository.GetPaging(this, pagingFilter, configure);
            }
            return ToPaging<TModel>(pagingFilter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public SixnetPagingInfo<TModel> ToPaging(int page, int pageSize, Action<SixnetDataOperationOptions> configure = null)
        {
            return ToPaging(SixnetPagingFilter.Create(page, pageSize), configure);
        }

        #endregion 

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<SixnetDataOperationOptions> configure = null)
        {
            return Update(fieldsAssignmentExpression.GetFieldsAssignment(), configure);
        }

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field </param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Max value</returns>
        public TValue Max<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null)
        {
            Select(SixnetExpressionHelper.GetDataField(field, SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MAX)));
            return Max<TValue>(configure);
        }

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field </param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Min value</returns>
        public TValue Min<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null)
        {
            Select(SixnetExpressionHelper.GetDataField(field, SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MIN)));
            return Min<TValue>(configure);
        }

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field </param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Sum value</returns>
        public TValue Sum<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null)
        {
            Select(SixnetExpressionHelper.GetDataField(field, SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.SUM)));
            return Sum<TValue>(configure);
        }

        #endregion

        #region Avg

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="field">Field </param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Average value</returns>
        public TValue Avg<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null)
        {
            Select(SixnetExpressionHelper.GetDataField(field, SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.AVG)));
            return Avg<TValue>(configure);
        }

        #endregion

        #endregion
    }
}
