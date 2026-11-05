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
    public interface ISixnetQueryService<T>
    {
        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        SixnetPagingInfo<T> GetPaging(SixnetPagingFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get paging
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<SixnetPagingInfo<T>> GetPagingAsync(SixnetPagingFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        T Get(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<T> GetAsync(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        List<T> GetList(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Filter</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(SixnetQueryableFilter filter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="version">Version</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        T Get(dynamic id, long? version = null, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="version">Version</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<T> GetAsync(dynamic id, long? version = null, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list by ids
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        List<T> GetList(IEnumerable<dynamic> ids, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data list by ids
        /// </summary>
        /// <param name="ids">Ids</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(IEnumerable<dynamic> ids, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<T> GetAsync(Expression<Func<T, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        List<T> GetList(Expression<Func<T, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="includeArchived">Include archived</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition, bool includeArchived = false, Action<SixnetDataOperationOptions> configure = null);
    }
}
