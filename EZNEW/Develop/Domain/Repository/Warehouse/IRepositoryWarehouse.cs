using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Paging;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// Repository data warehouse contract
    /// </summary>
    public interface IRepositoryWarehouse<TEntity, TDataAccess> where TEntity : BaseEntity<TEntity>, new() where TDataAccess : IDataAccess<TEntity>
    {
        #region Save

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        IActivationRecord Save(IEnumerable<TEntity> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        IActivationRecord Save(TEntity data, ActivationOptions activationOptions = null);

        #endregion

        #region Remove

        /// <summary>
        /// Remove datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        IActivationRecord Remove(IEnumerable<TEntity> datas, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        IActivationRecord Remove(TEntity data, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        IActivationRecord Remove(IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Modify

        /// <summary>
        /// Modify
        /// </summary>
        /// <param name="modifyExpression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return the activation record</returns>
        IActivationRecord Modify(IModify modifyExpression, IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Query

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        Task<TEntity> GetAsync(IQuery query);

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        Task<List<TEntity>> GetListAsync(IQuery query);

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        Task<IPaging<TEntity>> GetPagingAsync(IQuery query);

        /// <summary>
        /// Exist data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        Task<bool> ExistAsync(IQuery query);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        Task<long> CountAsync(IQuery query);

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        Task<TValue> MaxAsync<TValue>(IQuery query);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        Task<TValue> MinAsync<TValue>(IQuery query);

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        Task<TValue> SumAsync<TValue>(IQuery query);

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        Task<TValue> AvgAsync<TValue>(IQuery query);

        #endregion

        #region Life source

        /// <summary>
        /// Get life source
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return the activation record</returns>
        DataLifeSource GetLifeSource(TEntity data);

        /// <summary>
        /// Modify life source
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="lifeSource">Life source</param>
        void ModifyLifeSource(TEntity data, DataLifeSource lifeSource);

        #endregion
    }
}
