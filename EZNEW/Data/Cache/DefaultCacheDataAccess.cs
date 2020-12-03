using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Entity;
using EZNEW.Paging;
using EZNEW.Data.Cache.Command;
using EZNEW.DependencyInjection;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Default cache data access
    /// </summary>
    /// <typeparam name="TDatabaseAccess"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class DefaultCacheDataAccess<TDatabaseAccess, TEntity> where TDatabaseAccess : IDataAccess<TEntity> where TEntity : BaseEntity<TEntity>, new()
    {
        readonly TDatabaseAccess databaseAccess = ContainerManager.Resolve<TDatabaseAccess>();
        readonly IDataCacheProvider dataCacheProvider = ContainerManager.Resolve<IDataCacheProvider>();

        #region Add

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return a ICommand object</returns>
        public virtual ICommand Add(TEntity data)
        {
            return dataCacheProvider.Add(new AddDataCacheCommand<TEntity>()
            {
                Data = data,
                AddDataToDatabaseProxy = databaseAccess.Add
            });
        }

        /// <summary>
        /// Add datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return a ICommand object collection</returns>
        public virtual List<ICommand> Add(IEnumerable<TEntity> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<ICommand>(0);
            }
            List<ICommand> commands = new List<ICommand>();
            foreach (var data in datas)
            {
                commands.Add(Add(data));
            }
            return commands;
        }

        #endregion

        #region Modify

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="oldData">Old data</param>
        /// <returns>Return a ICommand object</returns>
        public virtual ICommand Modify(TEntity newData, TEntity oldData)
        {
            return dataCacheProvider.Modify(new ModifyDataCacheCommand<TEntity>()
            {
                NewData = newData,
                OldData = oldData,
                ModifyDatabaseDataProxy = databaseAccess.Modify
            });
        }

        /// <summary>
        /// Modify data by condition
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="oldData">Old data</param>
        /// <param name="query">Query Condition</param>
        /// <returns>Return a ICommand object</returns>
        public virtual ICommand Modify(TEntity newData, TEntity oldData, IQuery query)
        {
            return dataCacheProvider.Modify(new ModifyDataByConditionCacheCommand<TEntity>()
            {
                NewData = newData,
                OldData = oldData,
                Query = query,
                ModifyDatabaseDataProxy = databaseAccess.Modify,
                GetDataListProxy = databaseAccess.GetList
            });
        }

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="modifyExpression">Modify expression</param>
        /// <param name="query">Query condition</param>
        /// <returns>Return a ICommand object</returns>
        public virtual ICommand Modify(IModify modifyExpression, IQuery query)
        {
            return dataCacheProvider.Modify<TEntity>(new ModifyByConditionCacheCommand<TEntity>()
            {
                Query = query,
                Modify = modifyExpression,
                ModifyDatabaseDataProxy = databaseAccess.Modify,
                GetDataListProxy = databaseAccess.GetList
            });
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return a ICommand object</returns>
        public virtual ICommand Delete(TEntity data)
        {
            return dataCacheProvider.Remove(new RemoveDataCacheCommand<TEntity>()
            {
                Data = data,
                RemoveDatabaseDataProxy = databaseAccess.Delete
            });
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns></returns>
        public virtual ICommand Delete(IQuery query)
        {
            return dataCacheProvider.Remove(new RemoveByConditionCacheCommand<TEntity>()
            {
                Query = query,
                RemoveDatabaseDataProxy = databaseAccess.Delete,
                GetDataListProxy = databaseAccess.GetList
            });
        }

        #endregion

        #region Check data

        /// <summary>
        /// Check data
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns></returns>
        public bool Exist(IQuery query)
        {
            return ExistAsync(query).Result;
        }

        /// <summary>
        /// Check data
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistAsync(IQuery query)
        {
            return await dataCacheProvider.ExistAsync<TEntity>(new ExistDataCacheCommand<TEntity>()
            {
                Query = query,
                CheckDatabaseDataProxyAsync = databaseAccess.ExistAsync,
                GetDatabaseDataProxyAsync = databaseAccess.GetAsync
            }).ConfigureAwait(false);
        }

        #endregion

        #region Get

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns>Return data object</returns>
        public virtual TEntity Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns>Return data object</returns>
        public virtual async Task<TEntity> GetAsync(IQuery query)
        {
            return await dataCacheProvider.GetAsync(new GetDataCacheCommand<TEntity>()
            {
                Query = query,
                GetDatabaseDataProxyAsync = databaseAccess.GetAsync
            }).ConfigureAwait(false);
        }

        #endregion

        #region Get data list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns>Return data list</returns>
        public virtual List<TEntity> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns>Return data list</returns>
        public virtual async Task<List<TEntity>> GetListAsync(IQuery query)
        {
            return await dataCacheProvider.GetListAsync(new GetDataListCacheCommand<TEntity>()
            {
                Query = query,
                GetDatabaseDataListProxyAsync = databaseAccess.GetListAsync
            }).ConfigureAwait(false);
        }

        #endregion

        #region Get data paging

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns>Return data paging</returns>
        public virtual IPaging<TEntity> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns>Return data paging</returns>
        public virtual async Task<IPaging<TEntity>> GetPagingAsync(IQuery query)
        {
            return await dataCacheProvider.GetPagingAsync(new GetDataPagingCacheCommand<TEntity>()
            {
                Query = query,
                GetDatabaseDataPagingProxyAsync = databaseAccess.GetPagingAsync,
                GetDatabaseDataListProxyAsync = databaseAccess.GetListAsync
            }).ConfigureAwait(false);
        }

        #endregion

        #region Max

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query condition</param>
        /// <returns>Return the max value</returns>
        public virtual TValue Max<TValue>(IQuery query)
        {
            return MaxAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query condition</param>
        /// <returns>Return the max value</returns>
        public virtual async Task<TValue> MaxAsync<TValue>(IQuery query)
        {
            return await databaseAccess.MaxAsync<TValue>(query).ConfigureAwait(false);
        }

        #endregion

        #region Min

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query condition</param>
        /// <returns>Return min value</returns>
        public virtual TValue Min<TValue>(IQuery query)
        {
            return MinAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query condition</param>
        /// <returns>Return the min value</returns>
        public virtual async Task<TValue> MinAsync<TValue>(IQuery query)
        {
            return await databaseAccess.MinAsync<TValue>(query).ConfigureAwait(false);
        }

        #endregion

        #region Sum

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query condition</param>
        /// <returns>Return the sum value</returns>
        public virtual TValue Sum<TValue>(IQuery query)
        {
            return SumAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query condition</param>
        /// <returns>Return the sum value</returns>
        public virtual async Task<TValue> SumAsync<TValue>(IQuery query)
        {
            return await databaseAccess.SumAsync<TValue>(query).ConfigureAwait(false);
        }

        #endregion

        #region Avg

        /// <summary>
        /// Get avg value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query condition</param>
        /// <returns>Return the avg value</returns>
        public virtual TValue Avg<TValue>(IQuery query)
        {
            return AvgAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get avg value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query condition</param>
        /// <returns>Return the avg value</returns>
        public virtual async Task<TValue> AvgAsync<TValue>(IQuery query)
        {
            return await databaseAccess.AvgAsync<TValue>(query).ConfigureAwait(false);
        }

        #endregion

        #region Count

        /// <summary>
        /// Get count value
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns>Return the count value</returns>
        public virtual long Count(IQuery query)
        {
            return CountAsync(query).Result;
        }

        /// <summary>
        /// Get count value
        /// </summary>
        /// <param name="query">Query condition</param>
        /// <returns>Return the count value</returns>
        public virtual async Task<long> CountAsync(IQuery query)
        {
            return await databaseAccess.CountAsync(query).ConfigureAwait(false);
        }

        #endregion
    }
}
