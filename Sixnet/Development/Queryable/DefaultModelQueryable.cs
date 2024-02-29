using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sixnet.Development.Data;
using Sixnet.Development.Repository;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Default model queryable
    /// </summary>
    internal abstract partial class DefaultModelQueryable<TModel> : DefaultQueryable, ISixnetModelQueryable<TModel>
    {
        #region Constructor

        public DefaultModelQueryable(ISixnetQueryable sourceQueryable = null) : base(sourceQueryable) { }

        public DefaultModelQueryable(QueryableContext sourceQueryableContext = null) : base(sourceQueryableContext) { }

        #endregion

        #region Data access

        #region First

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="configure">Confirure options </param>
        /// <returns>Data</returns>
        public TModel First(Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is ISixnetRepository<TModel> firstRepository)
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
        public List<TModel> ToList(Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is ISixnetRepository<TModel> firstRepository)
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
        public PagingInfo<TModel> ToPaging(PagingFilter pagingFilter, Action<DataOperationOptions> configure = null)
        {
            if (queryableContext.Repository is ISixnetRepository<TModel> firstRepository)
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
        public PagingInfo<TModel> ToPaging(int page, int pageSize, Action<DataOperationOptions> configure = null)
        {
            return ToPaging(PagingFilter.Create(page, pageSize), configure);
        }

        #endregion 

        #region Update

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression">Fields assignment expression</param>
        /// <param name="configure">Confirure options </param>
        /// <returns>Affected data number</returns>
        public int Update(Expression<Func<TModel, bool>> fieldsAssignmentExpression, Action<DataOperationOptions> configure = null)
        {
            return Update(fieldsAssignmentExpression.GetFieldsAssignment(), configure);
        }

        #endregion

        #endregion
    }
}
