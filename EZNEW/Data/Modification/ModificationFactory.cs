using System;
using System.Linq.Expressions;

namespace EZNEW.Data.Modification
{
    /// <summary>
    /// Defines modification factory
    /// </summary>
    public class ModificationFactory
    {
        /// <summary>
        /// Create an IModification object
        /// </summary>
        /// <returns>Return a IModification object</returns>
        public static IModification Create()
        {
            return new ModificationInfo();
        }

        /// <summary>
        /// Create an IModification object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="expressions">Expressions</param>
        /// <returns>Return a IModification object</returns>
        public static IModification Create<T>(params Expression<Func<T, dynamic>>[] expressions)
        {
            return Create().Set(expressions);
        }
    }
}
