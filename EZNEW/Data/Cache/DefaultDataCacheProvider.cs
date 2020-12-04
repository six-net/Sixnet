using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Develop.Entity;
using EZNEW.Develop.CQuery;
using EZNEW.Paging;
using EZNEW.Develop.Command;
using EZNEW.Data.Cache.Command;
using EZNEW.Data.Cache.Policy;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Default data cache provider
    /// </summary>
    public class DefaultDataCacheProvider : IDataCacheProvider
    {
        private static readonly DataCachePolicyService dataCachePolicyService = new DataCachePolicyService();

        #region Add data

        /// <summary>
        /// Add data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="cacheCommand">Data cache command</param>
        /// <returns>Return a ICommand instance</returns>
        public ICommand Add<T>(AddDataCacheCommand<T> cacheCommand) where T : BaseEntity<T>, new()
        {
            //get database add command
            ICommand databaseCommand = null;
            if (cacheCommand?.AddDataToDatabaseProxy != null)
            {
                databaseCommand = cacheCommand.AddDataToDatabaseProxy(cacheCommand.Data);
            }
            if (databaseCommand != null)
            {
                //trigger cache policy
                dataCachePolicyService.AddData(cacheCommand, databaseCommand);
            }
            return databaseCommand;
        }

        #endregion

        #region Check data

        /// <summary>
        /// Check data
        /// </summary>
        /// <param name="command">Cache action command</param>
        /// <returns>Return determine data whether is exist</returns>
        public bool Exist<T>(ExistDataCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            return ExistAsync(command).Result;
        }

        /// <summary>
        /// Check data
        /// </summary>
        /// <param name="command">Cache action command</param>
        /// <returns>Return determine data whether is exist</returns>
        public async Task<bool> ExistAsync<T>(ExistDataCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            if (command == null)
            {
                return false;
            }
            Type dataType = typeof(T);
            IQuery checkQuery;
            if (command.Query != null)
            {
                var primaryKeys = EntityManager.GetPrimaryKeys(dataType);
                checkQuery = command.Query.LightClone();
                checkQuery.ClearOrder();
                checkQuery.ClearQueryFields();
                checkQuery.ClearNotQueryFields();
                checkQuery.AddQueryFields(primaryKeys.ToArray());
            }
            else
            {
                checkQuery = QueryManager.Create<T>();
            }
            checkQuery.QuerySize = 1;
            var data = await GetAsync(new GetDataCacheCommand<T>()
            {
                GetDatabaseDataProxyAsync = command.GetDatabaseDataProxyAsync,
                Query = checkQuery
            }).ConfigureAwait(false);
            return data != null;
        }

        #endregion

        #region Get data

        /// <summary>
        /// Get data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data object</returns>
        public T Get<T>(GetDataCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            return GetAsync(command).Result;
        }

        /// <summary>
        /// get data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return Data object</returns>
        public async Task<T> GetAsync<T>(GetDataCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            var query = command.Query;
            if (query != null)
            {
                query.QuerySize = 1;
            }
            var queryResult = dataCachePolicyService.Query<T>(query);
            if (queryResult != null && !queryResult.QueryDatabase)
            {
                return queryResult.Datas?.FirstOrDefault();
            }
            if (command.GetDatabaseDataProxyAsync != null)
            {
                var data = await command.GetDatabaseDataProxyAsync(query).ConfigureAwait(false);
                dataCachePolicyService.QueryCallback(new QueryDataCallbackContext<T>()
                {
                    Datas = new T[1] { data },
                    Query = query,
                    PrimaryCacheKeys = queryResult.PrimaryCacheKeys,
                    OtherCacheKeys = queryResult.OtherCacheKeys
                });
                return data;
            }
            return default;
        }

        #endregion

        #region Get data list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data list</returns>
        public List<T> GetList<T>(GetDataListCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            return GetListAsync(command).Result;
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data list</returns>
        public async Task<List<T>> GetListAsync<T>(GetDataListCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            var query = command.Query;
            var queryResult = dataCachePolicyService.Query<T>(query);
            var queryDatasProxy = command.GetDatabaseDataListProxyAsync;
            List<T> dataList = queryResult.Datas;
            if (queryResult.QueryDatabase && queryDatasProxy != null)
            {
                dataList = await queryDatasProxy(query).ConfigureAwait(false);
                dataCachePolicyService.QueryCallback(new QueryDataCallbackContext<T>()
                {
                    Query = query,
                    Datas = dataList,
                    PrimaryCacheKeys = queryResult.PrimaryCacheKeys,
                    OtherCacheKeys = queryResult.OtherCacheKeys
                });
            }
            return dataList ?? new List<T>(0);
        }

        #endregion

        #region Query paging

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data paging</returns>
        public IPaging<T> GetPaging<T>(GetDataPagingCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            return GetPagingAsync(command).Result;
        }

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data paging</returns>
        public async Task<IPaging<T>> GetPagingAsync<T>(GetDataPagingCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            var pageQueryProxy = command.GetDatabaseDataPagingProxyAsync;
            var query = command.Query;
            if (pageQueryProxy == null)
            {
                return Pager.Empty<T>();
            }
            Type dataType = typeof(T);

            #region first,query paging by primary keys

            var primaryKeys = EntityManager.GetPrimaryKeys(dataType);
            IQuery pagingQuery = null;
            if (query != null)
            {
                pagingQuery = query.LightClone();
                pagingQuery.ClearQueryFields();
                pagingQuery.ClearNotQueryFields();
                pagingQuery.AddQueryFields(primaryKeys.ToArray());
            }
            IPaging<T> dataPaging = await pageQueryProxy(pagingQuery).ConfigureAwait(false);

            #endregion

            if (dataPaging.IsNullOrEmpty())
            {
                return Pager.Empty<T>();
            }

            #region second,query data by primary keys

            IQuery subQuery = QueryManager.AppendEntityIdentityCondition(dataPaging);
            if (query != null)
            {
                subQuery.AddQueryFields(query.QueryFields.ToArray());
                subQuery.AddNotQueryFields(query.NotQueryFields.ToArray());
            }
            var queryDatasProxy = command.GetDatabaseDataListProxyAsync;
            List<T> dataList = await GetListAsync(new GetDataListCacheCommand<T>()
            {
                Query = subQuery,
                GetDatabaseDataListProxyAsync = queryDatasProxy,
            }).ConfigureAwait(false);

            #endregion

            if (query != null)
            {
                dataList = query.Sort(dataList).ToList();//data sort
            }
            return Pager.Create(dataPaging.Page, dataPaging.PageSize, dataPaging.TotalCount, dataList);
        }

        #endregion

        #region Modify data

        /// <summary>
        /// Modify data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return a ICommand object</returns>
        public ICommand Modify<T>(ModifyDataCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            var dataModifyProxy = command.ModifyDatabaseDataProxy;
            var databaseCommand = dataModifyProxy(command.NewData, command.OldData);
            if (databaseCommand != null)
            {
                //trigger cache policy
                dataCachePolicyService.Modify(command, databaseCommand);
            }
            return databaseCommand;
        }

        /// <summary>
        /// Modify data by condition
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return a ICommand object</returns>
        public ICommand Modify<T>(ModifyByConditionCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            var dataModifyProxy = command.ModifyDatabaseDataProxy;
            var databaseCommand = dataModifyProxy(command.Modify, command.Query);
            if (databaseCommand != null)
            {
                //trigger cache policy
                dataCachePolicyService.ModifyByCondition(command, databaseCommand);
            }
            return databaseCommand;
        }

        /// <summary>
        /// Modify data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return a ICommand object</returns>
        public ICommand Modify<T>(ModifyDataByConditionCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            var dataModifyProxy = command.ModifyDatabaseDataProxy;
            var query = command.Query;
            var databaseCommand = dataModifyProxy(command.NewData, command.OldData, command.Query);
            if (databaseCommand != null)
            {
                //trigger cache policy
                dataCachePolicyService.ModifyDataByCondition(command, databaseCommand);
            }
            return databaseCommand;
        }

        #endregion

        #region Remove data

        /// <summary>
        /// Remove data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns></returns>
        public ICommand Remove<T>(RemoveDataCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            if (command?.RemoveDatabaseDataProxy == null)
            {
                return null;
            }
            var removeProxy = command.RemoveDatabaseDataProxy;
            var databaseCommand = removeProxy(command.Data);
            if (databaseCommand != null)
            {
                //trigger cache policy
                dataCachePolicyService.Remove(command, databaseCommand);
            }
            return databaseCommand;
        }

        /// <summary>
        /// Remove data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns></returns>
        public ICommand Remove<T>(RemoveByConditionCacheCommand<T> command) where T : BaseEntity<T>, new()
        {
            var removeProxy = command.RemoveDatabaseDataProxy;
            var databaseCommand = removeProxy(command.Query);
            if (databaseCommand != null)
            {
                //trigger cache policy
                dataCachePolicyService.RemoveByCondition(command, databaseCommand);
            }
            return databaseCommand;
        }

        #endregion
    }
}
