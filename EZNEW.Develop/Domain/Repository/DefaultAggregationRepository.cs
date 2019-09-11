using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;
using EZNEW.Framework.Paging;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Framework.Extension;
using EZNEW.Develop.Entity;
using EZNEW.Framework.IoC;
using EZNEW.Framework;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.Domain.Repository.Event;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Default Repository
    /// </summary>
    public abstract class DefaultAggregationRepository<DT, ET, DAI> : DefaultAggregationRootRepository<DT> where DT : IAggregationRoot<DT> where ET : BaseEntity<ET> where DAI : IDataAccess<ET>
    {
        protected IRepositoryWarehouse<ET, DAI> repositoryWarehouse = ContainerManager.Resolve<IRepositoryWarehouse<ET, DAI>>();
        static Type entityType = typeof(ET);

        static DefaultAggregationRepository()
        {
            WarehouseManager.RegisterDefaultWarehouse<ET, DAI>();
        }

        #region impl

        /// <summary>
        /// get life source
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public sealed override DataLifeSource GetLifeSource(IAggregationRoot data)
        {
            if (data == null)
            {
                return DataLifeSource.New;
            }
            return repositoryWarehouse.GetLifeSource(data.MapTo<ET>());
        }

        /// <summary>
        /// modify life source
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="lifeSource">life source</param>
        public sealed override void ModifyLifeSource(IAggregationRoot data, DataLifeSource lifeSource)
        {
            if (data == null)
            {
                return;
            }
            repositoryWarehouse.ModifyLifeSource(data.MapTo<ET>(), lifeSource);
        }

        #endregion

        #region function

        /// <summary>
        /// Execute Save
        /// </summary>
        /// <param name="data">objects</param>
        protected override async Task<IActivationRecord> ExecuteSaveAsync(DT data)
        {
            var entity = data?.MapTo<ET>();
            return await SaveEntityAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute Remove
        /// </summary>
        /// <param name="data">data</param>
        protected override async Task<IActivationRecord> ExecuteRemoveAsync(DT data)
        {
            if (data == null)
            {
                return null;
            }
            var entity = data.MapTo<ET>();
            return await RemoveEntityAsync(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute Remove
        /// </summary>
        /// <param name="query">query model</param>
        protected override async Task<IActivationRecord> ExecuteRemoveAsync(IQuery query)
        {
            return await repositoryWarehouse.RemoveAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<DT> GetDataAsync(IQuery query)
        {
            var entityData = await repositoryWarehouse.GetAsync(query).ConfigureAwait(false);
            DT data = default(DT);
            if (entityData != null)
            {
                data = entityData.MapTo<DT>();
            }
            return data;
        }

        /// <summary>
        /// Get Data List
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<List<DT>> GetDataListAsync(IQuery query)
        {
            var entityDataList = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            if (entityDataList.IsNullOrEmpty())
            {
                return new List<DT>(0);
            }
            var datas = entityDataList.Select(c => c.MapTo<DT>());
            return datas.ToList();
        }

        /// <summary>
        /// Get Object Paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<IPaging<DT>> GetDataPagingAsync(IQuery query)
        {
            var entityPaging = await repositoryWarehouse.GetPagingAsync(query).ConfigureAwait(false);
            var dataPaging = entityPaging.ConvertTo<DT>();
            return dataPaging;
        }

        /// <summary>
        /// Check Data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<bool> IsExistAsync(IQuery query)
        {
            return await repositoryWarehouse.ExistAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<long> CountValueAsync(IQuery query)
        {
            return await repositoryWarehouse.CountAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="VT">Data Type</typeparam>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<VT> MaxValueAsync<VT>(IQuery query)
        {
            return await repositoryWarehouse.MaxAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// get Min Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        protected override async Task<VT> MinValueAsync<VT>(IQuery query)
        {
            return await repositoryWarehouse.MinAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<VT> SumValueAsync<VT>(IQuery query)
        {
            return await repositoryWarehouse.SumAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns></returns>
        protected override async Task<VT> AvgValueAsync<VT>(IQuery query)
        {
            return await repositoryWarehouse.AvgAsync<VT>(query).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute Modify
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        protected override async Task<IActivationRecord> ExecuteModifyAsync(IModify expression, IQuery query)
        {
            return await repositoryWarehouse.ModifyAsync(expression, query).ConfigureAwait(false);
        }

        /// <summary>
        /// save entity
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        protected virtual async Task<IActivationRecord> SaveEntityAsync(params ET[] datas)
        {
            return await repositoryWarehouse.SaveAsync(datas).ConfigureAwait(false);
        }

        /// <summary>
        /// save entity
        /// </summary>
        /// <param name="datas"></param>
        protected virtual void SaveEntity(params ET[] datas)
        {
            var record = SaveEntityAsync(datas).Result;
            if (record != null)
            {
                WorkFactory.RegisterActivationRecord(record);
            }
        }

        /// <summary>
        /// remove entity
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        protected virtual async Task<IActivationRecord> RemoveEntityAsync(params ET[] datas)
        {
            return await repositoryWarehouse.RemoveAsync(datas).ConfigureAwait(false);
        }

        /// <summary>
        /// remove entity
        /// </summary>
        /// <param name="datas"></param>
        protected virtual void RemoveEntity(params ET[] datas)
        {
            var record = RemoveEntityAsync(datas).Result;
            if (record != null)
            {
                WorkFactory.RegisterActivationRecord(record);
            }
        }

        #endregion

        #region global condition

        #region Append Repository Condition

        /// <summary>
        /// append repository condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        IQuery AppendRepositoryCondition(IQuery originQuery, QueryUsageScene usageScene)
        {
            if (originQuery == null)
            {
                originQuery = QueryFactory.Create();
                originQuery.SetEntityType(entityType);
            }
            else
            {
                originQuery.SetEntityType(entityType);
            }

            //primary query
            GlobalConditionFilter conditionFilter = new GlobalConditionFilter()
            {
                OriginQuery = originQuery,
                UsageSceneEntityType = entityType,
                EntityType = entityType,
                SourceType = QuerySourceType.Repository,
                UsageScene = usageScene
            };
            var conditionFilterResult = QueryManager.GlobalConditionFilter(conditionFilter);
            if (conditionFilterResult != null)
            {
                conditionFilterResult.AppendTo(originQuery);
            }
            //subqueries
            if (!originQuery.Subqueries.IsNullOrEmpty())
            {
                foreach (var squery in originQuery.Subqueries)
                {
                    AppendSubqueryCondition(squery, conditionFilter);
                }
            }
            //join
            if (!originQuery.JoinItems.IsNullOrEmpty())
            {
                foreach (var jitem in originQuery.JoinItems)
                {
                    AppendJoinQueryCondition(jitem.JoinQuery, conditionFilter);
                }
            }
            return originQuery;
        }

        #endregion

        #region Append Subqueries Condition

        /// <summary>
        /// append subqueries condition
        /// </summary>
        /// <param name="subquery">subquery</param>
        /// <param name="conditionFilter">condition filter</param>
        void AppendSubqueryCondition(IQuery subquery, GlobalConditionFilter conditionFilter)
        {
            if (subquery == null)
            {
                return;
            }
            conditionFilter.SourceType = QuerySourceType.Subuery;
            conditionFilter.EntityType = subquery.EntityType;
            conditionFilter.OriginQuery = subquery;
            var conditionFilterResult = QueryManager.GlobalConditionFilter(conditionFilter);
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
        /// append join query condition
        /// </summary>
        /// <param name="joinQuery">join query</param>
        /// <param name="conditionFilter">condition filter</param>
        void AppendJoinQueryCondition(IQuery joinQuery, GlobalConditionFilter conditionFilter)
        {
            if (joinQuery == null)
            {
                return;
            }
            conditionFilter.SourceType = QuerySourceType.JoinQuery;
            conditionFilter.EntityType = joinQuery.EntityType;
            conditionFilter.OriginQuery = joinQuery;
            var conditionFilterResult = QueryManager.GlobalConditionFilter(conditionFilter);
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
        /// append remove condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected override IQuery AppendRemoveCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Remove);
        }

        #endregion

        #region Append Modify Extra Condition

        /// <summary>
        /// append modify condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected override IQuery AppendModifyCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Modify);
        }

        #endregion

        #region Append Query Extra Condition

        /// <summary>
        /// append query condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected override IQuery AppendQueryCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Query);
        }

        #endregion

        #region Append Exist Extra Condition

        /// <summary>
        /// append exist condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected override IQuery AppendExistCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Exist);
        }

        #endregion

        #region Append Count Extra Condition

        /// <summary>
        /// append count condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected override IQuery AppendCountCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Count);
        }

        #endregion

        #region Append Max Extra Condition

        /// <summary>
        /// append max condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected override IQuery AppendMaxCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Max);
        }

        #endregion

        #region Append Min Extra Condition

        /// <summary>
        /// append min condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected override IQuery AppendMinCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Min);
        }

        #endregion

        #region Append Sum Extra Condition

        /// <summary>
        /// append sum condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected override IQuery AppendSumCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Sum);
        }

        #endregion

        #region Append Avg Extra Condition

        /// <summary>
        /// append avg condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected override IQuery AppendAvgCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Avg);
        }

        #endregion

        #endregion
    }
}
