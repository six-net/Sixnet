using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;
using EZNEW.Framework.Paging;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Framework.Extension;
using EZNEW.Develop.Entity;
using EZNEW.Framework.IoC;
using EZNEW.Framework;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.Domain.Repository.Event;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Default Repository
    /// </summary>
    public abstract class DefaultAggregationRepository<DT, ET, DAI> : DefaultAggregationRootRepository<DT> where DT : IAggregationRoot<DT> where ET : BaseEntity<ET> where DAI : IDataAccess<ET>
    {
        protected IRepositoryWarehouse<ET, DAI> repositoryWarehouse = ContainerManager.Resolve<IRepositoryWarehouse<ET, DAI>>();

        static DefaultAggregationRepository()
        {
            if (!ContainerManager.IsRegister<IRepositoryWarehouse<ET, DAI>>())
            {
                ContainerManager.Register<IRepositoryWarehouse<ET, DAI>, DefaultRepositoryWarehouse<ET, DAI>>();
            }
        }

        #region impl

        /// <summary>
        /// get life source
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public sealed override DataLifeSource GetLifeSource(IAggregationRoot data)
        {
            if (data == null)
            {
                return DataLifeSource.New;
            }
            return repositoryWarehouse.GetLifeSource(data.MapTo<ET>());
        }

        /// <summary>
        /// modify life source
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="lifeSource">life source</param>
        public sealed override void ModifyLifeSource(IAggregationRoot data, DataLifeSource lifeSource)
        {
            if (data == null)
            {
                return;
            }
            repositoryWarehouse.ModifyLifeSource(data.MapTo<ET>(), lifeSource);
        }

        #endregion

        #region function

        /// <summary>
        /// Execute Save
        /// </summary>
        /// <param name="data">objects</param>
        protected override async Task<IActivationRecord> ExecuteSaveAsync(DT data)
        {
            if (data == null)
            {
                return null;
            }
            var entity = data.MapTo<ET>();
            return await SaveEntityAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute Remove
        /// </summary>
        /// <param name="data">data</param>
        protected override async Task<IActivationRecord> ExecuteRemoveAsync(DT data)
        {
            if (data == null)
            {
                return null;
            }
            var entity = data.MapTo<ET>();
            return await RemoveEntityAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute Remove
        /// </summary>
        /// <param name="query">query model</param>
        protected override async Task<IActivationRecord> ExecuteRemoveAsync(IQuery query)
        {
            return await repositoryWarehouse.RemoveAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<DT> GetDataAsync(IQuery query)
        {
            var entityData = await repositoryWarehouse.GetAsync(query).ConfigureAwait(false);
            DT data = default(DT);
            if (entityData != null)
            {
                data = entityData.MapTo<DT>();
            }
            return data;
        }

        /// <summary>
        /// Get Data List
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<List<DT>> GetDataListAsync(IQuery query)
        {
            var entityDataList = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            if (entityDataList.IsNullOrEmpty())
            {
                return new List<DT>(0);
            }
            var datas = entityDataList.Select(c => c.MapTo<DT>());
            return datas.ToList();
        }

        /// <summary>
        /// Get Object Paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<IPaging<DT>> GetDataPagingAsync(IQuery query)
        {
            var entityPaging = await repositoryWarehouse.GetPagingAsync(query).ConfigureAwait(false);
            var dataPaging = entityPaging.ConvertTo<DT>();
            return dataPaging;
        }

        /// <summary>
        /// Check Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<bool> IsExistAsync(IQuery query)
        {
            return await repositoryWarehouse.ExistAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<long> CountValueAsync(IQuery query)
        {
            return await repositoryWarehouse.CountAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="VT">Data Type</typeparam>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<VT> MaxValueAsync<VT>(IQuery query)
        {
            return await repositoryWarehouse.MaxAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// get Min Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        protected override async Task<VT> MinValueAsync<VT>(IQuery query)
        {
            return await repositoryWarehouse.MinAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<VT> SumValueAsync<VT>(IQuery query)
        {
            return await repositoryWarehouse.SumAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<VT> AvgValueAsync<VT>(IQuery query)
        {
            return await repositoryWarehouse.AvgAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute Modify
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        protected override async Task<IActivationRecord> ExecuteModifyAsync(IModify expression, IQuery query)
        {
            return await repositoryWarehouse.ModifyAsync(expression, query).ConfigureAwait(false);
        }

        /// <summary>
        /// save entity
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        protected virtual async Task<IActivationRecord> SaveEntityAsync(params ET[] datas)
        {
            return await repositoryWarehouse.SaveAsync(datas).ConfigureAwait(false);
        }

        /// <summary>
        /// save entity
        /// </summary>
        /// <param name="datas"></param>
        protected virtual void SaveEntity(params ET[] datas)
        {
            var record = SaveEntityAsync(datas).Result;
            if (record != null)
            {
                WorkFactory.RegisterActivationRecord(record);
            }
        }

        /// <summary>
        /// remove entity
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        protected virtual async Task<IActivationRecord> RemoveEntityAsync(params ET[] datas)
        {
            return await repositoryWarehouse.RemoveAsync(datas).ConfigureAwait(false);
        }

        /// <summary>
        /// remove entity
        /// </summary>
        /// <param name="datas"></param>
        protected virtual void RemoveEntity(params ET[] datas)
        {
            var record = RemoveEntityAsync(datas).Result;
            if (record != null)
            {
                WorkFactory.RegisterActivationRecord(record);
            }
        } 

        #endregion
    }
}
