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
using Sixnet.Model.Paging;

namespace Sixnet.Development.Domain.Service
{
    public class SixnetQueryService<T> : SixnetBaseDomainService<T>, ISixnetQueryService<T> where T : SixnetBaseEntity<T>, new()
    {
        /// <summary>
        /// Handle queryable
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="version"></param>
        /// <param name="includeArchived"></param>
        /// <returns></returns>
        ISixnetQueryable HandleQueryable(ISixnetQueryable queryable, long? version = null, bool includeArchived = false)
        {
            queryable ??= SixnetQuerier.Create<T>();

            if (isVersioning)
            {
                if (version.HasValue && version.Value > 0)
                {
                    queryable.FromAllVersions();
                    var versionCriterion = SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(versionFieldName, typeof(T)), SixnetConstantField.Create(version.Value));
                    queryable.Where(versionCriterion);
                }
                else
                {
                    var historicalCriterion = SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(historyTagFieldName, typeof(T)), SixnetConstantField.Create(false));
                    queryable.Where(historicalCriterion);
                }
            }

            queryable.IncludeArchivedIf(includeArchived);

            return queryable;
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public SixnetPagingInfo<T> GetPaging(SixnetPagingFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetPaging(HandleQueryable(filter.CreateQueryable(true)), filter, configure);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<SixnetPagingInfo<T>> GetPagingAsync(SixnetPagingFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetPagingAsync(HandleQueryable(filter.CreateQueryable(true)), filter, configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public T Get(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.Get(HandleQueryable(filter.CreateQueryable()), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<T> GetAsync(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetAsync(HandleQueryable(filter.CreateQueryable()), configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public List<T> GetList(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetList(HandleQueryable(filter.CreateQueryable()), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<List<T>> GetListAsync(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetListAsync(HandleQueryable(filter.CreateQueryable()), configure);
        }

        /// <summary>
        /// Get data by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="version">Version</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public T Get(dynamic id, long? version = null, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return HandleQueryable(GetIdQueryable(id), version, includeArchived).First<T>(configure);
        }

        /// <summary>
        /// Get data by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="version">Version</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<T> GetAsync(dynamic id, long? version = null, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return HandleQueryable(GetIdQueryable(id), version, includeArchived).FirstAsync<T>(configure);
        }

        /// <summary>
        /// Get data list by ids
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public List<T> GetList(IEnumerable<dynamic> ids, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return HandleQueryable(GetIdsQueryable(ids), null, includeArchived).ToList<T>(configure);
        }

        /// <summary>
        /// Get data list by ids
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<List<T>> GetListAsync(IEnumerable<dynamic> ids, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return HandleQueryable(GetIdsQueryable(ids), null, includeArchived).ToListAsync<T>(configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.Get(HandleQueryable(SixnetQuerier.Create(condition), null, includeArchived), configure);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<T> GetAsync(Expression<Func<T, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetAsync(HandleQueryable(SixnetQuerier.Create(condition), null, includeArchived), configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public List<T> GetList(Expression<Func<T, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetList(HandleQueryable(SixnetQuerier.Create(condition), null, includeArchived), configure);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        public Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null)
        {
            return repository.GetListAsync(HandleQueryable(SixnetQuerier.Create(condition), null, includeArchived), configure);
        }
    }
}
