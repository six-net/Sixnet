using EZNEW.Develop.CQuery;
using EZNEW.Framework.Paging;
using EZNEW.Develop.Domain.Aggregation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.Entity;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.UnitOfWork;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// IRepository
    /// </summary>
    /// <typeparam name="T">IAggregationRoot</typeparam>
    public interface IAggregationRepository<T> where T : IAggregationRoot<T>
    {
        #region save data

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        void Save(T data, ActivationOption activationOption = null);

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void Save(IEnumerable<T> datas, ActivationOption activationOption = null);

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        Task SaveAsync(T data, ActivationOption activationOption = null);

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        Task SaveAsync(IEnumerable<T> datas, ActivationOption activationOption = null);

        #endregion

        #region remove data

        /// <summary>
        /// remove data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        void Remove(T data, ActivationOption activationOption = null);

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        void Remove(IEnumerable<T> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        Task RemoveAsync(T data, ActivationOption activationOption = null);

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        Task RemoveAsync(IEnumerable<T> datas, ActivationOption activationOption = null);

        #endregion

        #region remove by condition

        /// <summary>
        /// remove data by condition
        /// </summary>
        /// <param name="query">query model</param>
        /// <param name="activationOption">activation option</param>
        void Remove(IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// remove data by condition
        /// </summary>
        /// <param name="query">query model</param>
        /// <param name="activationOption">activation option</param>
        Task RemoveAsync(IQuery query, ActivationOption activationOption = null);

        #endregion

        #region modify

        /// <summary>
        /// modify data
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        /// <param name="activationOption">activation option</param>
        void Modify(IModify expression, IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// modify data
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        /// <param name="activationOption">activation option</param>
        Task ModifyAsync(IModify expression, IQuery query, ActivationOption activationOption = null);

        #endregion

        #region get data

        /// <summary>
        /// get data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data</returns>
        T Get(IQuery query);

        /// <summary>
        /// get data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data</returns>
        Task<T> GetAsync(IQuery query);

        #endregion

        #region get data list

        /// <summary>
        /// get data list
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data list</returns>
        List<T> GetList(IQuery query);

        /// <summary>
        /// get data list
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data list</returns>
        Task<List<T>> GetListAsync(IQuery query);

        #endregion

        #region get data paging

        /// <summary>
        /// get data paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data paging</returns>
        IPaging<T> GetPaging(IQuery query);

        /// <summary>
        /// get data paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data paging</returns>
        Task<IPaging<T>> GetPagingAsync(IQuery query);

        #endregion

        #region exist

        /// <summary>
        /// exist data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        bool Exist(IQuery query);

        /// <summary>
        /// exist data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        Task<bool> ExistAsync(IQuery query);

        #endregion

        #region get data count

        /// <summary>
        /// get data count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        long Count(IQuery query);

        /// <summary>
        /// get data count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        Task<long> CountAsync(IQuery query);

        #endregion

        #region get max value

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>max value</returns>
        DT Max<DT>(IQuery query);

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>max value</returns>
        Task<DT> MaxAsync<DT>(IQuery query);

        #endregion

        #region get min value

        /// <summary>
        /// get min value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        DT Min<DT>(IQuery query);

        /// <summary>
        /// get min value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        Task<DT> MinAsync<DT>(IQuery query);

        #endregion

        #region get sum value

        /// <summary>
        /// get sum value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum value</returns>
        DT Sum<DT>(IQuery query);

        /// <summary>
        /// get sum value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum value</returns>
        Task<DT> SumAsync<DT>(IQuery query);

        #endregion

        #region get average value

        /// <summary>
        /// get average value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average value</returns>
        DT Avg<DT>(IQuery query);

        /// <summary>
        /// get average value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average value</returns>
        Task<DT> AvgAsync<DT>(IQuery query);

        #endregion

        #region get life status

        /// <summary>
        /// get life status
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        DataLifeSource GetLifeSource(IAggregationRoot data);

        #endregion

        #region modify life source

        /// <summary>
        /// modify life source
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="lifeSource">life source</param>
        void ModifyLifeSource(IAggregationRoot data, DataLifeSource lifeSource);

        #endregion
    }
}
