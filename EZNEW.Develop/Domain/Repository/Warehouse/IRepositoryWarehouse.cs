using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository.Warehouse
{
    /// <summary>
    /// repository warehouse
    /// </summary>
    public interface IRepositoryWarehouse<ET, DAI> where ET : BaseEntity<ET>, new() where DAI : IDataAccess<ET>
    {
        #region save

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        Task<IActivationRecord> SaveAsync(IEnumerable<ET> datas, ActivationOption activationOption = null);

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        Task<IActivationRecord> SaveAsync(ET data, ActivationOption activationOption = null);

        #endregion

        #region remove

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        Task<IActivationRecord> RemoveAsync(IEnumerable<ET> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        Task<IActivationRecord> RemoveAsync(ET data, ActivationOption activationOption = null);

        /// <summary>
        /// remove by condition
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        Task<IActivationRecord> RemoveAsync(IQuery query, ActivationOption activationOption = null);

        #endregion

        #region modify

        /// <summary>
        /// modify
        /// </summary>
        /// <param name="modifyExpression">modify expression</param>
        /// <param name="query">query</param>
        /// <param name="activationOption">activation option</param>
        /// <returns></returns>
        Task<IActivationRecord> ModifyAsync(IModify modifyExpression, IQuery query, ActivationOption activationOption = null);

        #endregion

        #region query

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data</returns>
        Task<ET> GetAsync(IQuery query);

        /// <summary>
        /// query data list
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object list</returns>
        Task<List<ET>> GetListAsync(IQuery query);

        /// <summary>
        /// query data paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>object paging</returns>
        Task<IPaging<ET>> GetPagingAsync(IQuery query);

        /// <summary>
        /// exist data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        Task<bool> ExistAsync(IQuery query);

        /// <summary>
        /// Get Data Count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        Task<long> CountAsync(IQuery query);

        /// <summary>
        /// Get Max Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>max value</returns>
        Task<VT> MaxAsync<VT>(IQuery query);

        /// <summary>
        /// Get Min Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        Task<VT> MinAsync<VT>(IQuery query);

        /// <summary>
        /// Get Sum Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum value</returns>
        Task<VT> SumAsync<VT>(IQuery query);

        /// <summary>
        /// Get Average Value
        /// </summary>
        /// <typeparam name="VT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average value</returns>
        Task<VT> AvgAsync<VT>(IQuery query);

        #endregion

        #region life source

        /// <summary>
        /// get life source
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        DataLifeSource GetLifeSource(ET data);

        /// <summary>
        /// modify life source
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="lifeSource">life source</param>
        void ModifyLifeSource(ET data, DataLifeSource lifeSource);

        #endregion
    }
}
