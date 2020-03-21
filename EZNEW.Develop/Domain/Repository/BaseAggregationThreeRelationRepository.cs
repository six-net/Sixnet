using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    public abstract class BaseAggregationThreeRelationRepository<T, First, Second, Third, ET, DAI> : DefaultAggregationRepository<T, ET, DAI> where T : IAggregationRoot<T> where ET : BaseEntity<ET>, new() where DAI : IDataAccess<ET>
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

        /// <summary>
        /// get list by third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract List<T> GetListByThird(IEnumerable<Third> datas);

        /// <summary>
        /// get list by third
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns></returns>
        public abstract Task<List<T>> GetListByThirdAsync(IEnumerable<Third> datas);

        #endregion

        #region remove

        /// <summary>
        /// remove by first datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public abstract void RemoveByFirst(IEnumerable<First> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove by second datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <<param name="activationOption">activation option</param>
        public abstract void RemoveBySecond(IEnumerable<Second> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove by third datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public abstract void RemoveByThird(IEnumerable<Third> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove by first
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        public abstract void RemoveByFirst(IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// remove by second
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        public abstract void RemoveBySecond(IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// remove by third
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        public abstract void RemoveByThird(IQuery query, ActivationOption activationOption = null);

        #endregion
    }
}
