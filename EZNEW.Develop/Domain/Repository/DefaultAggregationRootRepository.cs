using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Domain.Repository.Event;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// default aggregation root repository
    /// </summary>
    /// <typeparam name="T">data type</typeparam>
    public abstract class DefaultAggregationRootRepository<T> : BaseAggregationRepository<T> where T : IAggregationRoot<T>
    {
        #region impl methods

        #region save data

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void Save(T data, ActivationOption activationOption = null)
        {
            SaveAsync(data, activationOption).Wait();
        }

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        public sealed override async Task SaveAsync(T data, ActivationOption activationOption = null)
        {
            await SaveAsync(new T[1] { data }, activationOption).ConfigureAwait(false);
        }

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void Save(IEnumerable<T> datas, ActivationOption activationOption = null)
        {
            SaveAsync(datas, activationOption).Wait();
        }

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override async Task SaveAsync(IEnumerable<T> datas, ActivationOption activationOption = null)
        {
            #region verify parameters

            if (datas.IsNullOrEmpty())
            {
                throw new Exception($"{nameof(datas)} is null or empty");
            }
            foreach (var obj in datas)
            {
                if (obj == null)
                {
                    continue;
                }
                if (!obj.CanBeSave)
                {
                    throw new Exception("data cann't to be save");
                }
            }

            #endregion

            var records = new List<IActivationRecord>(datas.Count());
            foreach (var data in datas)
            {
                var record = await ExecuteSaveAsync(data, activationOption).ConfigureAwait(false);//execute save
                if (record == null)
                {
                    continue;
                }
                records.Add(record);
            }
            RepositoryEventBus.PublishSave(GetType(), datas, activationOption);
            WorkFactory.RegisterActivationRecord(records.ToArray());
        }

        #endregion

        #region remove data

        /// <summary>
        /// remove data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void Remove(T data, ActivationOption activationOption = null)
        {
            RemoveAsync(data, activationOption).Wait();
        }

        /// <summary>
        /// remove data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        public sealed override async Task RemoveAsync(T data, ActivationOption activationOption = null)
        {
            await RemoveAsync(new T[1] { data }, activationOption).ConfigureAwait(false);
        }

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void Remove(IEnumerable<T> datas, ActivationOption activationOption = null)
        {
            RemoveAsync(datas, activationOption).Wait();
        }

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override async Task RemoveAsync(IEnumerable<T> datas, ActivationOption activationOption = null)
        {
            #region verify parameters

            if (datas.IsNullOrEmpty())
            {
                throw new Exception("datas is null or empty");
            }
            foreach (var obj in datas)
            {
                if (obj == null)
                {
                    throw new Exception("remove object data is null");
                }
                if (!obj.CanBeRemove)
                {
                    throw new Exception("object data cann't to be remove");
                }
            }

            #endregion

            var records = new List<IActivationRecord>(datas.Count());
            foreach (var data in datas)
            {
                var record = await ExecuteRemoveAsync(data, activationOption).ConfigureAwait(false);//execute remove
                if (record == null)
                {
                    continue;
                }
                records.Add(record);
            }
            RepositoryEventBus.PublishRemove(GetType(), datas, activationOption);
            WorkFactory.RegisterActivationRecord(records.ToArray());
        }

        #endregion

        #region remove by condition

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query object</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void Remove(IQuery query, ActivationOption activationOption = null)
        {
            RemoveAsync(query, activationOption).Wait();
        }

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query object</param>
        /// <param name="activationOption">activation option</param>
        public sealed override async Task RemoveAsync(IQuery query, ActivationOption activationOption = null)
        {
            //append condition
            query = AppendRemoveCondition(query);
            var record = await ExecuteRemoveAsync(query, activationOption).ConfigureAwait(false);
            if (record == null)
            {
                return;
            }
            RepositoryEventBus.PublishRemove<T>(GetType(), query, activationOption);
            WorkFactory.RegisterActivationRecord(record);
        }

        #endregion

        #region modify

        /// <summary>
        /// modify data
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query object</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void Modify(IModify expression, IQuery query, ActivationOption activationOption = null)
        {
            ModifyAsync(expression, query, activationOption).Wait();
        }

        /// <summary>
        /// modify data
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query object</param>
        /// <param name="activationOption">activation option</param>
        public sealed override async Task ModifyAsync(IModify expression, IQuery query, ActivationOption activationOption = null)
        {
            //append condition
            query = AppendModifyCondition(query);
            var record = await ExecuteModifyAsync(expression, query, activationOption).ConfigureAwait(false);
            if (record == null)
            {
                return;
            }
            RepositoryEventBus.PublishModify<T>(GetType(), expression, query, activationOption);
            WorkFactory.RegisterActivationRecord(record);
        }

        #endregion

        #region get data

        /// <summary>
        /// get data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data</returns>
        public sealed override T Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// get data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data</returns>
        public sealed override async Task<T> GetAsync(IQuery query)
        {
            //append condition
            query = AppendQueryCondition(query);
            var data = await GetDataAsync(query).ConfigureAwait(false);
            var dataList = new List<T>() { data };
            QueryCallback(query, false, dataList);
            RepositoryEventBus.PublishQuery(GetType(), dataList, result =>
              {
                  QueryEventResult<T> queryResult = result as QueryEventResult<T>;
                  if (queryResult != null)
                  {
                      dataList = queryResult.Datas;
                  }
              });
            return dataList.IsNullOrEmpty() ? default(T) : dataList.FirstOrDefault();
        }

        #endregion

        #region get data list

        /// <summary>
        /// get data list
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>object list</returns>
        public sealed override List<T> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// get data list
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>object list</returns>
        public sealed override async Task<List<T>> GetListAsync(IQuery query)
        {
            //append condition
            query = AppendQueryCondition(query);
            var datas = await GetDataListAsync(query).ConfigureAwait(false);
            QueryCallback(query, true, datas);
            RepositoryEventBus.PublishQuery(GetType(), datas, result =>
            {
                QueryEventResult<T> queryResult = result as QueryEventResult<T>;
                if (queryResult != null)
                {
                    datas = queryResult.Datas;
                }
            });
            return datas ?? new List<T>(0);
        }

        #endregion

        #region get data paging

        /// <summary>
        /// get data paging
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data paging</returns>
        public sealed override IPaging<T> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// get data paging
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data paging</returns>
        public sealed override async Task<IPaging<T>> GetPagingAsync(IQuery query)
        {
            //append condition
            query = AppendQueryCondition(query);
            var paging = await GetDataPagingAsync(query).ConfigureAwait(false);
            IEnumerable<T> datas = paging;
            QueryCallback(query, true, datas);
            RepositoryEventBus.PublishQuery(GetType(), datas, result =>
            {
                QueryEventResult<T> queryResult = result as QueryEventResult<T>;
                if (queryResult != null)
                {
                    datas = queryResult.Datas;
                }
            });
            return new Paging<T>(paging.Page, paging.PageSize, paging.TotalCount, datas);
        }

        #endregion

        #region exist

        /// <summary>
        /// wheather has any data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns></returns>
        public sealed override bool Exist(IQuery query)
        {
            return ExistAsync(query).Result;
        }

        /// <summary>
        /// wheather has any data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns></returns>
        public sealed override async Task<bool> ExistAsync(IQuery query)
        {
            //append condition
            query = AppendExistCondition(query);
            return await IsExistAsync(query).ConfigureAwait(false);
        }

        #endregion

        #region count

        /// <summary>
        /// get data count
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns></returns>
        public sealed override long Count(IQuery query)
        {
            return CountAsync(query).Result;
        }

        /// <summary>
        /// get data count
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns></returns>
        public sealed override async Task<long> CountAsync(IQuery query)
        {
            //append condition
            query = AppendCountCondition(query);
            return await CountValueAsync(query).ConfigureAwait(false);
        }

        #endregion

        #region max value

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns>max value</returns>
        public sealed override VT Max<VT>(IQuery query)
        {
            return MaxAsync<VT>(query).Result;
        }

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns>max value</returns>
        public sealed override async Task<VT> MaxAsync<VT>(IQuery query)
        {
            //append condition
            query = AppendMaxCondition(query);
            return await MaxValueAsync<VT>(query).ConfigureAwait(false);
        }

        #endregion

        #region min value

        /// <summary>
        /// get min value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns>min value</returns>
        public sealed override VT Min<VT>(IQuery query)
        {
            return MinAsync<VT>(query).Result;
        }

        /// <summary>
        /// get min value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns>min value</returns>
        public sealed override async Task<VT> MinAsync<VT>(IQuery query)
        {
            //append condition
            query = AppendMinCondition(query);
            return await MinValueAsync<VT>(query).ConfigureAwait(false);
        }

        #endregion

        #region sum value

        /// <summary>
        /// get sum value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns>sum value</returns>
        public sealed override VT Sum<VT>(IQuery query)
        {
            return SumAsync<VT>(query).Result;
        }

        /// <summary>
        /// get sum value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns>sum value</returns>
        public sealed override async Task<VT> SumAsync<VT>(IQuery query)
        {
            //append condition
            query = AppendSumCondition(query);
            return await SumValueAsync<VT>(query).ConfigureAwait(false);
        }

        #endregion

        #region average value

        /// <summary>
        /// get average value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns>average value</returns>
        public sealed override VT Avg<VT>(IQuery query)
        {
            return AvgAsync<VT>(query).Result;
        }

        /// <summary>
        /// get average value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns>average value</returns>
        public sealed override async Task<VT> AvgAsync<VT>(IQuery query)
        {
            //append condition
            query = AppendAvgCondition(query);
            return await AvgValueAsync<VT>(query).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region functions

        /// <summary>
        /// execute save
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        protected abstract Task<IActivationRecord> ExecuteSaveAsync(T data, ActivationOption activationOption = null);

        /// <summary>
        /// execute remove
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        protected abstract Task<IActivationRecord> ExecuteRemoveAsync(T data, ActivationOption activationOption = null);

        /// <summary>
        /// execute remove by condition
        /// </summary>
        /// <param name="query">query object</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        protected abstract Task<IActivationRecord> ExecuteRemoveAsync(IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// get data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns></returns>
        protected abstract Task<T> GetDataAsync(IQuery query);

        /// <summary>
        /// get data List
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns></returns>
        protected abstract Task<List<T>> GetDataListAsync(IQuery query);

        /// <summary>
        /// get data paging
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns></returns>
        protected abstract Task<IPaging<T>> GetDataPagingAsync(IQuery query);

        /// <summary>
        /// check data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns></returns>
        protected abstract Task<bool> IsExistAsync(IQuery query);

        /// <summary>
        /// get data count
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns></returns>
        protected abstract Task<long> CountValueAsync(IQuery query);

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="VT">Data Type</typeparam>
        /// <param name="query">query object</param>
        /// <returns></returns>
        protected abstract Task<VT> MaxValueAsync<VT>(IQuery query);

        /// <summary>
        /// get Min Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns>min value</returns>
        protected abstract Task<VT> MinValueAsync<VT>(IQuery query);

        /// <summary>
        /// get sum value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns></returns>
        protected abstract Task<VT> SumValueAsync<VT>(IQuery query);

        /// <summary>
        /// get average value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query object</param>
        /// <returns></returns>
        protected abstract Task<VT> AvgValueAsync<VT>(IQuery query);

        /// <summary>
        /// execute modify
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query object</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        protected abstract Task<IActivationRecord> ExecuteModifyAsync(IModify expression, IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// query callback
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="batchReturn">batch return</param>
        /// <param name="datas">datas</param>
        protected virtual void QueryCallback(IQuery query, bool batchReturn, IEnumerable<T> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            foreach (var data in datas)
            {
                if (data == null)
                {
                    continue;
                }
                if (batchReturn)
                {
                    data.CloseLazyMemberLoad();
                }
                if (!(query?.LoadPropertys.IsNullOrEmpty() ?? true))
                {
                    data.SetLoadPropertys(query.LoadPropertys);
                }
            }
        }

        #endregion

        #region global condition

        #region append remove extra condition

        /// <summary>
        /// append remove condition
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <returns></returns>
        protected virtual IQuery AppendRemoveCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region append modify extra condition

        /// <summary>
        /// append modify condition
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <returns></returns>
        protected virtual IQuery AppendModifyCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region append query extra condition

        /// <summary>
        /// append query condition
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <returns></returns>
        protected virtual IQuery AppendQueryCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region append exist extra condition

        /// <summary>
        /// append exist condition
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <returns></returns>
        protected virtual IQuery AppendExistCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region append count extra condition

        /// <summary>
        /// append count condition
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <returns></returns>
        protected virtual IQuery AppendCountCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region append max extra condition

        /// <summary>
        /// append max condition
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <returns></returns>
        protected virtual IQuery AppendMaxCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region append min extra condition

        /// <summary>
        /// append min condition
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <returns></returns>
        protected virtual IQuery AppendMinCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region append sum extra condition

        /// <summary>
        /// append sum condition
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <returns></returns>
        protected virtual IQuery AppendSumCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region append avg extra condition

        /// <summary>
        /// append avg condition
        /// </summary>
        /// <param name="originalQuery">original query</param>
        /// <returns></returns>
        protected virtual IQuery AppendAvgCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #endregion
    }
}
