using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// modify object factory
    /// </summary>
    public class ModifyFactory
    {
        #region functions

        /// <summary>
        /// create a new IModify object
        /// </summary>
        /// <returns>IModify object</returns>
        public static IModify Create()
        {
            return new ModifyExpression();
        }

        /// <summary>
        /// create a new IModify object
        /// </summary>
        /// <typeparam name="T">Model Type</typeparam>
        /// <param name="fields">fields</param>
        /// <returns>IModify object</returns>
        public static IModify Create<T>(params Expression<Func<T, dynamic>>[] fields)
        {
            IModify modify = Create();
            modify.Set<T>(fields);
            return modify;
        }

        #endregion
    }
}
