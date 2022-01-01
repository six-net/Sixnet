namespace EZNEW.Data.Configuration
{
    /// <summary>
    /// Batch execution configuration
    /// </summary>
    public class BatchExecutionConfiguration
    {
        /// <summary>
        /// Gets or sets the group statements count
        /// </summary>
        public int GroupStatementsCount { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the group parameters count
        /// </summary>
        public int GroupParametersCount { get; set; } = 2000;

        /// <summary>
        /// Gets the default batch execute configuration
        /// </summary>
        public static readonly BatchExecutionConfiguration Default = new BatchExecutionConfiguration();
    }
}
