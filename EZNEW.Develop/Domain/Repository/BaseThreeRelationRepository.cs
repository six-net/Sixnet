using EZNEW.Develop.CQuery;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    public abstract class BaseThreeRelationRepository<First, Second, Third>
    {
        #region save

        /// <summary>
        /// save
        /// </summary>
        /// <param name="datas">datas</param>
        public abstract void Save(IEnumerable<Tuple<First, Second, Third>> datas);

        /// <summary>
        /// save async
        /// </summary>
        /// <param name="datas">datas</param>
        public abstract Task SaveAsync(IEnumerable<Tuple<First, Second, Third>> datas);

        /// <summary>
        /// save by first type datas
        /// </summary>
        /// <param name="datas">datas</param>
        public abstract void SaveByFirst(IEnumerable<First> datas);

        /// <summary>
        /// save by second type datas
        /// </summary>
        /// <param name="datas">datas</param>
        public abstract void SaveBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// save by third type datas
        /// </summary>
        /// <param name="datas">datas</param>
        public abstract void SaveByThird(IEnumerable<Third> datas);

        #endregion

        #region remove

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="datas">datas</param>
        public abstract void Remove(IEnumerable<Tuple<First, Second, Third>> datas);

        /// <summary>
        /// save async
        /// </summary>
        /// <param name="datas">datas</param>
        public abstract Task RemoveAsync(IEnumerable<Tuple<First, Second, Third>> datas);

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        public abstract void Remove(IQuery query);

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        public abstract Task RemoveAsync(IQuery query);

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        public abstract void RemoveByFirst(IEnumerable<First> datas);

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        public abstract void RemoveByFirst(IQuery query);

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="record">activation record</param>
        public abstract void RemoveBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// remove by second
        /// </summary>
        /// <param name="query">query</param>
        public abstract void RemoveBySecond(IQuery query);

        /// <summary>
        /// remove by third datas
        /// </summary>
        /// <param name="datas">datas</param>
        public abstract void RemoveByThird(IEnumerable<Third> datas);

        /// <summary>
        /// remove by third
        /// </summary>
        /// <param name="query">query</param>
        public abstract void RemoveByThird(IQuery query);

        #endregion

        #region query

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract Tuple<First, Second, Third> Get(IQuery query);

        /// <summary>
        /// get relation data
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract Task<Tuple<First, Second, Third>> GetAsync(IQuery query);

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract List<Tuple<First, Second, Third>> GetList(IQuery query);

        /// <summary>
        /// get relation data list
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract Task<List<Tuple<First, Second, Third>>> GetListAsync(IQuery query);

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract IPaging<Tuple<First, Second, Third>> GetPaging(IQuery query);

        /// <summary>
        /// get relation paging
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public abstract Task<IPaging<Tuple<First, Second, Third>>> GetPagingAsync(IQuery query);

        /// <summary>
        /// get First by Second
        /// </summary>
        /// <param name="datas">second datas</param>
        /// <returns></returns>
        public abstract List<First> GetFirstListBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// get First by Second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract Task<List<First>> GetFirstListBySecondAsync(IEnumerable<Second> datas);

        /// <summary>
        /// get first by third
        /// </summary>
        /// <param name="datas">third datas</param>
        /// <returns></returns>
        public abstract List<First> GetFirstListByThird(IEnumerable<Third> datas);

        /// <summary>
        /// get first by third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract Task<List<First>> GetFirstListByThirdAsync(IEnumerable<Third> datas);

        /// <summary>
        /// get Second by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract List<Second> GetSecondListByFirst(IEnumerable<First> datas);

        /// <summary>
        /// get Second by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract Task<List<Second>> GetSecondListByFirstAsync(IEnumerable<First> datas);

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract List<Second> GetSecondListByThird(IEnumerable<Third> datas);

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract Task<List<Second>> GetSecondListByThirdAsync(IEnumerable<Third> datas);

        /// <summary>
        /// get Third by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract List<Third> GetThirdListByFirst(IEnumerable<First> datas);

        /// <summary>
        /// get Third by First
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract Task<List<Third>> GetThirdListByFirstAsync(IEnumerable<First> datas);

        /// <summary>
        /// get Third by Second
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract List<Third> GetThirdListBySecond(IEnumerable<Second> datas);

        /// <summary>
        /// get Second by Third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract Task<List<Third>> GetThirdListBySecondAsync(IEnumerable<Second> datas);

        #endregion
    }
}
