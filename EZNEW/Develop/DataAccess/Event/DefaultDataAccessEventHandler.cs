using System;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataAccess.Event
{
    /// <summary>
    /// Default data access event handler
    /// </summary>
    public class DefaultDataAccessEventHandler<Event> : IDataAccessEventHandler where Event : class, IDataAccessEvent
    {
        /// <summary>
        /// Gets or sets execute operation
        /// </summary>
        public Func<Event, DataAccessEventExecuteResult> ExecuteEventOperation
        {
            get; set;
        }

        /// <summary>
        /// Execute data access event
        /// </summary>
        /// <param name="dataAccessEvent">Data access event</param>
        /// <returns>Return data access event result</returns>
        public DataAccessEventExecuteResult Execute(IDataAccessEvent dataAccessEvent)
        {
            if (ExecuteEventOperation == null)
            {
                return DataAccessEventExecuteResult.EmptyResult("Did't set any event operation");
            }
            var eventData = dataAccessEvent as Event;
            if (eventData == null)
            {
                return DataAccessEventExecuteResult.EmptyResult("Data access event is null");
            }
            return ExecuteEventOperation(eventData);
        }
    }
}
