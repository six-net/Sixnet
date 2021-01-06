using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Domain.Repository.Event;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.DependencyInjection;
using EZNEW.Paging;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Default three relation repository
    /// </summary>
    /// <typeparam name="TFirstModel">The first model</typeparam>
    /// <typeparam name="TSecondModel">The second model</typeparam>
    /// <typeparam name="TThirdModel">The third model</typeparam>
    /// <typeparam name="TEntity">Entity</typeparam>
    /// <typeparam name="TDataAccess">DataAccess</typeparam>
    public abstract class DefaultThreeRelationRepository<TFirstModel, TSecondModel, TThirdModel, TEntity, TDataAccess> : BaseThreeRelationRepository<TFirstModel, TSecondModel, TThirdModel> where TSecondModel : IAggregationRoot<TSecondModel> where TFirstModel : IAggregationRoot<TFirstModel> where TThirdModel : IAggregationRoot<TThirdModel> where TEntity : BaseEntity<TEntity>, new() where TDataAccess : IDataAccess<TEntity>
    {
        readonly IRepositoryWarehouse<TEntity, TDataAccess> repositoryWarehouse = ContainerManager.Resolve<IRepositoryWarehouse<TEntity, TDataAccess>>();
        
        static readonly Type entityType = typeof(TEntity);

        static DefaultThreeRelationRepository()
        {
            WarehouseManager.RegisterDefaultWarehouse<TEntity, TDataAccess>();
        }

        #region Save

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Save(IEnumerable<Tuple<TFirstModel, TSecondModel, TThirdModel>> datas, ActivationOptions activationOptions = null)
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

        /// <summary>
        /// Save by third datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void SaveByThird(IEnumerable<TThirdModel> datas, ActivationOptions activationOptions = null)
        {
            var saveRecords = ExecuteSaveByThird(datas, activationOptions);
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
        /// Save async
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void Remove(IEnumerable<Tuple<TFirstModel, TSecondModel, TThirdModel>> datas, ActivationOptions activationOptions = null)
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
            RepositoryEventBus.PublishRemove<Tuple<TFirstModel, TSecondModel, TThirdModel>>(GetType(), newQuery, activationOptions);
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

        /// <summary>
        /// Remove by third datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void RemoveByThird(IEnumerable<TThirdModel> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var query = CreateQueryByThird(datas);
            Remove(query, activationOptions);
        }

        /// <summary>
        /// Remove by third
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public sealed override void RemoveByThird(IQuery query, ActivationOptions activationOptions = null)
        {
            var removeQuery = CreateQueryByThird(query);
            Remove(removeQuery, activationOptions);
        }

        #endregion

        #region Query

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation data</returns>
        public sealed override Tuple<TFirstModel, TSecondModel, TThirdModel> Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation data</returns>
        public sealed override async Task<Tuple<TFirstModel, TSecondModel, TThirdModel>> GetAsync(IQuery query)
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
        public sealed override List<Tuple<TFirstModel, TSecondModel, TThirdModel>> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// Get relation data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation datas</returns>
        public sealed override async Task<List<Tuple<TFirstModel, TSecondModel, TThirdModel>>> GetListAsync(IQuery query)
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
        public sealed override IPaging<Tuple<TFirstModel, TSecondModel, TThirdModel>> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// Get relation paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public sealed override async Task<IPaging<Tuple<TFirstModel, TSecondModel, TThirdModel>>> GetPagingAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var paging = await ExecuteGetPagingAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return paging;
        }

        /// <summary>
        /// Get First by Second
        /// </summary>
        /// <param name="datas">Datas</param>
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
        /// Get first by third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override List<TFirstModel> GetFirstListByThird(IEnumerable<TThirdModel> datas)
        {
            return GetFirstListByThirdAsync(datas).Result;
        }

        /// <summary>
        /// Get first by third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override async Task<List<TFirstModel>> GetFirstListByThirdAsync(IEnumerable<TThirdModel> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<TFirstModel>(0);
            }
            var query = CreateQueryByThird(datas);
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

        /// <summary>
        /// Get Second by Third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override List<TSecondModel> GetSecondListByThird(IEnumerable<TThirdModel> datas)
        {
            return GetSecondListByThirdAsync(datas).Result;
        }

        /// <summary>
        /// Get Second by Third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override async Task<List<TSecondModel>> GetSecondListByThirdAsync(IEnumerable<TThirdModel> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<TSecondModel>(0);
            }
            var query = CreateQueryByThird(datas);
            var relationDatas = await GetListAsync(query).ConfigureAwait(false);
            return relationDatas?.Select(c => c.Item2).ToList() ?? new List<TSecondModel>(0);
        }

        /// <summary>
        /// Get Third by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override List<TThirdModel> GetThirdListByFirst(IEnumerable<TFirstModel> datas)
        {
            return GetThirdListByFirstAsync(datas).Result;
        }

        /// <summary>
        /// Get Third by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override async Task<List<TThirdModel>> GetThirdListByFirstAsync(IEnumerable<TFirstModel> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<TThirdModel>(0);
            }
            var query = CreateQueryByFirst(datas);
            var relationDatas = await GetListAsync(query).ConfigureAwait(false);
            return relationDatas?.Select(c => c.Item3).ToList() ?? new List<TThirdModel>(0);
        }

        /// <summary>
        /// Get Third by Second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override List<TThirdModel> GetThirdListBySecond(IEnumerable<TSecondModel> datas)
        {
            return GetThirdListBySecondAsync(datas).Result;
        }

        /// <summary>
        /// Get Second by Third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public sealed override async Task<List<TThirdModel>> GetThirdListBySecondAsync(IEnumerable<TSecondModel> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<TThirdModel>(0);
            }
            var query = CreateQueryBySecond(datas);
            var relationDatas = await GetListAsync(query).ConfigureAwait(false);
            return relationDatas?.Select(c => c.Item3).ToList() ?? new List<TThirdModel>(0);
        }

        #endregion

        #region Functions

        /// <summary>
        /// Execute save
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation records</returns>
        public virtual List<IActivationRecord> ExecuteSave(IEnumerable<Tuple<TFirstModel, TSecondModel, TThirdModel>> datas, ActivationOptions activationOptions = null)
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
        /// <returns>Return activation records</returns>
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
        /// Execute save by third datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation records</returns>
        public virtual List<IActivationRecord> ExecuteSaveByThird(IEnumerable<TThirdModel> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByThird(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(repositoryWarehouse.Save(entity, activationOptions));
            }
            return records;
        }

        /// <summary>
        /// execute Remove
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return activation records</returns>
        public virtual List<IActivationRecord> ExecuteRemove(IEnumerable<Tuple<TFirstModel, TSecondModel, TThirdModel>> datas, ActivationOptions activationOptions = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByRelationData(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(repositoryWarehouse.Remove(entity, activationOptions));
            }
            return records;
        }

        /// <summary>
        /// execute Remove
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
        /// Create data by third
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return data</returns>
        public virtual TEntity CreateEntityByThird(TThirdModel data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation data</returns>
        public virtual async Task<Tuple<TFirstModel, TSecondModel, TThirdModel>> ExecuteGetAsync(IQuery query)
        {
            var entity = await repositoryWarehouse.GetAsync(query).ConfigureAwait(false);
            return CreateRelationDataByEntity(entity);
        }

        /// <summary>
        /// Get relation data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation datas</returns>
        public virtual async Task<List<Tuple<TFirstModel, TSecondModel, TThirdModel>>> ExecuteGetListAsync(IQuery query)
        {
            var entityList = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            if (entityList.IsNullOrEmpty())
            {
                return new List<Tuple<TFirstModel, TSecondModel, TThirdModel>>(0);
            }
            return entityList.Select(c => CreateRelationDataByEntity(c)).ToList();
        }

        /// <summary>
        /// Get relation data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public virtual async Task<IPaging<Tuple<TFirstModel, TSecondModel, TThirdModel>>> ExecuteGetPagingAsync(IQuery query)
        {
            var entityPaging = await repositoryWarehouse.GetPagingAsync(query).ConfigureAwait(false);
            if (entityPaging.IsNullOrEmpty())
            {
                return Pager.Empty<Tuple<TFirstModel, TSecondModel, TThirdModel>>();
            }
            var datas = entityPaging.Select(c => CreateRelationDataByEntity(c));
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
        /// Create query by third type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return query object</returns>
        public abstract IQuery CreateQueryByThird(IEnumerable<TThirdModel> datas);

        /// <summary>
        /// Create query by third type datas query object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return query object</returns>
        public abstract IQuery CreateQueryByThird(IQuery query);

        /// <summary>
        /// Create entity by relation data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return data</returns>
        public abstract TEntity CreateEntityByRelationData(Tuple<TFirstModel, TSecondModel, TThirdModel> data);

        /// <summary>
        /// Create relation data by entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Return relation data</returns>
        public abstract Tuple<TFirstModel, TSecondModel, TThirdModel> CreateRelationDataByEntity(TEntity entity);

        #endregion

        #region Global condition

        #region Append Repository Condition

        /// <summary>
        /// Append repository condition
        /// </summary>
        /// <param name="originalQuery">Origin query</param>
        /// <returns>Return the newest query object</returns>
        IQuery AppendRepositoryCondition(IQuery originalQuery, QueryUsageScene usageScene)
        {
            if (originalQuery == null)
            {
                originalQuery = QueryManager.Create();
                originalQuery.SetEntityType(entityType);
            }
            else
            {
                originalQuery.SetEntityType(entityType);
            }

            //primary query
            GlobalConditionFilter conditionFilter = new GlobalConditionFilter()
            {
                OriginalQuery = originalQuery,
                UsageSceneEntityType = entityType,
                EntityType = entityType,
                SourceType = QuerySourceType.Repository,
                UsageScene = usageScene
            };
            var conditionFilterResult = QueryManager.GetGlobalCondition(conditionFilter);
            if (conditionFilterResult != null)
            {
                conditionFilterResult.AppendTo(originalQuery);
            }
            //subqueries
            if (!originalQuery.Subqueries.IsNullOrEmpty())
            {
                foreach (var squery in originalQuery.Subqueries)
                {
                    AppendSubqueryCondition(squery, conditionFilter);
                }
            }
            //join
            if (!originalQuery.JoinItems.IsNullOrEmpty())
            {
                foreach (var jitem in originalQuery.JoinItems)
                {
                    AppendJoinQueryCondition(jitem.JoinQuery, conditionFilter);
                }
            }
            return originalQuery;
        }

        #endregion

        #region Append Subqueries Condition

        /// <summary>
        /// Append subqueries condition
        /// </summary>
        /// <param name="subquery">Subquery</param>
        /// <param name="conditionFilter">Condition filter</param>
        void AppendSubqueryCondition(IQuery subquery, GlobalConditionFilter conditionFilter)
        {
            if (subquery == null)
            {
                return;
            }
            conditionFilter.SourceType = QuerySourceType.Subuery;
            conditionFilter.EntityType = subquery.GetEntityType();
            conditionFilter.OriginalQuery = subquery;
            var conditionFilterResult = QueryManager.GetGlobalCondition(conditionFilter);
            if (conditionFilterResult != null)
            {
                conditionFilterResult.AppendTo(subquery);
            }
            //subqueries
            if (!subquery.Subqueries.IsNullOrEmpty())
            {
                foreach (var squery in subquery.Subqueries)
                {
                    AppendSubqueryCondition(squery, conditionFilter);
                }
            }
            //join
            if (!subquery.JoinItems.IsNullOrEmpty())
            {
                foreach (var jitem in subquery.JoinItems)
                {
                    AppendJoinQueryCondition(jitem.JoinQuery, conditionFilter);
                }
            }
        }

        #endregion

        #region Append Join Condition

        /// <summary>
        /// Append join query condition
        /// </summary>
        /// <param name="joinQuery">Join query</param>
        /// <param name="conditionFilter">Condition filter</param>
        void AppendJoinQueryCondition(IQuery joinQuery, GlobalConditionFilter conditionFilter)
        {
            if (joinQuery == null)
            {
                return;
            }
            conditionFilter.SourceType = QuerySourceType.JoinQuery;
            conditionFilter.EntityType = joinQuery.GetEntityType();
            conditionFilter.OriginalQuery = joinQuery;
            var conditionFilterResult = QueryManager.GetGlobalCondition(conditionFilter);
            if (conditionFilterResult != null)
            {
                conditionFilterResult.AppendTo(joinQuery);
            }
            //subqueries
            if (!joinQuery.Subqueries.IsNullOrEmpty())
            {
                foreach (var squery in joinQuery.Subqueries)
                {
                    AppendSubqueryCondition(squery, conditionFilter);
                }
            }
            //join query
            if (!joinQuery.JoinItems.IsNullOrEmpty())
            {
                foreach (var jitem in joinQuery.JoinItems)
                {
                    AppendJoinQueryCondition(jitem.JoinQuery, conditionFilter);
                }
            }
        }

        #endregion

        #region Append Remove Extra Condition

        /// <summary>
        /// Append Remove condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendRemoveCondition(IQuery originalQuery)
        {
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Remove);
        }

        #endregion

        #region Append Query Extra Condition

        /// <summary>
        /// Append query condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected virtual IQuery AppendQueryCondition(IQuery originalQuery)
        {
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Query);
        }

        #endregion

        #endregion
    }
}
