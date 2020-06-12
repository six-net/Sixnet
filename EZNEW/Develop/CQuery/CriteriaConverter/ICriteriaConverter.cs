namespace EZNEW.Develop.CQuery.CriteriaConverter
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
    }
}
