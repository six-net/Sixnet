using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Domain.Repository.Event;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Extension;
using EZNEW.Framework.IoC;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    public abstract class DefaultRelationRepository<First, Second, ET, DAI> : BaseRelationRepository<First, Second> where Second : IAggregationRoot<Second> where First : IAggregationRoot<First> where ET : BaseEntity<ET>, new() where DAI : IDataAccess<ET>
    {
        IRepositoryWarehouse<ET, DAI> repositoryWarehouse = ContainerManager.Resolve<IRepositoryWarehouse<ET, DAI>>();
        static Type entityType = typeof(ET);

        static DefaultRelationRepository()
        {
            WarehouseManager.RegisterDefaultWarehouse<ET, DAI>();
        }

        #region save

        /// <summary>
        /// save
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void Save(IEnumerable<Tuple<First, Second>> datas, ActivationOption activationOption = null)
        {
            SaveAsync(datas, activationOption).Wait();
        }

        /// <summary>
        /// save async
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override async Task SaveAsync(IEnumerable<Tuple<First, Second>> datas, ActivationOption activationOption = null)
        {
            var records = await ExecuteSaveAsync(datas, activationOption).ConfigureAwait(false);
            if (records.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave(GetType(), datas, activationOption);
            WorkFactory.RegisterActivationRecord(records.ToArray());
        }

        /// <summary>
        /// save by first type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void SaveByFirst(IEnumerable<First> datas, ActivationOption activationOption = null)
        {
            var saveRecords = ExecuteSaveByFirstAsync(datas, activationOption).Result;
            if (saveRecords.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave(GetType(), datas, activationOption);
            WorkFactory.RegisterActivationRecord(saveRecords.ToArray());
        }

        /// <summary>
        /// save by second type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void SaveBySecond(IEnumerable<Second> datas, ActivationOption activationOption = null)
        {
            var saveRecords = ExecuteSaveBySecondAsync(datas, activationOption).Result;
            if (saveRecords.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave(GetType(), datas, activationOption);
            WorkFactory.RegisterActivationRecord(saveRecords.ToArray());
        }

        #endregion

        #region remove

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void Remove(IEnumerable<Tuple<First, Second>> datas, ActivationOption activationOption = null)
        {
            RemoveAsync(datas, activationOption).Wait();
        }

        /// <summary>
        /// save async
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override async Task RemoveAsync(IEnumerable<Tuple<First, Second>> datas, ActivationOption activationOption = null)
        {
            var records = await ExecuteRemoveAsync(datas, activationOption).ConfigureAwait(false);
            if (records.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishRemove(GetType(), datas, activationOption);
            WorkFactory.RegisterActivationRecord(records.ToArray());
        }

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void Remove(IQuery query, ActivationOption activationOption = null)
        {
            RemoveAsync(query, activationOption).Wait();
        }

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        public sealed override async Task RemoveAsync(IQuery query, ActivationOption activationOption = null)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Remove, AppendRemoveCondition);
            var record = await ExecuteRemoveAsync(newQuery, activationOption).ConfigureAwait(false);
            if (record == null)
            {
                return;
            }
            RepositoryEventBus.PublishRemove<Tuple<First, Second>>(GetType(), newQuery, activationOption);
            WorkFactory.RegisterActivationRecord(record);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Remove);
        }

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void RemoveByFirst(IEnumerable<First> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var query = CreateQueryByFirst(datas);
            Remove(query, activationOption);
        }

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void RemoveByFirst(IQuery query, ActivationOption activationOption = null)
        {
            var removeQuery = CreateQueryByFirst(query);
            Remove(removeQuery, activationOption);
        }

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void RemoveBySecond(IEnumerable<Second> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            var query = CreateQueryBySecond(datas);
            Remove(query, activationOption);
        }

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        public sealed override void RemoveBySecond(IQuery query, ActivationOption activationOption = null)
        {
            var removeQuery = CreateQueryBySecond(query);
            Remove(removeQuery, activationOption);
        }

        #endregion

        #region query

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override Tuple<First, Second> Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override async Task<Tuple<First, Second>> GetAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var data = await ExecuteGetAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return data;
        }

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override List<Tuple<First, Second>> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public sealed override async Task<List<Tuple<First, Second>>> GetListAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var dataList = await ExecuteGetListAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return dataList;
        }

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override IPaging<Tuple<First, Second>> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override async Task<IPaging<Tuple<First, Second>>> GetPagingAsync(IQuery query)
        {
            var newQuery = RepositoryManager.HandleQueryObjectBeforeExecute(query, QueryUsageScene.Query, AppendQueryCondition);
            var paging = await ExecuteGetPagingAsync(newQuery).ConfigureAwait(false);
            RepositoryManager.HandleQueryObjectAfterExecute(query, newQuery, QueryUsageScene.Query);
            return paging;
        }

        /// <summary>
        /// get First by Second
        /// </summary>
        /// <param name="datas">second datas</param>
        /// <returns></returns>
        public sealed override List<First> GetFirstListBySecond(IEnumerable<Second> datas)
        {
            return GetFirstListBySecondAsync(datas).Result;
        }

        /// <summary>
        /// get First by Second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override async Task<List<First>> GetFirstListBySecondAsync(IEnumerable<Second> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<First>(0);
            }
            var query = CreateQueryBySecond(datas);
            var relationDatas = await GetListAsync(query).ConfigureAwait(false);
            return relationDatas?.Select(c => c.Item1).ToList() ?? new List<First>(0);
        }

        /// <summary>
        /// get Second by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override List<Second> GetSecondListByFirst(IEnumerable<First> datas)
        {
            return GetSecondListByFirstAsync(datas).Result;
        }

        /// <summary>
        /// get Second by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override async Task<List<Second>> GetSecondListByFirstAsync(IEnumerable<First> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<Second>(0);
            }
            var query = CreateQueryByFirst(datas);
            var relationDatas = await GetListAsync(query).ConfigureAwait(false);
            return relationDatas?.Select(c => c.Item2).ToList() ?? new List<Second>(0);
        }

        #endregion

        #region functions

        /// <summary>
        /// execute save
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public virtual async Task<List<IActivationRecord>> ExecuteSaveAsync(IEnumerable<Tuple<First, Second>> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByRelationData(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(await repositoryWarehouse.SaveAsync(entity, activationOption).ConfigureAwait(false));
            }
            return records;
        }

        /// <summary>
        /// execute save by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public virtual async Task<List<IActivationRecord>> ExecuteSaveByFirstAsync(IEnumerable<First> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByFirst(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(await repositoryWarehouse.SaveAsync(entity, activationOption).ConfigureAwait(false));
            }
            return records;
        }

        /// <summary>
        /// execute save by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public virtual async Task<List<IActivationRecord>> ExecuteSaveBySecondAsync(IEnumerable<Second> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityBySecond(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(await repositoryWarehouse.SaveAsync(entity, activationOption).ConfigureAwait(false));
            }
            return records;
        }

        /// <summary>
        /// execute remove
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public virtual async Task<List<IActivationRecord>> ExecuteRemoveAsync(IEnumerable<Tuple<First, Second>> datas, ActivationOption activationOption = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByRelationData(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(await repositoryWarehouse.RemoveAsync(entity, activationOption).ConfigureAwait(false));
            }
            return records;
        }

        /// <summary>
        /// execute remove
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        public virtual async Task<IActivationRecord> ExecuteRemoveAsync(IQuery query, ActivationOption activationOption = null)
        {
            return await repositoryWarehouse.RemoveAsync(query, activationOption);
        }

        /// <summary>
        /// create data by first
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public virtual ET CreateEntityByFirst(First data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// create data by second
        /// </summary>
        /// <param name="datas">data</param>
        /// <returns></returns>
        public virtual ET CreateEntityBySecond(Second datas)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public virtual async Task<Tuple<First, Second>> ExecuteGetAsync(IQuery query)
        {
            var entity = await repositoryWarehouse.GetAsync(query).ConfigureAwait(false);
            return CreateRelationDataByEntity(entity);
        }

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public virtual async Task<List<Tuple<First, Second>>> ExecuteGetListAsync(IQuery query)
        {
            var entityList = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            if (entityList.IsNullOrEmpty())
            {
                return new List<Tuple<First, Second>>(0);
            }
            return entityList.Select(c => CreateRelationDataByEntity(c)).ToList();
        }

        /// <summary>
        /// get relation data paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public virtual async Task<IPaging<Tuple<First, Second>>> ExecuteGetPagingAsync(IQuery query)
        {
            var entityPaging = await repositoryWarehouse.GetPagingAsync(query).ConfigureAwait(false);
            if (entityPaging.IsNullOrEmpty())
            {
                return Paging<Tuple<First, Second>>.EmptyPaging();
            }
            var datas = entityPaging.Select(c => CreateRelationDataByEntity(c));
            return new Paging<Tuple<First, Second>>(entityPaging.Page, entityPaging.PageSize, entityPaging.TotalCount, datas);
        }

        /// <summary>
        /// create query by first type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryByFirst(IEnumerable<First> datas);

        /// <summary>
        /// create query by first type datas query object
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryByFirst(IQuery query);

        /// <summary>
        /// create query by second type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// create query by second type datas query object
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryBySecond(IQuery query);

        /// <summary>
        /// create entity by relation data
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public abstract ET CreateEntityByRelationData(Tuple<First, Second> data);

        /// <summary>
        /// create relation data by entity
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns></returns>
        public abstract Tuple<First, Second> CreateRelationDataByEntity(ET entity);

        #endregion

        #region global condition

        #region append repository condition

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
                OriginalQuery = originQuery,
                UsageSceneEntityType = entityType,
                EntityType = entityType,
                SourceType = QuerySourceType.Repository,
                UsageScene = usageScene
            };
            var conditionFilterResult = QueryFactory.GlobalConditionFilter(conditionFilter);
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

        #region append subqueries condition

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
            conditionFilter.EntityType = subquery.GetEntityType();
            conditionFilter.OriginalQuery = subquery;
            var conditionFilterResult = QueryFactory.GlobalConditionFilter(conditionFilter);
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

        #region append join condition

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
            conditionFilter.EntityType = joinQuery.GetEntityType();
            conditionFilter.OriginalQuery = joinQuery;
            var conditionFilterResult = QueryFactory.GlobalConditionFilter(conditionFilter);
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

        #region append remove extra condition

        /// <summary>
        /// append remove condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected virtual IQuery AppendRemoveCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Remove);
        }

        #endregion

        #region append query extra condition

        /// <summary>
        /// append query condition
        /// </summary>
        /// <param name="originQuery">origin query</param>
        /// <returns></returns>
        protected virtual IQuery AppendQueryCondition(IQuery originQuery)
        {
            return AppendRepositoryCondition(originQuery, QueryUsageScene.Query);
        }

        #endregion

        #endregion
    }
}
