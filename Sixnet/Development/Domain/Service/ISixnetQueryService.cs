// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Data;
using Sixnet.Development.Queryable;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Domain.Service
{
    public interface ISixnetQueryService<TEntity, TId>
    {
        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        SixnetPagingInfo<TEntity> GetPaging(SixnetPagingFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Filter</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        SixnetPagingInfo<TEntity> GetPaging(ISixnetQueryable queryable, int page, int pageSize, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="condition">Filter</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        SixnetPagingInfo<TEntity> GetPaging(Expression<Func<TEntity, bool>> condition, int page, int pageSize, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<SixnetPagingInfo<TEntity>> GetPagingAsync(SixnetPagingFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Filter</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<SixnetPagingInfo<TEntity>> GetPagingAsync(ISixnetQueryable queryable, int page, int pageSize, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="condition">Filter</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<SixnetPagingInfo<TEntity>> GetPagingAsync(Expression<Func<TEntity, bool>> condition, int page, int pageSize, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        TEntity Get(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        TEntity Get(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        List<TEntity> GetList(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        List<TEntity> GetList(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<List<TEntity>> GetListAsync(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<List<TEntity>> GetListAsync(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="version">Version</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        TEntity Get(TId id, long? version = null, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="version">Version</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(TId id, long? version = null, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list by ids
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        List<TEntity> GetList(IEnumerable<TId> ids, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list by ids
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<List<TEntity>> GetListAsync(IEnumerable<TId> ids, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        TEntity Get(Expression<Func<TEntity, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        List<TEntity> GetList(Expression<Func<TEntity, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);
    }
}
