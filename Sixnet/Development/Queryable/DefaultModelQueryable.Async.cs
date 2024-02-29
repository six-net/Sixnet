using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data;
using Sixnet.Development.Repository;
using Sixnet.Model.Paging;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default model queryable
    /// </summary>
    internal abstract partial class DefaultModelQueryable<TModel>
    {
        #region Data access

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public async Task<TModel> FirstAsync(Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is ISixnetRepository<TModel> firstRepository)
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
        public async Task<List<TModel>> ToListAsync(Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is ISixnetRepository<TModel> firstRepository)
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
        public async Task<PagingInfo<TModel>> ToPagingAsync(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is ISixnetRepository<TModel> firstRepository)
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
        public async Task<PagingInfo<TModel>> ToPagingAsync(int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is ISixnetRepository<TModel> firstRepository)
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
        public Task<int> UpdateAsync(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<DataOperationOptions> configure = null)
        {
            return UpdateAsync(fieldsAssignmentExpression.GetFieldsAssignment(), configure);
        }

        #endregion

        #endregion
    }
}
