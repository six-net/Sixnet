using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Development.Query;
using EZNEW.Paging;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Entity;
using EZNEW.DependencyInjection;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.UnitOfWork;
using EZNEW.Data.Modification;
using EZNEW.Mapper;

namespace EZNEW.Development.Domain.Repository
{
    /// <summary>
    /// Defines default repository
    /// </summary>
    /// <typeparam name="TModel">Model object</typeparam>
    /// <typeparam name="TEntity">Entity</typeparam>
    /// <typeparam name="TDataAccess">Data access</typeparam>
    public class DefaultRepository<TModel, TEntity, TDataAccess>
        : DefaultRootRepository<TModel>
        where TModel : class, IModel<TModel>
        where TEntity : BaseEntity<TEntity>, new()
        where TDataAccess : IDataAccess<TEntity>
    {
        #region Fields

        /// <summary>
        /// Entity warehouse
        /// </summary>
        protected IEntityWarehouse<TEntity, TDataAccess> entityWarehouse = ContainerManager.Resolve<IEntityWarehouse<TEntity, TDataAccess>>();

        /// <summary>
        /// Entity type
        /// </summary>
        static readonly Type entityType = typeof(TEntity);

        #endregion

        #region Impl

        /// <summary>
        /// Get data source
        /// </summary>
        /// <param name="object">Model object</param>
        /// <returns>Return the object data source</returns>
        public sealed override DataSource GetDataSource(IModel @object)
        {
            if (@object == null)
            {
                return DataSource.New;
            }
            return entityWarehouse.GetEntitySource(@object.MapTo<TEntity>());
        }

        /// <summary>
        /// Modify object data source
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="source">Source</param>
        public sealed override void ModifyDataSource(IModel @object, DataSource source)
        {
            if (@object == null)
            {
                return;
            }
            entityWarehouse.ModifyEntitySource(@object.MapTo<TEntity>(), source);
        }

        #endregion

        #region Function

        /// <summary>
        /// Execute saving
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected override IActivationRecord ExecuteSaving(TModel @object, ActivationOptions activationOptions = null)
        {
            var entity = @object?.MapTo<TEntity>();
            return ExecuteSavingEntity(entity, activationOptions);
        }

        /// <summary>
        /// Execute removing
        /// </summary>
        /// <param name="object">Model object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected override IActivationRecord ExecuteRemoving(TModel @object, ActivationOptions activationOptions = null)
        {
            if (@object == null)
            {
                return null;
            }
            var entity = @object.MapTo<TEntity>();
            return ExecuteRemovingEntity(entity, activationOptions);
        }

        /// <summary>
        /// Execute removing
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return actionvaton record</returns>
        protected override IActivationRecord ExecuteRemoving(IQuery query, ActivationOptions activationOptions = null)
        {
            return entityWarehouse.Remove(query, activationOptions);
        }

        /// <summary>
        /// Get object list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return object list</returns>
        protected override async Task<List<TModel>> GetObjectListAsync(IQuery query)
        {
            var entityDataList = await entityWarehouse.GetListAsync(query).ConfigureAwait(false);
            if (entityDataList.IsNullOrEmpty())
            {
                return new List<TModel>(0);
            }
            var datas = entityDataList.Select(c => c.MapTo<TModel>());
            return datas.ToList();
        }

        /// <summary>
        /// Get object paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return object paging</returns>
        protected override async Task<PagingInfo<TModel>> GetObjectPagingAsync(IQuery query)
        {
            var entityPaging = await entityWarehouse.GetPagingAsync(query).ConfigureAwait(false);
            var dataPaging = entityPaging.ConvertTo<TModel>();
            return dataPaging;
        }

        /// <summary>
        /// Get list by current object
        /// </summary>
        /// <param name="currentObjects">Current objects</param>
        /// <param name="includeRemove">Indicates whether include remove data</param>
        /// <param name="onlyCompleteObject">Indicate whether only return complete object</param>
        /// <returns>Return object list</returns>
        protected override async Task<List<TModel>> GetObjectListByCurrentAsync(IEnumerable<TModel> currentObjects, bool includeRemove = false, bool onlyCompleteObject = false)
        {
            if (currentObjects.IsNullOrEmpty())
            {
                return new List<TModel>(0);
            }
            var entities = currentObjects.Select(c => c.MapTo<TEntity>());
            var warehouseEntityPackages = await entityWarehouse.GetWarehouseEntityPackagesAsync(entities.Select(c => c.GetIdentityValue()), includeRemove, onlyCompleteObject).ConfigureAwait(false);
            if (!warehouseEntityPackages.IsNullOrEmpty())
            {
                entities = entities.Except(warehouseEntityPackages.Select(c => c.LatestData), EntityCompare<TEntity>.Default);
            }
            IEnumerable<TModel> objects = null;
            if (!entities.IsNullOrEmpty())
            {
                var query = QueryManager.AppendEntityIdentityCondition(entities);
                objects = await GetListAsync(query).ConfigureAwait(false);
            }
            objects ??= Array.Empty<TModel>();
            if (!warehouseEntityPackages.IsNullOrEmpty())
            {
                objects = objects.Union(warehouseEntityPackages.Where(c => c.Operation != DataRecordOperation.Remove).Select(c => c.LatestData.MapTo<TModel>()));
            }
            return objects.ToList();
        }

        /// <summary>
        /// Indicates whether has data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether has data</returns>
        protected override async Task<bool> ExistsDataAsync(IQuery query)
        {
            return await entityWarehouse.ExistsAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        protected override async Task<long> CountValueAsync(IQuery query)
        {
            return await entityWarehouse.CountAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        protected override async Task<TValue> MaxValueAsync<TValue>(IQuery query)
        {
            return await entityWarehouse.MaxAsync<TValue>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        protected override async Task<TValue> MinValueAsync<TValue>(IQuery query)
        {
            return await entityWarehouse.MinAsync<TValue>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        protected override async Task<TValue> SumValueAsync<TValue>(IQuery query)
        {
            return await entityWarehouse.SumAsync<TValue>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        protected override async Task<TValue> AvgValueAsync<TValue>(IQuery query)
        {
            return await entityWarehouse.AvgAsync<TValue>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute modification
        /// </summary>
        /// <param name="modificationExpression">Modification expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected override IActivationRecord ExecuteModification(IModification modificationExpression, IQuery query, ActivationOptions activationOptions = null)
        {
            return entityWarehouse.Modify(modificationExpression, query, activationOptions);
        }

        /// <summary>
        /// Get query by relation objects
        /// </summary>
        /// <typeparam name="TRelationModel">Relation object type</typeparam>
        /// <param name="relationModels">Relation objects</param>
        /// <returns>Return a IQuery object</returns>
        protected override IQuery GetQueryByRelationData<TRelationModel>(IEnumerable<TRelationModel> relationModels)
        {
            if (relationModels.IsNullOrEmpty())
            {
                return null;
            }
            var relationEntityType = ModelManager.GetModelRelationEntityType(typeof(TRelationModel));
            return GetQueryByRelationDataQuery(QueryManager.AppendEntityIdentityCore(relationEntityType, relationModels.Select(c => ObjectMapper.MapTo(relationEntityType, c))));
        }

        /// <summary>
        /// Get query by relation object query
        /// </summary>
        /// <param name="relationModelQuery">Relation object query</param>
        /// <returns>Return a IQuery object</returns>
        protected override IQuery GetQueryByRelationDataQuery(IQuery relationModelQuery)
        {
            if (relationModelQuery == null)
            {
                return null;
            }
            return QueryManager.Create<TEntity>().EqualInnerJoin(relationModelQuery);
        }

        /// <summary>
        /// Execute saving entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected virtual IActivationRecord ExecuteSavingEntity(IEnumerable<TEntity> entities, ActivationOptions activationOptions = null)
        {
            return entityWarehouse.Save(entities, activationOptions);
        }

        /// <summary>
        /// Execute saving entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected virtual IActivationRecord ExecuteSavingEntity(TEntity entity, ActivationOptions activationOptions = null)
        {
            return entityWarehouse.Save(entity, activationOptions);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns></returns>
        protected virtual void SaveEntity(IEnumerable<TEntity> entities, ActivationOptions activationOptions = null)
        {
            var record = ExecuteSavingEntity(entities, activationOptions);
            if (record != null)
            {
                WorkManager.RegisterActivationRecord(record);
            }
        }

        /// <summary>
        /// Execute remove entitys
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected virtual IActivationRecord ExecuteRemovingEntity(IEnumerable<TEntity> entities, ActivationOptions activationOptions = null)
        {
            return entityWarehouse.Remove(entities, activationOptions);
        }

        /// <summary>
        /// Execute removing entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected virtual IActivationRecord ExecuteRemovingEntity(TEntity entity, ActivationOptions activationOptions = null)
        {
            return entityWarehouse.Remove(entity, activationOptions);
        }

        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns></returns>
        protected virtual void RemoveEntity(IEnumerable<TEntity> entities, ActivationOptions activationOptions = null)
        {
            var record = ExecuteRemovingEntity(entities, activationOptions);
            if (record != null)
            {
                WorkManager.RegisterActivationRecord(record);
            }
        }

        #endregion

        #region Global condition

        #region Append Removing extra condition

        /// <summary>
        /// Append removing condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendRemovingCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Remove);
        }

        #endregion

        #region Append modification extra condition

        /// <summary>
        /// Append modification condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendModificationCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Modify);
        }

        #endregion

        #region Append query extra condition

        /// <summary>
        /// Append querying condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendQueryingCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Query);
        }

        #endregion

        #region Append exists extra condition

        /// <summary>
        /// Append exists condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendExistsCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Exists);
        }

        #endregion

        #region Append count extra condition

        /// <summary>
        /// Append count condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendCountCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Count);
        }

        #endregion

        #region Append max extra condition

        /// <summary>
        /// Append max condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendMaxCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Max);
        }

        #endregion

        #region Append min extra condition

        /// <summary>
        /// Append min condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendMinCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Min);
        }

        #endregion

        #region Append sum extra condition

        /// <summary>
        /// Append sum condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendSumCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Sum);
        }

        #endregion

        #region Append avg extra condition

        /// <summary>
        /// Append avg condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendAvgCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Avg);
        }

        #endregion

        #endregion
    }
}
