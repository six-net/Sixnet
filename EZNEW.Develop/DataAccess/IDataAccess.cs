using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataAccess
{
    /// <summary>
    /// data access
    /// </summary>
    public interface IDataAccess<T>
    {
        #region add

        /// <summary>
        /// add data
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>icommand</returns>
        ICommand Add(T obj);

        /// <summary>
        /// add data
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>icommand</returns>
        Task<ICommand> AddAsync(T obj);

        /// <summary>
        /// add data list
        /// </summary>
        /// <param name="objList">object list</param>
        /// <returns>icommand list</returns>
        List<ICommand> Add(IEnumerable<T> objList);

        /// <summary>
        /// add data list
        /// </summary>
        /// <param name="objList">object list</param>
        /// <returns>icommand list</returns>
        Task<List<ICommand>> AddAsync(IEnumerable<T> objList);

        #endregion

        #region edit data

        /// <summary>
        /// edit data
        /// </summary>
        /// <param name="newData">new data</param>
        /// <param name="oldData">old data</param>
        /// <returns></returns>
        ICommand Modify(T newData, T oldData);

        /// <summary>
        /// edit data
        /// </summary>
        /// <param name="newData">new data</param>
        /// <param name="oldData">old data</param>
        /// <returns></returns>
        Task<ICommand> ModifyAsync(T newData, T oldData);

        /// <summary>
        /// edit data
        /// </summary>
        /// <param name="newData">new data</param>
        /// <param name="oldData">old data</param>
        /// <param name="query">query object</param>
        /// <returns>ICommand object</returns>
        ICommand Modify(T newData, T oldData, IQuery query);

        /// <summary>
        /// edit data
        /// </summary>
        /// <param name="newData">new data</param>
        /// <param name="oldData">old data</param>
        /// <param name="query">query object</param>
        /// <returns>ICommand object</returns>
        Task<ICommand> ModifyAsync(T newData, T oldData, IQuery query);

        /// <summary>
        /// edit data with expression
        /// </summary>
        /// <param name="modifyExpression">modify expression</param>
        /// <param name="query">query object</param>
        /// <returns>ICommand object</returns>
        ICommand Modify(IModify modifyExpression, IQuery query);

        /// <summary>
        /// edit data with expression
        /// </summary>
        /// <param name="modifyExpression">modify expression</param>
        /// <param name="query">query object</param>
        /// <returns>ICommand object</returns>
        Task<ICommand> ModifyAsync(IModify modifyExpression, IQuery query);

        #endregion

        #region delete data

        /// <summary>
        /// delete obj
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        ICommand Delete(T obj);

        /// <summary>
        /// delete obj
        /// </summary>
        /// <param name="obj">obj</param>
        /// <returns></returns>
        Task<ICommand> DeleteAsync(T obj);

        /// <summary>
        /// delete data
        /// </summary>
        /// <param name="query">delete query</param>
        /// <returns>ICommand object</returns>
        ICommand Delete(IQuery query);

        /// <summary>
        /// delete data
        /// </summary>
        /// <param name="query">delete query</param>
        /// <returns>ICommand object</returns>
        Task<ICommand> DeleteAsync(IQuery query);

        #endregion

        #region query data

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data</returns>
        T Get(IQuery query);

        /// <summary>
        /// query data
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data</returns>
        Task<T> GetAsync(IQuery query);

        /// <summary>
        /// query data list
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data list</returns>
        List<T> GetList(IQuery query);

        /// <summary>
        /// query data list
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data list</returns>
        Task<List<T>> GetListAsync(IQuery query);

        /// <summary>
        /// query data with paging
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data paging</returns>
        IPaging<T> GetPaging(IQuery query);

        /// <summary>
        /// query data with paging
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data paging</returns>
        Task<IPaging<T>> GetPagingAsync(IQuery query);

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>whether data is exist</returns>
        bool Exist(IQuery query);

        /// <summary>
        /// determine whether data is exist
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>whether data is exist</returns>
        Task<bool> ExistAsync(IQuery query);

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>max value</returns>
        DT Max<DT>(IQuery query);

        /// <summary>
        /// get max value
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>max value</returns>
        Task<DT> MaxAsync<DT>(IQuery query);

        /// <summary>
        /// get minvalue
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>minvalue</returns>
        DT Min<DT>(IQuery query);

        /// <summary>
        /// get minvalue
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>minvalue</returns>
        Task<DT> MinAsync<DT>(IQuery query);

        /// <summary>
        /// caculate sum
        /// </summary>
        /// <typeparam name="DT">data value</typeparam>
        /// <param name="query">query value</param>
        /// <returns>caculated value</returns>
        DT Sum<DT>(IQuery query);

        /// <summary>
        /// caculate sum
        /// </summary>
        /// <typeparam name="DT">data value</typeparam>
        /// <param name="query">query value</param>
        /// <returns>caculated value</returns>
        Task<DT> SumAsync<DT>(IQuery query);

        /// <summary>
        /// caculate average
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>average value</returns>
        DT Avg<DT>(IQuery query);

        /// <summary>
        /// caculate average
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="query">query object</param>
        /// <returns>average value</returns>
        Task<DT> AvgAsync<DT>(IQuery query);

        /// <summary>
        /// caculate count
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data count</returns>
        long Count(IQuery query);

        /// <summary>
        /// caculate count
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>data count</returns>
        Task<long> CountAsync(IQuery query);

        #endregion
    }
}
