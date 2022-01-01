using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Entity;
using EZNEW.Development.UnitOfWork;
using EZNEW.DependencyInjection;
using EZNEW.Paging;
using EZNEW.Development.Domain.Repository.Warehouse.Storage;
using EZNEW.Exceptions;

namespace EZNEW.Development.Domain.Repository.Warehouse
{
    /// <summary>
    /// Defines default entity warehouse
    /// </summary>
    public class DefaultEntityWarehouse<TEntity, TDataAccess> : IEntityWarehouse<TEntity, TDataAccess> where TEntity : BaseEntity<TEntity>, new() where TDataAccess : IDataAccess<TEntity>
    {
        #region Fields

        /// <summary>
        /// Data access
        /// </summary>
        readonly TDataAccess dataAccess = ContainerManager.Resolve<TDataAccess>();

        #endregion

        #region Save

        /// <summary>
        /// Save entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        public IActivationRecord Save(IEnumerable<TEntity> entities, ActivationOptions activationOptions = null)
        {
            if (entities.IsNullOrEmpty())
            {
                return null;
            }
            var packageRecord = DefaultActivationRecord<TEntity, TDataAccess>.CreatePackageRecord();
            foreach (var data in entities)
            {
                var dataRecord = Save(data, activationOptions);
                packageRecord.AddFollowRecord(dataRecord);
            }
            return packageRecord;
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        public IActivationRecord Save(TEntity entity, ActivationOptions activationOptions = null)
        {
            EntityStorage<TEntity>.GetCurrentEntityStorage(true, dataAccess).Save(entity);
            var identityValue = entity.GetIdentityValue();
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateSavingRecord(identityValue, activationOptions);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Remove entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        public IActivationRecord Remove(IEnumerable<TEntity> entities, ActivationOptions activationOptions = null)
        {
            if (entities.IsNullOrEmpty())
            {
                return null;
            }
            var packageRecord = DefaultActivationRecord<TEntity, TDataAccess>.CreatePackageRecord();
            foreach (var data in entities)
            {
                var dataRecord = Remove(data, activationOptions);
                packageRecord.AddFollowRecord(dataRecord);
            }
            return packageRecord;
        }

        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Activation record</returns>
        public IActivationRecord Remove(TEntity entity, ActivationOptions activationOptions = null)
        {
            EntityStorage<TEntity>.GetCurrentEntityStorage(true, dataAccess).Remove(entity, activationOptions);
            var identityValue = entity.GetIdentityValue();
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateRemovingObjectRecord(identityValue, activationOptions);
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the activation record</returns>
        public IActivationRecord Remove(IQuery query, ActivationOptions activationOptions = null)
        {
            EntityStorage<TEntity>.GetCurrentEntityStorage(true, dataAccess).Remove(query);
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateRemovingByConditionRecord(query, activationOptions);
        }

        #endregion

        #region Modify

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modification">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <returns>Activation record</returns>
        public IActivationRecord Modify(IModification modification, IQuery query, ActivationOptions activationOptions = null)
        {
            EntityStorage<TEntity>.GetCurrentEntityStorage(true, dataAccess).Modify(modification, query);
            return DefaultActivationRecord<TEntity, TDataAccess>.CreateModificationRecord(modification, query, activationOptions);
        }

        #endregion

        #region Query

        /// <summary>
        /// Get entity
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return entity</returns>
        public async Task<TEntity> GetAsync(IQuery query)
        {
            var entity = await dataAccess.GetAsync(query).ConfigureAwait(false);
            var storage = EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess);
            if (storage != null)
            {
                return storage.Merge(entity, query);
            }
            return entity;
        }

        /// <summary>
        /// Query entity list
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Entity list</returns>
        public async Task<List<TEntity>> GetListAsync(IQuery query)
        {
            var entities = await dataAccess.GetListAsync(query).ConfigureAwait(false);
            var storage = EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess);
            if (storage != null)
            {
                return storage.Merge(entities, query);
            }
            return entities;
        }

        /// <summary>
        /// Query entity paging
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Entity paging</returns>
        public async Task<PagingInfo<TEntity>> GetPagingAsync(IQuery query)
        {
            var paging = await dataAccess.GetPagingAsync(query).ConfigureAwait(false);
            var storage = EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess);
            if (storage != null)
            {
                return storage.MergePaging(paging, query);
            }
            return paging;
        }

        /// <summary>
        /// Determines whether has entity
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Whether exists data</returns>
        public async Task<bool> ExistsAsync(IQuery query)
        {
            bool hasValue = false;
            var storage = EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess);
            if (storage != null)
            {
                var checkResult = storage.Exists(query);
                if (checkResult != null)
                {
                    hasValue = checkResult.IsExists;
                    query = checkResult.CheckQuery;
                }
            }
            return hasValue || await dataAccess.ExistsAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Count value
        /// </summary>
        /// <param name="query">Query model</param>
        /// <returns>Count value</returns>
        public async Task<long> CountAsync(IQuery query)
        {
            long countValue = 0;
            var storage = EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess);
            if (storage != null)
            {
                var countResult = storage.Count(query);
                if (countResult != null)
                {
                    countValue += countResult.Count;
                    query = countResult.CountQuery;
                }
            }
            return countValue += await dataAccess.CountAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query model</param>
        /// <returns>Max value</returns>
        public async Task<TValue> MaxAsync<TValue>(IQuery query)
        {
            dynamic maxValue = default(TValue);
            bool validValue = false;
            var storage = EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess);
            if (storage != null)
            {
                var maxResult = storage.Max<TValue>(query);
                if (maxResult != null)
                {
                    maxValue = maxResult.Value;
                    query = maxResult.AggregateQuery;
                    validValue = maxResult.ValidValue;
                }
            }
            dynamic storageMaxValue = await dataAccess.MaxAsync<TValue>(query).ConfigureAwait(false);
            return validValue ? (maxValue > storageMaxValue ? maxValue : storageMaxValue) : maxValue;
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query model</param>
        /// <returns>Min value</returns>
        public async Task<TValue> MinAsync<TValue>(IQuery query)
        {
            dynamic minValue = default(TValue);
            bool validValue = false;
            var storage = EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess);
            if (storage != null)
            {
                var minResult = storage.Min<TValue>(query);
                if (minResult != null)
                {
                    minValue = minResult.Value;
                    query = minResult.AggregateQuery;
                    validValue = minResult.ValidValue;
                }
            }
            dynamic storageMinValue = await dataAccess.MinAsync<TValue>(query).ConfigureAwait(false);
            return validValue ? (minValue < storageMinValue ? minValue : storageMinValue) : minValue;
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query model</param>
        /// <returns>Sum value</returns>
        public async Task<TValue> SumAsync<TValue>(IQuery query)
        {
            dynamic sumValue = default(TValue);
            bool validValue = false;
            var storage = EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess);
            if (storage != null)
            {
                var sumResult = storage.Sum<TValue>(query);
                if (sumResult != null)
                {
                    sumValue = sumResult.Value;
                    query = sumResult.AggregateQuery;
                    validValue = sumResult.ValidValue;
                }
            }
            dynamic storageSumValue = await dataAccess.SumAsync<TValue>(query).ConfigureAwait(false);
            return validValue ? sumValue + storageSumValue : storageSumValue;
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query model</param>
        /// <returns>Average value</returns>
        public async Task<TValue> AvgAsync<TValue>(IQuery query)
        {
            long warehouseCount = 0;
            var storage = EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess);
            if (storage != null)
            {
                warehouseCount = storage.Count(query)?.Count ?? 0;
            }
            if (warehouseCount > 0)
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

        /// <summary>
        /// Get warehouse entities
        /// </summary>
        /// <param name="identityValues">Entity identity values</param>
        /// <param name="includeRemove">Whether include remove data</param>
        /// <param name="onlyCompleteObject">Indicate whether only return complete object</param>
        /// <returns></returns>
        public async Task<List<TEntity>> GetWarehouseEntitiesAsync(IEnumerable<string> identityValues, bool includeRemove = false, bool onlyCompleteObject = false)
        {
            return (await Task.FromResult(EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess)?.GetStorageEntities(identityValues, includeRemove, onlyCompleteObject) ?? Array.Empty<TEntity>()).ConfigureAwait(false)).ToList();
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
            return (await Task.FromResult(EntityStorage<TEntity>.GetCurrentEntityStorage(false, dataAccess)?.GetStorageEntityPackages(identityValues, includeRemove, onlyCompleteObject) ?? Array.Empty<EntityPackage<TEntity>>()).ConfigureAwait(false)).ToList();
        }

        #endregion

        #region Source

        /// <summary>
        /// Get entity source
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity source</returns>
        public DataSource GetEntitySource(TEntity entity)
        {
            if (entity == null)
            {
                return DataSource.New;
            }

            return EntityStorage<TEntity>.GetCurrentEntityStorage(true, dataAccess)?.GetEntitySource(entity) ?? DataSource.New;
        }

        /// <summary>
        /// Modify entity source
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="source">Source</param>
        public void ModifyEntitySource(TEntity entity, DataSource source)
        {
            EntityStorage<TEntity>.GetCurrentEntityStorage(true, dataAccess).ModifyEntitySource(entity, source);
        }

        #endregion
    }
}
