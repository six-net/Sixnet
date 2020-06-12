namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// Modify value
    /// </summary>
    public interface IModifyValue
    {
        /// <summary>
        /// Gets the calculate value
        /// </summary>
        dynamic Value { get; }

        /// <summary>
        /// Gets the modifyed value
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Return the modify value</returns>
        dynamic GetModifyValue(dynamic originalValue);
    }
}
