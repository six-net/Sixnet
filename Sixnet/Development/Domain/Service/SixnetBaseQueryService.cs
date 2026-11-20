// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Sixnet.DependencyInjection;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Development.Repository;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Domain.Service
{
    public class SixnetBaseQueryService<TEntity, TId> : SixnetBaseDomainService<TEntity, TId>, ISixnetQueryService<TEntity, TId> where TEntity : SixnetBaseEntity<TEntity>, new()
    {
        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public SixnetPagingInfo<TEntity> GetPaging(SixnetPagingFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            filter ??= new SixnetPagingFilter();
            return repository.GetPaging(HandleQueryable(filter.CreateQueryable(true)), filter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Filter</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public SixnetPagingInfo<TEntity> GetPaging(ISixnetQueryable queryable, int page, int pageSize, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetPaging(HandleQueryable(queryable), page, pageSize, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="condition">Filter</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public SixnetPagingInfo<TEntity> GetPaging(Expression<Func<TEntity, bool>> condition, int page, int pageSize, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            var queryable = SixnetExpressionHelper.GetQueryable(condition) ?? SixnetQuerier.Create();
            queryable.IncludeArchivedIf(includeArchived);
            return GetPaging(queryable, page, pageSize, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<SixnetPagingInfo<TEntity>> GetPagingAsync(SixnetPagingFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            filter ??= new SixnetPagingFilter();
            return repository.GetPagingAsync(HandleQueryable(filter.CreateQueryable(true)), filter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="queryable">Filter</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<SixnetPagingInfo<TEntity>> GetPagingAsync(ISixnetQueryable queryable, int page, int pageSize, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetPagingAsync(HandleQueryable(queryable), page, pageSize, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="condition">Filter</param>
        /// <param name="page">Page</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<SixnetPagingInfo<TEntity>> GetPagingAsync(Expression<Func<TEntity, bool>> condition, int page, int pageSize, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            var queryable = SixnetExpressionHelper.GetQueryable(condition) ?? SixnetQuerier.Create();
            queryable.IncludeArchivedIf(includeArchived);
            return GetPagingAsync(queryable, page, pageSize, configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public TEntity Get(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.Get(HandleQueryable(queryable), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public TEntity Get(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.Get(HandleQueryable(filter?.CreateQueryable()), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<TEntity> GetAsync(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetAsync(HandleQueryable(queryable), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<TEntity> GetAsync(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetAsync(HandleQueryable(filter?.CreateQueryable()), configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public List<TEntity> GetList(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetList(HandleQueryable(queryable), configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public List<TEntity> GetList(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetList(HandleQueryable(filter?.CreateQueryable()), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<List<TEntity>> GetListAsync(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetListAsync(HandleQueryable(queryable), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<List<TEntity>> GetListAsync(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetListAsync(HandleQueryable(filter?.CreateQueryable()), configure);
        }

        /// <summary>
        /// Get data by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="version">Version</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public TEntity Get(TId id, long? version = null, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return HandleQueryable(GetIdQueryable(id), version > 0, version, includeArchived).First<TEntity>(configure);
        }

        /// <summary>
        /// Get data by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="version">Version</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<TEntity> GetAsync(TId id, long? version = null, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return HandleQueryable(GetIdQueryable(id), version > 0, version, includeArchived).FirstAsync<TEntity>(configure);
        }

        /// <summary>
        /// Get data list by ids
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public List<TEntity> GetList(IEnumerable<TId> ids, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return HandleQueryable(GetIdsQueryable(ids), false, null, includeArchived).ToList<TEntity>(configure);
        }

        /// <summary>
        /// Get data list by ids
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<List<TEntity>> GetListAsync(IEnumerable<TId> ids, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return HandleQueryable(GetIdsQueryable(ids), false, null, includeArchived).ToListAsync<TEntity>(configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public TEntity Get(Expression<Func<TEntity, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.Get(HandleQueryable(SixnetQuerier.Create(condition), false, null, includeArchived), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetAsync(HandleQueryable(SixnetQuerier.Create(condition), false, null, includeArchived), configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public List<TEntity> GetList(Expression<Func<TEntity, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetList(HandleQueryable(SixnetQuerier.Create(condition), false, null, includeArchived), configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetListAsync(HandleQueryable(SixnetQuerier.Create(condition), false, null, includeArchived), configure);
        }
    }
}
