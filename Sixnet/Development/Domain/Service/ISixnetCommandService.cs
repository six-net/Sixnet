// "Company © 2025. All rights reserved."

using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Domain.Model;
using Sixnet.Exceptions;

namespace Sixnet.Development.Domain.Service
{
    public interface ISixnetCommandService<TEntity, TId>
    {
        #region Save

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="saveInfo"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        TEntity Save(SixnetSaveDataInfo<TEntity> saveInfo, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        TEntity Save(TEntity newData, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="newDatas"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        List<TEntity> Save(IEnumerable<TEntity> newDatas, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="saveInfo"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task<TEntity> SaveAsync(SixnetSaveDataInfo<TEntity> saveInfo, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task<TEntity> SaveAsync(TEntity newData, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="newDatas"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task<List<TEntity>> SaveAsync(IEnumerable<TEntity> newDatas, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        int Delete(IEnumerable<TId> ids, Action<SixnetDataOperationOptions> configure);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        int Delete(IEnumerable<TId> ids);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        int Delete(TId id, Action<SixnetDataOperationOptions> configure);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        int Delete(TId id);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        Task<int> DeleteAsync(IEnumerable<TId> ids, Action<SixnetDataOperationOptions> configure);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        Task<int> DeleteAsync(IEnumerable<TId> ids);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        Task<int> DeleteAsync(TId id, Action<SixnetDataOperationOptions> configure);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        Task<int> DeleteAsync(TId id);

        #endregion
    }
}
