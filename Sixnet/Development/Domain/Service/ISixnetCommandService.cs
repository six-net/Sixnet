// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Data;
using Sixnet.Development.Domain.Parameter;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Domain.Service
{
    public interface ISixnetCommandService<T>
    {
        #region Save

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        T Save(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="newData"></param>
        /// <returns></returns>
        Task<T> SaveAsync(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null);

        #endregion

        #region Delete

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        void Delete(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        void Delete(dynamic id, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        Task DeleteAsync(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        Task DeleteAsync(dynamic id, Action<SixnetDataOperationOptions> configure = null);

        #endregion
    }
}
