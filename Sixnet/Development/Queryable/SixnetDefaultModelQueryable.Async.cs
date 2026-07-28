// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

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
    internal abstract partial class SixnetDefaultModelQueryable<TModel>
    {
        #region From

        /// <summary>
        /// As a temp table
        /// </summary>
        /// <returns></returns>
        public new async Task<ISixnetQueryable<TModel>> AsTempTableAsync()
        {
            return await AsTempTableAsync<TModel>();
        }

        #endregion

        #region Data access

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public async Task<TModel> FirstAsync(Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<TModel> firstRepository)
            {
                return await firstRepository.GetAsync(this, configure).ConfigureAwait(false);
            }
            return await FirstAsync<TModel>(configure).ConfigureAwait(false);
        }

        #endregion

        #region List

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data list</returns>
        public async Task<List<TModel>> ToListAsync(Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<TModel> firstRepository)
            {
                return await firstRepository.GetListAsync(this, configure).ConfigureAwait(false);
            }
            return await ToListAsync<TModel>(configure).ConfigureAwait(false);
        }

        #endregion

        #region Paging

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="pagingFilter">Paging filter</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public async Task<SixnetPagingInfo<TModel>> ToPagingAsync(SixnetPagingFilter pagingFilter, Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<TModel> firstRepository)
            {
                return await firstRepository.GetPagingAsync(this, pagingFilter, configure).ConfigureAwait(false);
            }
            return await ToPagingAsync<TModel>(pagingFilter, configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Paging data</returns>
        public async Task<SixnetPagingInfo<TModel>> ToPagingAsync(int page, int pageSize, Action<SixnetDataOperationOptions> configure = null)
        {
            if (queryableInfo.Repository is ISixnetRepository<TModel> firstRepository)
            {
                return await firstRepository.GetPagingAsync(this, page, pageSize, configure).ConfigureAwait(false);
            }
            return await ToPagingAsync<TModel>(page, pageSize, configure).ConfigureAwait(false);
        }

        #endregion 

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public Task<int> UpdateAsync(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateAsync(fieldsAssignmentExpression.GetFieldsAssignment(), configure);
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
        public Task<TValue> MaxAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null)
        {
            SelectFields(SixnetExpressionHelper.GetDataField(field, SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MAX)));
            return MaxAsync<TValue>(configure);
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
        public Task<TValue> MinAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null)
        {
            SelectFields(SixnetExpressionHelper.GetDataField(field, SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.MIN)));
            return MinAsync<TValue>(configure);
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
        public Task<TValue> SumAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null)
        {
            SelectFields(SixnetExpressionHelper.GetDataField(field, SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.SUM)));
            return SumAsync<TValue>(configure);
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
        public Task<TValue> AvgAsync<TValue>(Expression<Func<TModel, TValue>> field, Action<SixnetDataOperationOptions> configure = null)
        {
            SelectFields(SixnetExpressionHelper.GetDataField(field, SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.AVG)));
            return AvgAsync<TValue>(configure);
        }

        #endregion

        #endregion
    }
}
