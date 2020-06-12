using System;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Domain.Aggregation;
using EZNEW.Develop.UnitOfWork;

namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// Condition event handler
    /// </summary>
    public class ConditionEventHandler : IRepositoryEventHandler
    {
        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the handler repository type
        /// </summary>
        public Type HandlerRepositoryType { get; set; }

        /// <summary>
        /// Gets or sets the object type
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Gets or sets operation
        /// </summary>
        public ConditionOperation Operation { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="option">Activation option</param>
        /// <returns>Return repository event execute result</returns>
        public IRepositoryEventExecuteResult Execute(IQuery query, ActivationOption option = null)
        {
            if (Operation == null)
            {
                return DataOperationEventExecuteResult.Empty;
            }
            Operation(query, option);
            return DataOperationEventExecuteResult.Empty;
        }
    }
}
