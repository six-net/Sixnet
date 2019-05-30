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
    /// <summary>
    /// 处理三者想关联数据存储
    /// </summary>
    /// <typeparam name="First">第一种关联数据</typeparam>
    /// <typeparam name="Second">第二种关联数据</typeparam>
    /// <typeparam name="Third">第三种关联数据</typeparam>
    /// <typeparam name="ET">实体类型</typeparam>
    /// <typeparam name="DAI">数据访问接口</typeparam>
    public abstract class DefaultThreeRelationRepository<First, Second, Third, ET, DAI> : BaseThreeRelationRepository<First, Second, Third> where Second : IAggregationRoot<Second> where First : IAggregationRoot<First> where Third : IAggregationRoot<Third> where ET : BaseEntity<ET> where DAI : IDataAccess<ET>
    {
        IRepositoryWarehouse<ET, DAI> repositoryWarehouse = ContainerManager.Resolve<IRepositoryWarehouse<ET, DAI>>();

        static DefaultThreeRelationRepository()
        {
            if (!ContainerManager.IsRegister<IRepositoryWarehouse<ET, DAI>>())
            {
                ContainerManager.Register<IRepositoryWarehouse<ET, DAI>, DefaultRepositoryWarehouse<ET, DAI>>();
            }
        }

        #region save

        /// <summary>
        /// save
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override void Save(IEnumerable<Tuple<First, Second, Third>> datas)
        {
            SaveAsync(datas).Wait();
        }

        /// <summary>
        /// save async
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override async Task SaveAsync(IEnumerable<Tuple<First, Second, Third>> datas)
        {
            var records = await ExecuteSaveAsync(datas).ConfigureAwait(false);
            if (records.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave<Tuple<First, Second, Third>>(GetType(), datas);
            WorkFactory.RegisterActivationRecord(records.ToArray());
        }

        /// <summary>
        /// save by first type datas
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override void SaveByFirst(IEnumerable<First> datas)
        {
            var saveRecords = ExecuteSaveByFirstAsync(datas).Result;
            if (saveRecords.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave<First>(GetType(), datas);
            WorkFactory.RegisterActivationRecord(saveRecords.ToArray());
        }

        /// <summary>
        /// save by second type datas
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override void SaveBySecond(IEnumerable<Second> datas)
        {
            var saveRecords = ExecuteSaveBySecondAsync(datas).Result;
            if (saveRecords.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave<Second>(GetType(), datas);
            WorkFactory.RegisterActivationRecord(saveRecords.ToArray());
        }

        /// <summary>
        /// save by third datas
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override void SaveByThird(IEnumerable<Third> datas)
        {
            var saveRecords = ExecuteSaveByThirdAsync(datas).Result;
            if (saveRecords.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishSave<Third>(GetType(), datas);
            WorkFactory.RegisterActivationRecord(saveRecords.ToArray());
        }

        #endregion

        #region remove

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override void Remove(IEnumerable<Tuple<First, Second, Third>> datas)
        {
            RemoveAsync(datas).Wait();
        }

        /// <summary>
        /// save async
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override async Task RemoveAsync(IEnumerable<Tuple<First, Second, Third>> datas)
        {
            var records = await ExecuteRemoveAsync(datas).ConfigureAwait(false);
            if (records.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishRemove<Tuple<First, Second, Third>>(GetType(), datas);
            WorkFactory.RegisterActivationRecord(records.ToArray());
        }

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        public sealed override void Remove(IQuery query)
        {
            RemoveAsync(query).Wait();
        }

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        public sealed override async Task RemoveAsync(IQuery query)
        {
            var record = await ExecuteRemoveAsync(query).ConfigureAwait(false);
            if (record == null)
            {
                return;
            }
            RepositoryEventBus.PublishRemove<Tuple<First, Second, Third>>(GetType(), query);
            WorkFactory.RegisterActivationRecord(record);
        }

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override void RemoveByFirst(IEnumerable<First> datas)
        {
            var removeRecord = ExecuteRemoveByFirst(datas);
            if (removeRecord == null)
            {
                return;
            }
            RepositoryEventBus.PublishRemove<First>(GetType(), datas);
            WorkFactory.RegisterActivationRecord(removeRecord);
        }

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        public sealed override void RemoveByFirst(IQuery query)
        {
            var removeQuery = CreateQueryByFirst(query);
            Remove(removeQuery);
        }

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override void RemoveBySecond(IEnumerable<Second> datas)
        {
            var removeRecord = ExecuteRemoveBySecond(datas);
            if (removeRecord == null)
            {
                return;
            }
            RepositoryEventBus.PublishRemove<Second>(GetType(), datas);
            WorkFactory.RegisterActivationRecord(removeRecord);
        }

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        public sealed override void RemoveBySecond(IQuery query)
        {
            var removeQuery = CreateQueryBySecond(query);
            Remove(removeQuery);
        }

        /// <summary>
        /// remove by third datas
        /// </summary>
        /// <param name="datas">datas</param>
        public sealed override void RemoveByThird(IEnumerable<Third> datas)
        {
            var removeRecord = ExecuteRemoveByThird(datas);
            if (removeRecord == null)
            {
                return;
            }
            RepositoryEventBus.PublishRemove<Third>(GetType(), datas);
            WorkFactory.RegisterActivationRecord(removeRecord);
        }

        /// <summary>
        /// remove by third
        /// </summary>
        /// <param name="query">query</param>
        public sealed override void RemoveByThird(IQuery query)
        {
            var removeQuery = CreateQueryByThird(query);
            Remove(removeQuery);
        }

        #endregion

        #region query

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override Tuple<First, Second, Third> Get(IQuery query)
        {
            return GetAsync(query).Result;
        }

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override async Task<Tuple<First, Second, Third>> GetAsync(IQuery query)
        {
            return await ExecuteGetAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override List<Tuple<First, Second, Third>> GetList(IQuery query)
        {
            return GetListAsync(query).Result;
        }

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public sealed override async Task<List<Tuple<First, Second, Third>>> GetListAsync(IQuery query)
        {
            return await ExecuteGetListAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override IPaging<Tuple<First, Second, Third>> GetPaging(IQuery query)
        {
            return GetPagingAsync(query).Result;
        }

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public sealed override async Task<IPaging<Tuple<First, Second, Third>>> GetPagingAsync(IQuery query)
        {
            return await ExecuteGetPagingAsync(query).ConfigureAwait(false);
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
            return await ExecuteGetFirstListBySecondAsync(datas).ConfigureAwait(false);
        }

        /// <summary>
        /// get first by third
        /// </summary>
        /// <param name="datas">third datas</param>
        /// <returns></returns>
        public sealed override List<First> GetFirstListByThird(IEnumerable<Third> datas)
        {
            return GetFirstListByThirdAsync(datas).Result;
        }

        /// <summary>
        /// get first by third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override async Task<List<First>> GetFirstListByThirdAsync(IEnumerable<Third> datas)
        {
            return await ExecuteGetFirstListByThirdAsync(datas).ConfigureAwait(false);
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
            return await ExecuteGetSecondListByFirstAsync(datas).ConfigureAwait(false);
        }

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override List<Second> GetSecondListByThird(IEnumerable<Third> datas)
        {
            return GetSecondListByThirdAsync(datas).Result;
        }

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override async Task<List<Second>> GetSecondListByThirdAsync(IEnumerable<Third> datas)
        {
            return await ExecuteGetSecondListByThirdAsync(datas).ConfigureAwait(false);
        }

        /// <summary>
        /// get Third by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override List<Third> GetThirdListByFirst(IEnumerable<First> datas)
        {
            return GetThirdListByFirstAsync(datas).Result;
        }

        /// <summary>
        /// get Third by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override async Task<List<Third>> GetThirdListByFirstAsync(IEnumerable<First> datas)
        {
            return await ExecuteGetThirdListByFirstAsync(datas).ConfigureAwait(false);
        }

        /// <summary>
        /// get Third by Second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override List<Third> GetThirdListBySecond(IEnumerable<Second> datas)
        {
            return GetThirdListBySecondAsync(datas).Result;
        }

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override async Task<List<Third>> GetThirdListBySecondAsync(IEnumerable<Second> datas)
        {
            return await ExecuteGetThirdListBySecondAsync(datas).ConfigureAwait(false);
        }

        #endregion

        #region functions

        /// <summary>
        /// execute save
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<IActivationRecord>> ExecuteSaveAsync(IEnumerable<Tuple<First, Second, Third>> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByRelationData(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(await repositoryWarehouse.SaveAsync(entity).ConfigureAwait(false));
            }
            return records;
        }

        /// <summary>
        /// execute save by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<IActivationRecord>> ExecuteSaveByFirstAsync(IEnumerable<First> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByFirst(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(await repositoryWarehouse.SaveAsync(entity).ConfigureAwait(false));
            }
            return records;
        }

        /// <summary>
        /// execute save by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<IActivationRecord>> ExecuteSaveBySecondAsync(IEnumerable<Second> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityBySecond(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(await repositoryWarehouse.SaveAsync(entity).ConfigureAwait(false));
            }
            return records;
        }

        /// <summary>
        /// execute save by third datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<IActivationRecord>> ExecuteSaveByThirdAsync(IEnumerable<Third> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByThird(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(await repositoryWarehouse.SaveAsync(entity).ConfigureAwait(false));
            }
            return records;
        }

        /// <summary>
        /// execute remove
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<IActivationRecord>> ExecuteRemoveAsync(IEnumerable<Tuple<First, Second, Third>> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var entitys = datas.Select(c => CreateEntityByRelationData(c)).ToList();
            List<IActivationRecord> records = new List<IActivationRecord>(entitys.Count);
            foreach (var entity in entitys)
            {
                records.Add(await repositoryWarehouse.RemoveAsync(entity).ConfigureAwait(false));
            }
            return records;
        }

        /// <summary>
        /// execute remove
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public virtual async Task<IActivationRecord> ExecuteRemoveAsync(IQuery query)
        {
            return await repositoryWarehouse.RemoveAsync(query);
        }

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual IActivationRecord ExecuteRemoveByFirst(IEnumerable<First> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var query = CreateQueryByFirst(datas);
            var removeRecord = repositoryWarehouse.RemoveAsync(query).Result;
            return removeRecord;
        }

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual IActivationRecord ExecuteRemoveBySecond(IEnumerable<Second> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var query = CreateQueryBySecond(datas);
            var removeRecord = repositoryWarehouse.RemoveAsync(query).Result;
            return removeRecord;
        }

        /// <summary>
        /// remove by third datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual IActivationRecord ExecuteRemoveByThird(IEnumerable<Third> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return null;
            }
            var query = CreateQueryByThird(datas);
            var removeRecord = repositoryWarehouse.RemoveAsync(query).Result;
            return removeRecord;
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
        /// <param name="data">data</param>
        /// <returns></returns>
        public virtual ET CreateEntityBySecond(Second data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// create data by third
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public virtual ET CreateEntityByThird(Third data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public virtual async Task<Tuple<First, Second, Third>> ExecuteGetAsync(IQuery query)
        {
            var entity = await repositoryWarehouse.GetAsync(query).ConfigureAwait(false);
            return CreateRelationDataByEntity(entity);
        }

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public virtual async Task<List<Tuple<First, Second, Third>>> ExecuteGetListAsync(IQuery query)
        {
            var entityList = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            if (entityList.IsNullOrEmpty())
            {
                return new List<Tuple<First, Second, Third>>(0);
            }
            return entityList.Select(c => CreateRelationDataByEntity(c)).ToList();
        }

        /// <summary>
        /// get relation data paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public virtual async Task<IPaging<Tuple<First, Second, Third>>> ExecuteGetPagingAsync(IQuery query)
        {
            var entityPaging = await repositoryWarehouse.GetPagingAsync(query).ConfigureAwait(false);
            if (entityPaging.IsNullOrEmpty())
            {
                return Paging<Tuple<First, Second, Third>>.EmptyPaging();
            }
            var datas = entityPaging.Select(c => CreateRelationDataByEntity(c));
            return new Paging<Tuple<First, Second, Third>>(entityPaging.Page, entityPaging.PageSize, entityPaging.TotalCount, datas);
        }

        /// <summary>
        /// get First by Second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<First>> ExecuteGetFirstListBySecondAsync(IEnumerable<Second> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<First>(0);
            }
            var query = CreateQueryBySecond(datas);
            var entitys = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            return entitys.Select(c => CreateRelationDataByEntity(c).Item1).ToList();
        }

        /// <summary>
        /// get First by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<First>> ExecuteGetFirstListByThirdAsync(IEnumerable<Third> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<First>(0);
            }
            var query = CreateQueryByThird(datas);
            var entitys = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            return entitys.Select(c => CreateRelationDataByEntity(c).Item1).ToList();
        }

        /// <summary>
        /// get Second by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<Second>> ExecuteGetSecondListByFirstAsync(IEnumerable<First> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<Second>(0);
            }
            var query = CreateQueryByFirst(datas);
            var entitys = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            return entitys.Select(c => CreateRelationDataByEntity(c).Item2).ToList();
        }

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<Second>> ExecuteGetSecondListByThirdAsync(IEnumerable<Third> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<Second>(0);
            }
            var query = CreateQueryByThird(datas);
            var entitys = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            return entitys.Select(c => CreateRelationDataByEntity(c).Item2).ToList();
        }

        /// <summary>
        /// get Third by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<Third>> ExecuteGetThirdListByFirstAsync(IEnumerable<First> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<Third>(0);
            }
            var query = CreateQueryByFirst(datas);
            var entitys = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            return entitys.Select(c => CreateRelationDataByEntity(c).Item3).ToList();
        }

        /// <summary>
        /// get Third by Second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public virtual async Task<List<Third>> ExecuteGetThirdListBySecondAsync(IEnumerable<Second> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<Third>(0);
            }
            var query = CreateQueryBySecond(datas);
            var entitys = await repositoryWarehouse.GetListAsync(query).ConfigureAwait(false);
            return entitys.Select(c => CreateRelationDataByEntity(c).Item3).ToList();
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
        /// create query by third type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryByThird(IEnumerable<Third> datas);

        /// <summary>
        /// create query by third type datas query object
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryByThird(IQuery query);

        /// <summary>
        /// create entity by relation data
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public abstract ET CreateEntityByRelationData(Tuple<First, Second, Third> data);

        /// <summary>
        /// create relation data by entity
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns></returns>
        public abstract Tuple<First, Second, Third> CreateRelationDataByEntity(ET entity);

        #endregion
    }
}
