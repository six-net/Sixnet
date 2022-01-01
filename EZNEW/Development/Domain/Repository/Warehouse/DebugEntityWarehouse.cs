using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Entity;
using EZNEW.Development.UnitOfWork;
using EZNEW.Paging;
using EZNEW.Development.Domain.Repository.Warehouse.Storage;
using EZNEW.Exceptions;
using System;
using System.Linq;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Defines debug entity warehouse
    /// </summary>
    public class DebugEntityWarehouse<TEntity, TDataAccess> : IEntityWarehouse<TEntity, TDataAccess> where TEntity : BaseEntity<TEntity>, new() where TDataAccess : IDataAccess<TEntity>
    {
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
                packageRecord.AddFollowRecord(dataRecord);
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
            EntityStorage<TEntity>.GetCurrentEntityStorage(true).Save(data);
            var identityValue = data.GetIdentityValue();
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateSavingRecord(identityValue, activationOptions);
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
                packageRecord.AddFollowRecord(dataRecord);
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
            EntityStorage<TEntity>.GetCurrentEntityStorage(true).Remove(data);
            var identityValue = data.GetIdentityValue();
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateRemovingObjectRecord(identityValue, activationOptions);
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the activation record</returns>
        public IActivationRecord Remove(IQuery query, ActivationOptions activationOptions = null)
        {
            EntityStorage<TEntity>.GetCurrentEntityStorage(true).Remove(query);
            var record = DefaultActivationRecord<TEntity, TDataAccess>.CreateRemovingByConditionRecord(query, activationOptions);
            return record;
        }

        #endregion

        #region Modify

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modifyExpression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <returns>Return activation record</returns>
        public IActivationRecord Modify(IModification modifyExpression, IQuery query, ActivationOptions activationOptions = null)
        {
            EntityStorage<TEntity>.GetCurrentEntityStorage(true).Modify(modifyExpression, query);
            var record = DefaultActivationRecord<TEntity, TDataAccess>.CreateModificationRecord(modifyExpression, query, activationOptions);
            return record;
        }

        #endregion

        #region Query

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return entity data</returns>
        public async Task<TEntity> GetAsync(IQuery query)
        {
            TEntity data = null;
            data = EntityStorage<TEntity>.GetCurrentEntityStorage().Merge(data, query);
            return await Task.FromResult(data).ConfigureAwait(false);
        }

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        public async Task<List<TEntity>> GetListAsync(IQuery query)
        {
            var datas = new List<TEntity>(0);
            datas = EntityStorage<TEntity>.GetCurrentEntityStorage().Merge(datas, query);
            return await Task.FromResult(datas).ConfigureAwait(false);
        }

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public async Task<PagingInfo<TEntity>> GetPagingAsync(IQuery query)
        {
            var paging = Pager.Empty<TEntity>();
            paging = EntityStorage<TEntity>.GetCurrentEntityStorage().MergePaging(paging, query);
            return await Task.FromResult(paging).ConfigureAwait(false);
        }

        /// <summary>
        /// Determines whether exists data
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Return whether exists data</returns>
        public async Task<bool> ExistsAsync(IQuery query)
        {
            var result = EntityStorage<TEntity>.GetCurrentEntityStorage().Exists(query);
            var isExist = result.IsExists;
            return await Task.FromResult(isExist).ConfigureAwait(false); ;
        }

        /// <summary>
        /// Count data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        public async Task<long> CountAsync(IQuery query)
        {
            long allCount = 0;
            var countResult = EntityStorage<TEntity>.GetCurrentEntityStorage().Count(query);
            allCount += countResult.Count;
            return await Task.FromResult(allCount).ConfigureAwait(false);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        public async Task<TValue> MaxAsync<TValue>(IQuery query)
        {
            var maxResult = EntityStorage<TEntity>.GetCurrentEntityStorage().Max<TValue>(query);
            dynamic resultVal = maxResult.Value;
            return await Task.FromResult(resultVal).ConfigureAwait(false);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        public async Task<TValue> MinAsync<TValue>(IQuery query)
        {
            var minResult = EntityStorage<TEntity>.GetCurrentEntityStorage().Min<TValue>(query);
            dynamic resultVal = minResult.Value;
            return await Task.FromResult(resultVal).ConfigureAwait(false);
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        public async Task<TValue> SumAsync<TValue>(IQuery query)
        {
            var sumResult = EntityStorage<TEntity>.GetCurrentEntityStorage().Sum<TValue>(query);
            dynamic resultVal = sumResult.Value;
            return await Task.FromResult(resultVal).ConfigureAwait(false);
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        public async Task<TValue> AvgAsync<TValue>(IQuery query)
        {
            dynamic sum = await SumAsync<TValue>(query).ConfigureAwait(false);
            var count = await CountAsync(query).ConfigureAwait(false);
            return sum / count;
        }

        /// <summary>
        /// Get warehouse entities
        /// </summary>
        /// <param name="identityValues">Entity identity values</param>
        /// <param name="includeRemove">Whether include remove data</param>
        /// <param name="onlyCompleteObject">Indicate whether only return complete object</param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetWarehouseEntitiesAsync(IEnumerable<string> identityValues, bool includeRemove = false, bool onlyCompleteObject = false)
        {
            return (await Task.FromResult(EntityStorage<TEntity>.GetCurrentEntityStorage()?.GetStorageEntities(identityValues, includeRemove, onlyCompleteObject) ?? Array.Empty<TEntity>()).ConfigureAwait(false)).ToList();
        }

        /// <summary>
        /// Get warehouse entity packages
        /// </summary>
        /// <param name="identityValues">Entity identity values</param>
        /// <param name="includeRemove">Whether include remove data</param>
        /// <param name="onlyCompleteObject">Indicate whether only return complete object</param>
        /// <returns></returns>
        public async Task<List<EntityPackage<TEntity>>> GetWarehouseEntityPackagesAsync(IEnumerable<string> identityValues, bool includeRemove = false, bool onlyCompleteObject = false)
        {
            return (await Task.FromResult(EntityStorage<TEntity>.GetCurrentEntityStorage()?.GetStorageEntityPackages(identityValues, includeRemove, onlyCompleteObject) ?? Array.Empty<EntityPackage<TEntity>>()).ConfigureAwait(false)).ToList();
        }

        #endregion

        #region Source

        /// <summary>
        /// Get data source
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Return data source</returns>
        public DataSource GetEntitySource(TEntity entity)
        {
            if (entity == null)
            {
                return DataSource.New;
            }
            return EntityStorage<TEntity>.GetCurrentEntityStorage(true).GetEntitySource(entity);
        }

        /// <summary>
        /// Modify entity data source
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="source">Data source</param>
        public void ModifyEntitySource(TEntity entity, DataSource source)
        {
            EntityStorage<TEntity>.GetCurrentEntityStorage(true).ModifyEntitySource(entity, source);
        }

        #endregion
    }
}
