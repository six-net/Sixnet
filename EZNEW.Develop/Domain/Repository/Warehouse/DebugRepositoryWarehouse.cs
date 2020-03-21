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
    public class DebugRepositoryWarehouse<ET, DAI> : IRepositoryWarehouse<ET, DAI> where ET : BaseEntity<ET>, new() where DAI : IDataAccess<ET>
    {
        #region save

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public async Task<IActivationRecord> SaveAsync(IEnumerable<ET> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var packageRecord = DefaultActivationRecord<ET, DAI>.CreatePackageRecord();
            foreach (var data in datas)
            {
                var dataRecord = await SaveAsync(data, activationOption).ConfigureAwait(false);
                packageRecord.AddFollowRecords(packageRecord);
            }
            return packageRecord;
        }

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public async Task<IActivationRecord> SaveAsync(ET data, ActivationOption activationOption = null)
        {
            await WarehouseManager.SaveAsync(data).ConfigureAwait(false);
            var identityValue = data.GetIdentityValue();
            return DefaultActivationRecord<ET, DAI>.CreateSaveRecord(identityValue, activationOption);
        }

        #endregion

        #region remove

        /// <summary>
        /// remove data
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public async Task<IActivationRecord> RemoveAsync(IEnumerable<ET> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var packageRecord = DefaultActivationRecord<ET, DAI>.CreatePackageRecord();
            foreach (var data in datas)
            {
                var dataRecord = await RemoveAsync(data, activationOption).ConfigureAwait(false);
                packageRecord.AddFollowRecords(dataRecord);
            }
            return packageRecord;
        }

        /// <summary>
        /// remove data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public async Task<IActivationRecord> RemoveAsync(ET data, ActivationOption activationOption = null)
        {
            await WarehouseManager.SaveAsync(data).ConfigureAwait(false);
            var identityValue = data.GetIdentityValue();
            return DefaultActivationRecord<ET, DAI>.CreateRemoveObjectRecord(identityValue, activationOption);
        }

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<IActivationRecord> RemoveAsync(IQuery query, ActivationOption activationOption = null)
        {
            await WarehouseManager.RemoveAsync<ET>(query);
            var record = DefaultActivationRecord<ET, DAI>.CreateRemoveByConditionRecord(query, activationOption);
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
        public async Task<IActivationRecord> ModifyAsync(IModify modifyExpression, IQuery query, ActivationOption activationOption = null)
        {
            await WarehouseManager.ModifyAsync<ET>(modifyExpression, query);
            var record = DefaultActivationRecord<ET, DAI>.CreateModifyRecord(modifyExpression, query, activationOption);
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
            ET data = null;
            data = WarehouseManager.Merge(data, query);
            return await Task.FromResult(data);
        }

        /// <summary>
        /// query data list
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object list</returns>
        public async Task<List<ET>> GetListAsync(IQuery query)
        {
            var datas = new List<ET>(0);
            datas = WarehouseManager.Merge<ET>(datas, query);
            return await Task.FromResult(datas);
        }

        /// <summary>
        /// query data paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object paging</returns>
        public async Task<IPaging<ET>> GetPagingAsync(IQuery query)
        {
            IPaging<ET> paging = Paging<ET>.EmptyPaging();
            paging = WarehouseManager.MergePaging(paging, query);
            return await Task.FromResult(paging);
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
            return isExist;
        }

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public async Task<long> CountAsync(IQuery query)
        {
            long allCount = 0;
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
            return resultVal;
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
            return resultVal;
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
            return resultVal;
        }

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average value</returns>
        public async Task<DT> AvgAsync<DT>(IQuery query)
        {
            dynamic sum = await SumAsync<DT>(query).ConfigureAwait(false);
            var count = await CountAsync(query).ConfigureAwait(false);
            return sum / count;
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
