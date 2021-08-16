namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Default work completed domain event handler
    /// </summary>
    /// <typeparam name="TEvent">Domain event</typeparam>
    public class DefaultWorkCompletedDomainEventHandler<TEvent> : DefaultDomainEventHandler<TEvent>, IDomainEventHandler where TEvent : class, IDomainEvent
    {
        /// <summary>
        /// Gets or sets the event execute time
        /// </summary>
        public EventTriggerTime ExecuteTime { get; } = EventTriggerTime.WorkCompleted;
    }
}
