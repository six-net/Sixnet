using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    public interface IRelationRepository<First, Second> where Second : IAggregationRoot<Second> where First : IAggregationRoot<First>
    {
        #region save

        /// <summary>
        /// save
        /// </summary>
        /// <param name="datas">datas</param>
        void Save(IEnumerable<Tuple<First, Second>> datas);

        /// <summary>
        /// save async
        /// </summary>
        /// <param name="datas">datas</param>
        Task SaveAsync(IEnumerable<Tuple<First, Second>> datas);

        /// <summary>
        /// save by first type datas
        /// </summary>
        /// <param name="datas">datas</param>
        void SaveByFirst(IEnumerable<First> datas);

        /// <summary>
        /// save by second type datas
        /// </summary>
        /// <param name="datas">datas</param>
        void SaveBySecond(IEnumerable<Second> datas);

        #endregion

        #region remove

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="datas">datas</param>
        void Remove(IEnumerable<Tuple<First, Second>> datas);

        /// <summary>
        /// save async
        /// </summary>
        /// <param name="datas">datas</param>
        Task RemoveAsync(IEnumerable<Tuple<First, Second>> datas);

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        void Remove(IQuery query);

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        Task RemoveAsync(IQuery query);

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        void RemoveByFirst(IEnumerable<First> datas);

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        void RemoveByFirst(IQuery query);

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        void RemoveBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// remove by second
        /// </summary>
        /// <param name="query">query</param>
        void RemoveBySecond(IQuery query);

        #endregion

        #region query

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        Tuple<First, Second> Get(IQuery query);

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        Task<Tuple<First, Second>> GetAsync(IQuery query);

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        List<Tuple<First, Second>> GetList(IQuery query);

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<List<Tuple<First, Second>>> GetListAsync(IQuery query);

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        IPaging<Tuple<First, Second>> GetPaging(IQuery query);

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        Task<IPaging<Tuple<First, Second>>> GetPagingAsync(IQuery query);

        /// <summary>
        /// get First by Second
        /// </summary>
        /// <param name="datas">second datas</param>
        /// <returns></returns>
        List<First> GetFirstListBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// get First by Second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        Task<List<First>> GetFirstListBySecondAsync(IEnumerable<Second> datas);

        /// <summary>
        /// get Second by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        List<Second> GetSecondListByFirst(IEnumerable<First> datas);

        /// <summary>
        /// get Second by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        Task<List<Second>> GetSecondListByFirstAsync(IEnumerable<First> datas);

        #endregion
    }
}
