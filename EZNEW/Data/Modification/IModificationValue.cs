namespace EZNEW.Data.Modification
{
    /// <summary>
    /// Defines modification value
    /// </summary>
    public interface IModificationValue
    {
        /// <summary>
        /// Gets the modification value
        /// </summary>
        dynamic Value { get; }

        /// <summary>
        /// Gets the modified value
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Return the modified value</returns>
        dynamic GetModifiedValue(dynamic originalValue);
    }
}
