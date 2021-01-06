using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.DependencyInjection;
using EZNEW.Paging;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// Default repository data warehouses
    /// </summary>
    public class DefaultRepositoryWarehouse<TEntity, TDataAccess> : IRepositoryWarehouse<TEntity, TDataAccess> where TEntity : BaseEntity<TEntity>, new() where TDataAccess : IDataAccess<TEntity>
    {
        /// <summary>
        /// Data access
        /// </summary>
        readonly TDataAccess dataAccess = ContainerManager.Resolve<TDataAccess>();

        #region Save

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        public IActivationRecord Save(IEnumerable<TEntity> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var packageRecord = DefaultActivationRecord<TEntity, TDataAccess>.CreatePackageRecord();
            foreach (var data in datas)
            {
                var dataRecord = Save(data, activationOptions);
                packageRecord.AddFollowRecords(dataRecord);
            }
            return packageRecord;
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        public IActivationRecord Save(TEntity data, ActivationOptions activationOptions = null)
        {
            WarehouseManager.Save(data);
            var identityValue = data.GetIdentityValue();
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateSaveRecord(identityValue, activationOptions);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        public IActivationRecord Remove(IEnumerable<TEntity> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var packageRecord = DefaultActivationRecord<TEntity, TDataAccess>.CreatePackageRecord();
            foreach (var data in datas)
            {
                var dataRecord = Remove(data, activationOptions);
                packageRecord.AddFollowRecords(dataRecord);
            }
            return packageRecord;
        }

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        public IActivationRecord Remove(TEntity data, ActivationOptions activationOptions = null)
        {
            WarehouseManager.Remove(data);
            var identityValue = data.GetIdentityValue();
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateRemoveObjectRecord(identityValue, activationOptions);
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the activation record</returns>
        public IActivationRecord Remove(IQuery query, ActivationOptions activationOptions = null)
        {
            WarehouseManager.Remove<TEntity>(query);
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateRemoveByConditionRecord(query, activationOptions);
        }

        #endregion

        #region Modify

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modifyExpression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <returns>Return the activation record</returns>
        public IActivationRecord Modify(IModify modifyExpression, IQuery query, ActivationOptions activationOptions = null)
        {
            WarehouseManager.Modify<TEntity>(modifyExpression, query);
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateModifyRecord(modifyExpression, query, activationOptions);
        }

        #endregion

        #region Query

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        public async Task<TEntity> GetAsync(IQuery query)
        {
            var data = await dataAccess.GetAsync(query).ConfigureAwait(false);
            return WarehouseManager.Merge(data, query);
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Return data list</returns>
        public async Task<List<TEntity>> GetListAsync(IQuery query)
        {
            var datas = await dataAccess.GetListAsync(query).ConfigureAwait(false);
            return WarehouseManager.Merge(datas, query);
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Return data paging</returns>
        public async Task<IPaging<TEntity>> GetPagingAsync(IQuery query)
        {
            var paging = await dataAccess.GetPagingAsync(query).ConfigureAwait(false);
            return WarehouseManager.MergePaging(paging, query);
        }

        /// <summary>
        /// Exist data
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Return whether data is exist</returns>
        public async Task<bool> ExistAsync(IQuery query)
        {
            var result = WarehouseManager.Exist<TEntity>(query);
            return result.IsExist || await dataAccess.ExistAsync(result.CheckQuery).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Return data count</returns>
        public async Task<long> CountAsync(IQuery query)
        {
            var countResult = WarehouseManager.Count<TEntity>(query);
            countResult.Count += await dataAccess.CountAsync(countResult.ComputeQuery).ConfigureAwait(false);
            return countResult.Count;
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query model</param>
        /// <returns>Return the max value</returns>
        public async Task<TValue> MaxAsync<TValue>(IQuery query)
        {
            var maxResult = WarehouseManager.Max<TEntity, TValue>(query);
            dynamic resultVal = maxResult.Value;
            dynamic maxValue = await dataAccess.MaxAsync<TValue>(maxResult.ComputeQuery).ConfigureAwait(false);
            return maxResult.ValidValue ? (resultVal > maxValue ? resultVal : maxValue) : maxValue;
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query model</param>
        /// <returns>Return the min value</returns>
        public async Task<TValue> MinAsync<TValue>(IQuery query)
        {
            var minResult = WarehouseManager.Min<TEntity, TValue>(query);
            dynamic resultVal = minResult.Value;
            dynamic minValue = await dataAccess.MinAsync<TValue>(minResult.ComputeQuery).ConfigureAwait(false);
            return minResult.ValidValue ? (resultVal < minValue ? resultVal : minValue) : minValue;
        }

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query model</param>
        /// <returns>Return the sum value</returns>
        public async Task<TValue> SumAsync<TValue>(IQuery query)
        {
            var sumResult = WarehouseManager.Sum<TEntity, TValue>(query);
            dynamic resultVal = sumResult.Value;
            dynamic sumValue = await dataAccess.SumAsync<TValue>(sumResult.ComputeQuery).ConfigureAwait(false);
            return sumResult.ValidValue ? resultVal + sumValue : sumValue;
        }

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query model</param>
        /// <returns>Return the average value</returns>
        public async Task<TValue> AvgAsync<TValue>(IQuery query)
        {
            var countResult = WarehouseManager.Count<TEntity>(query);
            if (countResult.Count > 0)
            {
                dynamic sum = await SumAsync<TValue>(query).ConfigureAwait(false);
                var count = await CountAsync(query).ConfigureAwait(false);
                return sum / count;
            }
            else
            {
                return await dataAccess.AvgAsync<TValue>(query).ConfigureAwait(false);
            }
        }

        #endregion

        #region Life source

        /// <summary>
        /// Get life source
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return the data life source</returns>
        public DataLifeSource GetLifeSource(TEntity data)
        {
            if (data == null)
            {
                return DataLifeSource.New;
            }
            return WarehouseManager.GetLifeSource(data);
        }

        /// <summary>
        /// Modify life status
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="lifeSource">Life source</param>
        public void ModifyLifeSource(TEntity data, DataLifeSource lifeSource)
        {
            WarehouseManager.ModifyLifeSource(data, lifeSource);
        }

        #endregion
    }
}
