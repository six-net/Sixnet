using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Development.Query;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Entity;
using EZNEW.Development.UnitOfWork;

namespace EZNEW.Development.Domain.Repository
{
    /// <summary>
    /// Defines base three relation repository
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    /// <typeparam name="TFirstRelationModel">The first relation model</typeparam>
    /// <typeparam name="TSecondRelationModel">The second relation model</typeparam>
    /// <typeparam name="TThirdRelationModel">The third relation model</typeparam>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TDataAccess">Data access</typeparam>
    public abstract class BaseThreeRelationRepository<TModel, TFirstRelationModel, TSecondRelationModel, TThirdRelationModel, TEntity, TDataAccess>
        : DefaultRepository<TModel, TEntity, TDataAccess>
        where TModel : class, IModel<TModel>
        where TEntity : BaseEntity<TEntity>, new()
        where TDataAccess : IDataAccess<TEntity>
    {
        #region Query

        /// <summary>
        /// Get list by first
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return data list</returns>
        public abstract List<TModel> GetListByFirst(IEnumerable<TFirstRelationModel> datas);

        /// <summary>
        /// Get list by first
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return data list</returns>
        public abstract Task<List<TModel>> GetListByFirstAsync(IEnumerable<TFirstRelationModel> datas);

        /// <summary>
        /// Get list by second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return data list</returns>
        public abstract List<TModel> GetListBySecond(IEnumerable<TSecondRelationModel> datas);

        /// <summary>
        /// Get list by second
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return data list</returns>
        public abstract Task<List<TModel>> GetListBySecondAsync(IEnumerable<TSecondRelationModel> datas);

        /// <summary>
        /// Get list by third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return data list</returns>
        public abstract List<TModel> GetListByThird(IEnumerable<TThirdRelationModel> datas);

        /// <summary>
        /// Get list by third
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return data list</returns>
        public abstract Task<List<TModel>> GetListByThirdAsync(IEnumerable<TThirdRelationModel> datas);

        #endregion

        #region Remove

        /// <summary>
        /// Remove by first datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByFirst(IEnumerable<TFirstRelationModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by second datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <<param name="activationOptions">Activation options</param>
        public abstract void RemoveBySecond(IEnumerable<TSecondRelationModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by third datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByThird(IEnumerable<TThirdRelationModel> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by first
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByFirst(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by second
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveBySecond(IQuery query, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by third
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        public abstract void RemoveByThird(IQuery query, ActivationOptions activationOptions = null);

        #endregion
    }
}
