using System.Threading.Tasks;

namespace EZNEW.Development.DataAccess.Event
{
    /// <summary>
    /// Data access event handler
    /// </summary>
    public interface IDataAccessEventHandler
    {
        /// <summary>
        /// Execute data access event
        /// </summary>
        /// <param name="dataAccessEvent">Dataaccess event</param>
        /// <returns>Return data access event result</returns>
        DataAccessEventExecutionResult Execute(IDataAccessEvent dataAccessEvent);
    }
}
