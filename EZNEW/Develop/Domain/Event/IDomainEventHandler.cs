using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Event
{
    /// <summary>
    /// Domain event handler contract
    /// </summary>
    public interface IDomainEventHandler
    {
        #region Properties

        /// <summary>
        /// Gets or sets the handler execute time
        /// </summary>
        EventTriggerTime ExecuteTime { get; }

        #endregion

        /// <summary>
        /// Execute domain event
        /// </summary>
        /// <param name="domainEvent">Domain event</param>
        /// <returns>Return event result</returns>
        DomainEventExecuteResult Execute(IDomainEvent domainEvent);
    }
}
