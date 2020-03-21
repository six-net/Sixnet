using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Develop.UnitOfWork;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Repository
{
    /// <summary>
    /// base aggregation repository
    /// </summary>
    public abstract class BaseAggregationRepository<T>
    {
        #region save data

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        public abstract void Save(T data, ActivationOption activationOption = null);

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public abstract void Save(IEnumerable<T> datas, ActivationOption activationOption = null);

        /// <summary>
        /// save data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        public abstract Task SaveAsync(T data, ActivationOption activationOption = null);

        /// <summary>
        /// save datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public abstract Task SaveAsync(IEnumerable<T> datas, ActivationOption activationOption = null);

        #endregion

        #region remove data

        /// <summary>
        /// remove data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        public abstract void Remove(T data, ActivationOption activationOption = null);

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public abstract void Remove(IEnumerable<T> datas, ActivationOption activationOption = null);

        /// <summary>
        /// remove data
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="activationOption">activation option</param>
        public abstract Task RemoveAsync(T data, ActivationOption activationOption = null);

        /// <summary>
        /// remove datas
        /// </summary>
        /// <param name="datas">datas</param>
        /// <param name="activationOption">activation option</param>
        public abstract Task RemoveAsync(IEnumerable<T> datas, ActivationOption activationOption = null);

        #endregion

        #region remove by condition

        /// <summary>
        /// remove data by condition
        /// </summary>
        /// <param name="query">query model</param>
        /// <param name="activationOption">activation option</param>
        public abstract void Remove(IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// remove data by condition
        /// </summary>
        /// <param name="query">query model</param>
        /// <param name="activationOption">activation option</param>
        public abstract Task RemoveAsync(IQuery query, ActivationOption activationOption = null);

        #endregion

        #region modify

        /// <summary>
        /// modify data
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        /// <param name="activationOption">activation option</param>
        public abstract void Modify(IModify expression, IQuery query, ActivationOption activationOption = null);

        /// <summary>
        /// modify data
        /// </summary>
        /// <param name="expression">modify expression</param>
        /// <param name="query">query model</param>
        /// <param name="activationOption">activation option</param>
        public abstract Task ModifyAsync(IModify expression, IQuery query, ActivationOption activationOption = null);

        #endregion

        #region get data

        /// <summary>
        /// get data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data</returns>
        public abstract T Get(IQuery query);

        /// <summary>
        /// get data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data</returns>
        public abstract Task<T> GetAsync(IQuery query);

        #endregion

        #region get data list

        /// <summary>
        /// get data list
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data list</returns>
        public abstract List<T> GetList(IQuery query);

        /// <summary>
        /// get data list
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data list</returns>
        public abstract Task<List<T>> GetListAsync(IQuery query);

        #endregion

        #region get data paging

        /// <summary>
        /// get data paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data paging</returns>
        public abstract IPaging<T> GetPaging(IQuery query);

        /// <summary>
        /// get data paging
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns>data paging</returns>
        public abstract Task<IPaging<T>> GetPagingAsync(IQuery query);

        #endregion

        #region exist

        /// <summary>
        /// exist data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public abstract bool Exist(IQuery query);

        /// <summary>
        /// exist data
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public abstract Task<bool> ExistAsync(IQuery query);

        #endregion

        #region get data count

        /// <summary>
        /// get data count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public abstract long Count(IQuery query);

        /// <summary>
        /// get data count
        /// </summary>
        /// <param name="query">query model</param>
        /// <returns></returns>
        public abstract Task<long> CountAsync(IQuery query);

        #endregion

        #region get max value

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>max value</returns>
        public abstract DT Max<DT>(IQuery query);

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>max value</returns>
        public abstract Task<DT> MaxAsync<DT>(IQuery query);

        #endregion

        #region get min value

        /// <summary>
        /// get min value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        public abstract DT Min<DT>(IQuery query);

        /// <summary>
        /// get min value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>min value</returns>
        public abstract Task<DT> MinAsync<DT>(IQuery query);

        #endregion

        #region get sum value

        /// <summary>
        /// get sum value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum value</returns>
        public abstract DT Sum<DT>(IQuery query);

        /// <summary>
        /// get sum value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>sum value</returns>
        public abstract Task<DT> SumAsync<DT>(IQuery query);

        #endregion

        #region get average value

        /// <summary>
        /// get average value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average value</returns>
        public abstract DT Avg<DT>(IQuery query);

        /// <summary>
        /// get average value
        /// </summary>
        /// <typeparam name="DT">DataType</typeparam>
        /// <param name="query">query model</param>
        /// <returns>average value</returns>
        public abstract Task<DT> AvgAsync<DT>(IQuery query);

        #endregion

        #region get life status

        /// <summary>
        /// get life status
        /// </summary>
        /// <param name="data">data</param>
        /// <returns></returns>
        public abstract DataLifeSource GetLifeSource(IAggregationRoot data);

        #endregion

        #region modify life source

        /// <summary>
        /// modify life source
        /// </summary>
        /// <param name="data">data</param>
        /// <param name="lifeSource">life source</param>
        public abstract void ModifyLifeSource(IAggregationRoot data, DataLifeSource lifeSource);

        #endregion
    }
}
