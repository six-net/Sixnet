// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Data;
using Sixnet.Development.Domain.Parameter;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Domain.Service
{
    public class SixnetCommandService<T> : SixnetBaseCommandService<T>, ISixnetCommandService<T>
        where T : SixnetBaseEntity<T>, new()
    {
        #region Save

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual T Save(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            return SaveData(parameter, configure);
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="newData"></param>
        /// <returns></returns>
        public virtual Task<T> SaveAsync(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            return SaveDataAsync(parameter, configure);
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
