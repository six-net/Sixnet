using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Paging;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Three relation repository contract
    /// </summary>
    /// <typeparam name="TFirstModel">The first model</typeparam>
    /// <typeparam name="TSecondModel">The second model</typeparam>
    /// <typeparam name="TThirdModel">The third model</typeparam>
    public interface IThreeRelationRepository<TFirstModel, TSecondModel, TThirdModel> where TFirstModel : IAggregationRoot<TFirstModel> where TSecondModel : IAggregationRoot<TSecondModel> where TThirdModel : IAggregationRoot<TThirdModel>
    {
        #region Save

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void Save(IEnumerable<Tuple<TFirstModel, TSecondModel, TThirdModel>> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save by first type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void SaveByFirst(IEnumerable<TFirstModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save by second type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void SaveBySecond(IEnumerable<TSecondModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save by third type datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void SaveByThird(IEnumerable<TThirdModel> datas, ActivationOptions activationOptions = null);

        #endregion

        #region Remove

        /// <summary>
        /// Remove datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void Remove(IEnumerable<Tuple<TFirstModel, TSecondModel, TThirdModel>> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void Remove(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by first datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveByFirst(IEnumerable<TFirstModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by first
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveByFirst(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by second datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveBySecond(IEnumerable<TSecondModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by second
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveBySecond(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by third datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveByThird(IEnumerable<TThirdModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by third
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveByThird(IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Query

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation data</returns>
        Tuple<TFirstModel, TSecondModel, TThirdModel> Get(IQuery query);

        /// <summary>
        /// Get relation data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation data</returns>
        Task<Tuple<TFirstModel, TSecondModel, TThirdModel>> GetAsync(IQuery query);

        /// <summary>
        /// Get relation data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return relation datas</returns>
        List<Tuple<TFirstModel, TSecondModel, TThirdModel>> GetList(IQuery query);

        /// <summary>
        /// Get relation data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return ralation datas</returns>
        Task<List<Tuple<TFirstModel, TSecondModel, TThirdModel>>> GetListAsync(IQuery query);

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        IPaging<Tuple<TFirstModel, TSecondModel, TThirdModel>> GetPaging(IQuery query);

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        Task<IPaging<Tuple<TFirstModel, TSecondModel, TThirdModel>>> GetPagingAsync(IQuery query);

        /// <summary>
        /// Get First by Second
        /// </summary>
        /// <param name="datas">second datas</param>
        /// <returns>Return datas</returns>
        List<TFirstModel> GetFirstListBySecond(IEnumerable<TSecondModel> datas);

        /// <summary>
        /// Get First by Second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        Task<List<TFirstModel>> GetFirstListBySecondAsync(IEnumerable<TSecondModel> datas);

        /// <summary>
        /// Get first by third
        /// </summary>
        /// <param name="datas">third datas</param>
        /// <returns>Return datas</returns>
        List<TFirstModel> GetFirstListByThird(IEnumerable<TThirdModel> datas);

        /// <summary>
        /// Get first by third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        Task<List<TFirstModel>> GetFirstListByThirdAsync(IEnumerable<TThirdModel> datas);

        /// <summary>
        /// Get Second by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        List<TSecondModel> GetSecondListByFirst(IEnumerable<TFirstModel> datas);

        /// <summary>
        /// Get Second by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        Task<List<TSecondModel>> GetSecondListByFirstAsync(IEnumerable<TFirstModel> datas);

        /// <summary>
        /// Get Second by Third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        List<TSecondModel> GetSecondListByThird(IEnumerable<TThirdModel> datas);

        /// <summary>
        /// Get Second by Third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        Task<List<TSecondModel>> GetSecondListByThirdAsync(IEnumerable<TThirdModel> datas);

        /// <summary>
        /// Get Third by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        List<TThirdModel> GetThirdListByFirst(IEnumerable<TFirstModel> datas);

        /// <summary>
        /// Get Third by First
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        Task<List<TThirdModel>> GetThirdListByFirstAsync(IEnumerable<TFirstModel> datas);

        /// <summary>
        /// Get Third by Second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        List<TThirdModel> GetThirdListBySecond(IEnumerable<TSecondModel> datas);

        /// <summary>
        /// Get Second by Third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        Task<List<TThirdModel>> GetThirdListBySecondAsync(IEnumerable<TSecondModel> datas);

        #endregion
    }
}
