using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    public abstract class BaseAggregationRelationRepository<T, First, Second,ET,DAI> : DefaultAggregationRepository<T,ET,DAI> where T : IAggregationRoot<T> where ET : BaseEntity<ET> where DAI : IDataAccess<ET>
    {
        #region query

        /// <summary>
        /// get list by first
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract List<T> GetListByFirst(IEnumerable<First> datas);

        /// <summary>
        /// get list by first
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract Task<List<T>> GetListByFirstAsync(IEnumerable<First> datas);

        /// <summary>
        /// get list by second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract List<T> GetListBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// get list by second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract Task<List<T>> GetListBySecondAsync(IEnumerable<Second> datas);

        #endregion

        #region remove

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        public abstract void RemoveByFirst(IEnumerable<First> datas);

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        public abstract void RemoveBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        public abstract void RemoveByFirst(IQuery query);

        /// <summary>
        /// remove by second
        /// </summary>
        /// <param name="query">query</param>
        public abstract void RemoveBySecond(IQuery query);

        #endregion
    }
}
