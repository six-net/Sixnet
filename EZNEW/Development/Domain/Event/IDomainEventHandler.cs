using System.Threading.Tasks;

namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Domain event handler contract
    /// </summary>
    public interface IDomainEventHandler
    {
        #region Properties

        /// <summary>
        /// Gets or sets the handler execution time
        /// </summary>
        EventTriggerTime TriggerTime { get; }

        #endregion

        /// <summary>
        /// Execute domain event
        /// </summary>
        /// <param name="domainEvent">Domain event</param>
        /// <returns>Return event result</returns>
        DomainEventResult Execute(IDomainEvent domainEvent);
    }
}
