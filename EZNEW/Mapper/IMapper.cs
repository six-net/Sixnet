namespace EZNEW.Mapper
{
    /// <summary>
    /// Object mapper contract
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// Convert an object to a specified type object
        /// </summary>
        /// <typeparam name="TTarget">Target data type</typeparam>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Return the target data object</returns>
        TTarget MapTo<TTarget>(object sourceObject);
    }
}
