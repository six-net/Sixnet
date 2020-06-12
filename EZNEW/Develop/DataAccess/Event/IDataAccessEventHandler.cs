using System.Threading.Tasks;

namespace EZNEW.Develop.DataAccess.Event
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
        DataAccessEventExecuteResult Execute(IDataAccessEvent dataAccessEvent);
    }
}
