﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Development.Query;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Domain.Aggregation;
using EZNEW.Development.Domain.Repository.Event;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.Entity;
using EZNEW.Development.UnitOfWork;
using EZNEW.DependencyInjection;
using EZNEW.Paging;

namespace EZNEW.Development.Domain.Repository
{
    /// <summary>
    /// Default relation repository
    /// </summary>
    /// <typeparam name="TFirstModel">The first model</typeparam>
    /// <typeparam name="TSecondModel">The second model</typeparam>
    /// <typeparam name="TEntity">Entity</typeparam>
    /// <typeparam name="TDataAccess">Data access</typeparam>
    public abstract class DefaultRelationRepository<TFirstModel, TSecondModel, TEntity, TDataAccess> 
        : BaseRelationRepository<TFirstModel, TSecondModel> 
        where TSecondModel : IAggregationRoot<TSecondModel> 
        where TFirstModel : IAggregationRoot<TFirstModel> 
        where TEntity : BaseEntity<TEntity>, new() 
        where TDataAccess : IDataAccess<TEntity>
    {
        readonly IRepositoryWarehouse<TEntity, TDataAccess> repositoryWarehouse = ContainerManager.Resolve<IRepositoryWarehouse<TEntity, TDataAccess>>();

        static readonly Type entityType = typeof(TEntity);

        static DefaultRelationRepository()
        {
            WarehouseManager.RegisterDefaultWarehouse<TEntity, TDataAccess>();
        }

        #region Save

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Save(IEnumerable<Tuple<TFirstModel, TSecondModel>> datas, ActivationOptions activationOptions = null)
        {
            var records = ExecuteSave(datas, activationOptions);
            if (records.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave(GetType(), datas, activationOptions);
            WorkManager.RegisterActivationRecord(records);
        }

        /// <summary>
        /// Save by first type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void SaveByFirst(IEnumerable<TFirstModel> datas, ActivationOptions activationOptions = null)
        {
            var saveRecords = ExecuteSaveByFirst(datas, activationOptions);
            if (saveRecords.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave(GetType(), datas, activationOptions);
            WorkManager.RegisterActivationRecord(saveRecords);
        }

        /// <summary>
        /// Save by second type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void SaveBySecond(IEnumerable<TSecondModel> datas, ActivationOptions activationOptions = null)
        {
            var saveRecords = ExecuteSaveBySecond(datas, activationOptions);
            if (saveRecords.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave(GetType(), datas, activationOptions);
            WorkManager.RegisterActivationRecord(saveRecords);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(IEnumerable<Tuple<TFirstModel, TSecondModel>> datas, ActivationOptions activationOptions = null)
        {
            var records = ExecuteRemove(datas, activationOptions);
            if (records.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishRemove(GetType(), datas, activationOptions);
            WorkManager.RegisterActivationRecord(records);
        }

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(IQuery query, ActivationOptions activationOptions = null)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Remove, AppendRemoveCondition);
            var record = ExecuteRemove(newQuery, activationOptions);
            if (record == null)
            {
                return;
            }
            RepositoryEventBus.PublishRemove<Tuple<TFirstModel, TSecondModel>>(GetType(), newQuery, activationOptions);
            WorkManager.RegisterActivationRecord(record);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Remove);
        }

        /// <summary>
        /// Remove by first datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void RemoveByFirst(IEnumerable<TFirstModel> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var query = CreateQueryByFirst(datas);
            Remove(query, activationOptions);
        }

        /// <summary>
        /// Remove by first
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void RemoveByFirst(IQuery query, ActivationOptions activationOptions = null)
        {
            var removeQuery = CreateQueryByFirst(query);
            Remove(removeQuery, activationOptions);
        }

        /// <summary>
        /// Remove by second datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void RemoveBySecond(IEnumerable<TSecondModel> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var query = CreateQueryBySecond(datas);
            Remove(query, activationOptions);
        }

        /// <summary>
        /// Remove by first
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void RemoveBySecond(IQuery query, ActivationOptions activationOptions = null)
        {
            var removeQuery = CreateQueryBySecond(query);
            Remove(removeQuery, activationOptions);
        }

        #endregion

        #region Query

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation data</returns>
        public sealed override Tuple<TFirstModel, TSecondModel> Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation data</returns>
        public sealed override async Task<Tuple<TFirstModel, TSecondModel>> GetAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var data = await ExecuteGetAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return data;
        }

        /// <summary>
        /// Get relation data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation datas</returns>
        public sealed override List<Tuple<TFirstModel, TSecondModel>> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// Get relation data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation datas</returns>
        public sealed override async Task<List<Tuple<TFirstModel, TSecondModel>>> GetListAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var dataList = await ExecuteGetListAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return dataList;
        }

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public sealed override PagingInfo<Tuple<TFirstModel, TSecondModel>> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// Get relation paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public sealed override async Task<PagingInfo<Tuple<TFirstModel, TSecondModel>>> GetPagingAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var paging = await ExecuteGetPagingAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return paging;
        }

        /// <summary>
        /// Get First by Second
        /// </summary>
        /// <param name="datas">Second datas</param>
        /// <returns>Return datas</returns>
        public sealed override List<TFirstModel> GetFirstListBySecond(IEnumerable<TSecondModel> datas)
        {
            return GetFirstListBySecondAsync(datas).Result;
        }

        /// <summary>
        /// Get First by Second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override async Task<List<TFirstModel>> GetFirstListBySecondAsync(IEnumerable<TSecondModel> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<TFirstModel>(0);
            }
            var query = CreateQueryBySecond(datas);
            var relationDatas = await GetListAsync(query).ConfigureAwait(false);
            return relationDatas?.Select(c => c.Item1).ToList() ?? new List<TFirstModel>(0);
        }

        /// <summary>
        /// Get Second by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override List<TSecondModel> GetSecondListByFirst(IEnumerable<TFirstModel> datas)
        {
            return GetSecondListByFirstAsync(datas).Result;
        }

        /// <summary>
        /// Get Second by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override async Task<List<TSecondModel>> GetSecondListByFirstAsync(IEnumerable<TFirstModel> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<TSecondModel>(0);
            }
            var query = CreateQueryByFirst(datas);
            var relationDatas = await GetListAsync(query).ConfigureAwait(false);
            return relationDatas?.Select(c => c.Item2).ToList() ?? new List<TSecondModel>(0);
        }

        #endregion

        #region Functions

        /// <summary>
        /// Execute save
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public virtual List<IActivationRecord> ExecuteSave(IEnumerable<Tuple<TFirstModel, TSecondModel>> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByRelationData(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(repositoryWarehouse.Save(entity, activationOptions));
            }
            return records;
        }

        /// <summary>
        /// Execute save by first datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        public virtual List<IActivationRecord> ExecuteSaveByFirst(IEnumerable<TFirstModel> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByFirst(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(repositoryWarehouse.Save(entity, activationOptions));
            }
            return records;
        }

        /// <summary>
        /// Execute save by second datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        public virtual List<IActivationRecord> ExecuteSaveBySecond(IEnumerable<TSecondModel> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityBySecond(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(repositoryWarehouse.Save(entity, activationOptions));
            }
            return records;
        }

        /// <summary>
        /// Execute remove
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        public virtual List<IActivationRecord> ExecuteRemove(IEnumerable<Tuple<TFirstModel, TSecondModel>> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByRelationData(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(repositoryWarehouse.Remove(entity, activationOptions ?? ActivationOptions.ForceExecuteActivation));
            }
            return records;
        }

        /// <summary>
        /// Execute remove
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation record</returns>
        public virtual IActivationRecord ExecuteRemove(IQuery query, ActivationOptions activationOptions = null)
        {
            return repositoryWarehouse.Remove(query, activationOptions);
        }

        /// <summary>
        /// Create data by first
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return data</returns>
        public virtual TEntity CreateEntityByFirst(TFirstModel data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create data by second
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return data</returns>
        public virtual TEntity CreateEntityBySecond(TSecondModel data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation data</returns>
        public virtual async Task<Tuple<TFirstModel, TSecondModel>> ExecuteGetAsync(IQuery query)
        {
            var entity = await repositoryWarehouse.GetAsync(query).ConfigureAwait(false);
            return CreateRelationDataByEntity(entity);
        }

        /// <summary>
        /// Get relation data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation datas</returns>
        public virtual async Task<List<Tuple<TFirstModel, TSecondModel>>> ExecuteGetListAsync(IQuery query)
        {
            var entityList = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            if (entityList.IsNullOrEmpty())
            {
                return new List<Tuple<TFirstModel, TSecondModel>>(0);
            }
            return entityList.Select(c => CreateRelationDataByEntity(c)).ToList();
        }

        /// <summary>
        /// Get relation data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation data paging</returns>
        public virtual async Task<PagingInfo<Tuple<TFirstModel, TSecondModel>>> ExecuteGetPagingAsync(IQuery query)
        {
            var entityPaging = await repositoryWarehouse.GetPagingAsync(query).ConfigureAwait(false);
            if (entityPaging?.Items.IsNullOrEmpty() ?? true)
            {
                return Pager.Empty<Tuple<TFirstModel, TSecondModel>>();
            }
            var datas = entityPaging.Items.Select(c => CreateRelationDataByEntity(c));
            return Pager.Create(entityPaging.Page, entityPaging.PageSize, entityPaging.TotalCount, datas);
        }

        /// <summary>
        /// Create query by first type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return query object</returns>
        public abstract IQuery CreateQueryByFirst(IEnumerable<TFirstModel> datas);

        /// <summary>
        /// Create query by first type datas query object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return query object</returns>
        public abstract IQuery CreateQueryByFirst(IQuery query);

        /// <summary>
        /// Create query by second type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return query object</returns>
        public abstract IQuery CreateQueryBySecond(IEnumerable<TSecondModel> datas);

        /// <summary>
        /// Create query by second type datas query object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return query object</returns>
        public abstract IQuery CreateQueryBySecond(IQuery query);

        /// <summary>
        /// Create entity by relation data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return data</returns>
        public abstract TEntity CreateEntityByRelationData(Tuple<TFirstModel, TSecondModel> data);

        /// <summary>
        /// Create relation data by entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Return relation data</returns>
        public abstract Tuple<TFirstModel, TSecondModel> CreateRelationDataByEntity(TEntity entity);

        #endregion

        #region Global condition

        #region Append Remove extra condition

        /// <summary>
        /// Append Remove condition
        /// </summary>
        /// <param name="originQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendRemoveCondition(IQuery originQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originQuery, QueryUsageScene.Remove);
        }

        #endregion

        #region Append query extra condition

        /// <summary>
        /// Append query condition
        /// </summary>
        /// <param name="originQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendQueryCondition(IQuery originQuery)
        {
            return QueryManager.SetGlobalCondition(entityType, originQuery, QueryUsageScene.Query);
        }

        #endregion

        #endregion
    }
}
