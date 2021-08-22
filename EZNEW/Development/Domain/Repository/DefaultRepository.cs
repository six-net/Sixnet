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
using EZNEW.Development.Command.Modification;

namespace EZNEW.Development.Domain.Repository
{
    /// <summary>
    /// Defines default repository
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    /// <typeparam name="TEntity">Entity</typeparam>
    /// <typeparam name="TDataAccess">Data access</typeparam>
    public class DefaultRepository<TModel, TEntity, TDataAccess>
        : DefaultRootRepository<TModel>
        where TModel : class, IModel<TModel>
        where TEntity : BaseEntity<TEntity>, new()
        where TDataAccess : IDataAccess<TEntity>
    {
        protected IRepositoryWarehouse<TEntity, TDataAccess> repositoryWarehouse = ContainerManager.Resolve<IRepositoryWarehouse<TEntity, TDataAccess>>();

        static readonly Type entityType = typeof(TEntity);

        static DefaultRepository()
        {
            WarehouseManager.RegisterDefaultWarehouse<TEntity, TDataAccess>();
        }

        #region Impl

        /// <summary>
        /// Get life source
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return the data life source</returns>
        public sealed override DataLifeSource GetLifeSource(IModel data)
        {
            if (data == null)
            {
                return DataLifeSource.New;
            }
            return repositoryWarehouse.GetLifeSource(data.MapTo<TEntity>());
        }

        /// <summary>
        /// Modify life source
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="lifeSource">Life source</param>
        public sealed override void ModifyLifeSource(IModel data, DataLifeSource lifeSource)
        {
            if (data == null)
            {
                return;
            }
            repositoryWarehouse.ModifyLifeSource(data.MapTo<TEntity>(), lifeSource);
        }

        #endregion

        #region Function

        /// <summary>
        /// Execute save
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected override IActivationRecord ExecuteSave(TModel data, ActivationOptions activationOptions = null)
        {
            var entity = data?.MapTo<TEntity>();
            return ExecuteSaveEntity(entity, activationOptions);
        }

        /// <summary>
        /// Execute Remove
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected override IActivationRecord ExecuteRemove(TModel data, ActivationOptions activationOptions = null)
        {
            if (data == null)
            {
                return null;
            }
            var entity = data.MapTo<TEntity>();
            return ExecuteRemoveEntity(entity, activationOptions);
        }

        /// <summary>
        /// Execute Remove
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return actionvaton record</returns>
        protected override IActivationRecord ExecuteRemove(IQuery query, ActivationOptions activationOptions = null)
        {
            return repositoryWarehouse.Remove(query, activationOptions);
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        protected override async Task<TModel> GetDataAsync(IQuery query)
        {
            var entityData = await repositoryWarehouse.GetAsync(query).ConfigureAwait(false);
            TModel data = default;
            if (entityData != null)
            {
                data = entityData.MapTo<TModel>();
            }
            return data;
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        protected override async Task<List<TModel>> GetDataListAsync(IQuery query)
        {
            var entityDataList = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            if (entityDataList.IsNullOrEmpty())
            {
                return new List<TModel>(0);
            }
            var datas = entityDataList.Select(c => c.MapTo<TModel>());
            return datas.ToList();
        }

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        protected override async Task<PagingInfo<TModel>> GetDataPagingAsync(IQuery query)
        {
            var entityPaging = await repositoryWarehouse.GetPagingAsync(query).ConfigureAwait(false);
            var dataPaging = entityPaging.ConvertTo<TModel>();
            return dataPaging;
        }

        /// <summary>
        /// Get data by current data
        /// </summary>
        /// <param name="currentData">Current data</param>
        /// <returns>Return data</returns>
        protected override async Task<TModel> GetDataByCurrentDataAsync(TModel currentData)
        {
            if (currentData == null)
            {
                return default;
            }
            var entity = currentData.MapTo<TEntity>();
            var query = QueryManager.AppendEntityIdentityCondition(new TEntity[] { entity });
            return await GetAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Check data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        protected override async Task<bool> IsExistAsync(IQuery query)
        {
            return await repositoryWarehouse.ExistAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        protected override async Task<long> CountValueAsync(IQuery query)
        {
            return await repositoryWarehouse.CountAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        protected override async Task<TValue> MaxValueAsync<TValue>(IQuery query)
        {
            return await repositoryWarehouse.MaxAsync<TValue>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        protected override async Task<TValue> MinValueAsync<TValue>(IQuery query)
        {
            return await repositoryWarehouse.MinAsync<TValue>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        protected override async Task<TValue> SumValueAsync<TValue>(IQuery query)
        {
            return await repositoryWarehouse.SumAsync<TValue>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        protected override async Task<TValue> AvgValueAsync<TValue>(IQuery query)
        {
            return await repositoryWarehouse.AvgAsync<TValue>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute Modify
        /// </summary>
        /// <param name="expression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected override IActivationRecord ExecuteModify(IModification expression, IQuery query, ActivationOptions activationOptions = null)
        {
            return repositoryWarehouse.Modify(expression, query, activationOptions);
        }

        /// <summary>
        /// Execute save entity
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected virtual IActivationRecord ExecuteSaveEntity(IEnumerable<TEntity> datas, ActivationOptions activationOptions = null)
        {
            return repositoryWarehouse.Save(datas, activationOptions);
        }

        /// <summary>
        /// Execute save entity
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected virtual IActivationRecord ExecuteSaveEntity(TEntity data, ActivationOptions activationOptions = null)
        {
            return repositoryWarehouse.Save(data, activationOptions);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns></returns>
        protected virtual void SaveEntity(IEnumerable<TEntity> datas, ActivationOptions activationOptions = null)
        {
            var record = ExecuteSaveEntity(datas, activationOptions);
            if (record != null)
            {
                WorkManager.RegisterActivationRecord(record);
            }
        }

        /// <summary>
        /// Execute remove entitys
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected virtual IActivationRecord ExecuteRemoveEntity(IEnumerable<TEntity> datas, ActivationOptions activationOptions = null)
        {
            return repositoryWarehouse.Remove(datas, activationOptions);
        }

        /// <summary>
        /// Execute remove entity
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        protected virtual IActivationRecord ExecuteRemoveEntity(TEntity data, ActivationOptions activationOptions = null)
        {
            return repositoryWarehouse.Remove(data, activationOptions);
        }

        /// <summary>
        /// Remove entitys
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns></returns>
        protected virtual void RemoveEntity(IEnumerable<TEntity> datas, ActivationOptions activationOptions = null)
        {
            var record = ExecuteRemoveEntity(datas, activationOptions);
            if (record != null)
            {
                WorkManager.RegisterActivationRecord(record);
            }
        }

        #endregion

        #region Global condition

        #region Append Remove extra condition

        /// <summary>
        /// Append Remove condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendRemoveCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Remove);
        }

        #endregion

        #region Append Modify extra condition

        /// <summary>
        /// Append Modify condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendModifyCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Modify);
        }

        #endregion

        #region Append query extra condition

        /// <summary>
        /// Append query condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendQueryCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Query);
        }

        #endregion

        #region Append exist extra condition

        /// <summary>
        /// Append exist condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendExistCondition(IQuery originalQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originalQuery, QueryUsageScene.Exist);
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
