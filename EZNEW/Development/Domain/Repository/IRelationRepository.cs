using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Development.Query;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.UnitOfWork;

namespace EZNEW.Development.Domain.Repository
{
    /// <summary>
    /// Defines relation repository contract
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    /// <typeparam name="TFirstRelationModel">The first relation model</typeparam>
    /// <typeparam name="TSecondRelationModel">The second relation model</typeparam>
    public interface IRelationRepository<TModel, TFirstRelationModel, TSecondRelationModel> : IRepository<TModel> where TModel : IModel<TModel> where TSecondRelationModel : IModel<TSecondRelationModel> where TFirstRelationModel : IModel<TFirstRelationModel>
    {
        #region Query

        /// <summary>
        /// Get list by first
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        List<TModel> GetListByFirst(IEnumerable<TFirstRelationModel> datas);

        /// <summary>
        /// Get list by first
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        Task<List<TModel>> GetListByFirstAsync(IEnumerable<TFirstRelationModel> datas);

        /// <summary>
        /// Get list by second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        List<TModel> GetListBySecond(IEnumerable<TSecondRelationModel> datas);

        /// <summary>
        /// Get list by second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return datas</returns>
        Task<List<TModel>> GetListBySecondAsync(IEnumerable<TSecondRelationModel> datas);

        #endregion

        #region Remove

        /// <summary>
        /// Remove by first datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveByFirst(IEnumerable<TFirstRelationModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by second datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveBySecond(IEnumerable<TSecondRelationModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by first
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveByFirst(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by second
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void RemoveBySecond(IQuery query, ActivationOptions activationOptions = null);

        #endregion
    }
}
