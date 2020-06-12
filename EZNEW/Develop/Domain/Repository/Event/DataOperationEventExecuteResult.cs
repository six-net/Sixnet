namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// Data operation event result
    /// </summary>
    public class DataOperationEventExecuteResult : IRepositoryEventExecuteResult
    {
        /// <summary>
        /// Default operation event result
        /// </summary>
        public static readonly DataOperationEventExecuteResult Empty = new DataOperationEventExecuteResult();
    }
}
