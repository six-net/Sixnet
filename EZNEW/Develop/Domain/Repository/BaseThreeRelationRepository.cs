using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Paging;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Base three relation repository
    /// </summary>
    /// <typeparam name="TFirstModel">The first relation model</typeparam>
    /// <typeparam name="TSecondModel">The second relation model</typeparam>
    /// <typeparam name="TThirdModel">The third relation model</typeparam>
    public abstract class BaseThreeRelationRepository<TFirstModel, TSecondModel, TThirdModel>
    {
        #region Save

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void Save(IEnumerable<Tuple<TFirstModel, TSecondModel, TThirdModel>> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save by first type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void SaveByFirst(IEnumerable<TFirstModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save by second type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void SaveBySecond(IEnumerable<TSecondModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save by third type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void SaveByThird(IEnumerable<TThirdModel> datas, ActivationOptions activationOptions = null);

        #endregion

        #region Remove

        /// <summary>
        /// Remove datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void Remove(IEnumerable<Tuple<TFirstModel, TSecondModel, TThirdModel>> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void Remove(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by first datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByFirst(IEnumerable<TFirstModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by first
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByFirst(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by second datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveBySecond(IEnumerable<TSecondModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by second
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveBySecond(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by third datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByThird(IEnumerable<TThirdModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by third
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByThird(IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Query

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        public abstract Tuple<TFirstModel, TSecondModel, TThirdModel> Get(IQuery query);

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        public abstract Task<Tuple<TFirstModel, TSecondModel, TThirdModel>> GetAsync(IQuery query);

        /// <summary>
        /// Get relation data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return datas</returns>
        public abstract List<Tuple<TFirstModel, TSecondModel, TThirdModel>> GetList(IQuery query);

        /// <summary>
        /// Get relation data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return datas</returns>
        public abstract Task<List<Tuple<TFirstModel, TSecondModel, TThirdModel>>> GetListAsync(IQuery query);

        /// <summary>
        /// Get relation paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public abstract IPaging<Tuple<TFirstModel, TSecondModel, TThirdModel>> GetPaging(IQuery query);

        /// <summary>
        /// Get relation paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        public abstract Task<IPaging<Tuple<TFirstModel, TSecondModel, TThirdModel>>> GetPagingAsync(IQuery query);

        /// <summary>
        /// Get First by Second
        /// </summary>
        /// <param name="datas">second datas</param>
        /// <returns></returns>
        public abstract List<TFirstModel> GetFirstListBySecond(IEnumerable<TSecondModel> datas);

        /// <summary>
        /// Get First by Second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return data paging</returns>
        public abstract Task<List<TFirstModel>> GetFirstListBySecondAsync(IEnumerable<TSecondModel> datas);

        /// <summary>
        /// Get first by third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return data paging</returns>
        public abstract List<TFirstModel> GetFirstListByThird(IEnumerable<TThirdModel> datas);

        /// <summary>
        /// Get first by third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public abstract Task<List<TFirstModel>> GetFirstListByThirdAsync(IEnumerable<TThirdModel> datas);

        /// <summary>
        /// Get Second by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public abstract List<TSecondModel> GetSecondListByFirst(IEnumerable<TFirstModel> datas);

        /// <summary>
        /// Get Second by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public abstract Task<List<TSecondModel>> GetSecondListByFirstAsync(IEnumerable<TFirstModel> datas);

        /// <summary>
        /// Get Second by Third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public abstract List<TSecondModel> GetSecondListByThird(IEnumerable<TThirdModel> datas);

        /// <summary>
        /// Get Second by Third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public abstract Task<List<TSecondModel>> GetSecondListByThirdAsync(IEnumerable<TThirdModel> datas);

        /// <summary>
        /// Get Third by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public abstract List<TThirdModel> GetThirdListByFirst(IEnumerable<TFirstModel> datas);

        /// <summary>
        /// Get Third by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public abstract Task<List<TThirdModel>> GetThirdListByFirstAsync(IEnumerable<TFirstModel> datas);

        /// <summary>
        /// Get Third by Second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public abstract List<TThirdModel> GetThirdListBySecond(IEnumerable<TSecondModel> datas);

        /// <summary>
        /// Get Second by Third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        public abstract Task<List<TThirdModel>> GetThirdListBySecondAsync(IEnumerable<TSecondModel> datas);

        #endregion
    }
}
