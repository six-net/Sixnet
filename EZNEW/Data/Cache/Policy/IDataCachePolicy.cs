using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Develop.Entity;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Data cache policy
    /// </summary>
    public interface IDataCachePolicy
    {
        #region Starting

        /// <summary>
        /// Called before a add data database command execute
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="addDataContext">Add data context</param>
        /// <returns>Return policy result</returns>
        StartingResult OnAddStarting<T>(AddDataContext<T> addDataContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called before a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeDataContext">Remove data context</param>
        /// <returns>Return policy result</returns>
        StartingResult OnRemoveStarting<T>(RemoveDataContext<T> removeDataContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called before a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeByQueryContext">Remove by query context</param>
        /// <returns>Return policy result</returns>
        StartingResult OnRemoveByQueryStarting<T>(RemoveByQueryContext<T> removeByQueryContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called before a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeAllContext">Remove all context</param>
        /// <returns>Return policy result</returns>
        StartingResult OnRemoveAllStarting<T>(RemoveAllContext<T> removeAllContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called before a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateByQueryContext">Update by query context</param>
        /// <returns>Return policy result</returns>
        StartingResult OnUpdateByQueryStarting<T>(UpdateByQueryContext<T> updateByQueryContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called before a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateAllContext">Update all context</param>
        /// <returns>Return policy result</returns>
        StartingResult OnUpdateAllStarting<T>(UpdateAllContext<T> updateAllContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called before a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateDataContext">Update data context</param>
        /// <returns>Return policy result</returns>
        StartingResult OnUpdateStarting<T>(UpdateDataContext<T> updateDataContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called before a query command execute
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="queryDataContext">Query data context</param>
        /// <returns>Return query result</returns>
        QueryDataResult<T> OnQueryStarting<T>(QueryDataContext<T> queryDataContext) where T : BaseEntity<T>, new();

        #endregion

        #region Callback

        /// <summary>
        /// Called after a add data database command execute
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="addDataContext">Add data context</param>
        void OnAddCallback<T>(AddDataContext<T> addDataContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called after a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeDataContext">Remove data context</param>
        void OnRemoveCallback<T>(RemoveDataContext<T> removeDataContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called after a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeByQueryContext">Remove by query context</param>
        void OnRemoveByQueryCallback<T>(RemoveByQueryContext<T> removeByQueryContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called after a remove database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="removeAllContext">Remove all context</param>
        void OnRemoveAllCallback<T>(RemoveAllContext<T> removeAllContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called after a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateByQueryContext">Update by query context</param>
        void OnUpdateByQueryCallback<T>(UpdateByQueryContext<T> updateByQueryContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called after a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateAllContext">Update all context</param>
        void OnUpdateAllCallback<T>(UpdateAllContext<T> updateAllContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called after a update database command execute 
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="updateDataContext">Update data context</param>
        void OnUpdateCallback<T>(UpdateDataContext<T> updateDataContext) where T : BaseEntity<T>, new();

        /// <summary>
        /// Called after a query command execute
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="queryDataContext">Query data callback context</param>
        void OnQueryCallback<T>(QueryDataCallbackContext<T> queryDataCallbackContext) where T : BaseEntity<T>, new();

        #endregion
    }
}
