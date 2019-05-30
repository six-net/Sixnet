using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Domain.Repository.Event;
using EZNEW.Develop.Entity;
using EZNEW.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    public abstract class DefaultAggregationThreeRelationRepository<DT, First, Second, Third, ET, DAI> : BaseAggregationThreeRelationRepository<DT, First, Second, Third, ET, DAI> where DT : IAggregationRoot<DT> where Second : IAggregationRoot<Second> where First : IAggregationRoot<First> where Third : IAggregationRoot<Third> where ET : BaseEntity<ET> where DAI : IDataAccess<ET>
    {
        #region query

        /// <summary>
        /// get list by first
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override List<DT> GetListByFirst(IEnumerable<First> datas)
        {
            return GetListByFirstAsync(datas).Result;
        }

        /// <summary>
        /// get list by first
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override async Task<List<DT>> GetListByFirstAsync(IEnumerable<First> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<DT>(0);
            }
            var query = CreateQueryByFirst(datas);
            return await GetListAsync(query).ConfigureAwait(false);
        }

        /// <summary>
        /// get list by second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override List<DT> GetListBySecond(IEnumerable<Second> datas)
        {
            return GetListBySecondAsync(datas).Result;
        }

        /// <summary>
        /// get list by second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override async Task<List<DT>> GetListBySecondAsync(IEnumerable<Second> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<DT>(0);
            }
            var query = CreateQueryBySecond(datas);
            return await GetListAsync(query).ConfigureAwait(false);
        }


        /// <summary>
        /// get list by third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override List<DT> GetListByThird(IEnumerable<Third> datas)
        {
            return GetListByThirdAsync(datas).Result;
        }

        /// <summary>
        /// get list by third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public sealed override async Task<List<DT>> GetListByThirdAsync(IEnumerable<Third> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return new List<DT>(0);
            }
            var query = CreateQueryByThird(datas);
            return await GetListAsync(query).ConfigureAwait(false);
        }

        #endregion

        #region remove

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        public sealed override void RemoveByFirst(IEnumerable<First> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishRemove<First>(GetType(), datas);
            IQuery query = CreateQueryByFirst(datas);
            Remove(query);
        }

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        public sealed override void RemoveBySecond(IEnumerable<Second> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishRemove<Second>(GetType(), datas);
            IQuery query = CreateQueryBySecond(datas);
            Remove(query);
        }

        /// <summary>
        /// remove by third datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        public sealed override void RemoveByThird(IEnumerable<Third> datas)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
            }
            RepositoryEventBus.PublishRemove<Third>(GetType(), datas);
            IQuery query = CreateQueryByThird(datas);
            Remove(query);
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
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        public sealed override void RemoveBySecond(IQuery query)
        {
            var removeQuery = CreateQueryBySecond(query);
            Remove(removeQuery);
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

        #region functions

        /// <summary>
        /// create query by first type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryByFirst(IEnumerable<First> datas);

        /// <summary>
        /// create query by second type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// create query by third type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryByThird(IEnumerable<Third> datas);

        /// <summary>
        /// create query by first type datas query object
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryByFirst(IQuery query);

        /// <summary>
        /// create query by second type datas query object
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryBySecond(IQuery query);

        /// <summary>
        /// create query by third type datas query object
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract IQuery CreateQueryByThird(IQuery query);

        #endregion
    }
}
