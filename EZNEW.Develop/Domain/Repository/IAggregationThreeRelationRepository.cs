using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    public interface IAggregationThreeRelationRepository<T, First, Second, Third> : IAggregationRepository<T> where T : IAggregationRoot<T> where First : IAggregationRoot<First> where Second : IAggregationRoot<Second> where Third : IAggregationRoot<Third>
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

        /// <summary>
        /// get list by third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        List<T> GetListByThird(IEnumerable<Third> datas);

        /// <summary>
        /// get list by third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        Task<List<T>> GetListByThirdAsync(IEnumerable<Third> datas);

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
        /// remove by third datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        void RemoveByThird(IEnumerable<Third> datas);

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

        /// <summary>
        /// remove by third
        /// </summary>
        /// <param name="query">query</param>
        void RemoveByThird(IQuery query);

        #endregion
    }
}
