// "Company © 2025. All rights reserved."

using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Domain.Model;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Domain.Service
{
    internal class SixnetCommandService<T> : SixnetBaseCommandService<T>, ISixnetCommandService<T>
        where T : SixnetBaseEntity<T>, new()
    {
        #region Save

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual T Save(SixnetSaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            return SaveData(parameter, configure);
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="newData">New data</param>
        /// <returns></returns>
        public virtual T Save(T newData, Action<SixnetDataOperationOptions> configure = null)
        {
            return SaveData(new SixnetSaveDataParameter<T>()
            {
                Data = newData
            }, configure);
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="newData"></param>
        /// <returns></returns>
        public virtual Task<T> SaveAsync(SixnetSaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            return SaveDataAsync(parameter, configure);
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="newData">New data</param>
        /// <returns></returns>
        public virtual Task<T> SaveAsync(T newData, Action<SixnetDataOperationOptions> configure = null)
        {
            return SaveDataAsync(new SixnetSaveDataParameter<T>()
            {
                Data = newData
            }, configure);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        public virtual void Delete(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            DeleteData(ids, configure);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        public virtual void Delete(dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            DeleteData(id, configure);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        public virtual Task DeleteAsync(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteDataAsync(ids, configure);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        public virtual Task DeleteAsync(dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteDataAsync(id, configure);
        }

        #endregion
    }
}
