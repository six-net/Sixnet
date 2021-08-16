using System;
using System.Linq.Expressions;

namespace EZNEW.Development.Command.Modification
{
    /// <summary>
    /// Modification factory
    /// </summary>
    public class ModificationFactory
    {
        /// <summary>
        /// Create a new IModification object
        /// </summary>
        /// <returns>Return a IModification object</returns>
        public static IModification Create()
        {
            return new ModificationExpression();
        }

        /// <summary>
        /// Create a new IModification object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        /// <returns>Return a IModification object</returns>
        public static IModification Create<T>(params Expression<Func<T, dynamic>>[] fields)
        {
            return Create().Set(fields);
        }
    }
}
