using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Extension;
using EZNEW.Framework.IoC;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// repository warehouses
    /// </summary>
    public class DefaultRepositoryWarehouse<ET, DAI> : IRepositoryWarehouse<ET, DAI> where ET : BaseEntity<ET> where DAI : IDataAccess<ET>
    {
        #region save

        /// <summary>
        /// save data
        /// </summary>
        /// <typeparam name="ET">entity</typeparam>
        /// <typeparam name="DAI">persistent data service</typeparam>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public async Task<IActivationRecord> SaveAsync(params ET[] datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            await WarehouseManager.SaveAsync(datas);
            var packageRecord = DefaultActivationRecord<ET, DAI>.CreatePackageRecord();
            foreach (var data in datas)
            {
                var identityValue = data.GetIdentityValue();
                packageRecord.AddFollowRecords(DefaultActivationRecord<ET, DAI>.CreateSaveRecord(identityValue));
            }
            return packageRecord;
        }

        #endregion

        #region remove

        /// <summary>
        /// remove data
        /// </summary>
        /// <typeparam name="ET">entity</typeparam>
        /// <typeparam name="DAI">persistent data service</typeparam>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public async Task<IActivationRecord> RemoveAsync(params ET[] datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            await WarehouseManager.RemoveAsync(datas);
            var packageRecord = DefaultActivationRecord<ET, DAI>.CreatePackageRecord();
            foreach (var data in datas)
            {
                var identityValue = data.GetIdentityValue();
                packageRecord.AddFollowRecords(DefaultActivationRecord<ET, DAI>.CreateRemoveObjectRecord(identityValue));
            }
            return packageRecord;
        }

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IActivationRecord> RemoveAsync(IQuery query)
        {
            await WarehouseManager.RemoveAsync<ET>(query);
            var record = DefaultActivationRecord<ET, DAI>.CreateRemoveByConditionRecord(query);
            return record;
        }

        #endregion

        #region modify

        /// <summary>
        /// modify
        /// </summary>
        /// <param name="modifyExpression">modify expression</param>
        /// <param name="query">query</param>
        /// <returns></returns>
        public async Task<IActivationRecord> ModifyAsync(IModify modifyExpression, IQuery query)
        {
            await WarehouseManager.ModifyAsync<ET>(modifyExpression, query);
            var record = DefaultActivationRecord<ET, DAI>.CreateModifyRecord(modifyExpression, query);
            return record;
        }

        #endregion

        #region query

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data</returns>
        public async Task<ET> GetAsync(IQuery query)
        {
            var data = await ContainerManager.Resolve<DAI>().GetAsync(query).ConfigureAwait(false);
            data = WarehouseManager.Merge(data, query);
            return data;
        }

        /// <summary>
        /// query data list
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object list</returns>
        public async Task<List<ET>> GetListAsync(IQuery query)
        {
            var datas = await ContainerManager.Resolve<DAI>().GetListAsync(query).ConfigureAwait(false);
            datas = WarehouseManager.Merge<ET>(datas, query);
            return datas;
        }

        /// <summary>
        /// query data paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object paging</returns>
        public async Task<IPaging<ET>> GetPagingAsync(IQuery query)
        {
            var paging = await ContainerManager.Resolve<DAI>().GetPagingAsync(query).ConfigureAwait(false);
            paging = WarehouseManager.MergePaging(paging, query);
            return paging;
        }

        /// <summary>
        /// exist data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public async Task<bool> ExistAsync(IQuery query)
        {
            var result = await WarehouseManager.ExistAsync<ET>(query).ConfigureAwait(false);
            var isExist = result.IsExist;
            if (!isExist)
            {
                isExist = await ContainerManager.Resolve<DAI>().ExistAsync(result.CheckQuery).ConfigureAwait(false);
            }
            return isExist;
        }

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public async Task<long> CountAsync(IQuery query)
        {
            var allCount = await ContainerManager.Resolve<DAI>().CountAsync(query).ConfigureAwait(false);
            var countResult = await WarehouseManager.CountAsync<ET>(query).ConfigureAwait(false);
            allCount = allCount - countResult.PersistentDataCount + countResult.NewDataCount;
            return allCount;
        }

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>max value</returns>
        public async Task<DT> MaxAsync<DT>(IQuery query)
        {
            var maxResult = await WarehouseManager.MaxAsync<ET, DT>(query).ConfigureAwait(false);
            dynamic resultVal = maxResult.Value;
            dynamic maxValue = await ContainerManager.Resolve<DAI>().MaxAsync<DT>(maxResult.ComputeQuery).ConfigureAwait(false);
            return resultVal > maxValue ? resultVal : maxValue;
        }

        /// <summary>
        /// Get Min Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        public async Task<DT> MinAsync<DT>(IQuery query)
        {
            var minResult = await WarehouseManager.MinAsync<ET, DT>(query).ConfigureAwait(false);
            dynamic resultVal = minResult.Value;
            dynamic minValue = await ContainerManager.Resolve<DAI>().MinAsync<DT>(minResult.ComputeQuery).ConfigureAwait(false);
            return resultVal < minValue ? resultVal : minValue;
        }

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum value</returns>
        public async Task<DT> SumAsync<DT>(IQuery query)
        {
            var sumResult = await WarehouseManager.SumAsync<ET, DT>(query).ConfigureAwait(false);
            dynamic resultVal = sumResult.Value;
            dynamic sumValue = await ContainerManager.Resolve<DAI>().SumAsync<DT>(sumResult.ComputeQuery).ConfigureAwait(false);
            return resultVal + sumValue;
        }

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average value</returns>
        public async Task<DT> AvgAsync<DT>(IQuery query)
        {
            var countResult = await WarehouseManager.CountAsync<ET>(query).ConfigureAwait(false);
            if (countResult.TotalDataCount > 0)
            {
                dynamic sum = await SumAsync<DT>(query).ConfigureAwait(false);
                var count = await CountAsync(query).ConfigureAwait(false);
                return sum / count;
            }
            else
            {
                return await ContainerManager.Resolve<DAI>().AvgAsync<DT>(query).ConfigureAwait(false);
            }
        }

        #endregion

        #region life source

        /// <summary>
        /// get life source
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public DataLifeSource GetLifeSource(ET data)
        {
            if (data == null)
            {
                return DataLifeSource.New;
            }
            return WarehouseManager.GetLifeSource(data);
        }

        /// <summary>
        /// modify life status
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="lifeSource">life source</param>
        public void ModifyLifeSource(ET data, DataLifeSource lifeSource)
        {
            WarehouseManager.ModifyLifeSource(data, lifeSource);
        }

        #endregion
    }
}
