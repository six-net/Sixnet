namespace EZNEW.Development.Query.CriteriaConverter
{
    /// <summary>
    /// Criteria converter contract
    /// </summary>
    public interface ICriteriaConverter
    {
        /// <summary>
        /// Gets or sets the config name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the data
        /// </summary>
        object Data { get; set; }

        /// <summary>
        /// Clone a new converter
        /// </summary>
        /// <returns></returns>
        ICriteriaConverter Clone();
    }
}
