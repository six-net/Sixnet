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
    public abstract class DefaultAggregationRootRepository<DT> : BaseAggregationRepository<DT> where DT : IAggregationRoot<DT>
    {
        #region Impl Methods

        /// <summary>
        /// Save Objects
        /// </summary>
        /// <param name="objDatas">Objects</param>
        public sealed override void Save(params DT[] objDatas)
        {
            SaveAsync(objDatas).Wait();
        }

        /// <summary>
        /// Save Objects
        /// </summary>
        /// <param name="objDatas">Objects</param>
        public sealed override async Task SaveAsync(params DT[] objDatas)
        {
            #region Verify Parameters

            if (objDatas.IsNullOrEmpty())
            {
                throw new Exception("objDatas is null or empty");
            }
            foreach (var obj in objDatas)
            {
                if (obj == null)
                {
                    continue;
                }
                if (!obj.CanBeSave)
                {
                    throw new Exception("object data cann't to be save");
                }
            }

            #endregion

            var records = new List<IActivationRecord>(objDatas.Length);
            foreach (var data in objDatas)
            {
                var record = await ExecuteSaveAsync(data).ConfigureAwait(false);//Execute Save
                if (record == null)
                {
                    continue;
                }
                records.Add(record);
            }
            RepositoryEventBus.PublishSave(GetType(), objDatas);
            WorkFactory.RegisterActivationRecord(records.ToArray());
        }

        /// <summary>
        /// Remove Objects
        /// </summary>
        /// <param name="objDatas">objects</param>
        public sealed override void Remove(params DT[] objDatas)
        {
            RemoveAsync(objDatas).Wait();
        }

        /// <summary>
        /// Remove Objects
        /// </summary>
        /// <param name="objDatas">objects</param>
        public sealed override async Task RemoveAsync(params DT[] objDatas)
        {
            #region Verify Parameters

            if (objDatas == null || objDatas.Length <= 0)
            {
                throw new Exception("objDatas is null or empty");
            }
            foreach (var obj in objDatas)
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

            var records = new List<IActivationRecord>(objDatas.Length);
            foreach (var data in objDatas)
            {
                var record = await ExecuteRemoveAsync(data).ConfigureAwait(false);//execute remove
                if (record == null)
                {
                    continue;
                }
                records.Add(record);
            }
            RepositoryEventBus.PublishRemove(GetType(), objDatas);
            WorkFactory.RegisterActivationRecord(records.ToArray());
        }

        /// <summary>
        /// Remove Object
        /// </summary>
        /// <param name="query">query model</param>
        public sealed override void Remove(IQuery query)
        {
            RemoveAsync(query).Wait();
        }

        /// <summary>
        /// Remove Object
        /// </summary>
        /// <param name="query">query model</param>
        public sealed override async Task RemoveAsync(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = RemoveQueryFilter(query);
            var record = await ExecuteRemoveAsync(query).ConfigureAwait(false);
            if (record == null)
            {
                return;
            }
            RepositoryEventBus.PublishRemove<DT>(GetType(), query);
            WorkFactory.RegisterActivationRecord(record);
        }

        /// <summary>
        /// Modify Object
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        public sealed override void Modify(IModify expression, IQuery query)
        {
            ModifyAsync(expression, query).Wait();
        }

        /// <summary>
        /// Modify Object
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        public sealed override async Task ModifyAsync(IModify expression, IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = ModifyQueryFilter(query);
            var record = await ExecuteModifyAsync(expression, query).ConfigureAwait(false);
            if (record == null)
            {
                return;
            }
            RepositoryEventBus.PublishModify<DT>(GetType(), expression, query);
            WorkFactory.RegisterActivationRecord(record);
        }

        /// <summary>
        /// Get Object
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>Object</returns>
        public sealed override DT Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// Get Object
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>Object</returns>
        public sealed override async Task<DT> GetAsync(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = GetDataQueryFilter(query);
            var data = await GetDataAsync(query).ConfigureAwait(false);
            var dataList = new List<DT>() { data };
            QueryCallback(query, false, dataList);
            RepositoryEventBus.PublishQuery<DT>(GetType(), dataList, result =>
              {
                  QueryEventResult<DT> queryResult = result as QueryEventResult<DT>;
                  if (queryResult != null)
                  {
                      dataList = queryResult.Datas;
                  }
              });
            return dataList.IsNullOrEmpty() ? default(DT) : dataList.FirstOrDefault();
        }

        /// <summary>
        /// Get Object List
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object list</returns>
        public sealed override List<DT> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// Get Object List
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object list</returns>
        public sealed override async Task<List<DT>> GetListAsync(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = GetDataQueryFilter(query);
            var datas = await GetDataListAsync(query).ConfigureAwait(false);
            QueryCallback(query, true, datas);
            RepositoryEventBus.PublishQuery<DT>(GetType(), datas, result =>
            {
                QueryEventResult<DT> queryResult = result as QueryEventResult<DT>;
                if (queryResult != null)
                {
                    datas = queryResult.Datas;
                }
            });
            return datas ?? new List<DT>(0);
        }

        /// <summary>
        /// Get Object Paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object paging</returns>
        public sealed override IPaging<DT> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// Get Object Paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object paging</returns>
        public sealed override async Task<IPaging<DT>> GetPagingAsync(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = GetDataQueryFilter(query);
            var paging = await GetDataPagingAsync(query).ConfigureAwait(false);
            IEnumerable<DT> datas = paging;
            QueryCallback(query, true, datas);
            RepositoryEventBus.PublishQuery<DT>(GetType(), datas, result =>
            {
                QueryEventResult<DT> queryResult = result as QueryEventResult<DT>;
                if (queryResult != null)
                {
                    datas = queryResult.Datas;
                }
            });
            return new Paging<DT>(paging.Page, paging.PageSize, paging.TotalCount, datas);
        }

        /// <summary>
        /// Wheather Has Any Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public sealed override bool Exist(IQuery query)
        {
            return ExistAsync(query).Result;
        }

        /// <summary>
        /// Wheather Has Any Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public sealed override async Task<bool> ExistAsync(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = ExistQueryFilter(query);
            return await IsExistAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public sealed override long Count(IQuery query)
        {
            return CountAsync(query).Result;
        }

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public sealed override async Task<long> CountAsync(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = CountQueryFilter(query);
            return await CountValueAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>max value</returns>
        public sealed override VT Max<VT>(IQuery query)
        {
            return MaxAsync<VT>(query).Result;
        }

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>max value</returns>
        public sealed override async Task<VT> MaxAsync<VT>(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = MaxQueryFilter(query);
            return await MaxValueAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Min Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        public sealed override VT Min<VT>(IQuery query)
        {
            return MinAsync<VT>(query).Result;
        }

        /// <summary>
        /// Get Min Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        public sealed override async Task<VT> MinAsync<VT>(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = MinQueryFilter(query);
            return await MinValueAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum value</returns>
        public sealed override VT Sum<VT>(IQuery query)
        {
            return SumAsync<VT>(query).Result;
        }

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum value</returns>
        public sealed override async Task<VT> SumAsync<VT>(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = SumQueryFilter(query);
            return await SumValueAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average value</returns>
        public sealed override VT Avg<VT>(IQuery query)
        {
            return AvgAsync<VT>(query).Result;
        }

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average value</returns>
        public sealed override async Task<VT> AvgAsync<VT>(IQuery query)
        {
            //query filter
            query = GlobalQueryFilter(query);
            query = AvgQueryFilter(query);
            return await AvgValueAsync<VT>(query).ConfigureAwait(false);
        }

        #endregion

        #region Functions

        /// <summary>
        /// Execute Save
        /// </summary>
        /// <param name="data">objects</param>
        protected abstract Task<IActivationRecord> ExecuteSaveAsync(DT data);

        /// <summary>
        /// Execute Remove
        /// </summary>
        /// <param name="data">data</param>
        protected abstract Task<IActivationRecord> ExecuteRemoveAsync(DT data);

        /// <summary>
        /// Execute Remove
        /// </summary>
        /// <param name="query">query model</param>
        protected abstract Task<IActivationRecord> ExecuteRemoveAsync(IQuery query);

        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected abstract Task<DT> GetDataAsync(IQuery query);

        /// <summary>
        /// Get Data List
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected abstract Task<List<DT>> GetDataListAsync(IQuery query);

        /// <summary>
        /// Get Object Paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected abstract Task<IPaging<DT>> GetDataPagingAsync(IQuery query);

        /// <summary>
        /// Check Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected abstract Task<bool> IsExistAsync(IQuery query);

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected abstract Task<long> CountValueAsync(IQuery query);

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="VT">Data Type</typeparam>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected abstract Task<VT> MaxValueAsync<VT>(IQuery query);

        /// <summary>
        /// get Min Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        protected abstract Task<VT> MinValueAsync<VT>(IQuery query);

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected abstract Task<VT> SumValueAsync<VT>(IQuery query);

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected abstract Task<VT> AvgValueAsync<VT>(IQuery query);

        /// <summary>
        /// Execute Modify
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        protected abstract Task<IActivationRecord> ExecuteModifyAsync(IModify expression, IQuery query);

        /// <summary>
        /// query callback
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="batchReturn">batch return</param>
        /// <param name="datas">datas</param>
        protected virtual void QueryCallback(IQuery query, bool batchReturn, IEnumerable<DT> datas)
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

        #region Query Filter

        #region global query filter

        /// <summary>
        /// global query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery GlobalQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #region remove query filter

        /// <summary>
        /// remove query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery RemoveQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #region modify query filter

        /// <summary>
        /// modify query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery ModifyQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #region get data query filter

        /// <summary>
        /// get data query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery GetDataQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #region exist query filter

        /// <summary>
        /// exist query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery ExistQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #region count query filter

        /// <summary>
        /// count query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery CountQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #region max query filter

        /// <summary>
        /// count query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery MaxQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #region min query filter

        /// <summary>
        /// count query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery MinQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #region sum query filter

        /// <summary>
        /// count query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery SumQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #region avg query filter

        /// <summary>
        /// count query filter
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        public virtual IQuery AvgQueryFilter(IQuery originQuery)
        {
            return originQuery;
        }

        #endregion

        #endregion
    }
}
