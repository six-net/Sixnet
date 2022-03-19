using System;
using System.Collections.Generic;
using System.Linq;
using EZNEW.Exceptions;
using EZNEW.Development.Query;
using EZNEW.Development.Entity;
using EZNEW.Data.Modification;
using EZNEW.Development.UnitOfWork;
using EZNEW.Paging;
using EZNEW.Development.DataAccess;
using EZNEW.Model;
using EZNEW.Data;

namespace EZNEW.Development.Domain.Repository.Warehouse.Storage
{
    /// <summary>
    /// Defines entity storage
    /// </summary>
    public class EntityStorage<TEntity> where TEntity : BaseEntity<TEntity>, new()
    {
        #region Fields

        /// <summary>
        /// Remove query collection
        /// </summary>
        readonly List<IQuery> removeQueryCollection = new List<IQuery>();

        /// <summary>
        /// Modification collection
        /// </summary>
        readonly List<Tuple<IModification, IQuery>> modificationCollection = new List<Tuple<IModification, IQuery>>();

        #endregion

        #region Properties

        /// <summary>
        /// Entity collection.
        /// Key => Entity identity value.
        /// </summary>
        public Dictionary<string, EntityPackage<TEntity>> EntityCollection { get; private set; } = new Dictionary<string, EntityPackage<TEntity>>();

        /// <summary>
        /// Data access
        /// </summary>
        public IDataAccess<TEntity> DataAccess { get; set; }

        #endregion

        #region Init

        /// <summary>
        /// Init entity package from storage data
        /// </summary>
        /// <param name="storageEntities">Entities</param>
        /// <param name="query">Query object</param>
        internal void InitEntityPackageFromStorageData(IEnumerable<TEntity> storageEntities, IQuery query)
        {
            if (storageEntities.IsNullOrEmpty())
            {
                return;
            }
            foreach (var data in storageEntities)
            {
                InitEntityPackageFromStorageData(data, query);
            }
        }

        /// <summary>
        /// Init entity package from storage data
        /// </summary>
        /// <param name="storageEntity">Entity</param>
        internal EntityPackage<TEntity> InitEntityPackageFromStorageData(TEntity storageEntity, IQuery query)
        {
            if (storageEntity == null)
            {
                return null;
            }
            var identityValue = storageEntity.GetIdentityValue();
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                throw IdentityValueIsNullOrEmptyException();
            }
            if (EntityCollection.ContainsKey(identityValue))
            {
                return null;
            }
            EntityPackage<TEntity> dataPackage = EntityPackage<TEntity>.CreateStorageDataPackage(storageEntity, query);
            //remove
            bool isRemove = false;
            foreach (var removeQuery in removeQueryCollection)
            {
                var removeFunc = removeQuery?.GetValidationFunction<TEntity>();
                isRemove = removeFunc?.Invoke(storageEntity) ?? false;
                if (isRemove)
                {
                    dataPackage.Remove(true);
                    break;
                }
            }
            if (!isRemove)
            {
                // modify values
                foreach (var modifyItem in modificationCollection)
                {
                    var modifyFunc = modifyItem.Item2?.GetValidationFunction<TEntity>();
                    var allowModify = modifyFunc?.Invoke(storageEntity) ?? true;
                    if (allowModify)
                    {
                        dataPackage.Modify(modifyItem.Item1);
                    }
                }
            }
            SaveDataPackage(identityValue, dataPackage);
            return dataPackage;
        }

        /// <summary>
        /// Init entity package from new data
        /// </summary>
        /// <param name="newEntities">New entities</param>
        internal void InitEntityPackageFromNewData(IEnumerable<TEntity> newEntities)
        {
            if (newEntities == null || !newEntities.Any())
            {
                return;
            }
            foreach (var data in newEntities)
            {
                InitEntityPackageFromNewData(data);
            }
        }

        /// <summary>
        /// Init entity package from new data
        /// </summary>
        /// <param name="entity">Entity</param>
        internal EntityPackage<TEntity> InitEntityPackageFromNewData(TEntity entity)
        {
            if (entity == null)
            {
                return null;
            }
            var identityValue = entity.GetIdentityValue();
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                throw IdentityValueIsNullOrEmptyException();
            }
            if (EntityCollection.ContainsKey(identityValue))
            {
                return null;
            }
            EntityPackage<TEntity> dataPackage = EntityPackage<TEntity>.CreateNewDataPackage(entity);

            //remove
            bool isRemove = false;
            foreach (var removeQuery in removeQueryCollection)
            {
                var removeFunc = removeQuery?.GetValidationFunction<TEntity>();
                isRemove = removeFunc?.Invoke(entity) ?? false;
                if (isRemove)
                {
                    dataPackage.Remove(true);
                    break;
                }
            }

            SaveDataPackage(identityValue, dataPackage);
            return dataPackage;
        }

        #endregion

        #region Merge

        /// <summary>
        /// Merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <param name="entities">Datas</param>
        /// <param name="query">Query object</param>
        /// <returns>Return entity data list</returns>
        public List<TEntity> Merge(IEnumerable<TEntity> entities, IQuery query = null)
        {
            var mergeResultEntities = MergeEntityData(entities, query);
            if (query?.IsComplex ?? false)
            {
                return mergeResultEntities;
            }
            var allEntities = GetLatestEntities(query);
            if (query != null)
            {
                allEntities = query.SortData(allEntities);
                if (query.QuerySize > 0)
                {
                    allEntities = allEntities.Take(query.QuerySize);
                }
            }
            return allEntities.ToList();
        }

        /// <summary>
        /// Merge data from warehouse datas and save new value to warehouse
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return warehouse entity data</returns>
        public TEntity Merge(TEntity entity, IQuery query = null)
        {
            return Merge(new TEntity[1] { entity }, query)?.FirstOrDefault();
        }

        /// <summary>
        /// Merge paging
        /// </summary>
        /// <param name="originalPaging">Original paging</param>
        /// <param name="query">Query object</param>
        /// <returns>Return the newest data paging</returns>
        public PagingInfo<TEntity> MergePaging(PagingInfo<TEntity> originalPaging, IQuery query)
        {
            if (originalPaging == null)
            {
                originalPaging = Pager.Empty<TEntity>();
            }
            var mergeDatas = MergeEntityData(originalPaging.Items, query);
            if (!(query?.IsComplex ?? false))
            {
                
            }
            var newEntities = GetLatestEntities(query, true);
            if (!newEntities.IsNullOrEmpty() && query?.PagingInfo?.Page == originalPaging.PageCount)
            {
                mergeDatas.AddRange(newEntities);
            }
            originalPaging.Items = mergeDatas;
            return originalPaging;
        }

        /// <summary>
        /// Merge entity data
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <returns></returns>
        List<TEntity> MergeEntityData(IEnumerable<TEntity> entities, IQuery query)
        {
            List<TEntity> resultEntities = new List<TEntity>();
            if (!entities.IsNullOrEmpty())
            {
                foreach (var entity in entities)
                {
                    if (entity == null)
                    {
                        continue;
                    }
                    var dataPackage = GetDataPackage(entity);
                    var realData = entity;
                    if (dataPackage == null)
                    {
                        dataPackage = InitEntityPackageFromStorageData(entity, query);
                    }
                    else
                    {
                        realData = dataPackage.MergeStorageData(entity, query);
                    }
                    if (dataPackage.Operation == DataRecordOperation.Remove || realData == null)
                    {
                        continue;
                    }
                    resultEntities.Add(realData);
                }
            }
            return resultEntities;
        }

        #endregion

        #region Save

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public Result Save(TEntity entity)
        {
            if (entity == null)
            {
                return Result.FailedResult("Entity is null");
            }
            var dataPackage = GetDataPackage(entity);
            if (dataPackage == null)
            {
                dataPackage = InitEntityPackageFromNewData(entity);
            }
            return dataPackage?.Save(entity);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public Result Remove(TEntity entity, ActivationOptions activationOptions = null)
        {
            if (entity == null)
            {
                return Result.FailedResult("Entity is null");
            }
            var dataPackage = GetDataPackage(entity);
            if (dataPackage == null)
            {
                dataPackage = InitEntityPackageFromNewData(entity);
            }
            if (activationOptions?.ForceExecution ?? false)
            {
                dataPackage?.Remove(true);
            }
            else
            {
                dataPackage?.Remove();
            }
            return Result.SuccessResult();
        }

        /// <summary>
        /// Remove by query
        /// </summary>
        /// <param name="query">Query</param>
        public void Remove(IQuery query)
        {
            if (query == null)
            {
                return;
            }
            var validationFunc = query.GetValidationFunction<TEntity>();
            if (validationFunc == null)
            {
                return;
            }
            removeQueryCollection.Add(query);
            foreach (var dataPackage in EntityCollection.Values)
            {
                if (validationFunc(dataPackage.LatestData))
                {
                    dataPackage.Remove(true);
                }
            }
        }

        #endregion

        #region Modify 

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modifyExpression">Modify expression</param>
        /// <param name="query">Query object</param>
        public void Modify(IModification modifyExpression, IQuery query)
        {
            if (modifyExpression == null)
            {
                return;
            }
            modificationCollection.Add(new Tuple<IModification, IQuery>(modifyExpression, query));
            var queryFunc = query?.GetValidationFunction<TEntity>();
            foreach (var item in EntityCollection)
            {
                if (queryFunc?.Invoke(item.Value.LatestData) ?? true)
                {
                    item.Value?.Modify(modifyExpression);
                }
            }
        }

        #endregion

        #region Exists

        /// <summary>
        /// Determines if it contains a value
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the check result</returns>
        public WarehouseExistsResult Exists(IQuery query)
        {
            var dataPackages = GetDataPackages(query);
            WarehouseExistsResult result = new WarehouseExistsResult()
            {
                IsExists = dataPackages?.Any(c => c.Operation != DataRecordOperation.Remove) ?? false
            };
            if (!result.IsExists)
            {
                result.CheckQuery = AppendExcludeDataCondition(query, dataPackages);
            }
            return result;
        }

        #endregion

        #region Count

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the count result</returns>
        public WarehouseCountResult Count(IQuery query)
        {
            var dataPackages = GetDataPackages(query);
            long dataCount = 0;
            foreach (var dataPackage in dataPackages)
            {
                if (dataPackage.Operation == DataRecordOperation.Remove)
                {
                    continue;
                }
                dataCount++;
            }
            query = AppendExcludeDataCondition(query, dataPackages);
            return new WarehouseCountResult()
            {
                Count = dataCount,
                CountQuery = query
            };
        }

        #endregion

        #region Max

        /// <summary>
        /// Compute max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the compute result</returns>
        public WarehouseAggregateResult<TValue> Max<TValue>(IQuery query)
        {
            if (query == null || query.QueryFields.IsNullOrEmpty())
            {
                return new WarehouseAggregateResult<TValue>()
                {
                    Value = default,
                    AggregateQuery = query
                };
            }
            var dataPackages = GetDataPackages(query);
            string propertyName = query.QueryFields.ElementAt(0);
            var nowDataPackages = dataPackages.Where(c => c.Operation != DataRecordOperation.Remove);
            TValue value = default;
            bool validValue = false;
            if (!nowDataPackages.IsNullOrEmpty())
            {
                value = nowDataPackages.Max(c => c.LatestData.GetValue<TValue>(propertyName));
                validValue = true;
            }
            var result = new WarehouseAggregateResult<TValue>()
            {
                Value = value,
                ValidValue = validValue
            };
            result.AggregateQuery = AppendExcludeDataCondition(query, dataPackages);
            return result;
        }

        #endregion

        #region Min

        /// <summary>
        /// Compute min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the compute result</returns>
        public WarehouseAggregateResult<TValue> Min<TValue>(IQuery query)
        {
            if (query == null || query.QueryFields.IsNullOrEmpty())
            {
                return new WarehouseAggregateResult<TValue>()
                {
                    Value = default,
                    AggregateQuery = query
                };
            }
            var dataPackages = GetDataPackages(query);
            string propertyName = query.QueryFields.ElementAt(0);
            var nowDataPackages = dataPackages.Where(c => c.Operation != DataRecordOperation.Remove);
            TValue value = default;
            bool validValue = false;
            if (!nowDataPackages.IsNullOrEmpty())
            {
                value = nowDataPackages.Min(c => c.LatestData.GetValue<TValue>(propertyName));
                validValue = true;
            }
            var result = new WarehouseAggregateResult<TValue>()
            {
                Value = value,
                ValidValue = validValue
            };
            result.AggregateQuery = AppendExcludeDataCondition(query, dataPackages);
            return result;
        }

        #endregion

        #region Sum

        /// <summary>
        /// Compute sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return compute result</returns>
        public WarehouseAggregateResult<TValue> Sum<TValue>(IQuery query)
        {
            if (query == null || query.QueryFields.IsNullOrEmpty())
            {
                return new WarehouseAggregateResult<TValue>()
                {
                    Value = default,
                    AggregateQuery = query
                };
            }
            var dataPackages = GetDataPackages(query);
            string propertyName = query.QueryFields.ElementAt(0);
            var nowDataPackages = dataPackages.Where(c => c.Operation != DataRecordOperation.Remove).ToList();
            dynamic value = default(TValue);
            bool validValue = false;
            if (!nowDataPackages.IsNullOrEmpty())
            {
                nowDataPackages.ForEach(c =>
                {
                    value += c.LatestData.GetValue<TValue>(propertyName);
                });
                validValue = true;
            }
            var result = new WarehouseAggregateResult<TValue>()
            {
                Value = value,
                ValidValue = validValue
            };
            result.AggregateQuery = AppendExcludeDataCondition(query, dataPackages);
            return result;
        }

        #endregion

        #region Get storage data

        /// <summary>
        /// Get data package
        /// </summary>
        /// <param name="identityValue">Identity value</param>
        /// <returns>Return the entity package</returns>
        public EntityPackage<TEntity> GetDataPackage(string identityValue)
        {
            if (string.IsNullOrWhiteSpace(identityValue))
            {
                return null;
            }
            EntityCollection.TryGetValue(identityValue, out EntityPackage<TEntity> dataPackage);
            return dataPackage;
        }

        /// <summary>
        /// Get storage entities
        /// </summary>
        /// <param name="identityValues">Entity identity values</param>
        /// <param name="includeRemove">Whether include remove data</param>
        ///  <param name="onlyCompleteObject">Indicate whether only return complete object</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetStorageEntities(IEnumerable<string> identityValues, bool includeRemove = false, bool onlyCompleteObject = false)
        {
            return GetStorageEntityPackages(identityValues, includeRemove, onlyCompleteObject).Select(c => c.LatestData);
        }

        /// <summary>
        /// Get storage entity packages
        /// </summary>
        /// <param name="identityValues">Entity identity values</param>
        /// <param name="includeRemove">Whether include remove data</param>
        /// <param name="onlyCompleteObject">Indicate whether only return complete object</param>
        /// <returns></returns>
        public IEnumerable<EntityPackage<TEntity>> GetStorageEntityPackages(IEnumerable<string> identityValues, bool includeRemove = false, bool onlyCompleteObject = false)
        {
            if (identityValues.IsNullOrEmpty())
            {
                return Array.Empty<EntityPackage<TEntity>>();
            }
            return EntityCollection.Where(c => identityValues.Contains(c.Key) && (includeRemove || c.Value.Operation != DataRecordOperation.Remove) && (!onlyCompleteObject || c.Value.CompleteEntity)).Select(c => c.Value);
        }

        #endregion

        #region Source

        /// <summary>
        /// Get entity source
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Return entity source</returns>
        public DataSource GetEntitySource(TEntity entity)
        {
            if (entity == null)
            {
                return DataSource.New;
            }
            var dataPackage = GetDataPackage(entity.GetIdentityValue());
            return dataPackage?.Source ?? DataSource.New;
        }

        /// <summary>
        /// Modify entity source
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="source">Source</param>
        public void ModifyEntitySource(TEntity entity, DataSource source)
        {
            if (entity == null)
            {
                return;
            }
            var dataPackage = GetDataPackage(entity.GetIdentityValue());
            if (dataPackage == null)
            {
                dataPackage = InitEntityPackageFromNewData(entity);
            }
            dataPackage.ChangeDataSource(source);
        }

        #endregion

        #region Util

        /// <summary>
        /// Save data package
        /// </summary>
        /// <param name="identityValue">Identity value</param>
        /// <param name="dataPackage">Data package</param>
        void SaveDataPackage(string identityValue, EntityPackage<TEntity> dataPackage)
        {
            if (string.IsNullOrWhiteSpace(identityValue) || dataPackage == null)
            {
                return;
            }
            EntityCollection[identityValue] = dataPackage;
        }

        /// <summary>
        /// Get all latest entities
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAllLatestEntities(bool onlyNew = false)
        {
            return EntityCollection?.Where(c => c.Value.Operation != DataRecordOperation.Remove && (onlyNew ? c.Value.Source == DataSource.New : true)).Select(c => c.Value.LatestData);
        }

        /// <summary>
        /// Get latest entities
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return entity data list</returns>
        IEnumerable<TEntity> GetLatestEntities(IQuery query, bool onlyNew = false)
        {
            var allLatestEntities = GetAllLatestEntities(onlyNew);
            if (query != null)
            {
                var func = query.GetValidationFunction<TEntity>();
                if (func != null)
                {
                    allLatestEntities = allLatestEntities.Where(func);
                }
            }
            return allLatestEntities;
        }

        /// <summary>
        /// Get data package
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return the data package</returns>
        EntityPackage<TEntity> GetDataPackage(TEntity data)
        {
            if (data == null)
            {
                return null;
            }
            return GetDataPackage(data.GetIdentityValue());
        }

        /// <summary>
        /// Get data package
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>Return the entity data packages</returns>
        IEnumerable<EntityPackage<TEntity>> GetDataPackages(IQuery query)
        {
            IEnumerable<EntityPackage<TEntity>> dataPackages = EntityCollection?.Values;
            if (dataPackages.IsNullOrEmpty())
            {
                return Array.Empty<EntityPackage<TEntity>>();
            }
            if (query != null)
            {
                var func = query.GetValidationFunction<TEntity>();
                dataPackages = dataPackages.Where(c => func(c.LatestData));
            }
            return dataPackages;
        }

        /// <summary>
        ///  Get identity value is null or empty exception
        /// </summary>
        /// <returns></returns>
        EZNEWException IdentityValueIsNullOrEmptyException()
        {
            return new EZNEWException(string.Format("{0} identity value is null or empty", typeof(TEntity)));
        }

        /// <summary>
        /// Append exclude data condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <param name="dataPackages">Data packages</param>
        /// <returns>Return the newest IQuery object</returns>
        IQuery AppendExcludeDataCondition(IQuery originalQuery, IEnumerable<EntityPackage<TEntity>> dataPackages)
        {
            var sourceDatas = dataPackages.Where(c => c.Source == DataSource.Storage);
            if (!sourceDatas.IsNullOrEmpty())
            {
                originalQuery = QueryManager.AppendEntityIdentityCondition(sourceDatas.Select(c => c.OriginalData), originalQuery, true);
            }
            if (!removeQueryCollection.IsNullOrEmpty())
            {
                foreach (var removeQuery in removeQueryCollection)
                {
                    originalQuery = originalQuery.Except(removeQuery);
                }
            }
            return originalQuery;
        }

        /// <summary>
        /// Gets current entity storage
        /// </summary>
        /// <param name="required">Whether must get value</param>
        /// <param name="dataAccess">Data access</param>
        /// <returns></returns>
        public static EntityStorage<TEntity> GetCurrentEntityStorage(bool required = false, IDataAccess<TEntity> dataAccess = null)
        {
            var storage = WorkManager.Current?.GetEntityStorage<TEntity>();
            if (storage != null)
            {
                storage.DataAccess = dataAccess;
            }
            else if (required)
            {
                throw new EZNEWException("didn't init unit work");
            }
            return storage;
        }

        #endregion
    }
}
