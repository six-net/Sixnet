using System;
using System.Linq.Expressions;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// Modify object factory
    /// </summary>
    public class ModifyFactory
    {
        /// <summary>
        /// Create a new IModify object
        /// </summary>
        /// <returns>Return a IModify object</returns>
        public static IModify Create()
        {
            return new ModifyExpression();
        }

        /// <summary>
        /// Create a new IModify object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        /// <returns>Return a IModify object</returns>
        public static IModify Create<T>(params Expression<Func<T, dynamic>>[] fields)
        {
            IModify modify = Create();
            modify.Set(fields);
            return modify;
        }
    }
}
