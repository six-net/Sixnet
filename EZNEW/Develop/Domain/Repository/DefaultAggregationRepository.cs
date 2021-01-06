using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;
using EZNEW.Paging;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Entity;
using EZNEW.DependencyInjection;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Develop.Command.Modify;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Default aggregation repository
    /// </summary>
    /// <typeparam name="TModel">Aggregation model</typeparam>
    /// <typeparam name="TEntity">Entity</typeparam>
    /// <typeparam name="TDataAccess">Data access</typeparam>
    public abstract class DefaultAggregationRepository<TModel, TEntity, TDataAccess> : DefaultAggregationRootRepository<TModel> where TModel : AggregationRoot<TModel> where TEntity : BaseEntity<TEntity>, new() where TDataAccess : IDataAccess<TEntity>
    {
        protected IRepositoryWarehouse<TEntity, TDataAccess> repositoryWarehouse = ContainerManager.Resolve<IRepositoryWarehouse<TEntity, TDataAccess>>();

        static readonly Type entityType = typeof(TEntity);

        static DefaultAggregationRepository()
        {
            WarehouseManager.RegisterDefaultWarehouse<TEntity, TDataAccess>();
        }

        #region Impl

        /// <summary>
        /// Get life source
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return the data life source</returns>
        public sealed override DataLifeSource GetLifeSource(IAggregationRoot data)
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
        public sealed override void ModifyLifeSource(IAggregationRoot data, DataLifeSource lifeSource)
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
        protected override async Task<IPaging<TModel>> GetDataPagingAsync(IQuery query)
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
        protected override IActivationRecord ExecuteModify(IModify expression, IQuery query, ActivationOptions activationOptions = null)
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

        #region Append repository condition

        /// <summary>
        /// Append repository condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
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

        #region Append subqueries condition

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

        #region Append join condition

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

        #region Append Remove extra condition

        /// <summary>
        /// Append Remove condition
        /// </summary>
        /// <param name="originalQuery">Original query</param>
        /// <returns>Return the newest query object</returns>
        protected override IQuery AppendRemoveCondition(IQuery originalQuery)
        {
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Remove);
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
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Modify);
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
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Query);
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
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Exist);
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
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Count);
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
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Max);
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
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Min);
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
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Sum);
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
            return AppendRepositoryCondition(originalQuery, QueryUsageScene.Avg);
        }

        #endregion

        #endregion
    }
}
