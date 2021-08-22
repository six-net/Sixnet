namespace EZNEW.Development.Domain.Event
{
    /// <summary>
    /// Default domain event handler
    /// </summary>
    public class DefaultImmediatelyDomainEventHandler<TEvent> : DefaultDomainEventHandler<TEvent>, IDomainEventHandler where TEvent : class, IDomainEvent
    {
        /// <summary>
        /// Gets or sets the event trigger time
        /// </summary>
        public EventTriggerTime TriggerTime { get; } = EventTriggerTime.Immediately;
    }
}
