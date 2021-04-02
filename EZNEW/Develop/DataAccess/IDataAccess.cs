using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataAccess
{
    /// <summary>
    /// Data access contract
    /// </summary>
    public interface IDataAccess<T>
    {
        #region Add

        /// <summary>
        /// Add data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return add command</returns>
        ICommand Add(T data);

        /// <summary>
        /// Add datas
        /// </summary>
        /// <param name="datas">Datas</param>
        /// <returns>Return add commands</returns>
        List<ICommand> Add(IEnumerable<T> datas);

        #endregion

        #region Modify data

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="oldData">Old data</param>
        /// <returns>Return modify command</returns>
        ICommand Modify(T newData, T oldData);

        /// <summary>
        /// Modify data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="oldData">Old data</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modify command</returns>
        ICommand Modify(T newData, T oldData, IQuery query);

        /// <summary>
        /// Modify data with expression
        /// </summary>
        /// <param name="modifyExpression">Modify expression</param>
        /// <param name="query">Query object</param>
        /// <returns>Return modify command</returns>
        ICommand Modify(IModify modifyExpression, IQuery query);

        #endregion

        #region Delete data

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return delete data command</returns>
        ICommand Delete(T data);

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return delete command</returns>
        ICommand Delete(IQuery query);

        #endregion

        #region Query data

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        T Get(IQuery query);

        /// <summary>
        /// Query data
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data</returns>
        Task<T> GetAsync(IQuery query);

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        List<T> GetList(IQuery query);

        /// <summary>
        /// Query data list
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data list</returns>
        Task<List<T>> GetListAsync(IQuery query);

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        PagingInfo<T> GetPaging(IQuery query);

        /// <summary>
        /// Query data paging
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return data paging</returns>
        Task<PagingInfo<T>> GetPagingAsync(IQuery query);

        /// <summary>
        /// Determine whether data is exist
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        bool Exist(IQuery query);

        /// <summary>
        /// Determine whether data is exist
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return whether data is exist</returns>
        Task<bool> ExistAsync(IQuery query);

        /// <summary>
        /// Get max value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the max value</returns>
        TValue Max<TValue>(IQuery query);

        /// <summary>
        /// Get max value
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
        /// <returns>Return the min value</returns>
        TValue Min<TValue>(IQuery query);

        /// <summary>
        /// Get min value
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the min value</returns>
        Task<TValue> MinAsync<TValue>(IQuery query);

        /// <summary>
        /// Caculate sum
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query value</param>
        /// <returns>Return the caculated value</returns>
        TValue Sum<TValue>(IQuery query);

        /// <summary>
        /// Caculate sum
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the caculated value</returns>
        Task<TValue> SumAsync<TValue>(IQuery query);

        /// <summary>
        /// Caculate average
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        TValue Avg<TValue>(IQuery query);

        /// <summary>
        /// Caculate average
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="query">Query object</param>
        /// <returns>Return the average value</returns>
        Task<TValue> AvgAsync<TValue>(IQuery query);

        /// <summary>
        /// Caculate count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the data count</returns>
        long Count(IQuery query);

        /// <summary>
        /// Caculate count
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return the data count</returns>
        Task<long> CountAsync(IQuery query);

        #endregion
    }
}
