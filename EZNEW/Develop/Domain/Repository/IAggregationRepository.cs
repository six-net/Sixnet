using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Develop.CQuery;
using EZNEW.Paging;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Response;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// Aggregation repository contract
    /// </summary>
    /// <typeparam name="TModel">Aggregation model</typeparam>
    public interface IAggregationRepository<TModel> where TModel : IAggregationRoot<TModel>
    {
        #region Save data

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        TModel Save(TModel data, ActivationOptions activationOptions = null);

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        List<TModel> Save(IEnumerable<TModel> datas, ActivationOptions activationOptions = null);

        #endregion

        #region Remove data

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="activationOptions">Activation options</param>
        void Remove(TModel data, ActivationOptions activationOptions = null);

        /// <summary>
        /// Remove datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <param name="activationOptions">Activation options</param>
        void Remove(IEnumerable<TModel> datas, ActivationOptions activationOptions = null);

        #endregion

        #region Remove by condition

        /// <summary>
        /// Remove data by condition
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void Remove(IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Modify

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="expression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        void Modify(IModify expression, IQuery query, ActivationOptions activationOptions = null);

        #endregion

        #region Get data

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        TModel Get(IQuery query);

        /// <summary>
        /// Get data by current data
        /// </summary>
        /// <param name="data">Current data</param>
        /// <returns>Return data</returns>
        TModel Get(TModel data);

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        Task<TModel> GetAsync(IQuery query);

        /// <summary>
        /// Get data by current data
        /// </summary>
        /// <param name="data">Current data</param>
        /// <returns>Return data</returns>
        Task<TModel> GetAsync(TModel data);

        #endregion

        #region Get data list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        List<TModel> GetList(IQuery query);

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        Task<List<TModel>> GetListAsync(IQuery query);

        #endregion

        #region Get data paging

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        IPaging<TModel> GetPaging(IQuery query);

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        Task<IPaging<TModel>> GetPagingAsync(IQuery query);

        #endregion

        #region Exist

        /// <summary>
        /// Exist data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        bool Exist(IQuery query);

        /// <summary>
        /// Exist data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        Task<bool> ExistAsync(IQuery query);

        #endregion

        #region Get data count

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        long Count(IQuery query);

        /// <summary>
        /// Get data count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data count</returns>
        Task<long> CountAsync(IQuery query);

        #endregion

        #region Get max value

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Vata type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        TValue Max<TValue>(IQuery query);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Vata type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        Task<TValue> MaxAsync<TValue>(IQuery query);

        #endregion

        #region Get min value

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Vata type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return min value</returns>
        TValue Min<TValue>(IQuery query);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Vata type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return min value</returns>
        Task<TValue> MinAsync<TValue>(IQuery query);

        #endregion

        #region Get sum value

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Vata type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        TValue Sum<TValue>(IQuery query);

        /// <summary>
        /// Get sum value
        /// </summary>
        /// <typeparam name="TValue">Vata type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the sum value</returns>
        Task<TValue> SumAsync<TValue>(IQuery query);

        #endregion

        #region Get average value

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Vata type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        TValue Avg<TValue>(IQuery query);

        /// <summary>
        /// Get average value
        /// </summary>
        /// <typeparam name="TValue">Vata type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        Task<TValue> AvgAsync<TValue>(IQuery query);

        #endregion

        #region Get life status

        /// <summary>
        /// Get life status
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return the data life source</returns>
        DataLifeSource GetLifeSource(IAggregationRoot data);

        #endregion

        #region Modify life source

        /// <summary>
        /// Modify life source
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="lifeSource">Life source</param>
        void ModifyLifeSource(IAggregationRoot data, DataLifeSource lifeSource);

        #endregion
    }
}
