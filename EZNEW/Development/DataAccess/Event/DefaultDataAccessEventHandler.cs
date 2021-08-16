using System;
using System.Threading.Tasks;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Default data access event handler
    /// </summary>
    public class DefaultDataAccessEventHandler<Event> : IDataAccessEventHandler where Event : class, IDataAccessEvent
    {
        /// <summary>
        /// Gets or sets execute operation
        /// </summary>
        public Func<Event, DataAccessEventExecutionResult> ExecuteEventOperation
        {
            get; set;
        }

        /// <summary>
        /// Execute data access event
        /// </summary>
        /// <param name="dataAccessEvent">Data access event</param>
        /// <returns>Return data access event result</returns>
        public DataAccessEventExecutionResult Execute(IDataAccessEvent dataAccessEvent)
        {
            if (ExecuteEventOperation == null)
            {
                return DataAccessEventExecutionResult.EmptyResult("Did't set any event operation");
            }
            var eventData = dataAccessEvent as Event;
            if (eventData == null)
            {
                return DataAccessEventExecutionResult.EmptyResult("Data access event is null");
            }
            return ExecuteEventOperation(eventData);
        }
    }
}
