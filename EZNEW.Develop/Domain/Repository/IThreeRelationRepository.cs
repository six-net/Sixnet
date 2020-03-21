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
    public interface IThreeRelationRepository<First, Second, Third> where First : IAggregationRoot<First> where Second : IAggregationRoot<Second> where Third : IAggregationRoot<Third>
    {
        #region save

        /// <summary>
        /// save
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void Save(IEnumerable<Tuple<First, Second, Third>> datas, ActivationOption activationOption = null);

        /// <summary>
        /// save async
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        Task SaveAsync(IEnumerable<Tuple<First, Second, Third>> datas, ActivationOption activationOption = null);

        /// <summary>
        /// save by first type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void SaveByFirst(IEnumerable<First> datas, ActivationOption activationOption = null);

        /// <summary>
        /// save by second type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void SaveBySecond(IEnumerable<Second> datas, ActivationOption activationOption = null);

        /// <summary>
        /// save by third type datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void SaveByThird(IEnumerable<Third> datas, ActivationOption activationOption = null);

        #endregion

        #region remove

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void Remove(IEnumerable<Tuple<First, Second, Third>> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        Task RemoveAsync(IEnumerable<Tuple<First, Second, Third>> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        void Remove(IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        Task RemoveAsync(IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void RemoveByFirst(IEnumerable<First> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        void RemoveByFirst(IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void RemoveBySecond(IEnumerable<Second> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove by second
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        void RemoveBySecond(IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// remove by third datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void RemoveByThird(IEnumerable<Third> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove by third
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        void RemoveByThird(IQuery query, ActivationOption activationOption = null);

        #endregion

        #region query

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        Tuple<First, Second, Third> Get(IQuery query);

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        Task<Tuple<First, Second, Third>> GetAsync(IQuery query);

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        List<Tuple<First, Second, Third>> GetList(IQuery query);

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<List<Tuple<First, Second, Third>>> GetListAsync(IQuery query);

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        IPaging<Tuple<First, Second, Third>> GetPaging(IQuery query);

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        Task<IPaging<Tuple<First, Second, Third>>> GetPagingAsync(IQuery query);

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
        /// get first by third
        /// </summary>
        /// <param name="datas">third datas</param>
        /// <returns></returns>
        List<First> GetFirstListByThird(IEnumerable<Third> datas);

        /// <summary>
        /// get first by third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        Task<List<First>> GetFirstListByThirdAsync(IEnumerable<Third> datas);

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

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        List<Second> GetSecondListByThird(IEnumerable<Third> datas);

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        Task<List<Second>> GetSecondListByThirdAsync(IEnumerable<Third> datas);

        /// <summary>
        /// get Third by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        List<Third> GetThirdListByFirst(IEnumerable<First> datas);

        /// <summary>
        /// get Third by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        Task<List<Third>> GetThirdListByFirstAsync(IEnumerable<First> datas);

        /// <summary>
        /// get Third by Second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        List<Third> GetThirdListBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        Task<List<Third>> GetThirdListBySecondAsync(IEnumerable<Second> datas);

        #endregion
    }
}
