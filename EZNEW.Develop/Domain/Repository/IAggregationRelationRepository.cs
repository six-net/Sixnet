using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    public interface IAggregationRelationRepository<T, First, Second> : IAggregationRepository<T> where T : IAggregationRoot<T> where Second : IAggregationRoot<Second> where First : IAggregationRoot<First>
    {
        #region query

        /// <summary>
        /// get list by first
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        List<T> GetListByFirst(IEnumerable<First> datas);

        /// <summary>
        /// get list by first
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        Task<List<T>> GetListByFirstAsync(IEnumerable<First> datas);

        /// <summary>
        /// get list by second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        List<T> GetListBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// get list by second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        Task<List<T>> GetListBySecondAsync(IEnumerable<Second> datas);

        #endregion

        #region remove

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        void RemoveByFirst(IEnumerable<First> datas);

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        void RemoveBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        void RemoveByFirst(IQuery query);

        /// <summary>
        /// remove by second
        /// </summary>
        /// <param name="query">query</param>
        void RemoveBySecond(IQuery query);

        #endregion
    }
}
