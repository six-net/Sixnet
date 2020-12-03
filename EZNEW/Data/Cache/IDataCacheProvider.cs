using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.Entity;
using EZNEW.Paging;
using EZNEW.Data.Cache.Command;

namespace EZNEW.Data.Cache
{
    /// <summary>
    /// Data cache provider contract
    /// </summary>
    public interface IDataCacheProvider
    {
        #region Add 

        /// <summary>
        /// Set data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return a ICommand object</returns>
        ICommand Add<T>(AddDataCacheCommand<T> command) where T : BaseEntity<T>, new();

        #endregion

        #region Check data

        /// <summary>
        /// Check whether data is existed
        /// </summary>
        /// <param name="command">Cache action command</param>
        /// <returns>Return determine data whether is exist</returns>
        bool Exist<T>(ExistDataCacheCommand<T> command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Check whether data is existed
        /// </summary>
        /// <param name="command">Cache action command</param>
        /// <returns>Return determine data whether is exist</returns>
        Task<bool> ExistAsync<T>(ExistDataCacheCommand<T> command) where T : BaseEntity<T>, new();

        #endregion

        #region Get data

        /// <summary>
        /// Get data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data object</returns>
        T Get<T>(GetDataCacheCommand<T> command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Get data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data object</returns>
        Task<T> GetAsync<T>(GetDataCacheCommand<T> command) where T : BaseEntity<T>, new();

        #endregion

        #region Get data list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data list</returns>
        List<T> GetList<T>(GetDataListCacheCommand<T> command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data list</returns>
        Task<List<T>> GetListAsync<T>(GetDataListCacheCommand<T> command) where T : BaseEntity<T>, new();

        #endregion

        #region Query paging

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data paging</returns>
        IPaging<T> GetPaging<T>(GetDataPagingCacheCommand<T> command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Get data paging
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return data paging</returns>
        Task<IPaging<T>> GetPagingAsync<T>(GetDataPagingCacheCommand<T> command) where T : BaseEntity<T>, new();

        #endregion

        #region Modify data

        /// <summary>
        /// Modify data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Data modify command</param>
        /// <returns>Return a ICommand object</returns>
        ICommand Modify<T>(ModifyDataCacheCommand<T> command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Modify data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return a ICommand object</returns>
        ICommand Modify<T>(ModifyByConditionCacheCommand<T> command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Modify data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return a ICommand object</returns>
        ICommand Modify<T>(ModifyDataByConditionCacheCommand<T> command) where T : BaseEntity<T>, new();

        #endregion

        #region Remove data

        /// <summary>
        /// Remove data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return a ICommand object</returns>
        ICommand Remove<T>(RemoveByConditionCacheCommand<T> command) where T : BaseEntity<T>, new();

        /// <summary>
        /// Remove data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="command">Cache action command</param>
        /// <returns>Return a ICommand object</returns>
        ICommand Remove<T>(RemoveDataCacheCommand<T> command) where T : BaseEntity<T>, new();

        #endregion
    }
}
