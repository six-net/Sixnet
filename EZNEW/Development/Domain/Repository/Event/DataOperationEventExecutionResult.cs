namespace EZNEW.Development.Domain.Repository.Event
{
    /// <summary>
    /// Data operation event result
    /// </summary>
    public class DataOperationEventExecutionResult : IRepositoryEventExecutionResult
    {
        /// <summary>
        /// Default operation event result
        /// </summary>
        public static readonly DataOperationEventExecutionResult Empty = new DataOperationEventExecutionResult();
    }
}
