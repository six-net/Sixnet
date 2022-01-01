using System;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.UnitOfWork;

namespace EZNEW.Development.Domain.Repository.Event
{
    /// <summary>
    /// Modify event handler
    /// </summary>
    public class ModifyEventHandler : IRepositoryEventHandler
    {
        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets handler repository type
        /// </summary>
        public Type HandlerRepositoryType { get; set; }

        /// <summary>
        /// Gets or sets the object type
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the operation
        /// </summary>
        public ModifyOperation Operation { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="modify">Modeify expression</param>
        /// <param name="query">Query object</param>
        /// <param name="activationOptions">Activation options</param>
        /// <returns>Return repository event execute result</returns>
        public IRepositoryEventExecutionResult Execute(IModification modify, IQuery query, ActivationOptions activationOptions = null)
        {
            if (modify == null || Operation == null)
            {
                return DataOperationEventExecutionResult.Empty;
            }
            Operation(modify, query, activationOptions);
            return DataOperationEventExecutionResult.Empty;
        }
    }
}
