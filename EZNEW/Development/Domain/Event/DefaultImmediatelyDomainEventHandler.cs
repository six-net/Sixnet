namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Default domain event handler
    /// </summary>
    public class DefaultImmediatelyDomainEventHandler<TEvent> : DefaultDomainEventHandler<TEvent>, IDomainEventHandler where TEvent : class, IDomainEvent
    {
        /// <summary>
        /// Gets or sets the event execute time
        /// </summary>
        public EventTriggerTime ExecuteTime { get; } = EventTriggerTime.Immediately;
    }
}
