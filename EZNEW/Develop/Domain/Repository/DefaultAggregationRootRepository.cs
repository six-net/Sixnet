using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Domain.Repository.Event;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Fault;
using EZNEW.Paging;
using EZNEW.Response;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Default aggregation root repository
    /// </summary>
    /// <typeparam name="TModel">Aggregation model</typeparam>
    public abstract class DefaultAggregationRootRepository<TModel> : BaseAggregationRepository<TModel> where TModel : AggregationRoot<TModel>
    {
        #region Impl methods

        #region Save data

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override TModel Save(TModel data, ActivationOptions activationOptions = null)
        {
            return Save(new TModel[1] { data }, activationOptions)?.FirstOrDefault();
        }

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override List<TModel> Save(IEnumerable<TModel> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                throw new EZNEWException($"{nameof(datas)} is null or empty");
            }
            var records = new List<IActivationRecord>();
            var resultDatas = new List<TModel>();
            foreach (var data in datas)
            {
                if (data == null)
                {
                    continue;
                }
                var saveData = data;
                if (!saveData.IdentityValueIsNone() && saveData.IsNew)
                {
                    var nowData = Get(saveData);
                    if (nowData != null)
                    {
                        saveData = nowData.OnUpdating(saveData);
                    }
                }
                if (saveData.IsNew)
                {
                    saveData = saveData.OnAdding();
                }
                if (!saveData.CanBeSave)
                {
                    throw new EZNEWException($"Data:{saveData.IdentityValue} cann't to be save");
                }
                var record = ExecuteSave(saveData, activationOptions);
                if (record != null)
                {
                    records.Add(record);
                    resultDatas.Add(saveData);
                }
            }
            RepositoryEventBus.PublishSave(GetType(), datas, activationOptions);
            WorkManager.RegisterActivationRecord(records);
            return resultDatas;
        }

        #endregion

        #region Remove data

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(TModel data, ActivationOptions activationOptions = null)
        {
            Remove(new TModel[1] { data }, activationOptions);
        }

        /// <summary>
        /// Remove datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(IEnumerable<TModel> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                throw new EZNEWException($"{nameof(datas)} is null or empty");
            }
            var records = new List<IActivationRecord>();
            foreach (var data in datas)
            {
                if (data == null)
                {
                    throw new EZNEWException("remove object data is null");
                }
                if (!data.CanBeRemove)
                {
                    throw new EZNEWException($"Data:{data.IdentityValue} cann't to be remove");
                }
                var record = ExecuteRemove(data, activationOptions);//Execute remove
                if (record != null)
                {
                    records.Add(record);
                }
            }
            RepositoryEventBus.PublishRemove(GetType(), datas, activationOptions);
            WorkManager.RegisterActivationRecord(records);
        }

        #endregion

        #region Remove by condition

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(IQuery query, ActivationOptions activationOptions = null)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Remove, AppendRemoveCondition);
            var record = ExecuteRemove(newQuery, activationOptions);
            if (record != null)
            {
                RepositoryEventBus.PublishRemove<TModel>(GetType(), newQuery, activationOptions);
                WorkManager.RegisterActivationRecord(record);
                RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Remove);
            }
        }

        #endregion

        #region Modify

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="expression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Modify(IModify expression, IQuery query, ActivationOptions activationOptions = null)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Modify, AppendModifyCondition);
            var record = ExecuteModify(expression, newQuery, activationOptions);
            if (record != null)
            {
                RepositoryEventBus.PublishModify<TModel>(GetType(), expression, newQuery, activationOptions);
                WorkManager.RegisterActivationRecord(record);
                RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Modify);
            }
        }

        #endregion

        #region Get data

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        public sealed override TModel Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// Get data by current data
        /// </summary>
        /// <param name="currentData">Current data</param>
        /// <returns>Return data</returns>
        public sealed override TModel Get(TModel currentData)
        {
            return GetDataByCurrentDataAsync(currentData).Result;
        }

        /// <summary>
        /// Get data by current data
        /// </summary>
        /// <param name="currentData">Current data</param>
        /// <returns>Return data</returns>
        public sealed override async Task<TModel> GetAsync(TModel currentData)
        {
            return await GetDataByCurrentDataAsync(currentData).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        public sealed override async Task<TModel> GetAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var data = await GetDataAsync(newQuery).ConfigureAwait(false);
            var dataList = new List<TModel>(1) { data };
            QueryCallback(newQuery, false, dataList);
            RepositoryEventBus.PublishQuery(GetType(), dataList, newQuery, result =>
            {
                QueryEventResult<TModel> queryResult = result as QueryEventResult<TModel>;
                if (queryResult != null)
                {
                    dataList = queryResult.Datas;
                }
            });
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return dataList.IsNullOrEmpty() ? default : dataList.FirstOrDefault();
        }

        #endregion

        #region Get data list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        public sealed override List<TModel> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        public sealed override async Task<List<TModel>> GetListAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var datas = await GetDataListAsync(newQuery).ConfigureAwait(false);
            QueryCallback(newQuery, true, datas);
            RepositoryEventBus.PublishQuery(GetType(), datas, newQuery, result =>
            {
                QueryEventResult<TModel> queryResult = result as QueryEventResult<TModel>;
                if (queryResult != null)
                {
                    datas = queryResult.Datas;
                }
            });
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return datas ?? new List<TModel>(0);
        }

        #endregion

        #region Get data paging

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public sealed override IPaging<TModel> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public sealed override async Task<IPaging<TModel>> GetPagingAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var paging = await GetDataPagingAsync(newQuery).ConfigureAwait(false);
            IEnumerable<TModel> datas = paging;
            QueryCallback(newQuery, true, datas);
            RepositoryEventBus.PublishQuery(GetType(), datas, newQuery, result =>
            {
                QueryEventResult<TModel> queryResult = result as QueryEventResult<TModel>;
                if (queryResult != null)
                {
                    datas = queryResult.Datas;
                }
            });
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return Pager.Create(paging.Page, paging.PageSize, paging.TotalCount, datas);
        }

        #endregion

        #region Exist

        /// <summary>
        /// Whether has any data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        public sealed override bool Exist(IQuery query)
        {
            return ExistAsync(query).Result;
        }

        /// <summary>
        /// Whether has any data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        public sealed override async Task<bool> ExistAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Exist, AppendExistCondition);
            var existValue = await IsExistAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Exist);
            return existValue;
        }

        #endregion

        #region Count

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        public sealed override long Count(IQuery query)
        {
            return CountAsync(query).Result;
        }

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        public sealed override async Task<long> CountAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Count, AppendCountCondition);
            var countValue = await CountValueAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Count);
            return countValue;
        }

        #endregion

        #region Max value

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        public sealed override TValue Max<TValue>(IQuery query)
        {
            return MaxAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        public sealed override async Task<TValue> MaxAsync<TValue>(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Max, AppendMaxCondition);
            var maxValue = await MaxValueAsync<TValue>(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Max);
            return maxValue;
        }

        #endregion

        #region Min value

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        public sealed override TValue Min<TValue>(IQuery query)
        {
            return MinAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        public sealed override async Task<TValue> MinAsync<TValue>(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Min, AppendMinCondition);
            var minValue = await MinValueAsync<TValue>(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Min);
            return minValue;
        }

        #endregion

        #region Sum value

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        public sealed override TValue Sum<TValue>(IQuery query)
        {
            return SumAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        public sealed override async Task<TValue> SumAsync<TValue>(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Sum, AppendSumCondition);
            var sumValue = await SumValueAsync<TValue>(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Sum);
            return sumValue;
        }

        #endregion

        #region Average value

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return average value</returns>
        public sealed override TValue Avg<TValue>(IQuery query)
        {
            return AvgAsync<TValue>(query).Result;
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return average value</returns>
        public sealed override async Task<TValue> AvgAsync<TValue>(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Avg, AppendAvgCondition);
            var avgValue = await AvgValueAsync<TValue>(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Avg);
            return avgValue;
        }

        #endregion

        #endregion

        #region Functions

        /// <summary>
        /// Execute save
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected abstract IActivationRecord ExecuteSave(TModel data, ActivationOptions activationOptions = null);

        /// <summary>
        /// Execute remove
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected abstract IActivationRecord ExecuteRemove(TModel data, ActivationOptions activationOptions = null);

        /// <summary>
        /// Execute Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected abstract IActivationRecord ExecuteRemove(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        protected abstract Task<TModel> GetDataAsync(IQuery query);

        /// <summary>
        /// Get data List
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return datas</returns>
        protected abstract Task<List<TModel>> GetDataListAsync(IQuery query);

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return datas</returns>
        protected abstract Task<IPaging<TModel>> GetDataPagingAsync(IQuery query);

        /// <summary>
        /// Get data by current data
        /// </summary>
        /// <param name="currentData">Current data</param>
        /// <returns>Return data</returns>
        protected abstract Task<TModel> GetDataByCurrentDataAsync(TModel currentData);

        /// <summary>
        /// Check data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        protected abstract Task<bool> IsExistAsync(IQuery query);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        protected abstract Task<long> CountValueAsync(IQuery query);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        protected abstract Task<TValue> MaxValueAsync<TValue>(IQuery query);

        /// <summary>
        /// Get Min Value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        protected abstract Task<TValue> MinValueAsync<TValue>(IQuery query);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        protected abstract Task<TValue> SumValueAsync<TValue>(IQuery query);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        protected abstract Task<TValue> AvgValueAsync<TValue>(IQuery query);

        /// <summary>
        /// Execute modify
        /// </summary>
        /// <param name="expression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected abstract IActivationRecord ExecuteModify(IModify expression, IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Query callback
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="batchReturn">Whether batch return</param>
        /// <param name="datas">Datas</param>
        protected virtual void QueryCallback(IQuery query, bool batchReturn, IEnumerable<TModel> datas)
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
                    data.SetLoadProperties(query.LoadPropertys);
                }
            }
        }

        #endregion

        #region Global condition

        #region Append remove extra condition

        /// <summary>
        /// Append remove condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendRemoveCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append modify extra condition

        /// <summary>
        /// Append modify condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendModifyCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append query extra condition

        /// <summary>
        /// Append query condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendQueryCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append exist extra condition

        /// <summary>
        /// Append exist condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendExistCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append count extra condition

        /// <summary>
        /// Append count condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendCountCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append max extra condition

        /// <summary>
        /// Append max condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendMaxCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append min extra condition

        /// <summary>
        /// Append min condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendMinCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append sum extra condition

        /// <summary>
        /// Append sum condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendSumCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #region Append avg extra condition

        /// <summary>
        /// Append avg condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendAvgCondition(IQuery originalQuery)
        {
            return originalQuery;
        }

        #endregion

        #endregion
    }
}
