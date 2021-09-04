using System;

namespace EZNEW.Mapper
{
    /// <summary>
    /// Object mapper contract
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Convert an object
        /// </summary>
        /// <typeparam name="TDestination">Destionation data type</typeparam>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Return the target data object</returns>
        TDestination MapTo<TDestination>(object sourceObject);

        /// <summary>
        /// Convert an object
        /// </summary>
        /// <param name="destinationType">Destination data type</param>
        /// <param name="source">Source object</param>
        /// <returns>Return ta destionation object</returns>
        object MapTo(Type destinationType, object source);
    }
}
